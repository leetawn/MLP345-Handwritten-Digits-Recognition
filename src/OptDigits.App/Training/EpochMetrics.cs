namespace OptDigits.Training;

// Metrics evaluated at a particular training epoch.
// Epoch: The 1-based epoch number.
// TrainLoss: Mean squared error evaluated on the training set.
// ValidationLoss: Mean squared error evaluated on the validation set.
// ValidationAccuracy: Classification accuracy on the validation set (range [0, 1]).
public sealed record EpochMetrics(
    int Epoch,
    double TrainLoss,
    double ValidationLoss,
    double ValidationAccuracy);
