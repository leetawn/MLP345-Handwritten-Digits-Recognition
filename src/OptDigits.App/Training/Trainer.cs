using System.Diagnostics;
using Backprop;
using OptDigits.Data;

namespace OptDigits.Training;

/// <summary>
/// Executes neural network training loops using stochastic gradient descent and evaluates mean squared error loss over datasets.
/// </summary>
/// <remarks>Owner: Partner A</remarks>
public sealed class Trainer
{
    private const int ClassCount = 10;

    /// <summary>
    /// Trains a <see cref="NeuralNet"/> on training samples across specified epochs using online SGD.
    /// Evaluates metrics every epoch and records them into history.
    /// Note: <see cref="TrainingOptions.LogEvery"/> only controls how frequently <paramref name="logger"/> is called
    /// (epoch 1, every LogEvery-th epoch, and the final epoch), whereas history contains an entry for every epoch.
    /// If losses diverge to NaN or Infinity, training stops early and returns the history accumulated so far.
    /// </summary>
    /// <param name="net">The neural network to train.</param>
    /// <param name="train">Training samples.</param>
    /// <param name="validation">Validation samples used for progress monitoring and model tuning.</param>
    /// <param name="options">Training configuration parameters.</param>
    /// <param name="logger">Optional loss logger for writing epoch metrics.</param>
    /// <returns>A <see cref="TrainingResult"/> containing training history and final performance metrics.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="net"/>, <paramref name="train"/>, or <paramref name="validation"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="options.Epochs"/> &lt; 1, <paramref name="train"/> is empty, or <paramref name="options.LogEvery"/> &lt; 1.</exception>
    public TrainingResult Train(
        NeuralNet net,
        IReadOnlyList<Sample> train,
        IReadOnlyList<Sample> validation,
        TrainingOptions options,
        ILossLogger? logger)
    {
        ArgumentNullException.ThrowIfNull(net);
        ArgumentNullException.ThrowIfNull(train);
        ArgumentNullException.ThrowIfNull(validation);

        if (options.Epochs < 1)
        {
            throw new ArgumentException("Total training epochs must be at least 1.", nameof(options));
        }

        if (train.Count == 0)
        {
            throw new ArgumentException("Training dataset must contain at least one sample.", nameof(train));
        }

        if (options.LogEvery < 1)
        {
            throw new ArgumentException("LogEvery interval must be at least 1.", nameof(options));
        }

        var rng = new Random(options.Seed);
        int[] indices = new int[train.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        var history = new List<EpochMetrics>(options.Epochs);
        var stopwatch = Stopwatch.StartNew();

        for (int epoch = 1; epoch <= options.Epochs; epoch++)
        {
            ShuffleIndices(indices, rng);

            for (int i = 0; i < indices.Length; i++)
            {
                var sample = train[indices[i]];
                FeedSample(net, sample);

                double[] target = LabelEncoder.OneHot(sample.Label, ClassCount);
                for (int k = 0; k < ClassCount; k++)
                {
                    net.setDesiredOutput(k, target[k]);
                }

                net.learn();
            }

            double trainLoss = ComputeLoss(net, train);
            double valLoss = validation.Count > 0 ? ComputeLoss(net, validation) : 0.0;
            double valAcc = validation.Count > 0 ? (double)CountCorrect(net, validation) / validation.Count : 0.0;

            var metrics = new EpochMetrics(epoch, trainLoss, valLoss, valAcc);
            history.Add(metrics);

            bool shouldLog = (epoch == 1) || (epoch % options.LogEvery == 0) || (epoch == options.Epochs);
            if (shouldLog)
            {
                logger?.Log(metrics);
            }

            // Early termination on divergence
            if (double.IsNaN(trainLoss) || double.IsInfinity(trainLoss) ||
                double.IsNaN(valLoss) || double.IsInfinity(valLoss))
            {
                if (!shouldLog)
                {
                    logger?.Log(metrics);
                }
                break;
            }
        }

        stopwatch.Stop();

        EpochMetrics finalMetrics = history[^1];
        return new TrainingResult(
            history,
            finalMetrics.TrainLoss,
            finalMetrics.ValidationLoss,
            finalMetrics.ValidationAccuracy,
            stopwatch.Elapsed);
    }

    /// <summary>
    /// Calculates the mean squared error (MSE) loss across all samples and output classes.
    /// Formula: MSE = (1 / (N * K)) * sum_i sum_k (target_k - output_k)^2.
    /// Runs forward inference only without updating weights.
    /// </summary>
    /// <param name="net">The neural network to evaluate.</param>
    /// <param name="samples">The samples to compute loss over.</param>
    /// <returns>Mean squared error loss across all samples and output units.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="net"/> or <paramref name="samples"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="samples"/> is empty.</exception>
    public static double ComputeLoss(NeuralNet net, IReadOnlyList<Sample> samples)
    {
        ArgumentNullException.ThrowIfNull(net);
        ArgumentNullException.ThrowIfNull(samples);

        if (samples.Count == 0)
        {
            throw new ArgumentException("Samples collection must not be empty.", nameof(samples));
        }

        double totalSquaredError = 0.0;

        for (int s = 0; s < samples.Count; s++)
        {
            var sample = samples[s];
            FeedSample(net, sample);
            net.run();

            double[] target = LabelEncoder.OneHot(sample.Label, ClassCount);
            for (int k = 0; k < ClassCount; k++)
            {
                double diff = target[k] - net.getOuputData(k);
                totalSquaredError += diff * diff;
            }
        }

        return totalSquaredError / (samples.Count * ClassCount);
    }

    private static void FeedSample(NeuralNet net, Sample sample)
    {
        for (int i = 0; i < sample.Features.Length; i++)
        {
            net.setInputs(i, sample.Features[i]);
        }
    }

    private static int CountCorrect(NeuralNet net, IReadOnlyList<Sample> samples)
    {
        int correct = 0;
        for (int s = 0; s < samples.Count; s++)
        {
            var sample = samples[s];
            FeedSample(net, sample);
            net.run();
            int predicted = net.Predict();
            if (predicted == sample.Label)
            {
                correct++;
            }
        }
        return correct;
    }

    private static void ShuffleIndices(int[] array, Random rng)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
