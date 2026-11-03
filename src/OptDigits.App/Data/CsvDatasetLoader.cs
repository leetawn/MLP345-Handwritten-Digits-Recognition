using System.Globalization;

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
    /// <exception cref="FileNotFoundException">Thrown when the dataset file does not exist.</exception>
    /// <exception cref="FormatException">Thrown when a row contains invalid data.</exception>
    public IReadOnlyList<Sample> Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                $"Dataset file '{path}' was not found. Run tools/download-data.sh or tools/download-data.ps1 first.",
                path);
        }

        var samples = new List<Sample>();
        int lineNumber = 0;
        foreach (string line in File.ReadLines(path))
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] tokens = line.Split(',');
            if (tokens.Length != 65)
            {
                throw new FormatException($"Line {lineNumber} must contain 65 comma-separated values, but contains {tokens.Length}.");
            }

            var features = new double[64];
            for (int featureIndex = 0; featureIndex < features.Length; featureIndex++)
            {
                int value = ParseInteger(tokens[featureIndex], lineNumber, featureIndex, 0, 16);
                features[featureIndex] = value;
            }

            int label = ParseInteger(tokens[64], lineNumber, 64, 0, 9);
            samples.Add(new Sample(features, label));
        }

        return samples;
    }

    private static int ParseInteger(string token, int lineNumber, int columnIndex, int minimum, int maximum)
    {
        string trimmedToken = token.Trim();
        if (!int.TryParse(trimmedToken, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
        {
            throw new FormatException($"Line {lineNumber}, column {columnIndex + 1} contains a non-numeric value: '{trimmedToken}'.");
        }

        if (value < minimum || value > maximum)
        {
            string valueType = columnIndex == 64 ? "label" : "feature";
            throw new FormatException($"Line {lineNumber}, column {columnIndex + 1} has {valueType} value {value}; expected {minimum}..{maximum}.");
        }

        return value;
    }
}
