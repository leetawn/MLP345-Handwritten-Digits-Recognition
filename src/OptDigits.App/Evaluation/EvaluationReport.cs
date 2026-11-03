using System.Globalization;
using System.Text;

namespace OptDigits.Evaluation;

// Detailed evaluation report containing overall accuracy, per-class metrics, and confusion matrix.
// Accuracy: Overall classification accuracy.
// PerClass: List of per-digit class metrics (precision, recall, F1, support).
// ConfusionMatrix: 2D confusion matrix where rows represent actual classes and columns represent predicted classes.
public sealed record EvaluationReport(
    double Accuracy,
    IReadOnlyList<ClassMetrics> PerClass,
    int[,] ConfusionMatrix)
{
    // Formats the evaluation metrics and confusion matrix into a human-readable console string.
    // Returns: Formatted multi-line report string.
    public string ToConsoleString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Overall Accuracy: {(Accuracy * 100).ToString("F2", CultureInfo.InvariantCulture)}%");
        builder.AppendLine();
        builder.AppendLine("Per-Class Metrics");
        builder.AppendLine($"{"Class",5} {"Precision",9} {"Recall",9} {"F1",9} {"Support",8}");
        builder.AppendLine(new string('-', 45));
        foreach (ClassMetrics metrics in PerClass)
        {
            builder.AppendLine(
                $"{metrics.Label,5} {metrics.Precision.ToString("F4", CultureInfo.InvariantCulture),9} " +
                $"{metrics.Recall.ToString("F4", CultureInfo.InvariantCulture),9} " +
                $"{metrics.F1.ToString("F4", CultureInfo.InvariantCulture),9} {metrics.Support,8}");
        }

        builder.AppendLine();
        builder.AppendLine("Confusion Matrix (actual rows, predicted columns)");
        builder.Append("actual\\predicted");
        for (int classIndex = 0; classIndex < ConfusionMatrix.GetLength(1); classIndex++)
        {
            builder.Append($"{classIndex,6}");
        }

        builder.AppendLine();
        for (int actual = 0; actual < ConfusionMatrix.GetLength(0); actual++)
        {
            builder.Append($"{actual,16}");
            for (int predicted = 0; predicted < ConfusionMatrix.GetLength(1); predicted++)
            {
                builder.Append($"{ConfusionMatrix[actual, predicted],6}");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    // Saves the confusion matrix to a CSV file.
    // Header: actual,pred_0,pred_1,pred_2,pred_3,pred_4,pred_5,pred_6,pred_7,pred_8,pred_9
    // path: Destination CSV file path.
    public void SaveConfusionMatrixCsv(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        string? directory = Path.GetDirectoryName(Path.GetFullPath(path));
        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        int columnCount = ConfusionMatrix.GetLength(1);
        writer.Write("actual\\predicted");
        for (int predicted = 0; predicted < columnCount; predicted++)
        {
            writer.Write($",{predicted.ToString(CultureInfo.InvariantCulture)}");
        }

        writer.WriteLine();
        int rowCount = ConfusionMatrix.GetLength(0);
        for (int actual = 0; actual < rowCount; actual++)
        {
            writer.Write(actual.ToString(CultureInfo.InvariantCulture));
            for (int predicted = 0; predicted < columnCount; predicted++)
            {
                writer.Write($",{ConfusionMatrix[actual, predicted].ToString(CultureInfo.InvariantCulture)}");
            }

            writer.WriteLine();
        }
    }
}
