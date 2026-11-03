namespace OptDigits.App;

// Resolves absolute paths to the repo root, dataset files, and output files.
public static class ProjectPaths
{
    private static readonly Lazy<string> _repoRoot = new(FindRepoRoot);

    // Absolute path to the repository root (located by finding the .sln file).
    public static string RepoRoot => _repoRoot.Value;

    // Absolute path to the raw training dataset (optdigits.tra).
    public static string TrainFile => Path.Combine(RepoRoot, "data", "raw", "optdigits.tra");

    // Absolute path to the raw held-out test dataset (optdigits.tes).
    public static string TestFile => Path.Combine(RepoRoot, "data", "raw", "optdigits.tes");

    // Absolute path to the results directory; creates it if missing.
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

    // Path to the training loss log CSV file.
    public static string TrainingLogCsv => Path.Combine(ResultsDir, "training_log.csv");

    // Path to the hyperparameter sweep results CSV file.
    public static string SweepCsv => Path.Combine(ResultsDir, "sweep.csv");

    // Path to the evaluation confusion matrix CSV file.
    public static string ConfusionMatrixCsv => Path.Combine(ResultsDir, "confusion_matrix.csv");

    // Path to the saved neural network weights file.
    public static string WeightsFile => Path.Combine(ResultsDir, "weights.txt");

    // Walks up from the executable directory until an .sln file is found.
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
