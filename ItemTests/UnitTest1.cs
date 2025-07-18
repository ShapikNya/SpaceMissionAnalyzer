using SpaceMissions;
using System.Xml.Linq;
namespace ItemTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(-1, "Item1", 100, 100)]
        [TestCase(1, "", 100, 100)]
        [TestCase(1, "Item3", -100, 100)]
        [TestCase(1, "Item4", +100, -100)]
        public void GIVEN_InvalidParameters_WHEN_CreatingPerson_THEN_ThrowsException(int id, string name, double weight, double price)
        {
            Assert.Throws<ArgumentException>(() => new Item(id, name, weight, price));
        }

        [Test]
        [TestCase(1, "Item1", 100, 100)]
        [TestCase(1, "Item1", 100)]
        public void GIVEN_CorrectParameters_WHEN_CreatingPerson_THEN_DoesNotThrow(int id, string name, double weight, double price=0)
        {
            Assert.DoesNotThrow(() => new Item(id, name, weight, price));
        }
    }
}