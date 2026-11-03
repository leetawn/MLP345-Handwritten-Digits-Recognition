namespace OptDigits.Training;

/// <summary>
/// Hyperparameter options configuring network training.
/// </summary>
/// <param name="Epochs">Total number of training epochs to run.</param>
/// <param name="LearningRateOut">Learning rate for hidden-to-output layer connections and output biases.</param>
/// <param name="LearningRateIn">Learning rate for input-to-hidden layer connections and hidden biases.</param>
/// <param name="HiddenSize">Number of neurons in the hidden layer.</param>
/// <param name="Seed">Random seed for reproducible initializations.</param>
/// <param name="LogEvery">Interval in epochs between progress logging and loss evaluations.</param>
public sealed record TrainingOptions(
    int Epochs,
    double LearningRateOut,
    double LearningRateIn,
    int HiddenSize,
    int Seed,
    int LogEvery);
