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
    // ── Layout constants (all pixel values verified) ──────────────────────────
    //
    //  Form ClientSize: 780 × 480
    //
    //  LEFT GroupBox  (10, 8) 308 × 460
    //    Canvas        (10, 22) 280 × 280   bottom=302
    //    Clear btn     (10, 310) 280 × 30   bottom=340
    //    "8×8:" lbl    (10, 356)             bottom≈372
    //    Preview box   (88, 350) 80 × 80    bottom=430   ← kept square
    //
    //  RIGHT GroupBox (328, 8) 440 × 460
    //    Digit panel   (10, 22) 130 × 130   bottom=152   (60pt font)
    //    "Activations:" (150, 22) auto
    //    Bars [0..9]   (10, 162) each 29 × 26 (28px step)
    //                  bar 9 bottom = 162 + 9*28 + 26 = 440  ✓

    private readonly Bitmap _canvasBitmap;
    private readonly Graphics _canvasGraphics;
    private readonly PictureBox _canvasBox;
    private readonly PictureBox _preview8x8Box;
    private readonly Label _predictedDigitLabel;
    private readonly DigitBarControl[] _digitBars = new DigitBarControl[10];

    private NeuralNet? _net;
    private Point? _lastPoint;
    private bool _isDrawing;
    private readonly Pen _drawPen;

    /// <summary>Initializes a new instance of <see cref="MainForm"/>.</summary>
    /// <param name="preloadedNet">An optional already-trained neural network instance. If null, loads from weights file.</param>
    public MainForm(NeuralNet? preloadedNet = null)
    {
        Text = "OptDigits MLP Demo";
        ClientSize = new Size(780, 480);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9f);

        _drawPen = new Pen(Color.Black, 20f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        // White canvas bitmap (280 × 280)
        _canvasBitmap = new Bitmap(280, 280);
        _canvasGraphics = Graphics.FromImage(_canvasBitmap);
        _canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        ClearCanvasBitmap();

        // ── LEFT GroupBox ─────────────────────────────────────────────────────
        var grpDraw = new GroupBox
        {
            Text = "Draw a digit",
            Location = new Point(10, 8),
            Size = new Size(308, 460)
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
            Size = new Size(280, 30)
        };
        btnClear.Click += (_, _) => ResetCanvas();
        grpDraw.Controls.Add(btnClear);

        var lblPreview = new Label
        {
            Text = "8×8 input:",
            Location = new Point(10, 362),
            AutoSize = true
        };
        grpDraw.Controls.Add(lblPreview);

        _preview8x8Box = new PictureBox
        {
            Location = new Point(88, 350),
            Size = new Size(80, 80),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White
        };
        _preview8x8Box.Paint += (_, pe) =>
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            if (_preview8x8Box.Image != null)
                pe.Graphics.DrawImage(_preview8x8Box.Image, _preview8x8Box.ClientRectangle);
        };
        grpDraw.Controls.Add(_preview8x8Box);

        var btnMenu = new Button
        {
            Text = "← Main Menu",
            Location = new Point(174, 370),
            Size = new Size(120, 36),
            UseVisualStyleBackColor = true
        };
        btnMenu.Click += (_, _) => Close();
        grpDraw.Controls.Add(btnMenu);

        Controls.Add(grpDraw);

        // ── RIGHT GroupBox ────────────────────────────────────────────────────
        var grpResult = new GroupBox
        {
            Text = "Prediction",
            Location = new Point(328, 8),
            Size = new Size(440, 460)
        };

        // Digit panel: fixed 130×130, digit label fills it
        var digitPanel = new Panel
        {
            Location = new Point(10, 22),
            Size = new Size(130, 130),
            BackColor = SystemColors.Control
        };
        grpResult.Controls.Add(digitPanel);

        _predictedDigitLabel = new Label
        {
            Text = "—",
            Font = new Font("Segoe UI", 60f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            AutoSize = false
        };
        digitPanel.Controls.Add(_predictedDigitLabel);

        var lblActivations = new Label
        {
            Text = "Class activations:",
            Location = new Point(150, 28),
            AutoSize = true
        };
        grpResult.Controls.Add(lblActivations);

        // 10 bars: starting at y=162, each 26px tall, 28px step
        // bar 9 bottom = 162 + 9*28 + 26 = 440 — fits inside GroupBox(h=460)
        for (int i = 0; i < 10; i++)
        {
            var bar = new DigitBarControl(i)
            {
                Location = new Point(10, 162 + i * 28),
                Size = new Size(418, 26)
            };
            _digitBars[i] = bar;
            grpResult.Controls.Add(bar);
        }

        Controls.Add(grpResult);

        if (preloadedNet != null)
        {
            _net = preloadedNet;
        }
        else
        {
            LoadModel();
        }
    }

    // ── Canvas helpers ────────────────────────────────────────────────────────

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

    // ── Mouse drawing ─────────────────────────────────────────────────────────

    private void Canvas_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        _isDrawing = true;
        _lastPoint = e.Location;
        _canvasGraphics.FillEllipse(Brushes.Black, e.X - 10, e.Y - 10, 20, 20);
        _canvasBox.Invalidate();
        RunInference();
    }

    private void Canvas_MouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDrawing || !_lastPoint.HasValue) return;
        _canvasGraphics.DrawLine(_drawPen, _lastPoint.Value, e.Location);
        _lastPoint = e.Location;
        _canvasBox.Invalidate();
        RunInference();
    }

    private void Canvas_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        _isDrawing = false;
        _lastPoint = null;
        RunInference();
    }

    // ── Inference ─────────────────────────────────────────────────────────────

    private void RunInference()
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

        for (int i = 0; i < 64; i++) _net.setInputs(i, features[i]);
        _net.run();

        double[] activations = new double[10];
        for (int k = 0; k < 10; k++) activations[k] = _net.getOuputData(k);

        int predicted = _net.Predict();
        _predictedDigitLabel.Text = predicted.ToString();
        for (int k = 0; k < 10; k++)
            _digitBars[k].UpdateActivation(activations[k], isWinner: k == predicted);
    }

    // ── Model loading ─────────────────────────────────────────────────────────

    private void LoadModel()
    {
        var config = new AppConfig();
        _net = new NeuralNet(config.InputSize, config.HiddenSize, config.ClassCount);

        if (!File.Exists(ProjectPaths.WeightsFile))
        {
            MessageBox.Show(
                "No trained model found (weights.txt).\n\n" +
                "Run the following first:\n" +
                "  dotnet run --project src/OptDigits.App -- train",
                "Model Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try { _net.loadWeights(ProjectPaths.WeightsFile); }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load weights:\n{ex.Message}",
                "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Releases managed resources.</summary>
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

// ── DigitBarControl ───────────────────────────────────────────────────────────

/// <summary>
/// Lightweight owner-drawn progress bar showing raw MLP activation for one digit class.
/// </summary>
public sealed class DigitBarControl : UserControl
{
    private readonly int _digit;
    private double _activation;
    private bool _isWinner;
    private readonly Font _normalFont;
    private readonly Font _boldFont;

    /// <summary>Initializes a new <see cref="DigitBarControl"/> for digit class <paramref name="digit"/>.</summary>
    public DigitBarControl(int digit)
    {
        _digit = digit;
        _normalFont = new Font("Segoe UI", 8.5f);
        _boldFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        DoubleBuffered = true;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
    }

    /// <summary>Sets the activation level and repaints.</summary>
    public void UpdateActivation(double activation, bool isWinner)
    {
        _activation = Math.Clamp(activation, 0.0, 1.0);
        _isWinner = isWinner;
        Invalidate();
    }

    /// <summary>Paints label, track, fill, and percentage.</summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(SystemColors.Control);

        Font font = _isWinner ? _boldFont : _normalFont;

        // "Digit X:" label — fixed 40px wide
        string label = $"Digit {_digit}:";
        g.DrawString(label, font, SystemBrushes.ControlText, new RectangleF(0, 2, 56, Height));

        // Track region
        const int labelW = 58;
        const int pctW   = 50;
        int trackX = labelW;
        int trackW = Width - labelW - pctW - 2;
        int trackY = (Height - 12) / 2;

        using var trackBrush = new SolidBrush(SystemColors.ControlDark);
        g.FillRectangle(trackBrush, trackX, trackY, trackW, 12);

        // Fill
        int fillW = (int)(trackW * _activation);
        if (fillW > 0)
        {
            using var fillBrush = new SolidBrush(_isWinner ? Color.SteelBlue : Color.LightSteelBlue);
            g.FillRectangle(fillBrush, trackX, trackY, fillW, 12);
        }

        // Percentage label
        string pct = $"{_activation * 100:F1}%";
        g.DrawString(pct, font, SystemBrushes.ControlText,
            new RectangleF(Width - pctW, 2, pctW, Height),
            new StringFormat { Alignment = StringAlignment.Far });
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing) { _normalFont.Dispose(); _boldFont.Dispose(); }
        base.Dispose(disposing);
    }
}
