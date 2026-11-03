namespace OptDigits.Data;

// Represents a single dataset sample consisting of feature values and an integer class label.
// Features: Array of numeric feature values (e.g. 64 pixel intensities).
// Label: Ground-truth integer class label (0 to 9).
public sealed record Sample(double[] Features, int Label);
