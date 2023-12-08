namespace CraigMiller.Map.Core.Engine
{
    public static class MathHelper
    {
        public const double HalfPi= Math.PI / 2.0;
        public const float HalfPiF = MathF.PI / 2f;

        public static void AngleAndDistanceBetweenPoints(double xA, double yA, double xB, double yB, out double radians, out double distance)
        {
            double xDiff = xA - xB, yDiff = yA - yB;

            radians = Math.Atan2(yDiff, xDiff);
            distance = Math.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static void AngleAndDistanceBetweenPoints(float xA, float yA, float xB, float yB, out float radians, out float distance)
        {
            float xDiff = xA - xB, yDiff = yA - yB;

            radians = MathF.Atan2(yDiff, xDiff);
            distance = MathF.Sqrt((xDiff * xDiff) + (yDiff * yDiff));
        }

        public static double CanvasAngle(double radians) => (HalfPi + radians + Math.Tau) % Math.Tau;

        public static float CanvasAngle(float radians) => (HalfPiF + radians + MathF.Tau) % MathF.Tau;

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

        /// <summary>
        /// Normalises <paramref name="radians"/> to between 0 and 2PI
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float NormaliseAngle(float radians)
        {
            if (radians < 0)
            {
                return (radians % MathF.Tau) + MathF.Tau;
            }
            if (radians >= MathF.Tau)
            {
                return radians % MathF.Tau;
            }

            return radians;
        }

        public static double RadsToDegs(double radians) => radians / Math.PI * 180.0;

        public static float RadsToDegs(float radians) => radians / MathF.PI * 180f;

        public static double DegsToRads(double degrees) => degrees / 180.0 * Math.PI;

        public static float DegsToRads(float degrees) => degrees / 180f * MathF.PI;

        public static bool IsPointOnLine(double xA, double yA, double xB, double yB, double xCheck, double yCheck, double tolerance)
        {
            double xDiff = xB - xA;
            // Calculate slope and y intercept of the line
            double m = xDiff == 0 ? double.PositiveInfinity : (yB - yA) / xDiff;
            double b = yA - m * xA;

            // Calculate expected y coordinate on the line for the check point
            double yExpected = m * xCheck + b;

            return Math.Abs(yExpected - yCheck) <= tolerance;
        }

        public static bool IsPointOnLine(float xA, float yA, float xB, float yB, float xCheck, float yCheck, float tolerance)
        {
            float xDiff = xB - xA;
            // Calculate slope and y intercept of the line
            float m = xDiff == 0 ? float.PositiveInfinity : (yB - yA) / xDiff;
            float b = yA - m * xA;

            // Calculate expected y coordinate on the line for the check point
            float yExpected = m * xCheck + b;

            return MathF.Abs(yExpected - yCheck) <= tolerance;
        }
    }
}
