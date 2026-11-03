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
    private readonly Label _confidenceLabel;
    private readonly Label _statusLabel;
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
        Text = "OptDigits MLP - Real-Time Handwritten Digit Classifier";
        Size = new Size(880, 580);
        MinimumSize = new Size(850, 560);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(24, 26, 32);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        _drawPen = new Pen(Color.White, 20f)
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        _canvasBitmap = new Bitmap(280, 280);
        _canvasGraphics = Graphics.FromImage(_canvasBitmap);
        _canvasGraphics.SmoothingMode = SmoothingMode.AntiAlias;
        ClearCanvasBitmap();

        // Left Container (Canvas)
        var leftPanel = new Panel
        {
            Location = new Point(24, 20),
            Size = new Size(320, 500)
        };

        var canvasTitle = new Label
        {
            Text = "Draw a Digit (0 - 9)",
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(240, 240, 240),
            AutoSize = true,
            Location = new Point(0, 0)
        };
        leftPanel.Controls.Add(canvasTitle);

        var canvasSubtitle = new Label
        {
            Text = "Draw in the box below using your mouse",
            Font = new Font("Segoe UI", 9f),
            ForeColor = Color.FromArgb(160, 165, 175),
            AutoSize = true,
            Location = new Point(0, 26)
        };
        leftPanel.Controls.Add(canvasSubtitle);

        _canvasBox = new PictureBox
        {
            Location = new Point(0, 55),
            Size = new Size(280, 280),
            Image = _canvasBitmap,
            BorderStyle = BorderStyle.FixedSingle,
            Cursor = Cursors.Cross,
            BackColor = Color.Black
        };
        _canvasBox.MouseDown += Canvas_MouseDown;
        _canvasBox.MouseMove += Canvas_MouseMove;
        _canvasBox.MouseUp += Canvas_MouseUp;
        leftPanel.Controls.Add(_canvasBox);

        var btnClear = new Button
        {
            Text = "Clear Canvas",
            Location = new Point(0, 345),
            Size = new Size(135, 36),
            BackColor = Color.FromArgb(45, 52, 64),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnClear.FlatAppearance.BorderSize = 0;
        btnClear.Click += (s, e) => ResetCanvas();
        leftPanel.Controls.Add(btnClear);

        var btnReload = new Button
        {
            Text = "Reload Model",
            Location = new Point(145, 345),
            Size = new Size(135, 36),
            BackColor = Color.FromArgb(45, 52, 64),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f),
            Cursor = Cursors.Hand
        };
        btnReload.FlatAppearance.BorderSize = 0;
        btnReload.Click += (s, e) => LoadModel();
        leftPanel.Controls.Add(btnReload);

        // Preview 8x8 Feature Map
        var previewTitle = new Label
        {
            Text = "8x8 Pooled Input Features:",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(180, 185, 195),
            AutoSize = true,
            Location = new Point(0, 395)
        };
        leftPanel.Controls.Add(previewTitle);

        _preview8x8Box = new PictureBox
        {
            Location = new Point(0, 420),
            Size = new Size(80, 80),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.Black
        };
        _preview8x8Box.Paint += (s, pe) =>
        {
            pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            if (_preview8x8Box.Image != null)
            {
                pe.Graphics.DrawImage(_preview8x8Box.Image, _preview8x8Box.ClientRectangle);
            }
        };
        leftPanel.Controls.Add(_preview8x8Box);

        _statusLabel = new Label
        {
            Text = "Model: Initializing...",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(140, 145, 155),
            Location = new Point(90, 430),
            Size = new Size(200, 60)
        };
        leftPanel.Controls.Add(_statusLabel);

        Controls.Add(leftPanel);

        // Right Container (Real-Time Predictions)
        var rightPanel = new Panel
        {
            Location = new Point(360, 20),
            Size = new Size(480, 500)
        };

        var resultsTitle = new Label
        {
            Text = "Real-Time MLP Prediction",
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            ForeColor = Color.FromArgb(240, 240, 240),
            AutoSize = true,
            Location = new Point(0, 0)
        };
        rightPanel.Controls.Add(resultsTitle);

        // Top Winner Banner Card
        var bannerCard = new Panel
        {
            Location = new Point(0, 40),
            Size = new Size(470, 95),
            BackColor = Color.FromArgb(32, 36, 45)
        };

        var bannerHeader = new Label
        {
            Text = "TOP PREDICTION",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(130, 140, 160),
            Location = new Point(16, 12),
            AutoSize = true
        };
        bannerCard.Controls.Add(bannerHeader);

        _predictedDigitLabel = new Label
        {
            Text = "—",
            Font = new Font("Segoe UI", 44f, FontStyle.Bold),
            ForeColor = Color.FromArgb(46, 204, 113),
            Location = new Point(14, 25),
            AutoSize = true
        };
        bannerCard.Controls.Add(_predictedDigitLabel);

        _confidenceLabel = new Label
        {
            Text = "Draw a digit on the canvas to see predictions",
            Font = new Font("Segoe UI", 10.5f),
            ForeColor = Color.FromArgb(180, 190, 205),
            Location = new Point(110, 48),
            AutoSize = true
        };
        bannerCard.Controls.Add(_confidenceLabel);

        rightPanel.Controls.Add(bannerCard);

        // 10 Class Activation Bars
        int barStartY = 150;
        int barHeight = 30;
        for (int i = 0; i < 10; i++)
        {
            var bar = new DigitBarControl(i)
            {
                Location = new Point(0, barStartY + i * (barHeight + 4)),
                Size = new Size(470, barHeight)
            };
            _digitBars[i] = bar;
            rightPanel.Controls.Add(bar);
        }

        Controls.Add(rightPanel);

        LoadModel();
    }

    private void ClearCanvasBitmap()
    {
        _canvasGraphics.Clear(Color.Black);
        _canvasBox?.Invalidate();
    }

    private void ResetCanvas()
    {
        ClearCanvasBitmap();
        _lastPoint = null;
        _predictedDigitLabel.Text = "—";
        _predictedDigitLabel.ForeColor = Color.FromArgb(130, 140, 160);
        _confidenceLabel.Text = "Draw a digit on the canvas to see predictions";
        _preview8x8Box.Image = null;

        for (int i = 0; i < 10; i++)
        {
            _digitBars[i].UpdateActivation(0.0, isWinner: false);
        }
    }

    private void Canvas_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isDrawing = true;
            _lastPoint = e.Location;
            _canvasGraphics.FillEllipse(Brushes.White, e.X - 10, e.Y - 10, 20, 20);
            _canvasBox.Invalidate();
            RunRealTimeInference();
        }
    }

    private void Canvas_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_isDrawing && _lastPoint.HasValue)
        {
            _canvasGraphics.DrawLine(_drawPen, _lastPoint.Value, e.Location);
            _lastPoint = e.Location;
            _canvasBox.Invalidate();
            RunRealTimeInference();
        }
    }

    private void Canvas_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isDrawing = false;
            _lastPoint = null;
            RunRealTimeInference();
        }
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
            _predictedDigitLabel.ForeColor = Color.FromArgb(130, 140, 160);
            _confidenceLabel.Text = "Draw a digit to predict";
            for (int i = 0; i < 10; i++) _digitBars[i].UpdateActivation(0.0, false);
            return;
        }

        for (int i = 0; i < 64; i++)
        {
            _net.setInputs(i, features[i]);
        }
        _net.run();

        double[] activations = new double[10];
        for (int k = 0; k < 10; k++)
        {
            activations[k] = _net.getOuputData(k);
        }

        int predicted = _net.Predict();
        double sumAct = activations.Sum();
        double pseudoProb = sumAct > 0.0001 ? (activations[predicted] / sumAct) : activations[predicted];

        _predictedDigitLabel.Text = predicted.ToString();
        _predictedDigitLabel.ForeColor = Color.FromArgb(46, 204, 113);
        _confidenceLabel.Text = $"Confidence: {pseudoProb * 100:F1}% (Raw activation: {activations[predicted]:F4})";

        for (int k = 0; k < 10; k++)
        {
            _digitBars[k].UpdateActivation(activations[k], isWinner: (k == predicted));
        }
    }

    private void LoadModel()
    {
        string weightsPath = ProjectPaths.WeightsFile;
        var config = new AppConfig();

        if (File.Exists(weightsPath))
        {
            try
            {
                _net = new NeuralNet(config.InputSize, config.HiddenSize, config.ClassCount);
                _net.loadWeights(weightsPath);
                _statusLabel.Text = $"Model: 64-{config.HiddenSize}-10\nLoaded: weights.txt\nStatus: Ready";
                _statusLabel.ForeColor = Color.FromArgb(46, 204, 113);
                ResetCanvas();
                return;
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error loading weights: {ex.Message}";
                _statusLabel.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }
        else
        {
            _statusLabel.Text = "weights.txt not found.\nRun 'train' command\nto generate model.";
            _statusLabel.ForeColor = Color.FromArgb(241, 196, 15);
        }

        // Fallback default initialized net
        _net = new NeuralNet(config.InputSize, config.HiddenSize, config.ClassCount);
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
    /// <param name="digit">Target digit class index (0-9).</param>
    public DigitBarControl(int digit)
    {
        _digit = digit;
        DoubleBuffered = true;
        Height = 28;
    }

    /// <summary>
    /// Updates the bar activation value and winning highlight status.
    /// </summary>
    /// <param name="activation">Normalized activation level [0.0, 1.0].</param>
    /// <param name="isWinner">Whether this digit is the current winning predicted class.</param>
    public void UpdateActivation(double activation, bool isWinner)
    {
        _activation = Math.Clamp(activation, 0.0, 1.0);
        _isWinner = isWinner;
        Invalidate();
    }

    /// <summary>
    /// Paints the digit class label, background progress track, and activation fill.
    /// </summary>
    /// <param name="e">A <see cref="PaintEventArgs"/> containing paint event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        int width = Width;
        int height = Height;

        // Background
        using var bgBrush = new SolidBrush(Color.FromArgb(32, 36, 45));
        g.FillRectangle(bgBrush, 0, 0, width, height);

        // Digit Label
        string digitStr = $"Digit {_digit}";
        using var font = new Font("Segoe UI", 9.5f, _isWinner ? FontStyle.Bold : FontStyle.Regular);
        using var textBrush = new SolidBrush(_isWinner ? Color.White : Color.FromArgb(170, 175, 185));
        g.DrawString(digitStr, font, textBrush, 8, 4);

        // Progress bar fill area
        int barLeft = 85;
        int barWidth = width - 165;
        int barTop = 7;
        int barHeight = height - 14;

        using var trackBrush = new SolidBrush(Color.FromArgb(42, 47, 58));
        g.FillRectangle(trackBrush, barLeft, barTop, barWidth, barHeight);

        int fillWidth = (int)(barWidth * _activation);
        if (fillWidth > 0)
        {
            Color fillColor = _isWinner ? Color.FromArgb(46, 204, 113) : Color.FromArgb(52, 152, 219);
            using var fillBrush = new SolidBrush(fillColor);
            g.FillRectangle(fillBrush, barLeft, barTop, fillWidth, barHeight);
        }

        // Percentage text
        string pctStr = $"{_activation * 100:F1}%";
        using var pctBrush = new SolidBrush(_isWinner ? Color.FromArgb(46, 204, 113) : Color.FromArgb(150, 155, 165));
        g.DrawString(pctStr, font, pctBrush, width - 65, 4);
    }
}
