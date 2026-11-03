namespace OptDigits.Data;

// Scales feature values linearly into the unit range [0, 1].
public sealed class MinMaxScaler
{
    private readonly double? _fixedMin;
    private readonly double? _fixedMax;
    private double[]? _minimums;
    private double[]? _maximums;

    // Initializes a new instance of MinMaxScaler.
    // If fixed minimum and maximum values are supplied, fitting is a no-op.
    // fixedMin: Optional known minimum feature value (e.g., 0.0).
    // fixedMax: Optional known maximum feature value (e.g., 16.0).
    public MinMaxScaler(double? fixedMin = null, double? fixedMax = null)
    {
        _fixedMin = fixedMin;
        _fixedMax = fixedMax;
    }

    // Computes the per-feature minimum and maximum from training samples unless fixed bounds were provided.
    // samples: The training samples to fit on.
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

    // Transforms a single sample's features using the fitted or fixed min/max bounds without clamping.
    // Formula: x_scaled = (x - min) / (max - min).
    // s: The sample to transform.
    // Returns: A new Sample with scaled features and unchanged label.
    // Throws InvalidOperationException: Thrown when the scaler has not been fitted.
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

    // Transforms an entire collection of samples using the fitted or fixed min/max bounds.
    // samples: Collection of samples to scale.
    // Returns: Read-only list of scaled samples.
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
