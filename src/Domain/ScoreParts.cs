namespace Domain;

public record ScoreParts(double Output, double Projects, double Languages, double Time)
{
    public static readonly ScoreParts Max = new(Output: 40, Projects: 15, Languages: 20, Time: 25);

    public double Total => Output + Projects + Languages + Time;
}
