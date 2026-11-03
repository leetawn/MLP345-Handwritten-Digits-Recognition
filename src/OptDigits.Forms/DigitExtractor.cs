using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OptDigits.Forms;

/// <summary>
/// Preprocesses freehand drawing on canvas into normalized 64-dimensional feature vectors matching the UCI optdigits format.
/// </summary>
public static class DigitExtractor
{
    /// <summary>
    /// Extracts 64 normalized features from a canvas bitmap.
    /// Returns (features, downsampledPreview, hasDrawing).
    /// </summary>
    public static (double[] Features, Bitmap Preview8x8, bool HasDrawing) ExtractFeatures(Bitmap canvas)
    {
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;
        int activePixelCount = 0;

        // 1. Detect bounding box of drawn strokes
        for (int y = 0; y < canvas.Height; y++)
        {
            for (int x = 0; x < canvas.Width; x++)
            {
                Color c = canvas.GetPixel(x, y);
                // Background is black (0,0,0), stroke is white/bright
                if (c.R > 40 || c.G > 40 || c.B > 40)
                {
                    activePixelCount++;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        // Return empty if no significant strokes
        if (activePixelCount < 15 || minX > maxX || minY > maxY)
        {
            var emptyFeatures = new double[64];
            var emptyBmp = new Bitmap(8, 8);
            using (var g = Graphics.FromImage(emptyBmp))
            {
                g.Clear(Color.Black);
            }
            return (emptyFeatures, emptyBmp, false);
        }

        int strokeWidth = maxX - minX + 1;
        int strokeHeight = maxY - minY + 1;
        int maxDim = Math.Max(strokeWidth, strokeHeight);

        // 2. Center in a square bounding box with 20% margin
        int padding = (int)(maxDim * 0.20);
        int squareSize = maxDim + 2 * padding;

        float offsetX = minX - (squareSize - strokeWidth) / 2.0f;
        float offsetY = minY - (squareSize - strokeHeight) / 2.0f;

        // 3. Render into a 32x32 bitmap (optdigits source bitmap resolution)
        using var bmp32 = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bmp32))
        {
            g.Clear(Color.Black);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var srcRect = new RectangleF(offsetX, offsetY, squareSize, squareSize);
            var destRect = new Rectangle(0, 0, 32, 32);
            g.DrawImage(canvas, destRect, srcRect, GraphicsUnit.Pixel);
        }

        // 4. Compute 4x4 block pooling across 8x8 grid -> 64 features [0..16] / 16.0
        double[] features = new double[64];
        var preview = new Bitmap(8, 8);

        for (int by = 0; by < 8; by++)
        {
            for (int bx = 0; bx < 8; bx++)
            {
                double blockSum = 0.0;
                for (int py = 0; py < 4; py++)
                {
                    for (int px = 0; px < 4; px++)
                    {
                        int ix = bx * 4 + px;
                        int iy = by * 4 + py;
                        Color p = bmp32.GetPixel(ix, iy);
                        // Normalized intensity in [0, 1]
                        double intensity = (p.R + p.G + p.B) / (3.0 * 255.0);
                        if (intensity > 0.25)
                        {
                            blockSum += Math.Min(1.0, intensity * 1.2);
                        }
                    }
                }

                // Clamped to range [0, 16]
                double count = Math.Min(16.0, blockSum);
                double normalized = count / 16.0;
                features[by * 8 + bx] = normalized;

                int gray = (int)Math.Clamp(normalized * 255.0, 0, 255);
                preview.SetPixel(bx, by, Color.FromArgb(gray, gray, gray));
            }
        }

        return (features, preview, true);
    }
}

