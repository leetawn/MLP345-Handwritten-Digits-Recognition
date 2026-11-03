namespace OptDigits.Experiments;

/// <summary>
/// Defines hyperparameter search dimensions for parameter sweep experiments.
/// </summary>
/// <param name="LearningRates">Candidate learning rates for the output layer (lrIn is calculated as lrOut * 0.75 to preserve the 0.2 / 0.15 ratio).</param>
/// <param name="HiddenSizes">Candidate numbers of hidden layer neurons.</param>
/// <param name="EpochCounts">Candidate epoch counts.</param>
public sealed record ExperimentGrid(
    IReadOnlyList<double> LearningRates,
    IReadOnlyList<int> HiddenSizes,
    IReadOnlyList<int> EpochCounts)
{
    /// <summary>
    /// Gets the default hyperparameter search grid with standard parameter variations.
    /// LearningRates: { 0.01, 0.05, 0.1, 0.2, 0.5 }
    /// HiddenSizes: { 16, 32, 64 }
    /// EpochCounts: { 20, 50, 100 }
    /// Note: LearningRateIn is calculated as LearningRateOut * 0.75 to preserve the original 0.2 / 0.15 ratio.
    /// </summary>
    public static ExperimentGrid Default => new(
        new double[] { 0.01, 0.05, 0.1, 0.2, 0.5 },
        new int[] { 16, 32, 64 },
        new int[] { 20, 50, 100 });
}
