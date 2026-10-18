using Domain;

namespace Api.Tests;

internal class GivenAGeneration
{
    private Generation _generation = Generation.Start(gitHubId: 9_001, "polled-dev", Cutoff.Default, ApiFactory.Now);

    public GivenAGeneration StartedMinutesAgo(int minutes)
    {
        _generation = _generation with { StartedAt = ApiFactory.Now.AddMinutes(-minutes) };
        return this;
    }

    public GivenAGeneration WithCutoffOn(DateOnly date)
    {
        _generation = _generation with { Cutoff = Cutoff.On(date) };
        return this;
    }

    public GivenAGeneration Completed()
    {
        _generation = _generation.Complete();
        return this;
    }

    public GivenAGeneration FailedBecause(string message)
    {
        _generation = _generation.Fail(message);
        return this;
    }

    public Generation Build() => _generation;
}
