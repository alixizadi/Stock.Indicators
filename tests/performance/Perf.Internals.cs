namespace Tests.Performance;

// INTERNAL FUNCTIONS

public class InternalsPerformance
{
    [Params(20, 50, 250, 1000)]
    public int Periods;

    private double[] values;

    // standard deviation
    [GlobalSetup(Targets = new[] { nameof(StdDev) })]
    public void Setup()
        => values = TestData.GetLongish(Periods)
            .Select(x => (double)x.Close)
            .ToArray();

    [Benchmark]
    public object StdDev() => values.StdDev();
}
