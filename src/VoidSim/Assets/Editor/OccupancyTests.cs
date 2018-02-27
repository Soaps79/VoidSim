using System.Linq;
using Assets.Placeables.Nodes;
using Assets.WorldMaterials.Population;
using NUnit.Framework;

namespace Assets.Editor
{
    public class OccupancyTests
    {
        [Test]
        public void PopContainer_OccupantCount()
        {
            const int maxOccupancy = 5;
            var container = new PopContainer(new PopContainerParams { MaxCapacity = maxOccupancy });
            
            Assert.AreEqual(maxOccupancy, container.CurrentOccupancy.Count);
        }

        [Test]
        public void PopContainer_AddOccupant()
        {
            const int maxOccupancy = 1;
            const int id = 55;
            var person = new Person{Id = id };

            var container = new PopContainer(new PopContainerParams { MaxCapacity = maxOccupancy });
            container.AddPerson(person);

            Assert.AreEqual(maxOccupancy, container.CurrentOccupancy.Count);

            var occupancy = container.CurrentOccupancy.First();
            Assert.IsTrue(occupancy.IsOccupied);
            Assert.IsNotNull(occupancy.OccupiedBy);
            Assert.AreEqual(id, occupancy.OccupiedBy.Id);
        }

        [Test]
        public void PopContainer_AddReserve()
        {
            const int maxOccupancy = 1;
            const int id = 55;
            var person = new Person { Id = id };

            var container = new PopContainer(new PopContainerParams { MaxCapacity = maxOccupancy });
            container.AddReserved(person);

            Assert.AreEqual(maxOccupancy, container.CurrentOccupancy.Count);

            var occupancy = container.CurrentOccupancy.First();
            Assert.AreEqual(id, occupancy.ReservedBy);
            Assert.IsTrue(occupancy.IsReserved);
        }

        [Test]
        public void PopContainer_RemoveReserve()
        {
            const int maxOccupancy = 1;
            const int id = 55;
            var person = new Person { Id = id };

            var container = new PopContainer(new PopContainerParams { MaxCapacity = maxOccupancy });
            container.AddReserved(person);
            container.RemoveReserved(person);
            
            Assert.AreEqual(maxOccupancy, container.CurrentOccupancy.Count);

            var occupancy = container.CurrentOccupancy.First();
            Assert.IsFalse(occupancy.IsReserved);
        }

        [Test]
        public void PopContainer_PlaceInReserve()
        {
            const int maxOccupancy = 5;
            const int firstId = 55;
            var firstPerson = new Person { Id = firstId };

            const int secondId = 56;
            var secondPerson = new Person { Id = secondId };

            var container = new PopContainer(new PopContainerParams {MaxCapacity = maxOccupancy});
            container.AddReserved(firstPerson);
            container.AddReserved(secondPerson);
            container.AddPerson(firstPerson);

            var occupied = container.CurrentOccupancy.Where(i => i.IsOccupied).ToList();
            Assert.AreEqual(1, occupied.Count);

            var occupancy = occupied.First();
            Assert.AreEqual(firstPerson, occupancy.OccupiedBy);
            Assert.AreEqual(firstId, occupancy.ReservedBy);
        }

        [Test]
        public void PopContainer_RemovePerson()
        {
            const int maxOccupancy = 5;
            const int firstId = 55;
            var firstPerson = new Person {Id = firstId };

            const int secondId = 56;
            var secondPerson = new Person { Id = secondId };

            var container = new PopContainer(new PopContainerParams { MaxCapacity = maxOccupancy });
            container.AddPerson(firstPerson);
            container.AddPerson(secondPerson);
            container.RemovePerson(firstPerson);

            var occupied = container.CurrentOccupancy.Where(i => i.IsOccupied).ToList();
            Assert.AreEqual(1, occupied.Count);
            Assert.AreEqual(secondPerson, occupied.First().OccupiedBy);
        }

    }
}