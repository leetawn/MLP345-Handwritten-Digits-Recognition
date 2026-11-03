using Backprop;
using OptDigits.Data;
using OptDigits.Evaluation;
using OptDigits.Experiments;
using OptDigits.Training;

namespace OptDigits.App;

// CLI entry point supporting train, sweep, and evaluate commands.
public static class Program
{
    // Parses the first argument as a command and dispatches to the appropriate handler.
    // Returns 0 for success, 1 for error, 2 for NotImplementedException.
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
            {
                PrintUsage();
                return 0;
            }

            string command = args[0].ToLowerInvariant();
            string[] commandArgs = args.Skip(1).ToArray();

            return command switch
            {
                "train" => RunTrain(commandArgs),
                "sweep" => RunSweep(commandArgs),
                "evaluate" => RunEvaluate(commandArgs),
                _ => HandleUnknownCommand(command)
            };
        }
        catch (NotImplementedException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static int RunTrain(string[] args)
    {
        var config = new AppConfig();
        int hidden = config.HiddenSize;
        int epochs = config.Epochs;
        double lrOut = config.LearningRateOut;
        double lrIn = config.LearningRateIn;
        int seed = config.Seed;
        int logEvery = config.LogEvery;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--hidden" when i + 1 < args.Length:
                    hidden = int.Parse(args[++i]);
                    break;
                case "--epochs" when i + 1 < args.Length:
                    epochs = int.Parse(args[++i]);
                    break;
                case "--lr-out" when i + 1 < args.Length:
                    lrOut = double.Parse(args[++i]);
                    break;
                case "--lr-in" when i + 1 < args.Length:
                    lrIn = double.Parse(args[++i]);
                    break;
                case "--seed" when i + 1 < args.Length:
                    seed = int.Parse(args[++i]);
                    break;
                case "--log-every" when i + 1 < args.Length:
                    logEvery = int.Parse(args[++i]);
                    break;
                default:
                    Console.WriteLine($"Unknown or incomplete argument: {args[i]}");
                    break;
            }
        }

        Console.WriteLine("=== OptDigits Handwritten Digit Recognition ===");
        Console.WriteLine($"Configuration: Hidden={hidden}, Epochs={epochs}, LrOut={lrOut}, LrIn={lrIn}, Seed={seed}, LogEvery={logEvery}");

        var (train, val, test) = PrepareData(config);

        var options = new TrainingOptions(epochs, lrOut, lrIn, hidden, seed, logEvery);
        var net = NetworkFactory.Create(options, config.InputSize, config.ClassCount);

        using (var logger = new CsvLossLogger(ProjectPaths.TrainingLogCsv))
        {
            var trainer = new Trainer();
            var result = trainer.Train(net, train, val, options, logger);
            Console.WriteLine($"Training completed in {result.Duration.TotalSeconds:F2}s");
        }

        Console.WriteLine("\nEvaluating on held-out test set...");
        var evaluator = new Evaluator();
        var report = evaluator.Evaluate(net, test);

        Console.WriteLine(report.ToConsoleString());
        report.SaveConfusionMatrixCsv(ProjectPaths.ConfusionMatrixCsv);
        Console.WriteLine($"Confusion matrix saved to: {ProjectPaths.ConfusionMatrixCsv}");

        net.saveWeights(ProjectPaths.WeightsFile);
        Console.WriteLine($"Trained weights saved to: {ProjectPaths.WeightsFile}");

        return 0;
    }

    private static int RunSweep(string[] args)
    {
        var config = new AppConfig();
        Console.WriteLine("=== Running Hyperparameter Grid Sweep ===");

        var (train, val, _) = PrepareData(config);

        var baseOptions = new TrainingOptions(
            config.Epochs,
            config.LearningRateOut,
            config.LearningRateIn,
            config.HiddenSize,
            config.Seed,
            config.LogEvery);

        var runner = new ExperimentRunner();
        var results = runner.Run(ExperimentGrid.Default, train, val, baseOptions, ProjectPaths.SweepCsv);

        Console.WriteLine($"\nSweep complete. Results written to: {ProjectPaths.SweepCsv}");
        Console.WriteLine("\nTop 5 Configurations by Validation Accuracy:");
        Console.WriteLine("Rank | LrOut | LrIn  | Hidden | Epochs | Val Acc  | Val Loss | Time (s)");
        Console.WriteLine("------------------------------------------------------------------");

        var top5 = results.OrderByDescending(r => r.ValidationAccuracy).Take(5).ToList();
        for (int i = 0; i < top5.Count; i++)
        {
            var r = top5[i];
            Console.WriteLine($"{i + 1,4} | {r.LearningRateOut,5:F2} | {r.LearningRateIn,5:F3} | {r.HiddenSize,6} | {r.Epochs,6} | {r.ValidationAccuracy * 100,6:F2}% | {r.FinalValidationLoss,8:F6} | {r.Seconds,8:F2}");
        }

        return 0;
    }

    private static int RunEvaluate(string[] args)
    {
        var config = new AppConfig();
        if (!File.Exists(ProjectPaths.WeightsFile))
        {
            Console.Error.WriteLine($"Weights file not found at: {ProjectPaths.WeightsFile}. Run 'train' first.");
            return 1;
        }

        if (!File.Exists(ProjectPaths.TestFile))
        {
            Console.Error.WriteLine($"Test dataset not found at: {ProjectPaths.TestFile}. Ensure optdigits.tes is placed in data/raw/.");
            return 1;
        }

        Console.WriteLine("=== Evaluating Model On Held-Out Test Set ===");
        IDatasetLoader loader = new CsvDatasetLoader();
        var rawTest = loader.Load(ProjectPaths.TestFile);

        var scaler = new MinMaxScaler(0.0, config.PixelMax);
        scaler.Fit(rawTest);
        var testSet = scaler.TransformAll(rawTest);

        var net = new NeuralNet(config.InputSize, config.HiddenSize, config.ClassCount);
        net.loadWeights(ProjectPaths.WeightsFile);

        var evaluator = new Evaluator();
        var report = evaluator.Evaluate(net, testSet);

        Console.WriteLine(report.ToConsoleString());
        return 0;
    }

    private static (IReadOnlyList<Sample> Train, IReadOnlyList<Sample> Validation, IReadOnlyList<Sample> Test) PrepareData(AppConfig config)
    {
        IDatasetLoader loader = new CsvDatasetLoader();
        var rawTrain = loader.Load(ProjectPaths.TrainFile);
        var rawTest = loader.Load(ProjectPaths.TestFile);

        // Fit scaler on training samples only
        var scaler = new MinMaxScaler(0.0, config.PixelMax);
        scaler.Fit(rawTrain);

        var scaledTrain = scaler.TransformAll(rawTrain);
        var scaledTest = scaler.TransformAll(rawTest);

        var (train, val) = DatasetSplitter.Split(scaledTrain, config.ValidationFraction, config.Seed);

        return (train, val, scaledTest);
    }

    private static int HandleUnknownCommand(string command)
    {
        Console.Error.WriteLine($"Unknown command '{command}'.");
        PrintUsage();
        return 1;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage: dotnet run --project src/OptDigits.App -- <command> [options]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  train      Train the MLP classifier on optdigits dataset and evaluate on test set");
        Console.WriteLine("  sweep      Execute grid search over hyperparameter combinations");
        Console.WriteLine("  evaluate   Load saved weights (results/weights.txt) and evaluate on test set");
        Console.WriteLine("  --help     Show this usage information");
        Console.WriteLine();
        Console.WriteLine("Train Options:");
        Console.WriteLine("  --hidden <N>       Number of hidden layer neurons (default: 32)");
        Console.WriteLine("  --epochs <N>       Number of training epochs (default: 50)");
        Console.WriteLine("  --lr-out <X>       Hidden-to-output learning rate (default: 0.5)");
        Console.WriteLine("  --lr-in <X>        Input-to-hidden learning rate (default: 0.375)");
        Console.WriteLine("  --seed <N>         Random seed for weight init and splitting (default: 42)");
        Console.WriteLine("  --log-every <N>    Epoch logging interval (default: 5)");
    }
}
