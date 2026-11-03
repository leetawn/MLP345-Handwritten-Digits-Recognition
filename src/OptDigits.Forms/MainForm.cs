using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Backprop;
using OptDigits.App;

namespace OptDigits.Forms;

/// <summary>
/// Main window form offering an interactive drawing canvas with real-time digit classification.
/// </summary>
public sealed class MainForm : Form
{
    private readonly Bitmap _canvasBitmap;
    private readonly Graphics _canvasGraphics;
    private readonly PictureBox _canvasBox;
    private readonly PictureBox _preview8x8Box;
    private readonly Label _predictedDigitLabel;
    private readonly DigitBarControl[] _digitBars = new DigitBarControl[10];

    private NeuralNet? _net;
    private Point? _lastPoint = null;
    private bool _isDrawing = false;
    private readonly Pen _drawPen;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        Text = "OptDigits MLP Demo";
        ClientSize = new Size(720, 420);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9f);

        _drawPen = new Pen(Color.Black, 18f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        _canvasBitmap = new Bitmap(280, 280);
        _canvasGraphics = Graphics.FromImage(_canvasBitmap);
        _canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        ClearCanvasBitmap();

        // ── Left panel: canvas + clear + 8x8 preview ──────────────────────────
        var grpDraw = new GroupBox
        {
            Text = "Draw a digit",
            Location = new Point(12, 8),
            Size = new Size(306, 396),
            Font = new Font("Segoe UI", 9f)
        };

        _canvasBox = new PictureBox
        {
            Location = new Point(10, 22),
            Size = new Size(280, 280),
            Image = _canvasBitmap,
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Cross,
            BackColor = Color.White
        };
        _canvasBox.MouseDown += Canvas_MouseDown;
        _canvasBox.MouseMove += Canvas_MouseMove;
        _canvasBox.MouseUp += Canvas_MouseUp;
        grpDraw.Controls.Add(_canvasBox);

        var btnClear = new Button
        {
            Text = "Clear",
            Location = new Point(10, 310),
            Size = new Size(280, 30),
        };
        btnClear.Click += (s, e) => ResetCanvas();
        grpDraw.Controls.Add(btnClear);

        var lblPreview = new Label
        {
            Text = "8×8 input features:",
            Location = new Point(10, 350),
            AutoSize = true
        };
        grpDraw.Controls.Add(lblPreview);

        _preview8x8Box = new PictureBox
        {
            Location = new Point(130, 344),
            Size = new Size(64, 64),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        _preview8x8Box.Paint += (s, pe) =>
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            if (_preview8x8Box.Image != null)
                pe.Graphics.DrawImage(_preview8x8Box.Image, _preview8x8Box.ClientRectangle);
        };
        grpDraw.Controls.Add(_preview8x8Box);

        Controls.Add(grpDraw);

        // ── Right panel: prediction digit + activation bars ────────────────────
        var grpResult = new GroupBox
        {
            Text = "Prediction",
            Location = new Point(328, 8),
            Size = new Size(380, 396),
            Font = new Font("Segoe UI", 9f)
        };

        // Large predicted digit display — fixed-size panel so it never clips
        var digitPanel = new Panel
        {
            Location = new Point(8, 20),
            Size = new Size(100, 100),
            BackColor = SystemColors.Control
        };
        grpResult.Controls.Add(digitPanel);

        _predictedDigitLabel = new Label
        {
            Text = "—",
            Font = new Font("Segoe UI", 72f, FontStyle.Bold),
            ForeColor = SystemColors.ControlText,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            AutoSize = false
        };
        digitPanel.Controls.Add(_predictedDigitLabel);

        var lblBarsHeader = new Label
        {
            Text = "Class activations:",
            Location = new Point(116, 22),
            AutoSize = true
        };
        grpResult.Controls.Add(lblBarsHeader);

        // 10 activation bars stacked on the right side
        int barY = 40;
        for (int i = 0; i < 10; i++)
        {
            var bar = new DigitBarControl(i)
            {
                Location = new Point(8, barY + i * 34),
                Size = new Size(360, 30)
            };
            _digitBars[i] = bar;
            grpResult.Controls.Add(bar);
        }

        Controls.Add(grpResult);

        LoadModel();
    }

    private void ClearCanvasBitmap()
    {
        _canvasGraphics.Clear(Color.White);
        _canvasBox?.Invalidate();
    }

    private void ResetCanvas()
    {
        ClearCanvasBitmap();
        _lastPoint = null;
        _predictedDigitLabel.Text = "—";
        _preview8x8Box.Image = null;
        _preview8x8Box.Invalidate();
        for (int i = 0; i < 10; i++)
            _digitBars[i].UpdateActivation(0.0, isWinner: false);
    }

    private void Canvas_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        _isDrawing = true;
        _lastPoint = e.Location;
        _canvasGraphics.FillEllipse(Brushes.Black, e.X - 9, e.Y - 9, 18, 18);
        _canvasBox.Invalidate();
        RunRealTimeInference();
    }

    private void Canvas_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDrawing || !_lastPoint.HasValue) return;
        _canvasGraphics.DrawLine(_drawPen, _lastPoint.Value, e.Location);
        _lastPoint = e.Location;
        _canvasBox.Invalidate();
        RunRealTimeInference();
    }

    private void Canvas_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        _isDrawing = false;
        _lastPoint = null;
        RunRealTimeInference();
    }

    private void RunRealTimeInference()
    {
        if (_net == null) return;

        var (features, preview, hasDrawing) = DigitExtractor.ExtractFeatures(_canvasBitmap);
        _preview8x8Box.Image = preview;
        _preview8x8Box.Invalidate();

        if (!hasDrawing)
        {
            _predictedDigitLabel.Text = "—";
            for (int i = 0; i < 10; i++) _digitBars[i].UpdateActivation(0.0, false);
            return;
        }

        for (int i = 0; i < 64; i++)
            _net.setInputs(i, features[i]);
        _net.run();

        double[] activations = new double[10];
        for (int k = 0; k < 10; k++)
            activations[k] = _net.getOuputData(k);

        int predicted = _net.Predict();
        _predictedDigitLabel.Text = predicted.ToString();

        for (int k = 0; k < 10; k++)
            _digitBars[k].UpdateActivation(activations[k], isWinner: k == predicted);
    }

    private void LoadModel()
    {
        string weightsPath = ProjectPaths.WeightsFile;
        var config = new AppConfig();
        _net = new NeuralNet(config.InputSize, config.HiddenSize, config.ClassCount);

        if (File.Exists(weightsPath))
        {
            try { _net.loadWeights(weightsPath); }
            catch
            {
                MessageBox.Show(
                    "weights.txt found but could not be loaded.\nRun 'dotnet run --project src/OptDigits.App -- train' first.",
                    "Model Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            MessageBox.Show(
                "No trained model found.\nRun 'dotnet run --project src/OptDigits.App -- train' to generate weights.txt first.",
                "Model Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// Releases unmanaged and managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _drawPen.Dispose();
            _canvasGraphics.Dispose();
            _canvasBitmap.Dispose();
        }
        base.Dispose(disposing);
    }
}

/// <summary>
/// Custom bar control representing activation probability for a single digit class.
/// </summary>
public sealed class DigitBarControl : UserControl
{
    private readonly int _digit;
    private double _activation = 0.0;
    private bool _isWinner = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DigitBarControl"/> class for the specified digit class.
    /// </summary>
    /// <param name="digit">Target digit class index (0–9).</param>
    public DigitBarControl(int digit)
    {
        _digit = digit;
        DoubleBuffered = true;
    }

    /// <summary>
    /// Updates the bar activation value and winning highlight state, then repaints.
    /// </summary>
    /// <param name="activation">Normalized activation level [0.0, 1.0].</param>
    /// <param name="isWinner">Whether this digit is the current top prediction.</param>
    public void UpdateActivation(double activation, bool isWinner)
    {
        _activation = Math.Clamp(activation, 0.0, 1.0);
        _isWinner = isWinner;
        Invalidate();
    }

    /// <summary>
    /// Paints the digit label, track background, and activation fill bar.
    /// </summary>
    /// <param name="e">Paint event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;

        // Digit label (e.g. "3:")
        string label = $"{_digit}:";
        using var labelFont = new Font("Segoe UI", 9f, _isWinner ? FontStyle.Bold : FontStyle.Regular);
        g.DrawString(label, labelFont, SystemBrushes.ControlText, 2, 7);

        // Track background
        int barLeft = 26;
        int barRight = Width - 50;
        int barWidth = barRight - barLeft;
        int barTop = 8;
        int barHeight = Height - 16;

        using var trackBrush = new SolidBrush(SystemColors.ControlDark);
        g.FillRectangle(trackBrush, barLeft, barTop, barWidth, barHeight);

        // Fill
        int fillWidth = (int)(barWidth * _activation);
        if (fillWidth > 0)
        {
            Color fillColor = _isWinner ? Color.SteelBlue : Color.FromArgb(160, 195, 220);
            using var fillBrush = new SolidBrush(fillColor);
            g.FillRectangle(fillBrush, barLeft, barTop, fillWidth, barHeight);
        }

        // Percentage label
        string pct = $"{_activation * 100:F1}%";
        using var pctFont = new Font("Segoe UI", 8f, _isWinner ? FontStyle.Bold : FontStyle.Regular);
        var pctSize = g.MeasureString(pct, pctFont);
        g.DrawString(pct, pctFont, SystemBrushes.ControlText, Width - pctSize.Width - 2, (Height - pctSize.Height) / 2);
    }
}
