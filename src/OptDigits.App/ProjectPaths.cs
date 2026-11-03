namespace OptDigits.App;

/// <summary>
/// Provides absolute paths to repository root, dataset files, and results output directory.
/// </summary>
public static class ProjectPaths
{
    private static readonly Lazy<string> _repoRoot = new(FindRepoRoot);

    /// <summary>
    /// Gets the absolute path to the repository root by locating the solution file.
    /// </summary>
    public static string RepoRoot => _repoRoot.Value;

    /// <summary>
    /// Gets the absolute path to the raw training dataset (optdigits.tra).
    /// </summary>
    public static string TrainFile => Path.Combine(RepoRoot, "data", "raw", "optdigits.tra");

    /// <summary>
    /// Gets the absolute path to the raw held-out test dataset (optdigits.tes).
    /// </summary>
    public static string TestFile => Path.Combine(RepoRoot, "data", "raw", "optdigits.tes");

    /// <summary>
    /// Gets the absolute path to the results directory, ensuring it is created if missing.
    /// </summary>
    public static string ResultsDir
    {
        get
        {
            string dir = Path.Combine(RepoRoot, "results");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }
    }

    /// <summary>
    /// Gets the path to the training loss log CSV file.
    /// </summary>
    public static string TrainingLogCsv => Path.Combine(ResultsDir, "training_log.csv");

    /// <summary>
    /// Gets the path to the hyperparameter sweep results CSV file.
    /// </summary>
    public static string SweepCsv => Path.Combine(ResultsDir, "sweep.csv");

    /// <summary>
    /// Gets the path to the evaluation confusion matrix CSV file.
    /// </summary>
    public static string ConfusionMatrixCsv => Path.Combine(ResultsDir, "confusion_matrix.csv");

    /// <summary>
    /// Gets the path to the saved neural network weights file.
    /// </summary>
    public static string WeightsFile => Path.Combine(ResultsDir, "weights.txt");

    private static string FindRepoRoot()
    {
        string? current = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "OptDigitsMlp.sln")) ||
                Directory.EnumerateFiles(current, "*.sln").Any())
            {
                return current;
            }

            string? parent = Directory.GetParent(current)?.FullName;
            if (parent == current) break;
            current = parent;
        }

        return Directory.GetCurrentDirectory();
    }
}
