namespace OptDigits.Experiments;

/// <summary>
/// Records results and execution time of a single hyperparameter configuration trial.
/// </summary>
/// <param name="LearningRateOut">Output layer learning rate.</param>
/// <param name="LearningRateIn">Input layer learning rate.</param>
/// <param name="HiddenSize">Number of hidden neurons.</param>
/// <param name="Epochs">Epochs trained.</param>
/// <param name="FinalTrainLoss">Final mean squared error on training data.</param>
/// <param name="FinalValidationLoss">Final mean squared error on validation data.</param>
/// <param name="ValidationAccuracy">Final classification accuracy on validation data.</param>
/// <param name="Seconds">Execution duration in seconds.</param>
public sealed record ExperimentRow(
    double LearningRateOut,
    double LearningRateIn,
    int HiddenSize,
    int Epochs,
    double FinalTrainLoss,
    double FinalValidationLoss,
    double ValidationAccuracy,
    double Seconds);
