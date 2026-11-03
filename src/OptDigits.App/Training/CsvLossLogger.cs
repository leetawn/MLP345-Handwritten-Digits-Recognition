namespace OptDigits.Training;

/// <summary>
/// Logs epoch training and validation loss metrics to a CSV file and echoes them to the console.
/// </summary>
/// <remarks>Owner: Partner A</remarks>
public sealed class CsvLossLogger : ILossLogger
{
    private readonly StreamWriter? _writer = null;

    /// <summary>
    /// Initializes a new instance of <see cref="CsvLossLogger"/>, creating the CSV file and writing header columns.
    /// Header: epoch,train_loss,validation_loss,validation_accuracy
    /// </summary>
    /// <param name="path">Destination file path for the CSV log.</param>
    /// <exception cref="NotImplementedException">Thrown because this constructor is a stub.</exception>
    public CsvLossLogger(string path)
    {
        // TODO 1: Ensure directory exists for destination path
        // TODO 2: Initialize StreamWriter on path
        // TODO 3: Write header line: "epoch,train_loss,validation_loss,validation_accuracy"
        // TODO 4: Flush writer
        throw new NotImplementedException("CsvLossLogger..ctor is not implemented yet");
    }

    /// <summary>
    /// Logs a single epoch's metrics to CSV and prints a progress line to the console.
    /// </summary>
    /// <param name="metrics">The metrics measured for the epoch.</param>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public void Log(EpochMetrics metrics)
    {
        // TODO 1: Format CSV row: $"{metrics.Epoch},{metrics.TrainLoss:F6},{metrics.ValidationLoss:F6},{metrics.ValidationAccuracy:F4}"
        // TODO 2: Write row to _writer and Flush()
        // TODO 3: Print user-friendly status to Console: $"Epoch {metrics.Epoch,3} | Train Loss: {metrics.TrainLoss:F6} | Val Loss: {metrics.ValidationLoss:F6} | Val Acc: {metrics.ValidationAccuracy * 100:F2}%"
        throw new NotImplementedException("CsvLossLogger.Log is not implemented yet");
    }

    /// <summary>
    /// Disposes the underlying stream writer resources.
    /// </summary>
    public void Dispose()
    {
        _writer?.Dispose();
    }
}
