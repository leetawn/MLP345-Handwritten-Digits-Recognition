namespace OptDigits.Data;

/// <summary>
/// Represents a single dataset sample consisting of feature values and an integer class label.
/// </summary>
/// <param name="Features">Array of numeric feature values (e.g. 64 pixel intensities).</param>
/// <param name="Label">Ground-truth integer class label (0 to 9).</param>
public sealed record Sample(double[] Features, int Label);
