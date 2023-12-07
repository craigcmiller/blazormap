namespace CraigMiller.Map.Core.Animation
{
    public static class RateFunctions
    {
        public static double AccelerateDecelerate(double ratio)
        {
            ratio *= 2.0;

            if (ratio <= 1.0)
            {
                ratio = Math.Pow(ratio, 2.0);
            }
            else
            {
                ratio = 2.0 - Math.Pow(1 - (ratio - 1.0), 2.0);
            }

            return ratio / 2.0;
        }

        public static double DecelerateToStop(double ratioOfDuration) => Math.Pow(1.0 - ratioOfDuration, 2.0);

        public static double Linear(double ratio) => ratio;
    }

    public delegate double RateFunction(double ratioOfDuration);
}
