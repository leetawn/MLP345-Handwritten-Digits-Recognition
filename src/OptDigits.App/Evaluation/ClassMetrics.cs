namespace OptDigits.Evaluation;

/// <summary>
/// Evaluation performance metrics for an individual digit class.
/// </summary>
/// <param name="Label">The digit class label (0 to 9).</param>
/// <param name="Precision">Precision score: TruePositives / (TruePositives + FalsePositives).</param>
/// <param name="Recall">Recall score: TruePositives / (TruePositives + FalseNegatives).</param>
/// <param name="F1">F1 score harmonic mean: 2 * Precision * Recall / (Precision + Recall).</param>
/// <param name="Support">Number of actual occurrences of this class in the evaluation set.</param>
public sealed record ClassMetrics(
    int Label,
    double Precision,
    double Recall,
    double F1,
    int Support);
