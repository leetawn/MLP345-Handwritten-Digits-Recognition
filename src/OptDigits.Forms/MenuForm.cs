using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Backprop;
using OptDigits.App;
using OptDigits.Data;
using OptDigits.Evaluation;
using OptDigits.Training;

namespace OptDigits.Forms;

/// <summary>
/// Main Menu form allowing users to either launch the drawing canvas with a pre-trained model
/// or train a custom MLP with real-time epoch metrics and visual progress.
/// </summary>
public sealed class MenuForm : Form
{
    private readonly Label _lblPretrainedStatus;
    private readonly Button _btnLaunchPretrained;

    private readonly NumericUpDown _numEpochs;
    private readonly NumericUpDown _numHidden;
    private readonly NumericUpDown _numLrOut;
    private readonly NumericUpDown _numLrIn;
    private readonly NumericUpDown _numSeed;
    private readonly Button _btnStartTrain;
    private readonly Button _btnResetDefaults;
    private readonly Button _btnLaunchTrained;

    private readonly ProgressBar _prgTraining;
    private readonly Label _lblTrainingStatus;
    private readonly TextBox _txtLog;

    private NeuralNet? _trainedNet;
    private bool _isTraining;

    /// <summary>Initializes a new instance of <see cref="MenuForm"/>.</summary>
    public MenuForm()
    {
        Text = "OptDigits MLP - Main Menu";
        ClientSize = new Size(820, 620);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9f);

        // ── HEADER ────────────────────────────────────────────────────────────
        var lblTitle = new Label
        {
            Text = "OptDigits MLP Handwritten Digit Classifier",
            Font = new Font("Segoe UI", 14f, FontStyle.Bold),
            Location = new Point(20, 14),
            AutoSize = true
        };
        Controls.Add(lblTitle);

        var lblSubtitle = new Label
        {
            Text = "3-layer neural network with online backpropagation on UCI 8x8 handwritten digit features.",
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(80, 80, 80),
            Location = new Point(22, 42),
            AutoSize = true
        };
        Controls.Add(lblSubtitle);

        // ── OPTION 1: PRE-TRAINED MODEL ───────────────────────────────────────
        var grpPretrained = new GroupBox
        {
            Text = " Option 1: Use Pre-Trained Model ",
            Location = new Point(20, 68),
            Size = new Size(780, 92)
        };

        _lblPretrainedStatus = new Label
        {
            Location = new Point(16, 26),
            Size = new Size(748, 20),
            Font = new Font("Segoe UI", 9f)
        };
        grpPretrained.Controls.Add(_lblPretrainedStatus);

        _btnLaunchPretrained = new Button
        {
            Text = "Launch Drawing Canvas (Pre-Trained)",
            Location = new Point(16, 48),
            Size = new Size(260, 32),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            UseVisualStyleBackColor = true
        };
        _btnLaunchPretrained.Click += (_, _) => LaunchCanvas(usePretrained: true);
        grpPretrained.Controls.Add(_btnLaunchPretrained);

        Controls.Add(grpPretrained);

        // ── OPTION 2: TRAIN CUSTOM MODEL ──────────────────────────────────────
        var grpTrain = new GroupBox
        {
            Text = " Option 2: Train Model & Live Epoch Metrics ",
            Location = new Point(20, 168),
            Size = new Size(780, 436)
        };

        // Left Panel: Hyperparameter Controls
        var lblHpHeader = new Label
        {
            Text = "Hyperparameters",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Location = new Point(16, 24),
            AutoSize = true
        };
        grpTrain.Controls.Add(lblHpHeader);

        int lblX = 16, numX = 145, y = 52, yStep = 30;

        // Epochs
        grpTrain.Controls.Add(new Label { Text = "Epochs:", Location = new Point(lblX, y + 2), AutoSize = true });
        _numEpochs = new NumericUpDown { Location = new Point(numX, y), Size = new Size(95, 23), Minimum = 1, Maximum = 200, Value = 50 };
        grpTrain.Controls.Add(_numEpochs);
        y += yStep;

        // Hidden units
        grpTrain.Controls.Add(new Label { Text = "Hidden Neurons:", Location = new Point(lblX, y + 2), AutoSize = true });
        _numHidden = new NumericUpDown { Location = new Point(numX, y), Size = new Size(95, 23), Minimum = 8, Maximum = 128, Value = 32 };
        grpTrain.Controls.Add(_numHidden);
        y += yStep;

        // Output LR
        grpTrain.Controls.Add(new Label { Text = "Output LR (η_out):", Location = new Point(lblX, y + 2), AutoSize = true });
        _numLrOut = new NumericUpDown { Location = new Point(numX, y), Size = new Size(95, 23), DecimalPlaces = 3, Increment = 0.05M, Minimum = 0.001M, Maximum = 2.0M, Value = 0.500M };
        grpTrain.Controls.Add(_numLrOut);
        y += yStep;

