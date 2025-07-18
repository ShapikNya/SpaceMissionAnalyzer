using SpaceMissions;
namespace SpaceshipTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(-1, "",-1,0,0)]
        [TestCase(-1, "Rocket",100,50,25)]
        [TestCase(1, "Rocket", 100, 0, 25)]
        public void GIVEN_InvalidParameters_WHEN_CreatingSpaceship_THEN_ThrowsException(int id, string model, double maxWeight, double maxSpeed, double fuelFlowRate)
        {
            Assert.Throws<ArgumentException>(() => new Spaceship(id, model, maxWeight, maxSpeed, fuelFlowRate));
        }

        [Test]
        [TestCase(1, "Rocket", 100, 50, 25)]
        [TestCase(140, "Rocket2", 2200, 50.12, 25)]
        public void GIVEN_CorrectParameters_WHEN_CreatingSpaceship_THEN_DoesNotThrow(int id, string model, double maxWeight, double maxSpeed, double fuelFlowRate)
        {
            Assert.DoesNotThrow(() => new Spaceship(id, model, maxWeight, maxSpeed, fuelFlowRate));
        }


    }
}