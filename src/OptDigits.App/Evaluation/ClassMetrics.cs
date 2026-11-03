namespace OptDigits.Evaluation;

// Evaluation performance metrics for an individual digit class.
// Label: The digit class label (0 to 9).
// Precision: Precision score: TruePositives / (TruePositives + FalsePositives).
// Recall: Recall score: TruePositives / (TruePositives + FalseNegatives).
// F1: F1 score harmonic mean: 2 * Precision * Recall / (Precision + Recall).
// Support: Number of actual occurrences of this class in the evaluation set.
public sealed record ClassMetrics(
    int Label,
    double Precision,
    double Recall,
    double F1,
    int Support);
