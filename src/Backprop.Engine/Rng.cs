namespace Backprop;

/// <summary>
/// Provides a shared, seedable pseudo-random number generator for initializing network weights and biases.
/// </summary>
public static class Rng
{
    private static Random _rand = new();

    /// <summary>
    /// Seeds the underlying random number generator to ensure reproducible initializations.
    /// </summary>
    /// <param name="seed">The integer seed value.</param>
    public static void Seed(int seed)
    {
        _rand = new Random(seed);
    }

    /// <summary>
    /// Generates a random initial weight or bias in the uniform distribution [-0.5, 0.5).
    /// </summary>
    /// <returns>A random floating-point value between -0.5 (inclusive) and 0.5 (exclusive).</returns>
    public static double NextWeight()
    {
        int maxLimit = 1000;
        int minLimit = -1000;
        return (double)_rand.Next(minLimit, maxLimit) / 2000.0;
    }
}
