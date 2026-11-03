namespace OptDigits.Training;

// Hyperparameter options configuring network training.
// Epochs: Total number of training epochs to run.
// LearningRateOut: Learning rate for hidden-to-output layer connections and output biases.
// LearningRateIn: Learning rate for input-to-hidden layer connections and hidden biases.
// HiddenSize: Number of neurons in the hidden layer.
// Seed: Random seed for reproducible initializations.
// LogEvery: Interval in epochs between progress logging and loss evaluations.
public sealed record TrainingOptions(
    int Epochs,
    double LearningRateOut,
    double LearningRateIn,
    int HiddenSize,
    int Seed,
    int LogEvery);
