namespace OptDigits.Data;

/// <summary>
/// Scales feature values linearly into the unit range [0, 1].
/// </summary>
/// <remarks>Owner: Partner B</remarks>
public sealed class MinMaxScaler
{
    private readonly double? _fixedMin;
    private readonly double? _fixedMax;
    private double[]? _minimums;
    private double[]? _maximums;

    /// <summary>
    /// Initializes a new instance of <see cref="MinMaxScaler"/>.
    /// If fixed minimum and maximum values are supplied, fitting is a no-op.
    /// </summary>
    /// <param name="fixedMin">Optional known minimum feature value (e.g., 0.0).</param>
    /// <param name="fixedMax">Optional known maximum feature value (e.g., 16.0).</param>
    public MinMaxScaler(double? fixedMin = null, double? fixedMax = null)
    {
        _fixedMin = fixedMin;
        _fixedMax = fixedMax;
    }

    /// <summary>
    /// Computes the per-feature minimum and maximum from training samples unless fixed bounds were provided.
    /// </summary>
    /// <param name="samples">The training samples to fit on.</param>
    public void Fit(IEnumerable<Sample> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);
        if (_fixedMin.HasValue && _fixedMax.HasValue)
        {
            return;
        }

        using IEnumerator<Sample> enumerator = samples.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            throw new ArgumentException("At least one sample is required to fit the scaler.", nameof(samples));
        }

        int featureCount = enumerator.Current.Features.Length;
        if (featureCount == 0)
        {
            throw new ArgumentException("Samples must contain at least one feature.", nameof(samples));
        }

        var minimums = (double[])enumerator.Current.Features.Clone();
        var maximums = (double[])enumerator.Current.Features.Clone();
        while (enumerator.MoveNext())
        {
            Sample sample = enumerator.Current;
            ValidateFeatureCount(sample, featureCount);
            for (int featureIndex = 0; featureIndex < featureCount; featureIndex++)
            {
                minimums[featureIndex] = Math.Min(minimums[featureIndex], sample.Features[featureIndex]);
                maximums[featureIndex] = Math.Max(maximums[featureIndex], sample.Features[featureIndex]);
            }
        }

        _minimums = minimums;
        _maximums = maximums;
    }

    /// <summary>
    /// Transforms a single sample's features to the [0, 1] interval using the fitted or fixed min/max bounds.
    /// Formula: x_scaled = (x - min) / (max - min).
    /// </summary>
    /// <param name="s">The sample to transform.</param>
    /// <returns>A new <see cref="Sample"/> with scaled features and unchanged label.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the scaler has not been fitted.</exception>
    public Sample Transform(Sample s)
    {
        ArgumentNullException.ThrowIfNull(s);
        double[] scaledFeatures = new double[s.Features.Length];
        if (_fixedMin.HasValue && _fixedMax.HasValue)
        {
            ScaleFeatures(s.Features, scaledFeatures, _fixedMin.Value, _fixedMax.Value);
            return new Sample(scaledFeatures, s.Label);
        }

        if (_minimums is null || _maximums is null)
        {
            throw new InvalidOperationException("The scaler must be fitted before Transform is called.");
        }

        ValidateFeatureCount(s, _minimums.Length);
        for (int featureIndex = 0; featureIndex < scaledFeatures.Length; featureIndex++)
        {
            scaledFeatures[featureIndex] = ScaleValue(
                s.Features[featureIndex], _minimums[featureIndex], _maximums[featureIndex]);
        }

        return new Sample(scaledFeatures, s.Label);
    }

    /// <summary>
    /// Transforms an entire collection of samples using the fitted or fixed min/max bounds.
    /// </summary>
    /// <param name="samples">Collection of samples to scale.</param>
    /// <returns>Read-only list of scaled samples.</returns>
    public IReadOnlyList<Sample> TransformAll(IEnumerable<Sample> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);
        return samples.Select(Transform).ToList();
    }

    private static void ScaleFeatures(double[] source, double[] destination, double minimum, double maximum)
    {
        for (int featureIndex = 0; featureIndex < source.Length; featureIndex++)
        {
            destination[featureIndex] = ScaleValue(source[featureIndex], minimum, maximum);
        }
    }

    private static double ScaleValue(double value, double minimum, double maximum)
    {
        return maximum == minimum ? 0.0 : (value - minimum) / (maximum - minimum);
    }

    private static void ValidateFeatureCount(Sample sample, int expectedFeatureCount)
    {
        if (sample.Features.Length != expectedFeatureCount)
        {
            throw new ArgumentException(
                $"All samples must have {expectedFeatureCount} features, but one sample has {sample.Features.Length}.",
                nameof(sample));
        }
    }
}
