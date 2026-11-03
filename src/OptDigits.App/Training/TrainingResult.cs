namespace OptDigits.Training;

/// <summary>
/// Summary results of a completed model training session.
/// </summary>
/// <param name="History">Collection of epoch-by-epoch evaluation metrics.</param>
/// <param name="FinalTrainLoss">Final training loss achieved.</param>
/// <param name="FinalValidationLoss">Final validation loss achieved.</param>
/// <param name="FinalValidationAccuracy">Final validation accuracy achieved.</param>
/// <param name="Duration">Total wall-clock duration of training.</param>
public sealed record TrainingResult(
    IReadOnlyList<EpochMetrics> History,
    double FinalTrainLoss,
    double FinalValidationLoss,
    double FinalValidationAccuracy,
    TimeSpan Duration);
