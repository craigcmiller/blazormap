using static CraigMiller.Map.Core.Engine.MathHelper;

namespace CraigMiller.Map.Core.Tests
{
    public class MathHelperTests
    {
        const double Tolerance = 1e-9;

        [Theory]
        [InlineData(0, 0)]
        [InlineData(180, 180)]
        [InlineData(360, 0)]
        [InlineData(-90, 270)]
        [InlineData(450, 90)]
        [InlineData(180 - 720, 180)]
        public void NormaliseAngleTests(double unnormalised, double expectedDegrees)
        {
            double rads = NormaliseAngle(DegsToRads(unnormalised));
            Assert.Equal(expectedDegrees, RadsToDegs(rads), Tolerance);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, 45, 45)]
        [InlineData(45, 0, -45)]
        [InlineData(45, 315, -90)]
        [InlineData(315, 45, 90)]
        [InlineData(190, 280, 90)]
        [InlineData(280, 190, -90)]
        public void AngleBetweenTests(double fromDegrees, double toDegrees, double expectedDeltaDegrees)
        {
            double rads = AngleBetween(DegsToRads(fromDegrees), DegsToRads(toDegrees));

            Assert.Equal(expectedDeltaDegrees, RadsToDegs(rads), Tolerance);
        }
    }
}