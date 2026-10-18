# Decisions

## Store the raw activity, build the report on read

MongoDB holds each user's `Profile` (the metadata gathered from GitHub), not the finished `Report`. Every `GET /api/reports/{handle}` builds the report from it, so a change to the score rules reaches old links. This matches the front end's mock.

## The sample report has its own route

`sam-builds` is a valid GitHub username, so the sample is served at `GET /api/sample-report` rather than under `/api/reports/{handle}`. It is generated in memory from the seeded generator and never stored.

## A punch-card repo must have commits in its window

The brief makes a repo punch-card eligible when the user wrote at least 90% of its commits and all their activity in it sits on one side of the cut-off. We also require that they committed in that window. Without it, a repo last touched before the pre window would count as eligible for pre: it would add an old punch card to the window and push `coverage.eligible` above `coverage.total`.

## Rounding follows JavaScript

The reference implementation uses `Math.round`, which rounds halves towards positive infinity. `Rounding.HalfUp` does the same, because .NET's default banker's rounding would move scores and line counts.
