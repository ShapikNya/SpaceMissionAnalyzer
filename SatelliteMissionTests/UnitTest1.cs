using SpaceMissions;
using System.Diagnostics;
namespace SatelliteMissionTests
{
    public class Tests
    {
        public SatelliteMission mission;

        [SetUp]
        public void Setup()
        {
                mission = new SatelliteMission(MissionType.Satellite, 1, "mission",
                new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") }, new Spaceship(1, "Falcon", 200, 200, 200),
                100, DateTime.Now, default, default
                );
        }

        [Test]
        [TestCase (2,50)]
        [TestCase(5, 20)]
        [TestCase(77, (double)100/77)]
        public void GIVEN_CorrectDragCoef_WHEN_CalculateOrbitDecay_THEN_ResultEqualCaseData(int dragCoef, double res)
        {
            Assert.AreEqual(mission.CalculateOrbitDecay(dragCoef), TimeSpan.FromDays(res));
        }


        [Test]
        public async Task GIVEN_CalculateOrbitDecayAsync_WHEN_ExecutionTime_THEN_IsReasonable()
        {
            var MaxDuration = TimeSpan.FromSeconds(11);
            var Timer = Stopwatch.StartNew();

            await mission.CalculateOrbitDecayAsync(1);

            Assert.That(Timer.Elapsed, Is.LessThan(MaxDuration));
        }

    }
}