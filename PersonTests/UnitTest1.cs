using SpaceMissions;
using System.Xml.Linq;

namespace PersonTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        [TestCase(-1, "Test")]
        [TestCase(1, "")]
        public void GIVEN_InvalidParameters_WHEN_CreatingPerson_THEN_ThrowsException(int id, string name)
        {
            Assert.Throws<ArgumentException>(() => new Person(id, name));
        }

        [Test]
        [TestCase(100,"Ivan")]
        public void GIVEN_CorrectParameters_WHEN_CreatingPerson_THEN_DoesNotThrow(int id, string name)
        {
            Assert.DoesNotThrow(() => new Person(id, name));
        }

    }
}