        // Input LR
        grpTrain.Controls.Add(new Label { Text = "Input LR (η_in):", Location = new Point(lblX, y + 2), AutoSize = true });
        _numLrIn = new NumericUpDown { Location = new Point(numX, y), Size = new Size(95, 23), DecimalPlaces = 3, Increment = 0.025M, Minimum = 0.001M, Maximum = 2.0M, Value = 0.375M };
        grpTrain.Controls.Add(_numLrIn);
        y += yStep;

        // Seed
        grpTrain.Controls.Add(new Label { Text = "Random Seed:", Location = new Point(lblX, y + 2), AutoSize = true });
        _numSeed = new NumericUpDown { Location = new Point(numX, y), Size = new Size(95, 23), Minimum = 1, Maximum = 999999, Value = 42 };
        grpTrain.Controls.Add(_numSeed);
        y += 38;

        // Train action buttons
        _btnStartTrain = new Button
        {
            Text = "▶ Start Training",
            Location = new Point(lblX, y),
            Size = new Size(224, 34),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            UseVisualStyleBackColor = true
        };
        _btnStartTrain.Click += BtnStartTrain_Click;
        grpTrain.Controls.Add(_btnStartTrain);
        y += 40;

        _btnResetDefaults = new Button
        {
            Text = "Reset to Tuned Defaults",
            Location = new Point(lblX, y),
            Size = new Size(224, 26),
            UseVisualStyleBackColor = true
        };
        _btnResetDefaults.Click += (_, _) => ResetToDefaults();
        grpTrain.Controls.Add(_btnResetDefaults);
        y += 42;

        _btnLaunchTrained = new Button
        {
            Text = "Open Canvas with Trained Model",
            Location = new Point(lblX, y),
            Size = new Size(224, 36),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Enabled = false,
            UseVisualStyleBackColor = true
        };
        _btnLaunchTrained.Click += (_, _) => LaunchCanvas(usePretrained: false);
        grpTrain.Controls.Add(_btnLaunchTrained);

        // Right Panel: Training progress and epoch log
        var lblLogHeader = new Label
        {
            Text = "Live Training Metrics & Epoch Log",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Location = new Point(255, 24),
            AutoSize = true
        };
        grpTrain.Controls.Add(lblLogHeader);

        _prgTraining = new ProgressBar
        {
            Location = new Point(255, 48),
            Size = new Size(508, 20),
            Minimum = 0,
            Maximum = 50,
            Value = 0
        };
        grpTrain.Controls.Add(_prgTraining);

        _lblTrainingStatus = new Label
        {
            Text = "Status: Idle. Click 'Start Training' to begin.",
            Location = new Point(255, 72),
            Size = new Size(508, 18),
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(60, 60, 60)
        };
        grpTrain.Controls.Add(_lblTrainingStatus);

