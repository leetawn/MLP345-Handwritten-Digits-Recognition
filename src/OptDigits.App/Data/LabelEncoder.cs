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
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public static double[] OneHot(int label, int classCount)
    {
        // TODO 1: Validate that label is between 0 and classCount - 1
        // TODO 2: Allocate a double[] of length classCount initialized to 0.0
        // TODO 3: Assign 1.0 to index [label]
        // TODO 4: Return the one-hot array
        throw new NotImplementedException("LabelEncoder.OneHot is not implemented yet");
    }

    /// <summary>
    /// Finds the index of the maximum value in a list of numeric values (argmax).
    /// </summary>
    /// <param name="values">Collection of numeric values (e.g. neuron activation scores).</param>
    /// <returns>Zero-based index of the maximum element.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public static int ArgMax(IReadOnlyList<double> values)
    {
        // TODO 1: Check that values is not empty; throw ArgumentException if empty
        // TODO 2: Track maxIndex = 0 and maxValue = values[0]
        // TODO 3: Iterate through values from index 1 to values.Count - 1
        // TODO 4: If values[i] > maxValue, set maxValue = values[i] and maxIndex = i
        // TODO 5: Return maxIndex
        throw new NotImplementedException("LabelEncoder.ArgMax is not implemented yet");
    }
}
