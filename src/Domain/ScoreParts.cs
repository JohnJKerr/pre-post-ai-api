namespace Domain;

public record ScoreParts(double Output, double Projects, double Languages, double Hours)
{
    public static readonly ScoreParts Max = new(Output: 45, Projects: 15, Languages: 10, Hours: 30);

    public double Total => Output + Projects + Languages + Hours;
}
