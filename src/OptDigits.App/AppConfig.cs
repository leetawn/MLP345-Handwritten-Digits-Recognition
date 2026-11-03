namespace OptDigits.App;

/// <summary>
/// Global application configuration settings and default hyperparameters.
/// </summary>
/// <param name="InputSize">Number of input features per sample (64 pixels).</param>
/// <param name="ClassCount">Number of target classes (digits 0 to 9).</param>
/// <param name="PixelMax">Maximum pixel value in raw optdigits data (16).</param>
/// <param name="HiddenSize">Default number of hidden layer neurons.</param>
/// <param name="Epochs">Default number of training epochs.</param>
/// <param name="LearningRateOut">Default output layer learning rate.</param>
/// <param name="LearningRateIn">Default input-to-hidden layer learning rate.</param>
/// <param name="Seed">Default random seed for reproducible weight initialization and splitting.</param>
/// <param name="LogEvery">Interval in epochs between progress logging and loss evaluations.</param>
/// <param name="ValidationFraction">Fraction of training samples reserved for validation.</param>
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
