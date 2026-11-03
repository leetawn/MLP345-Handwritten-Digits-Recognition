namespace OptDigits.Data;

/// <summary>
/// Provides utility methods for one-hot encoding integer labels and computing argmax class indices.
/// </summary>
/// <remarks>Owner: Partner B</remarks>
public static class LabelEncoder
{
    /// <summary>
    /// Converts a categorical integer class label into a one-hot vector representation of length <paramref name="classCount"/>.
    /// </summary>
    /// <param name="label">The integer class index (e.g. 0 to 9).</param>
    /// <param name="classCount">Total number of classes.</param>
    /// <returns>A double array with 1.0 at index <paramref name="label"/> and 0.0 elsewhere.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the label is not a valid class index.</exception>
    public static double[] OneHot(int label, int classCount)
    {
        if (label < 0 || label >= classCount)
        {
            throw new ArgumentOutOfRangeException(nameof(label), label, $"Label must be in the range [0, {classCount}).");
        }

        var encoded = new double[classCount];
        encoded[label] = 1.0;
        return encoded;
    }

    /// <summary>
    /// Finds the index of the maximum value in a list of numeric values (argmax).
    /// </summary>
    /// <param name="values">Collection of numeric values (e.g. neuron activation scores).</param>
    /// <returns>Zero-based index of the maximum element.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> is empty.</exception>
    public static int ArgMax(IReadOnlyList<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Count == 0)
        {
            throw new ArgumentException("At least one value is required.", nameof(values));
        }

        int maxIndex = 0;
        double maxValue = values[0];
        for (int index = 1; index < values.Count; index++)
        {
            if (values[index] > maxValue)
            {
                maxValue = values[index];
                maxIndex = index;
            }
        }

        return maxIndex;
    }
}
