namespace OptDigits.Data;

// Splits a dataset into training and validation subsets using stratified sampling.
public static class DatasetSplitter
{
    // Partitions a collection of samples into training and validation sets, preserving class distributions.
    // samples: The full dataset to split.
    // validationFraction: Fraction of samples to assign to the validation set (e.g. 0.2 for 20%).
    // seed: Random seed for reproducible shuffling.
    // Returns: A tuple containing the stratified Train and Validation read-only sample lists.
    // Throws ArgumentOutOfRangeException: Thrown when validationFraction is not between 0 and 1.
    public static (IReadOnlyList<Sample> Train, IReadOnlyList<Sample> Validation) Split(
        IReadOnlyList<Sample> samples,
        double validationFraction,
        int seed)
    {
        if (validationFraction <= 0.0 || validationFraction >= 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(validationFraction), validationFraction, "Validation fraction must be between 0 and 1.");
        }

        ArgumentNullException.ThrowIfNull(samples);
        var random = new Random(seed);
        var train = new List<Sample>(samples.Count);
        var validation = new List<Sample>();

        foreach (IGrouping<int, Sample> classGroup in samples.GroupBy(sample => sample.Label))
        {
            var shuffledClass = classGroup.ToList();
            Shuffle(shuffledClass, random);

            int validationCount = (int)Math.Round(shuffledClass.Count * validationFraction);
            if (shuffledClass.Count >= 2)
            {
                validationCount = Math.Max(1, validationCount);
            }

            validation.AddRange(shuffledClass.Take(validationCount));
            train.AddRange(shuffledClass.Skip(validationCount));
        }

        Shuffle(train, random);
        Shuffle(validation, random);
        return (train, validation);
    }

    private static void Shuffle<T>(IList<T> values, Random random)
    {
        for (int index = values.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }
    }
}
