using System.Diagnostics;
using System.Globalization;
using Backprop;
using OptDigits.App;
using OptDigits.Data;
using OptDigits.Training;

namespace OptDigits.Experiments;

/// <summary>
/// Executes systematic hyperparameter sweeps over specified combinations of learning rates, hidden layer sizes, and epochs.
/// </summary>
/// <remarks>Owner: Partner A</remarks>
public sealed class ExperimentRunner
{
    /// <summary>
    /// Runs a grid search over hyperparameters, logging results incrementally to a CSV file and returning all row metrics.
    /// Header: lr_out,lr_in,hidden_size,epochs,train_loss,validation_loss,validation_accuracy,seconds
    /// </summary>
    /// <param name="grid">The grid specifying candidate learning rates, hidden sizes, and epoch counts.</param>
    /// <param name="train">Training samples.</param>
    /// <param name="validation">Validation samples.</param>
    /// <param name="baseOptions">Base training options supplying default seed and log intervals.</param>
    /// <param name="outputCsvPath">File path where sweep results CSV is saved.</param>
    /// <returns>A read-only list of <see cref="ExperimentRow"/> records containing metrics for every evaluated trial, ordered by loop iteration.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="grid"/>, <paramref name="train"/>, <paramref name="validation"/>, or <paramref name="outputCsvPath"/> is null.</exception>
    public IReadOnlyList<ExperimentRow> Run(
        ExperimentGrid grid,
        IReadOnlyList<Sample> train,
        IReadOnlyList<Sample> validation,
        TrainingOptions baseOptions,
        string outputCsvPath)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(train);
        ArgumentNullException.ThrowIfNull(validation);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputCsvPath);

        string? dir = Path.GetDirectoryName(outputCsvPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        int totalRuns = grid.LearningRates.Count * grid.HiddenSizes.Count * grid.EpochCounts.Count;
        var rows = new List<ExperimentRow>(totalRuns);

        using var writer = new StreamWriter(outputCsvPath, append: false);
        writer.WriteLine("lr_out,lr_in,hidden_size,epochs,train_loss,validation_loss,validation_accuracy,seconds");
        writer.Flush();

        int runIndex = 0;

        foreach (double lrOut in grid.LearningRates)
        {
            double lrIn = lrOut * 0.75;

            foreach (int hidden in grid.HiddenSizes)
            {
                foreach (int epochs in grid.EpochCounts)
                {
                    runIndex++;

                    var trialOptions = new TrainingOptions(
                        epochs,
                        lrOut,
                        lrIn,
                        hidden,
                        baseOptions.Seed,
                        baseOptions.LogEvery);

                    var net = NetworkFactory.Create(trialOptions);
                    var trainer = new Trainer();

                    var sw = Stopwatch.StartNew();
                    var result = trainer.Train(net, train, validation, trialOptions, logger: null);
                    sw.Stop();

                    double finalTrainLoss = result.FinalTrainLoss;
                    double finalValLoss = result.FinalValidationLoss;
                    double finalValAcc = result.FinalValidationAccuracy;

                    if (double.IsNaN(finalTrainLoss) || double.IsInfinity(finalTrainLoss) ||
                        double.IsNaN(finalValLoss) || double.IsInfinity(finalValLoss))
                    {
                        finalTrainLoss = double.NaN;
                        finalValLoss = double.NaN;
                        finalValAcc = 0.0;
                    }

                    var row = new ExperimentRow(
                        lrOut,
                        lrIn,
                        hidden,
                        epochs,
                        finalTrainLoss,
                        finalValLoss,
                        finalValAcc,
                        sw.Elapsed.TotalSeconds);

                    rows.Add(row);

                    string line = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0},{1},{2},{3},{4:F6},{5:F6},{6:F4},{7:F2}",
                        row.LearningRateOut,
                        row.LearningRateIn,
                        row.HiddenSize,
                        row.Epochs,
                        row.FinalTrainLoss,
                        row.FinalValidationLoss,
                        row.ValidationAccuracy,
                        row.Seconds);

                    writer.WriteLine(line);
                    writer.Flush();

                    Console.WriteLine(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "[{0,2}/{1}] lrOut={2:F2}, lrIn={3:F4}, hidden={4,2}, epochs={5,3} => val_acc={6,6:P2}, time={7,5:F2}s",
                            runIndex,
                            totalRuns,
                            lrOut,
                            lrIn,
                            hidden,
                            epochs,
                            finalValAcc,
                            sw.Elapsed.TotalSeconds));
                }
            }
        }

        return rows;
    }
}
