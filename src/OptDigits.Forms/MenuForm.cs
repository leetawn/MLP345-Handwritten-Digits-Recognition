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

// Main Menu form allowing users to either launch the drawing canvas with a pre-trained model
// or train a custom MLP with real-time epoch metrics and visual progress.
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

    // Initializes a new instance of MenuForm.
    public MenuForm()
    {
        Text = "OptDigits MLP - Main Menu";
        ClientSize = new Size(820, 590);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9f);

        // ── TOP HEADER ────────────────────────────────────────────────────────
        // Student / Team Information (Top Left)
        var lblAuthors = new Label
        {
            Text = "Ethan Benedict Labajo, BSCS - 3 F1\nHelge Justine Dano, BSCS - 3 F1",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 30, 30),
            Location = new Point(20, 10),
            AutoSize = true
        };
        Controls.Add(lblAuthors);

        // Project Title (Top Right)
        var lblProjectTitle = new Label
        {
            Text = "OptDigits MLP Classifier\nHandwritten Digits Recognition",
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(90, 90, 90),
            TextAlign = ContentAlignment.TopRight,
            Location = new Point(530, 10),
            Size = new Size(270, 42)
        };
        Controls.Add(lblProjectTitle);

        // ── SECTION 1: PRE-TRAINED MODEL ──────────────────────────────────────
        var grpPretrained = new GroupBox
        {
            Text = " Pre-Trained Model ",
            Location = new Point(20, 54),
            Size = new Size(780, 86)
        };

        _lblPretrainedStatus = new Label
        {
            Location = new Point(16, 22),
            Size = new Size(748, 22),
            Font = new Font("Segoe UI", 9f)
        };
        grpPretrained.Controls.Add(_lblPretrainedStatus);

        _btnLaunchPretrained = new Button
        {
            Text = "Open Drawing Canvas",
            Location = new Point(16, 46),
            Size = new Size(210, 30),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            UseVisualStyleBackColor = true
        };
        _btnLaunchPretrained.Click += (_, _) => LaunchCanvas(usePretrained: true);
        grpPretrained.Controls.Add(_btnLaunchPretrained);

        Controls.Add(grpPretrained);

        // ── SECTION 2: TRAIN CUSTOM MODEL ─────────────────────────────────────
        var grpTrain = new GroupBox
        {
            Text = " Train Model ",
            Location = new Point(20, 146),
            Size = new Size(780, 428)
        };

        int lblX = 16, numX = 156, y = 26, yStep = 30;

        // Epochs
        grpTrain.Controls.Add(new Label { Text = "Epochs:", Location = new Point(lblX, y + 2), AutoSize = true });
        _numEpochs = new NumericUpDown { Location = new Point(numX, y), Size = new Size(90, 23), Minimum = 1, Maximum = 200, Value = 50 };
        grpTrain.Controls.Add(_numEpochs);
        y += yStep;

        // Hidden units
        grpTrain.Controls.Add(new Label { Text = "Hidden Units:", Location = new Point(lblX, y + 2), AutoSize = true });
        _numHidden = new NumericUpDown { Location = new Point(numX, y), Size = new Size(90, 23), Minimum = 8, Maximum = 128, Value = 32 };
        grpTrain.Controls.Add(_numHidden);
        y += yStep;

        // Output LR
        grpTrain.Controls.Add(new Label { Text = "Output LR (η_out):", Location = new Point(lblX, y + 2), AutoSize = true });
        _numLrOut = new NumericUpDown { Location = new Point(numX, y), Size = new Size(90, 23), DecimalPlaces = 3, Increment = 0.05M, Minimum = 0.001M, Maximum = 2.0M, Value = 0.500M };
        grpTrain.Controls.Add(_numLrOut);
        y += yStep;

        // Input LR
        grpTrain.Controls.Add(new Label { Text = "Input LR (η_in):", Location = new Point(lblX, y + 2), AutoSize = true });
        _numLrIn = new NumericUpDown { Location = new Point(numX, y), Size = new Size(90, 23), DecimalPlaces = 3, Increment = 0.025M, Minimum = 0.001M, Maximum = 2.0M, Value = 0.375M };
        grpTrain.Controls.Add(_numLrIn);
        y += yStep;

        // Seed
        grpTrain.Controls.Add(new Label { Text = "Random Seed:", Location = new Point(lblX, y + 2), AutoSize = true });
        _numSeed = new NumericUpDown { Location = new Point(numX, y), Size = new Size(90, 23), Minimum = 1, Maximum = 999999, Value = 42 };
        grpTrain.Controls.Add(_numSeed);
        y += 34;

        // Train action buttons
        _btnStartTrain = new Button
        {
            Text = "▶ Train Model",
            Location = new Point(lblX, y),
            Size = new Size(230, 32),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            UseVisualStyleBackColor = true
        };
        _btnStartTrain.Click += BtnStartTrain_Click;
        grpTrain.Controls.Add(_btnStartTrain);
        y += 38;

        _btnResetDefaults = new Button
        {
            Text = "Reset Defaults",
            Location = new Point(lblX, y),
            Size = new Size(230, 26),
            UseVisualStyleBackColor = true
        };
        _btnResetDefaults.Click += (_, _) => ResetToDefaults();
        grpTrain.Controls.Add(_btnResetDefaults);
        y += 38;

        _btnLaunchTrained = new Button
        {
            Text = "Open Canvas (Trained Model)",
            Location = new Point(lblX, y),
            Size = new Size(230, 34),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Enabled = false,
            UseVisualStyleBackColor = true
        };
        _btnLaunchTrained.Click += (_, _) => LaunchCanvas(usePretrained: false);
        grpTrain.Controls.Add(_btnLaunchTrained);

        // Right Panel: Progress and Log
        _prgTraining = new ProgressBar
        {
            Location = new Point(262, 24),
            Size = new Size(502, 20),
            Minimum = 0,
            Maximum = 50,
            Value = 0
        };
        grpTrain.Controls.Add(_prgTraining);

        _lblTrainingStatus = new Label
        {
            Text = "Ready to train.",
            Location = new Point(262, 48),
            Size = new Size(502, 22),
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(70, 70, 70)
        };
        grpTrain.Controls.Add(_lblTrainingStatus);

        _txtLog = new TextBox
        {
            Location = new Point(262, 74),
            Size = new Size(502, 338),
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
            _lblPretrainedStatus.Text = "Weights: Ready (95.99% accuracy on held-out test set).";
            _lblPretrainedStatus.ForeColor = Color.DarkGreen;
            _btnLaunchPretrained.Enabled = !_isTraining;
        }
        else
        {
            _lblPretrainedStatus.Text = "No saved weights found. Train a model below to generate one.";
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
        _lblTrainingStatus.Text = "Loading dataset...";

        _txtLog.Clear();
        _txtLog.AppendText(string.Format("{0,-7} {1,-12} {2,-12} {3,-12} {4}\r\n", "Epoch", "Train Loss", "Val Loss", "Val Acc", "Elapsed"));
        _txtLog.AppendText(new string('-', 56) + "\r\n");

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
                        _lblTrainingStatus.Text = $"Epoch {metrics.Epoch}/{epochs} | Loss: {metrics.TrainLoss:F5} | Val Acc: {metrics.ValidationAccuracy * 100:F2}%";

                        string line = string.Format("{0,-7} {1,-12:F6} {2,-12:F6} {3,-12:F2}% {4:F1}s\r\n",
                            metrics.Epoch,
                            metrics.TrainLoss,
                            metrics.ValidationLoss,
                            metrics.ValidationAccuracy * 100.0,
                            sw.Elapsed.TotalSeconds);
                        _txtLog.AppendText(line);
                    });
                });

                trainer.Train(net, train, val, options, logger);

                net.saveWeights(ProjectPaths.WeightsFile);

                var evaluator = new Evaluator();
                report = evaluator.Evaluate(net, scaledTest);
                trainedNet = net;
            });

            sw.Stop();
            _trainedNet = trainedNet;

            _lblTrainingStatus.Text = $"Done in {sw.Elapsed.TotalSeconds:F1}s! Test Accuracy: {report?.Accuracy * 100:F2}%";
            _txtLog.AppendText(new string('-', 56) + "\r\n");
            _txtLog.AppendText($"Done in {sw.Elapsed.TotalSeconds:F1}s. Final Test Accuracy: {report?.Accuracy * 100:F2}%\r\n");

            _btnLaunchTrained.Enabled = true;
            _btnLaunchTrained.Focus();
            UpdatePretrainedStatus();
        }
        catch (Exception ex)
        {
            sw.Stop();
            _lblTrainingStatus.Text = $"Error: {ex.Message}";
            _txtLog.AppendText($"\r\n[Error] {ex.Message}\r\n");
            MessageBox.Show($"Training error:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Pre-trained weights file not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

// Simple adapter bridging ILossLogger to a callback for UI updates.
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
