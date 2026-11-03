namespace Backprop;

// Shared seedable random number generator for network weight initialization.
public static class Rng
{
    private static Random _rand = new();

    // Seeds the RNG for reproducible weight initialization.
    public static void Seed(int seed)
    {
        _rand = new Random(seed);
    }

    // Returns a random value in [-0.5, 0.5) for weight/bias initialization.
    public static double NextWeight()
    {
        int maxLimit = 1000;
        int minLimit = -1000;
        return (double)_rand.Next(minLimit, maxLimit) / 2000.0;
    }
}
