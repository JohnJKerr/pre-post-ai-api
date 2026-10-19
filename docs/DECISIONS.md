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

The queue is a bounded `Channel` read by four workers, so different users' jobs run at once. A restart loses jobs in flight, which is the intended failure, because the token is gone. A generation still pending after 30 minutes reads as failed.

## Metadata comes from GraphQL, a few repos at a time

The GitHub adapter makes only GraphQL calls:
- **Repos:** listed most recently pushed first, with their languages. Listing stops at the first repo last pushed before the pre window starts, because neither it nor any repo after it can have activity in either window.
- **Commits:** each repo's default-branch history between the start of the pre window and the end of the post window, filtered to the user. Every commit carries its time, additions and deletions. The adapter totals them by week (Sundays, in UTC) and into a punch card. The punch card uses the commit's own UTC offset, so it shows the user's local hours.
- **Contribution calendar:** a year at a time, as before.

This replaced the REST `stats/contributors`, `stats/punch_card` and `languages` endpoints. The stats endpoints answer 202 until GitHub has computed them, and an account whose stats were cold waited up to 30 s per repo. Their punch card also covered every author, for the repo's whole life, with no dates. Per-commit timestamps are what an active-hours measure needs.

Two consequences:
- A repo's commit total (all authors) is now counted within the windows, not over its whole life, and the punch card holds only the user's own commits.
- Listing commits costs about 1.3 s per 100, a third of that for GitHub to count lines. Done one repo after another, an account with 3,500 commits across 30 repos took two minutes. So four repos are fetched at once, which is far below GraphQL's secondary limits (100 concurrent requests, and a CPU-time budget). A call that is rate limited anyway (429, or 403 with `Retry-After` or no requests remaining) is retried after the wait GitHub asks for, up to three attempts.

## Data Protection keys stay in memory

ASP.NET Data Protection now guards only the OAuth state of a sign-in under way, for about 15 minutes, so its keys are ephemeral. A restart in the middle of a sign-in sends that user to the front end's `/?error=sign-in-failed` to try again. With more than one instance, a sign-in must return to the instance that started it (sticky sessions); otherwise persist the keys again.

## Repo names are never kept

The report doesn't need to know what a repo is called, only how much of the work it holds, whether it's private, and when it started. So the GitHub adapter drops repo names, and real reports list every project with `name: null`. That goes further than the hard rule that private names never leave the back end: no name, public or private, reaches the database. Only the sample report has names, because its repos are made up and it must match the front end's fixture.
