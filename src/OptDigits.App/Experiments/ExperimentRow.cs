namespace OptDigits.Experiments;

// Records results and execution time of a single hyperparameter configuration trial.
// LearningRateOut: Output layer learning rate.
// LearningRateIn: Input layer learning rate.
// HiddenSize: Number of hidden neurons.
// Epochs: Epochs trained.
// FinalTrainLoss: Final mean squared error on training data.
// FinalValidationLoss: Final mean squared error on validation data.
// ValidationAccuracy: Final classification accuracy on validation data.
// Seconds: Execution duration in seconds.
public sealed record ExperimentRow(
    double LearningRateOut,
    double LearningRateIn,
    int HiddenSize,
    int Epochs,
    double FinalTrainLoss,
    double FinalValidationLoss,
    double ValidationAccuracy,
    double Seconds);
