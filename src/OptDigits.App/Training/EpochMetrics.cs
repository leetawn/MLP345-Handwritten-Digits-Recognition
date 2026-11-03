namespace OptDigits.Training;

/// <summary>
/// Metrics evaluated at a particular training epoch.
/// </summary>
/// <param name="Epoch">The 1-based epoch number.</param>
/// <param name="TrainLoss">Mean squared error evaluated on the training set.</param>
/// <param name="ValidationLoss">Mean squared error evaluated on the validation set.</param>
/// <param name="ValidationAccuracy">Classification accuracy on the validation set (range [0, 1]).</param>
public sealed record EpochMetrics(
    int Epoch,
    double TrainLoss,
    double ValidationLoss,
    double ValidationAccuracy);
