namespace OptDigits.Data;

/// <summary>
/// Loads handwritten digit samples from CSV formatted dataset files.
/// </summary>
/// <remarks>Owner: Partner B</remarks>
public sealed class CsvDatasetLoader : IDatasetLoader
{
    /// <summary>
    /// Reads and parses an optdigits raw CSV file into a collection of <see cref="Sample"/> objects.
    /// Each line contains 64 pixel values (0-16) followed by the integer class label (0-9).
    /// </summary>
    /// <param name="path">Path to the optdigits data file.</param>
    /// <returns>A read-only list of parsed digit samples.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public IReadOnlyList<Sample> Load(string path)
    {
        // TODO 1: Verify file exists at path, throw FileNotFoundException if missing
        // TODO 2: Initialize a List<Sample> to hold parsed records
        // TODO 3: Read each line from file, skipping empty lines or whitespace
        // TODO 4: Split each line by commas and parse 65 integer tokens
        // TODO 5: Copy the first 64 tokens into a double[] Features array
        // TODO 6: Parse the 65th token as the integer Label (0-9)
        // TODO 7: Append new Sample(features, label) to the list and return as IReadOnlyList<Sample>
        throw new NotImplementedException("CsvDatasetLoader.Load is not implemented yet");
    }
}
