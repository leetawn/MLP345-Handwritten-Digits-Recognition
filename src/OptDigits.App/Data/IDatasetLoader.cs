namespace OptDigits.Data;

// Defines the contract for loading digit samples from raw data files.
public interface IDatasetLoader
{
    // Loads digit samples from the specified file path.
    // path: Path to the raw dataset file.
    // Returns: A read-only list of parsed Sample records.
    IReadOnlyList<Sample> Load(string path);
}
