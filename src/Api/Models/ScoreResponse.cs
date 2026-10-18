using Domain;

namespace Api.Models;

public record ScoreResponse(
    double Total,
    ScorePartsResponse Parts,
    int Rung,
    string Verdict,
    string? Tag,
    IReadOnlyList<RungResponse> Ladder)
{
    public static ScoreResponse From(Score score)
    {
        var max = ScoreParts.Max;
        var parts = new ScorePartsResponse(
            new ScorePartResponse(score.Parts.Output, max.Output),
            new ScorePartResponse(score.Parts.Projects, max.Projects),
            new ScorePartResponse(score.Parts.Languages, max.Languages),
            new ScorePartResponse(score.Parts.Hours, max.Hours));
        var ladder = Score.Ladder.Select(r => new RungResponse(r.Min, r.Verdict)).ToArray();

        return new ScoreResponse(score.Total, parts, score.Rung, score.Verdict, score.Tag, ladder);
    }
}

public record ScorePartsResponse(ScorePartResponse Output, ScorePartResponse Projects, ScorePartResponse Languages, ScorePartResponse Hours);

public record ScorePartResponse(double Points, double Max);

public record RungResponse(double Min, string Verdict);
