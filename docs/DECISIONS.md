# Decisions

## Store the raw activity, build the report on read

MongoDB holds each user's `Profile` (the metadata gathered from GitHub), not the finished `Report`. Every `GET /api/reports/{handle}` builds the report from it, so a change to the score rules reaches old links. This matches the front end's mock.

## The sample report has its own route

`sam-builds` is a valid GitHub username, so the sample is served at `GET /api/sample-report` rather than under `/api/reports/{handle}`. It is generated in memory from the seeded generator and never stored.

## A punch-card repo must have commits in its window

The brief makes a repo punch-card eligible when the user wrote at least 90% of its commits and all their activity in it sits on one side of the cut-off. We also require that they committed in that window. Without it, a repo last touched before the pre window would count as eligible for pre: it would add an old punch card to the window and push `coverage.eligible` above `coverage.total`.

## Rounding follows JavaScript

The reference implementation uses `Math.round`, which rounds halves towards positive infinity. `Rounding.HalfUp` does the same, because .NET's default banker's rounding would move scores and line counts.

## 503 while GitHub is still computing stats

GitHub answers `stats/*` with 202 while it computes them. The adapter retries every 2 seconds, up to 15 attempts per call. If a repo still isn't ready, `POST /api/reports` answers 503 with a message to try again. The front end retries any status other than 401, 404 and 422 twice, and GitHub caches stats once computed, so a retry usually succeeds. POST stays blocking for now. Switching to 202 plus polling is still open.

## The session cookie carries the GitHub user token

Sign-in is the GitHub App's user authorisation flow. The encrypted, HttpOnly session cookie holds the user's login and their user access token, which `POST /api/reports` uses to read their metadata. A GitHub App user token takes no scopes: it can do only what the app's permissions allow.

## The session ends when the GitHub token does

Expiring GitHub App user tokens last 8 hours. Rather than keep the refresh token (valid for 6 months) in the cookie and refresh it, the session expires with the access token and doesn't slide. Generating a report is a one-off, so signing in again after 8 hours costs little, and the cookie never holds a long-lived credential. If GitHub still rejects a token, for example after the user revokes the app, `POST /api/reports` ends the session and answers 401.

## The cookie's encryption keys live in MongoDB

ASP.NET Data Protection encrypts the session cookie, and with it the GitHub token. Its keys are kept in the `dataProtectionKeys` collection, under the application name `pre-post-ai-api`, so a restart or a second instance can still read sessions. The keys aren't encrypted at rest: anyone who can read the database can read session cookies. Before production, consider `ProtectKeysWithCertificate` or a cloud key vault.

The tests run on a machine whose disk key ring (`~/.aspnet/DataProtection-Keys`) is shared by every test host, so "a session survives a restart" passes without Mongo. A second test proves the keys come from the database: a host using another database can't read the session.
