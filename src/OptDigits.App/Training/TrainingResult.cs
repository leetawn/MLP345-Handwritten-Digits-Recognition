namespace OptDigits.Training;

// Summary results of a completed model training session.
// History: Collection of epoch-by-epoch evaluation metrics.
// FinalTrainLoss: Final training loss achieved.
// FinalValidationLoss: Final validation loss achieved.
// FinalValidationAccuracy: Final validation accuracy achieved.
// Duration: Total wall-clock duration of training.
public sealed record TrainingResult(
    IReadOnlyList<EpochMetrics> History,
    double FinalTrainLoss,
    double FinalValidationLoss,
    double FinalValidationAccuracy,
    TimeSpan Duration);
