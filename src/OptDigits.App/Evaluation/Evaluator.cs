using Backprop;
using OptDigits.App;
using OptDigits.Data;

namespace OptDigits.Evaluation;

// Evaluates trained neural network models against labeled datasets.
public sealed class Evaluator
{
    // Performs model inference over all samples, computing accuracy, confusion matrix, precision, recall, and F1 scores.
    // net: The trained neural network model.
    // samples: The evaluation samples.
    // Returns: A comprehensive EvaluationReport.
    // Throws ArgumentException: Thrown when the sample list is empty.
    public EvaluationReport Evaluate(NeuralNet net, IReadOnlyList<Sample> samples)
    {
        ArgumentNullException.ThrowIfNull(net);
        ArgumentNullException.ThrowIfNull(samples);
        if (samples.Count == 0)
        {
            throw new ArgumentException("At least one sample is required for evaluation.", nameof(samples));
        }

        int classCount = new AppConfig().ClassCount;
        var confusionMatrix = new int[classCount, classCount];
        foreach (Sample sample in samples)
        {
            for (int featureIndex = 0; featureIndex < sample.Features.Length; featureIndex++)
            {
                net.setInputs(featureIndex, sample.Features[featureIndex]);
            }

            net.run();
            int predicted = net.Predict();
            confusionMatrix[sample.Label, predicted]++;
        }

        var perClass = new List<ClassMetrics>(classCount);
        int correct = 0;
        for (int classIndex = 0; classIndex < classCount; classIndex++)
        {
            correct += confusionMatrix[classIndex, classIndex];
            int truePositive = confusionMatrix[classIndex, classIndex];
            int falsePositive = 0;
            int falseNegative = 0;
            for (int otherClass = 0; otherClass < classCount; otherClass++)
            {
                if (otherClass != classIndex)
                {
                    falsePositive += confusionMatrix[otherClass, classIndex];
                    falseNegative += confusionMatrix[classIndex, otherClass];
                }
            }

            int support = truePositive + falseNegative;
            double precision = DivideOrZero(truePositive, truePositive + falsePositive);
            double recall = DivideOrZero(truePositive, support);
            double f1 = DivideOrZero(2.0 * precision * recall, precision + recall);
            perClass.Add(new ClassMetrics(classIndex, precision, recall, f1, support));
        }

        return new EvaluationReport((double)correct / samples.Count, perClass, confusionMatrix);
    }

    private static double DivideOrZero(double numerator, double denominator)
    {
        return denominator == 0.0 ? 0.0 : numerator / denominator;
    }
}
