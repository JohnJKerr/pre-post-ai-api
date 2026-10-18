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
dotnet user-secrets --project src/Api set GitHub:AppSlug <app slug, as in github.com/apps/<slug>>
dotnet user-secrets --project src/Api set GitHub:ClientId <app client id>
dotnet user-secrets --project src/Api set GitHub:ClientSecret <app client secret>
dotnet run --project src/Api --launch-profile https
```

The GitHub App needs one repository permission, **Metadata: read**, and a callback URL of `https://<host>/api/auth/callback`. Turn on "Request user authorization (OAuth) during installation": a user who installs from the app's GitHub page returns to the callback, which restarts sign-in with the default cut-off. A user who signs in without having installed the app is sent to its install page, and no report is generated. `FrontEnd:Origin` is the front end's origin (`http://localhost:5173` in Development).

Behind a proxy or load balancer that ends TLS, list its address ranges in `ForwardedHeaders:KnownNetworks` (for example `["10.0.0.0/8"]`). The API then trusts its `X-Forwarded-Proto` and `X-Forwarded-Host`, so the OAuth `redirect_uri` is the public https address.

## Endpoints

| Method and path | Purpose |
|---|---|
| `GET /api/auth/sign-in?cutoff=YYYY-MM-DD` | Starts generating the user's report. Redirects to GitHub, then to the front end's `/generating?id={id}`. The cut-off is optional (default 2025-11-24). If it isn't a date, the API redirects to `/generate?error=invalid-cutoff`; if it's less than a full week ago, to `/generate?error=cutoff-too-recent`. A failed sign-in goes to `/`. |
| `GET /api/generations/{id}` | `{ status, handle, message }`, where `status` is `pending`, `ready` (`handle` names the report), `empty` (no commits in either window, nothing saved) or `failed` (`message` says why). 404 if unknown. |
| `GET /api/reports/{handle}` | A saved report (200) or 404 |
| `GET /api/sample-report` | The seeded sample report |

There's no session. Signing in hands the user's GitHub token to a background job that generates the report, and the token is never stored. Errors are `{ "message": string }`, written to be shown to people.
