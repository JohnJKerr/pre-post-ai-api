# Decisions

## Store the raw activity, build the report on read

MongoDB holds each user's `Profile` (the metadata gathered from GitHub), not the finished `Report`. Every `GET /api/reports/{handle}` builds the report from it, so a change to the score rules reaches old links. This matches the front end's mock.

## The sample report has its own route

`sam-builds` is a valid GitHub username, so the sample is served at `GET /api/sample-report` rather than under `/api/reports/{handle}`. It is generated in memory from the seeded generator and never stored.

## A punch-card repo must have commits in its window

The brief makes a repo punch-card eligible when the user wrote at least 90% of its commits and all their activity in it sits on one side of the cut-off. We also require that they committed in that window. Without it, a repo last touched before the pre window would count as eligible for pre: it would add an old punch card to the window and push `coverage.eligible` above `coverage.total`.

## Rounding follows JavaScript

The reference implementation uses `Math.round`, which rounds halves towards positive infinity. `Rounding.HalfUp` does the same, because .NET's default banker's rounding would move scores and line counts.

## Profiles are keyed by GitHub user id

A profile's `_id` is the numeric GitHub user id from `/user`, read at sign-in. Generating again replaces that user's profile and updates its handle, so after a rename the old handle returns 404. `GET /api/reports/{handle}` still looks up by handle (case-insensitive). The handle index isn't unique: someone can take a renamed user's old name before that user generates again, and then the most recently generated profile holds the handle.

Profiles saved before this change have an ObjectId `_id` and no `gitHubId`. A database that holds them also has the old unique `handle_1` index, which must be dropped, as should those profiles: their owners regenerate.

## Reports are generated in the background after sign-in, with no session

The only thing that needs authenticating is gathering a user's metadata, and the GitHub user token from the OAuth callback is all that takes. So signing in doesn't start a session:

1. `GET /api/auth/sign-in?cutoff=` checks the cut-off, then sends the user to GitHub.
2. The callback saves a pending `Generation` under an unguessable id and puts the user's token on an in-memory queue. It then redirects to the front end's `/generating?id=`.
3. A hosted service (`ReportGenerator`) fetches the metadata, saves the profile, and marks the generation `ready`, `empty` (no commits in either window, nothing saved) or `failed` with a message.
4. The front end polls `GET /api/generations/{id}`.

The token never leaves the back end and is never stored. Rejected alternatives:
- **A session cookie holding the token:** needs persisted encryption keys, CSRF checks and expiry handling.
- **A JWT of our own:** still needs a signing key, and it would carry GitHub's token.
- **Giving the token to the front end:** puts a credential that can list private repo names within reach of any XSS.
- **Hangfire:** serialises job arguments, and so the token, into its store.

The queue is a bounded `Channel` read by four workers. GitHub's secondary rate limits are per token, so different users' jobs can run at once while each job's calls stay sequential. A restart loses jobs in flight, which is the intended failure, because the token is gone. A generation still pending after 30 minutes reads as failed.

Because the job can wait as long as it needs, GitHub's 202 "still computing" is retried within the job. If a repo still isn't ready after 15 attempts, the generation fails and asks the user to sign in again in a minute.

## Repos are skipped rather than fetched in parallel

`/user/repos` lists every repo the user can see, and each one costs three calls. Repos last pushed before the pre window starts are skipped, because they can't have activity in either window. Within a job, the calls stay sequential: GitHub's REST guidance is to make requests serially rather than concurrently, to avoid secondary rate limits.

## Data Protection keys stay in memory

ASP.NET Data Protection now guards only the OAuth state of a sign-in under way, for about 15 minutes, so its keys are ephemeral. A restart in the middle of a sign-in sends that user to the front end's `/?error=sign-in-failed` to try again. With more than one instance, a sign-in must return to the instance that started it (sticky sessions); otherwise persist the keys again.

## Repo names are never kept

The report doesn't need to know what a repo is called, only how much of the work it holds, whether it's private, and when it started. So the GitHub adapter drops repo names, and real reports list every project with `name: null`. That goes further than the hard rule that private names never leave the back end: no name, public or private, reaches the database. Only the sample report has names, because its repos are made up and it must match the front end's fixture.

## The app asks for Metadata only, so stats come from REST

GraphQL commit history would avoid the stats endpoints' 202s and give each commit's time, which an active-hours measure needs. But reading commits needs the **Contents: read** permission, and GitHub's consent screen then tells users the app can read their code. That is too much to ask for a report about their habits. With **Metadata: read** alone, GraphQL returns no history for private repos, and a report built that way silently drops most of a user's work. So the adapter stays on REST `stats/contributors`, `stats/punch_card` and `languages`, which Metadata covers, and a cold account still waits for GitHub to compute its stats.

## The score weighs commits, new languages and active days

The score is out of 100, in four parts. Each part rounds to whole points before they're added.
- **Output (40):** commits score up to 28 and lines up to 12, each as `log10(post / pre)`, so 10× earns the full amount. A commit is a change someone chose to make. Lines swell with generated code and lockfiles: one week of 82k lines can dominate a year.
- **Languages (20):** 5 points for each new language, up to four. To count as new, a language must be at least 5% of the post window and at most 1% of the pre window, so a language hovering near a threshold can't score by chance.
- **Time (25):** `25 × log2(post / pre)` of active days from the contribution calendar, so twice as many days earns the full amount and fewer days score 0. Active days are dated and cover every repo, private ones included. Active hours would be a better measure, but they need commit timestamps (see above).
- **Projects (15):** unchanged, `15 × log2(post / pre) / 2`.

The out-of-hours share is no longer scored. It came only from punch-card-eligible repos, which can be a small fraction of the work.

Every part measures change relative to the user's own past. Someone already coding six days a week has little room to grow their active days, and that's intended: the score is about how much AI changed their work, not how much they work.

The verdicts on the ladder name how far the user has changed, from "I'm an artisan" to "I started the robot uprising", rather than what they run. A big score can come from a small base, and someone who went from never coding to two days a week has changed a lot without running a software factory. There is no tag under the verdict; the front end shows imagery for it instead.

## Each window is at most 26 weeks

The post window used to run from the cut-off to the last complete week, and the pre window mirrored it. An old cut-off such as the ChatGPT launch then spanned about eight years, so nearly every repo the user had pushed to since then had to be fetched, and one such generation failed after more than three minutes. Now pre is the 26 weeks before the cut-off week and post is the latest 26 complete weeks. A cut-off less than 26 weeks ago shrinks both windows to the complete weeks since it, so they never overlap and always match in length.

The weeks between the two windows count towards neither, except that a punch-card repo's side is still decided by the cut-off week, not by where the post window starts.
