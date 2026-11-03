namespace OptDigits.Training;

// Defines the contract for logging training progress and epoch metrics.
public interface ILossLogger : IDisposable
{
    // Records metrics for a single completed epoch.
    // metrics: Metrics for the epoch.
    void Log(EpochMetrics metrics);
}
