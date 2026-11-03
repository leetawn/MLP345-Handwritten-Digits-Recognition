using System.Globalization;
using System.IO;

namespace OptDigits.Training;

// Logs epoch training and validation loss metrics to a CSV file and echoes them to the console.
public sealed class CsvLossLogger : ILossLogger
{
    private readonly StreamWriter _writer;
    private readonly int? _totalEpochs;
    private bool _disposed;

    // Initializes a new instance of CsvLossLogger, creating the CSV file (overwriting existing) and writing header columns.
    // Header: epoch,train_loss,validation_loss,validation_accuracy
    // path: Destination file path for the CSV log.
    // totalEpochs: Optional total epoch count used for console progress formatting.
    public CsvLossLogger(string path, int? totalEpochs = null)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        _totalEpochs = totalEpochs;
        _writer = new StreamWriter(path, append: false);
        _writer.WriteLine("epoch,train_loss,validation_loss,validation_accuracy");
        _writer.Flush();
    }

    // Logs a single epoch's metrics to CSV using invariant culture formatting and prints a progress line to the console.
    // metrics: The metrics measured for the epoch.
    public void Log(EpochMetrics metrics)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        string line = string.Format(
            CultureInfo.InvariantCulture,
            "{0},{1:F6},{2:F6},{3:F4}",
            metrics.Epoch,
            metrics.TrainLoss,
            metrics.ValidationLoss,
            metrics.ValidationAccuracy);

        _writer.WriteLine(line);
        _writer.Flush();

        string epochDisplay = _totalEpochs.HasValue
            ? $"{metrics.Epoch,4}/{_totalEpochs.Value}"
            : $"{metrics.Epoch,4}";

        Console.WriteLine(
            string.Format(
                CultureInfo.InvariantCulture,
                "Epoch {0} | train loss {1:F6} | val loss {2:F6} | val acc {3:P2}",
                epochDisplay,
                metrics.TrainLoss,
                metrics.ValidationLoss,
                metrics.ValidationAccuracy));
    }

    // Disposes the underlying stream writer resources. Safe to call multiple times.
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _writer.Dispose();
    }
}
