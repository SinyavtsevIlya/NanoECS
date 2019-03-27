public static class IntExtensions
{
    public static int GetPowerOfTwoSize(int n)
    {
        if (n < 2)
        {
            return 2;
        }
        n--;
        n = n | (n >> 1);
        n = n | (n >> 2);
        n = n | (n >> 4);
        n = n | (n >> 8);
        n = n | (n >> 16);
        return n + 1;
    }
}