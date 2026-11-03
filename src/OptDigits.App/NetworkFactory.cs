using Backprop;
using OptDigits.Training;

namespace OptDigits.App;

// Creates NeuralNet instances seeded and configured from TrainingOptions.
public static class NetworkFactory
{
    // Seeds the RNG and creates a new NeuralNet with the given architecture and learning rates.
    public static NeuralNet Create(TrainingOptions options, int inputSize = 64, int outputSize = 10)
    {
        Rng.Seed(options.Seed);
        return new NeuralNet(inputSize, options.HiddenSize, outputSize, options.LearningRateOut, options.LearningRateIn);
    }
}
