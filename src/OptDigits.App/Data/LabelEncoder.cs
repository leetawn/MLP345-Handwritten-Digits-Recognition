namespace OptDigits.Data;

// Provides utility methods for one-hot encoding integer labels and computing argmax class indices.
public static class LabelEncoder
{
    // Converts a categorical integer class label into a one-hot vector representation of length classCount.
    // label: The integer class index (e.g. 0 to 9).
    // classCount: Total number of classes.
    // Returns: A double array with 1.0 at index label and 0.0 elsewhere.
    // Throws ArgumentOutOfRangeException: Thrown when the label is not a valid class index.
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

    // Finds the index of the maximum value in a list of numeric values (argmax).
    // values: Collection of numeric values (e.g. neuron activation scores).
    // Returns: Zero-based index of the maximum element.
    // Throws ArgumentException: Thrown when values is empty.
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
