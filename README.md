# Pre/Post API

ASP.NET Core API behind the Pre/Post front end. It signs users in through a GitHub App, gathers their repository metadata, and serves a finished report comparing how they coded before and after a cut-off.

## Projects

- `src/Domain`: windows, profile assembly, report and score rules. No package references.
- `src/External`: the GitHub REST and GraphQL adapter.
- `src/Storage`: MongoDB persistence.
- `src/Api`: controllers, sign-in, wiring.

Tests mirror them: `test/Domain.Tests`, `test/External.Tests` (GitHub responses served by a fake handler) and `test/Api.Tests` (HTTP against a MongoDB Testcontainer, so Docker must be running).

```bash
dotnet test --artifacts-path /tmp/pre-post-ai-api-artifacts test/Domain.Tests
dotnet test --artifacts-path /tmp/pre-post-ai-api-artifacts test/External.Tests
dotnet test --artifacts-path /tmp/pre-post-ai-api-artifacts test/Api.Tests
```

## Running locally

```bash
docker run -d -p 27017:27017 mongo:8
dotnet user-secrets --project src/Api set GitHub:ClientId <app client id>
dotnet user-secrets --project src/Api set GitHub:ClientSecret <app client secret>
dotnet run --project src/Api --launch-profile https
```

The GitHub App needs one repository permission, **Metadata: read**, and a callback URL of `https://<host>/api/auth/callback`.

## Endpoints

| Method and path | Purpose |
|---|---|
| `GET /api/auth/sign-in?returnUrl=/r/me` | Redirects to GitHub to authorise the app, then back to `returnUrl` |
| `POST /api/auth/sign-out` | Ends the session (204) |
| `GET /api/session` | 200 `{ handle }` or 401 |
| `POST /api/reports` | Generates and saves the signed-in user's report (201, 401, 422, 503) |
| `GET /api/reports/{handle}` | A saved report (200) or 404 |
| `GET /api/sample-report` | The seeded sample report |

Errors are `{ "message": string }`, written to be shown to people.
