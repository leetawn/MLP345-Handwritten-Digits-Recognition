namespace Backprop;

public static class Rng
{
    private static Random _rand = new();

    public static void Seed(int seed)
    {
        _rand = new Random(seed);
    }

    public static double NextWeight()
    {
        int maxLimit = 1000;
        int minLimit = -1000;
        return (double)_rand.Next(minLimit, maxLimit) / 2000.0;
    }
}