        _txtLog = new TextBox
        {
            Location = new Point(255, 94),
            Size = new Size(508, 328),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 8.5f),
            BackColor = Color.White
        };
        grpTrain.Controls.Add(_txtLog);

        Controls.Add(grpTrain);

        UpdatePretrainedStatus();
    }

    private void UpdatePretrainedStatus()
    {
        bool exists = File.Exists(ProjectPaths.WeightsFile);
        if (exists)
        {
            _lblPretrainedStatus.Text = "Trained weights found: results/weights.txt (Tuned: 32 hidden units, 50 epochs, 95.99% test acc).";
            _lblPretrainedStatus.ForeColor = Color.DarkGreen;
            _btnLaunchPretrained.Enabled = !_isTraining;
        }
        else
        {
            _lblPretrainedStatus.Text = "No saved weights found yet (results/weights.txt). Train a model below to create one.";
            _lblPretrainedStatus.ForeColor = Color.FromArgb(180, 80, 0);
            _btnLaunchPretrained.Enabled = false;
        }
    }

    private void ResetToDefaults()
    {
        _numEpochs.Value = 50;
        _numHidden.Value = 32;
        _numLrOut.Value = 0.500M;
        _numLrIn.Value = 0.375M;
        _numSeed.Value = 42;
    }

    private void SetControlsEnabled(bool enabled)
    {
        _isTraining = !enabled;
        _btnStartTrain.Enabled = enabled;
        _btnResetDefaults.Enabled = enabled;
        _numEpochs.Enabled = enabled;
        _numHidden.Enabled = enabled;
        _numLrOut.Enabled = enabled;
        _numLrIn.Enabled = enabled;
        _numSeed.Enabled = enabled;
        _btnLaunchPretrained.Enabled = enabled && File.Exists(ProjectPaths.WeightsFile);
    }

    private async void BtnStartTrain_Click(object? sender, EventArgs e)
    {
        if (_isTraining) return;

        SetControlsEnabled(false);
        _btnLaunchTrained.Enabled = false;

        int epochs = (int)_numEpochs.Value;
        int hidden = (int)_numHidden.Value;
        double lrOut = (double)_numLrOut.Value;
        double lrIn = (double)_numLrIn.Value;
        int seed = (int)_numSeed.Value;

        _prgTraining.Maximum = epochs;
        _prgTraining.Value = 0;
        _lblTrainingStatus.Text = "Loading dataset and preparing stratified split...";

        _txtLog.Clear();
        _txtLog.AppendText($"Starting Training (Hidden={hidden}, Epochs={epochs}, LrOut={lrOut:F3}, LrIn={lrIn:F3}, Seed={seed})\r\n");
        _txtLog.AppendText(new string('=', 64) + "\r\n");
        _txtLog.AppendText(string.Format("{0,-7} {1,-12} {2,-12} {3,-14} {4}\r\n", "Epoch", "Train MSE", "Val MSE", "Val Acc", "Elapsed"));
        _txtLog.AppendText(new string('-', 64) + "\r\n");

        NeuralNet? trainedNet = null;
        EvaluationReport? report = null;
        var sw = Stopwatch.StartNew();

        try
        {
            await Task.Run(() =>
            {
                var config = new AppConfig
                {
                    HiddenSize = hidden,
                    Epochs = epochs,
                    LearningRateOut = lrOut,
                    LearningRateIn = lrIn,
                    Seed = seed
                };

                // Prepare dataset
                IDatasetLoader loader = new CsvDatasetLoader();
                var rawTrain = loader.Load(ProjectPaths.TrainFile);
                var rawTest = loader.Load(ProjectPaths.TestFile);

                var scaler = new MinMaxScaler(0.0, config.PixelMax);
                scaler.Fit(rawTrain);

                var scaledTrain = scaler.TransformAll(rawTrain);
                var scaledTest = scaler.TransformAll(rawTest);

                var (train, val) = DatasetSplitter.Split(scaledTrain, config.ValidationFraction, config.Seed);

                var options = new TrainingOptions(epochs, lrOut, lrIn, hidden, seed, 1);
                var net = NetworkFactory.Create(options, config.InputSize, config.ClassCount);

                var trainer = new Trainer();

                using var logger = new FormLossLogger(metrics =>
                {
                    if (IsDisposed) return;
                    BeginInvoke(() =>
                    {
                        if (IsDisposed) return;
                        _prgTraining.Value = Math.Min(_prgTraining.Maximum, metrics.Epoch);
                        _lblTrainingStatus.Text = $"Epoch {metrics.Epoch}/{epochs} | Train Loss: {metrics.TrainLoss:F6} | Val Loss: {metrics.ValidationLoss:F6} | Val Acc: {metrics.ValidationAccuracy * 100:F2}%";

                        string line = string.Format("{0,-7} {1,-12:F6} {2,-12:F6} {3,-14:F2}% {4:F1}s\r\n",
                            metrics.Epoch,
                            metrics.TrainLoss,
                            metrics.ValidationLoss,
                            metrics.ValidationAccuracy * 100.0,
                            sw.Elapsed.TotalSeconds);
                        _txtLog.AppendText(line);
                    });
                });

                trainer.Train(net, train, val, options, logger);

                // Save weights to results/weights.txt
                net.saveWeights(ProjectPaths.WeightsFile);

                // Final evaluation on test set
                var evaluator = new Evaluator();
                report = evaluator.Evaluate(net, scaledTest);
                trainedNet = net;
            });

            sw.Stop();
            _trainedNet = trainedNet;

            _lblTrainingStatus.Text = $"Training finished in {sw.Elapsed.TotalSeconds:F2}s! Test Accuracy: {report?.Accuracy * 100:F2}%";
            _txtLog.AppendText(new string('=', 64) + "\r\n");
            _txtLog.AppendText($"[COMPLETE] Final Held-Out Test Accuracy: {report?.Accuracy * 100:F2}%\r\n");
            _txtLog.AppendText($"Weights saved to: {ProjectPaths.WeightsFile}\r\n");

            _btnLaunchTrained.Enabled = true;
            _btnLaunchTrained.Focus();
            UpdatePretrainedStatus();
        }
        catch (Exception ex)
        {
            sw.Stop();
            _lblTrainingStatus.Text = $"Training failed: {ex.Message}";
            _txtLog.AppendText($"\r\n[ERROR] {ex.Message}\r\n");
            MessageBox.Show($"Training error:\n{ex.Message}", "Training Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetControlsEnabled(true);
        }
    }

    private void LaunchCanvas(bool usePretrained)
    {
        NeuralNet? netToUse = null;

        if (usePretrained)
        {
            if (!File.Exists(ProjectPaths.WeightsFile))
            {
                MessageBox.Show("Pre-trained weights file not found. Please train a model first.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        else
        {
            netToUse = _trainedNet;
        }

        Hide();
        using (var canvasForm = new MainForm(netToUse))
        {
            canvasForm.ShowDialog(this);
        }
        Show();
        UpdatePretrainedStatus();
    }
}

/// <summary>
/// Simple adapter bridging <see cref="ILossLogger"/> to a callback for UI updates.
/// </summary>
internal sealed class FormLossLogger : ILossLogger
{
    private readonly Action<EpochMetrics> _onLog;

    public FormLossLogger(Action<EpochMetrics> onLog)
    {
        _onLog = onLog ?? throw new ArgumentNullException(nameof(onLog));
    }

    public void Log(EpochMetrics metrics) => _onLog(metrics);

    public void Dispose() { }
}
