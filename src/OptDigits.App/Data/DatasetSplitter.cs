namespace OptDigits.Data;

/// <summary>
/// Splits a dataset into training and validation subsets using stratified sampling.
/// </summary>
/// <remarks>Owner: Partner B</remarks>
public static class DatasetSplitter
{
    /// <summary>
    /// Partitions a collection of samples into training and validation sets, preserving class distributions.
    /// </summary>
    /// <param name="samples">The full dataset to split.</param>
    /// <param name="validationFraction">Fraction of samples to assign to the validation set (e.g. 0.2 for 20%).</param>
    /// <param name="seed">Random seed for reproducible shuffling.</param>
    /// <returns>A tuple containing the stratified Train and Validation read-only sample lists.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public static (IReadOnlyList<Sample> Train, IReadOnlyList<Sample> Validation) Split(
        IReadOnlyList<Sample> samples,
        double validationFraction,
        int seed)
    {
        // TODO 1: Validate validationFraction is in (0.0, 1.0)
        // TODO 2: Group samples by their integer Label to maintain class proportions (stratification)
        // TODO 3: Initialize Random with the provided seed
        // TODO 4: For each class group:
        //           a. Shuffle group elements deterministically using Fisher-Yates or OrderBy(r => rand.Next())
        //           b. Calculate validation count = (int)Math.Round(group.Count * validationFraction)
        //           c. Take validation count elements into validation bucket, rest into train bucket
        // TODO 5: Shuffle final train and validation lists deterministically
        // TODO 6: Return (trainList, valList) tuple
        throw new NotImplementedException("DatasetSplitter.Split is not implemented yet");
    }
}
