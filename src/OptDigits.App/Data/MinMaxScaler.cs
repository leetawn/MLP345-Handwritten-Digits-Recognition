namespace OptDigits.Data;

/// <summary>
/// Scales feature values linearly into the unit range [0, 1].
/// </summary>
/// <remarks>Owner: Partner B</remarks>
public sealed class MinMaxScaler
{
    private readonly double? _fixedMin;
    private readonly double? _fixedMax;

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
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public void Fit(IEnumerable<Sample> samples)
    {
        // TODO 1: Check if _fixedMin and _fixedMax are provided; if so, return immediately (no-op)
        // TODO 2: Otherwise iterate over all samples and find the min and max value for each feature index
        // TODO 3: Store per-feature minimum and maximum vectors for use in Transform
        throw new NotImplementedException("MinMaxScaler.Fit is not implemented yet");
    }

    /// <summary>
    /// Transforms a single sample's features to the [0, 1] interval using the fitted or fixed min/max bounds.
    /// Formula: x_scaled = (x - min) / (max - min).
    /// </summary>
    /// <param name="s">The sample to transform.</param>
    /// <returns>A new <see cref="Sample"/> with scaled features and unchanged label.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public Sample Transform(Sample s)
    {
        // TODO 1: Allocate a new double[] array matching s.Features.Length
        // TODO 2: For each feature, calculate (val - min) / (max - min) using fixed or fitted bounds
        // TODO 3: Clamp result to [0.0, 1.0] to guard against out-of-range values
        // TODO 4: Return new Sample(scaledFeatures, s.Label)
        throw new NotImplementedException("MinMaxScaler.Transform is not implemented yet");
    }

    /// <summary>
    /// Transforms an entire collection of samples using the fitted or fixed min/max bounds.
    /// </summary>
    /// <param name="samples">Collection of samples to scale.</param>
    /// <returns>Read-only list of scaled samples.</returns>
    /// <exception cref="NotImplementedException">Thrown because this method is a stub.</exception>
    public IReadOnlyList<Sample> TransformAll(IEnumerable<Sample> samples)
    {
        // TODO 1: Iterate over samples and call Transform on each
        // TODO 2: Return transformed samples as an IReadOnlyList<Sample>
        throw new NotImplementedException("MinMaxScaler.TransformAll is not implemented yet");
    }
}
