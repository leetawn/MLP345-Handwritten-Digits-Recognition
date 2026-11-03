namespace OptDigits.Data;

/// <summary>
/// Defines the contract for loading digit samples from raw data files.
/// </summary>
public interface IDatasetLoader
{
    /// <summary>
    /// Loads digit samples from the specified file path.
    /// </summary>
    /// <param name="path">Path to the raw dataset file.</param>
    /// <returns>A read-only list of parsed <see cref="Sample"/> records.</returns>
    IReadOnlyList<Sample> Load(string path);
}
