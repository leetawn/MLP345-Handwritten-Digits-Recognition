namespace OptDigits.Training;

/// <summary>
/// Defines the contract for logging training progress and epoch metrics.
/// </summary>
public interface ILossLogger : IDisposable
{
    /// <summary>
    /// Records metrics for a single completed epoch.
    /// </summary>
    /// <param name="metrics">Metrics for the epoch.</param>
    void Log(EpochMetrics metrics);
}
