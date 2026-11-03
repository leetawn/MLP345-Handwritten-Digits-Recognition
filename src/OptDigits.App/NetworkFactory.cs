using Backprop;
using OptDigits.Training;

namespace OptDigits.App;

/// <summary>
/// Factory for initializing neural networks with deterministic seeds and configured hyperparameters.
/// </summary>
public static class NetworkFactory
{
    /// <summary>
    /// Creates a new <see cref="NeuralNet"/> instance, initializing the random seed and layer architectures.
    /// </summary>
    /// <param name="options">Training options containing hidden size, learning rates, and seed.</param>
    /// <param name="inputSize">Number of input features (default 64).</param>
    /// <param name="outputSize">Number of output classes (default 10).</param>
    /// <returns>A newly configured and seeded <see cref="NeuralNet"/> instance.</returns>
    public static NeuralNet Create(TrainingOptions options, int inputSize = 64, int outputSize = 10)
    {
        Rng.Seed(options.Seed);
        return new NeuralNet(inputSize, options.HiddenSize, outputSize, options.LearningRateOut, options.LearningRateIn);
    }
}
