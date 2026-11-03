namespace OptDigits.App;

// Global application configuration with default hyperparameters for training and evaluation.
// InputSize: number of input features (64 pixel blocks per sample).
// ClassCount: number of output classes (digits 0–9).
// PixelMax: raw optdigits pixel maximum (16), used for min-max scaling.
// HiddenSize: default number of hidden layer neurons.
// Epochs: default training epoch count.
// LearningRateOut: default learning rate for hidden-to-output connections.
// LearningRateIn: default learning rate for input-to-hidden connections.
// Seed: default random seed for reproducible initialization and splitting.
// LogEvery: epoch interval between logged loss evaluations.
// ValidationFraction: fraction of training samples held out for validation.
public sealed record AppConfig(
    int InputSize = 64,
    int ClassCount = 10,
    double PixelMax = 16.0,
    int HiddenSize = 32,
    int Epochs = 50,
    double LearningRateOut = 0.5,
    double LearningRateIn = 0.375,
    int Seed = 42,
    int LogEvery = 5,
    double ValidationFraction = 0.2);
