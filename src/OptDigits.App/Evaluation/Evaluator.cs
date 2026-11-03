using Backprop;
using OptDigits.Data;

namespace OptDigits.Evaluation;

/// <summary>
/// Evaluates trained neural network models against labeled datasets.
/// </summary>
/// <remarks>Owner: Partner B</remarks>
public sealed class Evaluator
{
    /// <summary>
    /// Performs model inference over all samples, computing accuracy, confusion matrix, precision, recall, and F1 scores.
    /// </summary>
    /// <param name="net">The trained neural network model.</param>
    /// <param name="samples">The evaluation samples.</param>
    /// <returns>A comprehensive <see cref="EvaluationReport"/>.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public EvaluationReport Evaluate(NeuralNet net, IReadOnlyList<Sample> samples)
    {
        // TODO 1: Allocate int[10, 10] confusion matrix
        // TODO 2: For each sample in samples:
        //           a. Feed features to net: for (i=0; i<64; i++) net.setInputs(i, sample.Features[i]);
        //           b. Call predicted = net.Predict();
        //           c. Increment confusionMatrix[sample.Label, predicted]++;
        // TODO 3: Compute overall accuracy = sum of diagonal elements / total samples
        // TODO 4: For each digit class c from 0 to 9:
        //           a. truePositive = confusionMatrix[c, c]
        //           b. falsePositive = sum of column c excluding [c, c]
        //           c. falseNegative = sum of row c excluding [c, c]
        //           d. support = sum of row c
        //           e. precision = TP / (TP + FP), handling division by 0
        //           f. recall = TP / (TP + FN), handling division by 0
        //           g. f1 = (precision + recall > 0) ? (2 * precision * recall / (precision + recall)) : 0.0
        //           h. Add ClassMetrics(c, precision, recall, f1, support) to perClass list
        // TODO 5: Return new EvaluationReport(accuracy, perClass, confusionMatrix)
        throw new NotImplementedException("Evaluator.Evaluate is not implemented yet");
    }
}
