namespace CraigMiller.Map.Core.Engine
{
    public static class MathHelper
    {
        /// <summary>
        /// Calculates the angle in radians from <paramref name="fromRadians"/> to <paramref name="toRadians"/>
        /// </summary>
        /// <param name="fromRadians"></param>
        /// <param name="toRadians"></param>
        /// <returns></returns>
        public static double AngleBetween(double fromRadians, double toRadians)
        {
            double aDiff = fromRadians - toRadians;
            double bDiff = toRadians - fromRadians;

            double aNorm = NormaliseAngle(aDiff);
            double bNorm = NormaliseAngle(bDiff);

            double smallestAngle = Math.Min(aNorm, bNorm);

            return Math.Abs(NormaliseAngle(fromRadians + smallestAngle) - toRadians) < 1e-9
                ? smallestAngle
                : -smallestAngle;
        }

        /// <summary>
        /// Normalises <paramref name="radians"/> to between 0 and 2PI
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double NormaliseAngle(double radians)
        {
            if (radians < 0)
            {
                return (radians % Math.Tau) + Math.Tau;
            }
            if (radians >= Math.Tau)
            {
                return radians % Math.Tau;
            }

            return radians;
        }

        public static double RadsToDegs(double radians) => radians / Math.PI * 180.0;

        public static double DegsToRads(double degrees) => degrees / 180.0 * Math.PI;
    }
}
