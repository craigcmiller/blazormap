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

        /// <summary>
        /// Reverses the given rate function
        /// </summary>
        /// <param name="rateFunction"></param>
        /// <returns></returns>
        public static RateFunction Reverse(RateFunction rateFunction) => ratio => 1.0 - rateFunction(ratio);

        /// <summary>
        /// Makes the given rate function go forward and then reverse to its original state
        /// </summary>
        /// <param name="rateFunction"></param>
        /// <returns></returns>
        public static RateFunction FowardThenReverse(RateFunction rateFunction)
        {
            return ratio => ratio < 0.5 ? rateFunction(ratio * 2.0) : 1.0 - rateFunction((ratio - 0.5) * 2.0);
        }
    }

    public delegate double RateFunction(double ratioOfDuration);
}
