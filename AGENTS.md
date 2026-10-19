# AGENTS.md

`README.md` covers the projects, running locally and the endpoints. `docs/DECISIONS.md` records why things are the way they are: read the relevant section before changing GitHub fetching, scoring, storage or sign-in, and add a section when you make a decision of your own.

## GitHub access

- The GitHub App holds one permission, **Metadata: read**. Every GitHub call must work with that alone: REST stats, languages, repo listing and the contribution calendar do; commit history, file contents and private pull requests do not.
- Check a GitHub change against a real app user token. A `gh` CLI token reads private code and org repos the app cannot, so a query that works through `gh` can still return nothing for the app.
- An unreadable repo must fail loudly. Treating it as empty quietly drops the user's work from their report.
- Repo names stay inside the `External` adapter, and the user's token stays in memory for the job's lifetime.

## The front end

The front end lives in `../pre-post-ai`. `test/Api.Tests/Fixtures/sample-report.json` is the contract between the two: a change to the report's shape or score changes that fixture, and the front end needs the same change, including its mock, which computes the score itself.

## Scoring

The score measures how far a user's work changed relative to their own pre-cut-off past, never how much they work. The ladder's verdicts follow a person-to-machine progression in a playful first person ("I'm an artisan" … "I started the robot uprising"); the front end pairs each with imagery.

## Tests

Work test-first: a failing test, then the least code that passes it.

- One test class per context, named `When<Context>`; one `[Fact]` per behaviour, named as a sentence with underscores (`The_generation_fails`).
- One assertion per test. Each test is a whole method with `// Arrange`, `// Act`, `// Assert` comments.
- Set up through fluent `Given<Thing>` builders whose methods name domain state (`new GivenAShiftSide().WithActiveDays(161)`), and use hand-written fakes such as `FakeGitHub`.
- `Api.Tests` runs MongoDB in a Testcontainer, so Docker must be running.

## Commits

Commit each green slice on its own. Stage files by name: `src/Api/appsettings.Development.json` often carries local edits that stay uncommitted.
