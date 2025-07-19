using SpaceMissions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MissionAnalyzerTests
{
    public class Tests
    {
        public MissionAnalyzer Analyzer;

        [SetUp]
        public void Setup()
        {
            Person p1 = new Person(1, "Ivanov");
            Person p2 = new Person(2, "Petrov");
            Spaceship sp = new Spaceship(1, "Rocket-121", 1000, 200, 10);

            SatelliteMission st_m1 = new SatelliteMission(MissionType.Satellite, 101, "Satellite1",
                new List<(Person, string)> { (p1, "pilot"), (p2, "helper") }, sp, 500, DateTime.Now, true, MissionStatus.Finished);

            SatelliteMission st_m2 = new SatelliteMission(MissionType.Satellite, 102, "Satellite2",
                new List<(Person, string)> { (p1, "pilot"), (p2, "helper") }, sp, 1000, DateTime.Now, true, MissionStatus.Finished);

            SatelliteMission st_m3 = new SatelliteMission(MissionType.Satellite, 103, "Satellite3",
                new List<(Person, string)> { (p1, "pilot"), (p2, "helper") }, sp, 1500, DateTime.Now, true, MissionStatus.Finished);

            ColonizationMission cl_m1 = new ColonizationMission(MissionType.Colonization, 201, "Colonization1",
                new List<(Person, string)> { (p1, "pilot"), (p2, "helper") }, sp, 750, new DateTime(2000, 1, 1), MissionStatus.Finished);


            Analyzer = new MissionAnalyzer(new List<Mission> { st_m1, st_m2, st_m3, cl_m1 });
        }

        [Test]
        public async Task GIVEN_Missions_WHEN_AllCalculated_THEN_ResultEqualCaseData()
        {
            Assert.AreEqual(3750, await Analyzer.CalculateAllTotalCostAsync());

        }

        [Test]
        public void GIVEN_Missions_WHEN_GetLongMissions_THEN_LongestMission()
        {
            var result = Analyzer.GetLongMission(1);
            var resultList = result.ToList();
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Empty, "Словарь результатов пуст. Проверьте, что у миссий задан EndDate");
                if (result.Any())
                {
                    Assert.That(result, Has.Count.EqualTo(1));
                    Assert.That(resultList[0].Key.Id, Is.EqualTo(201));
                }
            });
        }

        [Test]
        [TestCase(MissionType.Satellite, 3)]
        [TestCase(MissionType.Colonization, 1)]
        public void GIVEN_Missions_WHEN_GetMissionByType_THEN_ResultEqualCaseData(MissionType missionType, int count)
        {
            Assert.AreEqual(Analyzer.GetMissionByType(missionType).Count(), count);
        }

        [Test]
        public void GIVEN_Missions_WHEN_GetAllAstronauts_THEN_ResultEqualCaseData()
        {
            var result = Analyzer.GetAllAstronauts();
            var resultList = result.ToList();

            Assert.Multiple(() =>
            {
                if (result.Any())
                {
                    Assert.That(result, Has.Count.EqualTo(2));

                    Assert.That(result[1].Name, Is.EqualTo("Ivanov"));
                    Assert.That(result[2].Name, Is.EqualTo("Petrov"));
                }
            });
        }

    }
}