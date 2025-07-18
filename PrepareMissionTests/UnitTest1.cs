using SpaceMissions;
namespace PrepareMissionTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        //constr

        [Test]
        [TestCaseSource(nameof(InvalidMissionTestCases))]
        public void GIVEN_InvalidParameters_WHEN_CreatingPrepareMission_THEN_ThrowsException(int id, string name, MissionType? missionType, Spaceship spaceship = null,
                             List<Item> items = null, List<(Person _astronaut, string _role)> astronauts = null, double budget = 0)
        {
            Assert.Throws<ArgumentException>(() => new PrepareMission(id, name, missionType, spaceship, items, astronauts, budget));
        }

        [Test]
        [TestCaseSource(nameof(CorrectMissionArgumentsCases))]
        public void GIVEN_CorrectParameters_WHEN_CreatingPrepareMission_THEN_DoesNotThrow(int id, string name, MissionType? missionType, Spaceship spaceship = null,
                             List<Item> items = null, List<(Person _astronaut, string _role)> astronauts = null, double budget = 0)
        {
            Assert.DoesNotThrow(() => new PrepareMission(id, name, missionType, spaceship, items, astronauts, budget));
        }


        //prop set
        [Test]
        [TestCaseSource(nameof(InvalidItemsSetTestCases))]
        public void GIVEN_CorrectParameter_WHEN_ItemsSet_THEN_ThrowsException(int id, string name, double weight, double price)
        {
            List<Item> itm = new List<Item>() { new Item(id, name, weight, price) };
            PrepareMission ms = new PrepareMission(1);
            ms.Budget = 100;
            ms.Spaceship = new Spaceship(1, "spaceship", 100, 100, 100);
            try
            {
                ms.Items = itm; Assert.Fail("Ожидалось исключение, но оно не было выброшено.");
            }
            catch
            {
                Assert.Pass();
            }
        }

        //methods

        [Test]
        [TestCaseSource(nameof(CorrectPrepareMissionCases))]
        public void GIVEN_MissionType_WHEN_ShipIsLaunching_THEN_СreatingMission(PrepareMission mission)
        {
            switch (mission.MissionType)
            {
                case MissionType.Satellite:
                    {
                        Assert.IsTrue(mission.Launch() is SatelliteMission); return;
                    }

                case MissionType.Colonization:
                    {
                        Assert.IsTrue(mission.Launch() is ColonizationMission); return;
                    }
            }

            Assert.Pass();
        }

        [Test]
        [TestCaseSource(nameof(InvalidItemsTestCases))]
        public void GIVEN_InvalidItem_WHEN_AddItem_THEN_ThrowsException(Item itm)
        {
            PrepareMission mission = new PrepareMission(
                10,
                "mission",
                MissionType.Satellite,
                new Spaceship(1, "Falcon", 200, 200, 200),
                new List<Item>() { new Item(1, "item", 1, 1) },
                new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
                10000
            );

            try
            {
                mission.AddItem(itm); Assert.Fail("Ожидалось исключение, но оно не было выброшено.");
            }
            catch
            {
                Assert.Pass();
            }

        }
        
            






        private static IEnumerable<TestCaseData> InvalidMissionTestCases()
        {
            yield return new TestCaseData(
                -1,
                "mission",
                MissionType.Satellite,
                new Spaceship(1, "Falcon", 200, 200, 200),
                null,
                new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
                10000
            ).SetName("Negative_Id");

            yield return new TestCaseData(
                1,
                "",
                MissionType.Satellite,
                new Spaceship(1, "Falcon", 200, 200, 200),
                new List<Item> { new Item(1, "Oxygen", 50) },
                null,
                10000
            ).SetName("Empty_Name");

            yield return new TestCaseData(
               1,
               "Mission",
               MissionType.Satellite,
               null,
               new List<Item> { new Item(1, "Oxygen", 50) },
               new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
               -10000
           ).SetName("Negative_Budget");

        }
        private static IEnumerable<TestCaseData> CorrectMissionArgumentsCases()
        {
            yield return new TestCaseData(
                10,
                "mission",
                MissionType.Satellite,
                new Spaceship(1, "Falcon", 200, 200, 200),
                null,
                new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
                10000
            ).SetName("SatelliteMission");

            yield return new TestCaseData(
               10,
               "mission",
               MissionType.Colonization,
               new Spaceship(1, "Falcon", 200, 200, 200),
               null,
               new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
               10000
           ).SetName("ColonizationMission");
        }
        private static IEnumerable<TestCaseData> InvalidItemsSetTestCases()
        {
            yield return new TestCaseData(
                1,
                "BigItem",
                1000000,
                1).SetName("HighWeight_Item");

            yield return new TestCaseData(
                1,
                "ExpensiveItem",
                1,
                100000).SetName("Expensive_Item");
        }
        private static IEnumerable<PrepareMission> CorrectPrepareMissionCases()
        {
            yield return new PrepareMission(
                10,
                "mission",
                MissionType.Satellite,
                new Spaceship(1, "Falcon", 200, 200, 200),
                new List<Item>() { new Item(1, "item", 1, 1) },
                new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
                10000
            );

            yield return new PrepareMission(
               10,
               "mission",
               MissionType.Colonization,
               new Spaceship(1, "Falcon", 200, 200, 200),
               new List<Item>() { new Item(1,"item",1,1) },
               new List<(Person, string)> { (new Person(1, "Ivan"), "pilot") },
               10000
           );
        }
        private static IEnumerable<Item> InvalidItemsTestCases()
        {
            yield return new Item(
                1,
                "BigItem",
                1000000,
                1);

            yield return new Item(
                1,
                "ExpensiveItem",
                1,
                100000);
        }











    }



}