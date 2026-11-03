namespace OptDigits.Evaluation;

/// <summary>
/// Detailed evaluation report containing overall accuracy, per-class metrics, and confusion matrix.
/// </summary>
/// <param name="Accuracy">Overall classification accuracy.</param>
/// <param name="PerClass">List of per-digit class metrics (precision, recall, F1, support).</param>
/// <param name="ConfusionMatrix">2D confusion matrix where rows represent actual classes and columns represent predicted classes.</param>
/// <remarks>Owner: Partner B</remarks>
public sealed record EvaluationReport(
    double Accuracy,
    IReadOnlyList<ClassMetrics> PerClass,
    int[,] ConfusionMatrix)
{
    /// <summary>
    /// Formats the evaluation metrics and confusion matrix into a human-readable console string.
    /// </summary>
    /// <returns>Formatted multi-line report string.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public string ToConsoleString()
    {
        // TODO 1: Use StringBuilder to construct a formatted text report
        // TODO 2: Append overall Accuracy percentage: $"Overall Accuracy: {Accuracy * 100:F2}%"
        // TODO 3: Append table header: "Class | Precision | Recall | F1-Score | Support"
        // TODO 4: Append each row in PerClass formatted to 4 decimal places
        // TODO 5: Append formatted confusion matrix grid with headers for actual/predicted
        // TODO 6: Return sb.ToString()
        throw new NotImplementedException("EvaluationReport.ToConsoleString is not implemented yet");
    }

    /// <summary>
    /// Saves the confusion matrix to a CSV file.
    /// Header: actual,pred_0,pred_1,pred_2,pred_3,pred_4,pred_5,pred_6,pred_7,pred_8,pred_9
    /// </summary>
    /// <param name="path">Destination CSV file path.</param>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public void SaveConfusionMatrixCsv(string path)
    {
        // TODO 1: Open StreamWriter at destination path
        // TODO 2: Write header row: "actual,pred_0,pred_1,pred_2,pred_3,pred_4,pred_5,pred_6,pred_7,pred_8,pred_9"
        // TODO 3: For each row i from 0 to 9:
        //           a. Build comma-delimited string starting with i followed by ConfusionMatrix[i, j] for j = 0..9
        //           b. Write line to writer
        // TODO 4: Flush and close writer
        throw new NotImplementedException("EvaluationReport.SaveConfusionMatrixCsv is not implemented yet");
    }
}
