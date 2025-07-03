using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json;
using SpaceMissions;


namespace SpaceMissions
{
    public interface IMissionDto
    {
        string ToJson();
    }

    public class WeightException : InvalidOperationException
    {
        public WeightException()
            : base("Unable to add item. Not enough weight.") { }
    }

    public class BudgetException : InvalidOperationException
    {
        public BudgetException()
            : base("Unable to add item. Not enough budget.") { }
    }

    public class MissionStatusException : InvalidOperationException
    {
        public MissionStatusException()
            : base("Mission is failed or finished.") { }
    }

    public enum MissionType
    {
        Colonization = 1,
        Satellite = 2
    }

    public enum MissionStatus
    {
        Active = 1,
        Paused = 2,
        Failed = 3,
        Finished = 4
    }

    public class Person
    {
        private readonly int _id;
        private readonly string _name;

        public int Id { get => _id; }
        public string Name { get => _name; }

        public Person(int id, string name)
        {
            if (id <0) throw new ArgumentException("id must be positive!");
            if (name is null) throw new ArgumentException("name must be not null!");

            _id = id; 
            _name = name;
        }
    }

    public class AstronautDto
    {
        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class Spaceship
    {
        private readonly int _id;
        private readonly string _model;
        private readonly double _maxWeight;
        private readonly double _maxSpeed;
        private readonly double _fuelFlowRate;

        public int Id { get => _id; }
        public string Model { get => _model; }
        public double MaxWeight { get => _maxWeight; }
        public double MaxSpeed { get => _maxSpeed; }
        public double FuelFlowRate { get => _fuelFlowRate; }

        public Spaceship(int id, string model, double maxWeight, double maxSpeed, double fuelFlowRate)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
            if (model is null) throw new ArgumentException("model must be not null!");
            if (maxWeight <= 0) throw new ArgumentException("maxWeight must be greater than zero!");
            if (maxSpeed <= 0) throw new ArgumentException("maxSpeed must be greater than zero!");
            if (fuelFlowRate <= 0) throw new ArgumentException("id must be greater than zero!");

            _id=id; _model= model; _maxWeight = maxWeight; _maxSpeed = maxSpeed; _fuelFlowRate= fuelFlowRate;
        }
    }


    public class SpaceshipDto
    {
        public string Model { get; set; }
        public double MaxSpeed { get; set; }
    }



    public class Item
    {
        private readonly int _id;
        private readonly string _name;
        private readonly double _weight;
        private readonly double? _price;

        public int Id { get => _id; }
        public string Name { get => _name; }
        public double Weight { get => _weight; }
        public double Price { get {if (_price.HasValue == true) return _price.Value; else return 0; } }

        public Item(int id, string name, double weight, double? price)
        {
            if ( id < 0) throw new ArgumentException("id must be positive!");
            if (name is null) throw new ArgumentException("name must be not null!");
            if (weight <= 0) throw new ArgumentException("weight must be greater than zero!");
            if (price == null) _price = 0; else _price = price;

                _id = id; _name = name; _weight = weight;
        }
    }


    public class PrepareMission
    {
        private readonly int _id;
        private string _name;
        private double _budget;
        private Spaceship _spaceship;
        private List<Item> _items;
        private List<(Person _astronaut, string _role)> _astronauts; //В 4.7.2 нет record
        private MissionType? _missionType;

        public int Id { get => _id; }
        public string Name { get => _name; set { if (value is null) throw new ArgumentException("name must be not null!"); else _name = value; } }
        public double Budget { get => _budget; set { if (value <= 0) throw new ArgumentException("budget must be greater than zero!"); else _budget = Budget; } }
        public Spaceship Spaceship { get => _spaceship; set { if (value is null) throw new ArgumentNullException("spaceship must be not null!"); else _spaceship = value; } }
        public List<Item> Items { get => _items; set {

                if (value is null) throw new ArgumentNullException("items must not null!");

                double itemsWeight=0, itemsCost=0;
                foreach (var itm in value)
                {
                    itemsWeight += itm.Weight;
                    itemsCost += itm.Price;
                }

                if (_spaceship.MaxWeight < itemsWeight) throw new WeightException();

                if (_budget < itemsWeight) throw new BudgetException();

                _items = value;
            }
        }
        public List<(Person _astronaut, string _role)> Astronauts { get => _astronauts; set { if (value is null) throw new ArgumentNullException("astronauts must be not null!"); _astronauts = value; } }
        public MissionType? MissionType { get => _missionType; set { _missionType = value; } }

        public PrepareMission(int id)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
            _id = id;

            _items = new List<Item>();
            _astronauts = new List<(Person _astronaut, string _role)>();
        }

        public PrepareMission(int id, string name, MissionType? missionType, Spaceship spaceship = null, List<Item> items = null, List<(Person _astronaut, string _role)> astronauts = null, double budget = 0)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
            if (name is null) throw new ArgumentException("name must be not null!");
            if (budget <= 0) throw new ArgumentException("budget must be greater than zero!");

            _id = id; _name = name; _budget = budget; _spaceship = spaceship; _items = items; _astronauts = astronauts; _missionType = missionType;

            if (items == null) _items = new List<Item>();
            if (astronauts == null) _astronauts = new List<(Person _astronaut, string _role)>();
        }

        public void AddItem(Item item)
        {
            if (item is null) throw new ArgumentNullException("item must not null!");

            double curentWeight = 0, curentCost = 0;
      
            foreach (var itm in _items)
            {
                curentWeight += itm.Weight;
                curentCost += itm.Price;
            }
                

            if (curentWeight + item.Weight > _spaceship.MaxWeight) throw new WeightException();
            if (curentWeight + item.Price > Budget) throw new BudgetException();

            _items.Add(item);
        }

        public void RemoveItem(int id)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");

            foreach (var itm in _items)
            {
                if (itm.Id == id) _items.Remove(itm);
            }
        }

        public void AddAstronaut(Person astronaut, string role)
        {
            if (astronaut == null) throw new ArgumentNullException("astronaut must be not null!");
            if (role == null) throw new ArgumentNullException("role must be not null!");

            _astronauts.Add((astronaut, role));

        }

        public void RemoveAstronaut(int id)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");

            foreach (var astr in _astronauts)
            {
                if (astr._astronaut.Id == id) _astronauts.Remove(astr);
            }

        }

        public double GetTotalWeight()
        {
            double totalWeight = 0;
            foreach (var itm in _items)
            {
                totalWeight += itm.Weight;
            }
            return totalWeight;
        }

        public double GetTotalCost()
        {
            double curentCost = 0;
            foreach (var itm in _items)
            {
                curentCost += itm.Price;
            }
            return curentCost;
        }

        public Mission Launch()
        {
            if (_name is null) throw new ArgumentNullException("name must be not null!");
            if (_budget == 0) throw new ArgumentNullException("budget must be not null!");
            if (_spaceship is null) throw new ArgumentNullException("spaceship must be not null!");
            if (_items.Count == 0) throw new ArgumentNullException("items must be not null!");
            if (_astronauts.Count == 0 || _astronauts == null) throw new ArgumentNullException("astronauts must be not null!");
            if (_missionType is null) throw new ArgumentNullException("missiontype must be not null!");


            //СОЗДАНИЕ МИССИЙ

            double totalCost = _items.Sum(a => a.Price);

            switch (_missionType)
            {
                case SpaceMissions.MissionType.Colonization:
                    {
                        return new ColonizationMission(_id, _name, _astronauts, _spaceship, totalCost, DateTime.Now);
                    }

                case SpaceMissions.MissionType.Satellite:
                    {
                        return new SatelliteMission(_id, _name, _astronauts, _spaceship, totalCost, DateTime.Now);
                    }

            }

            return null;
        }

    }

    public abstract class Mission
    {
        private readonly int _id; public int Id { get => _id; }
        private readonly string _name;  public string Name { get => _name; }
        private readonly DateTime _startDate; public DateTime StartDate { get => _startDate; }
        private DateTime? _endDate; public DateTime? EndDate { get => _endDate; set => _endDate = value; }
        private readonly List<(Person _astronaut, string _role)> _astronauts; public List<(Person _astronaut, string _role)> Astronauts { get => _astronauts; }
        private readonly Spaceship _spaceship; public Spaceship Spaceship { get => _spaceship; }
        private readonly double _totalCost; public double TotalCost { get => _totalCost; }
        private MissionStatus _missionStatus; public MissionStatus MissionStatus { get => _missionStatus; set => _missionStatus = value; }

        protected Mission(int id, string name, List<(Person astronaut, string role)> astronauts, Spaceship spaceship, double totalCost, DateTime startDate, MissionStatus status = MissionStatus.Active)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
            if (name is null) throw new ArgumentNullException("name must be not null!");
            if (astronauts.Count == 0 || astronauts == null) throw new ArgumentNullException("astronauts must be not null!");
            if (spaceship is null) throw new ArgumentNullException("spaceship must be not null!");
            if (totalCost == 0) throw new ArgumentNullException("budget must be not null!");
            
            if (startDate > DateTime.Now.AddSeconds(10)) 
            {
                throw new ArgumentException("Incorrect date");
            }

            _id = id; _name = name; _astronauts = astronauts; _astronauts = astronauts; _totalCost = totalCost; _missionStatus = status;  _startDate = startDate; _spaceship = spaceship;
        }
        public abstract IMissionDto GetStats();

        public void SetStatus(MissionStatus status)
        {
            if (_missionStatus == MissionStatus.Failed || _missionStatus == MissionStatus.Finished) throw new MissionStatusException();
            _missionStatus=status;
            if (status == MissionStatus.Failed || status == MissionStatus.Finished) EndDate = DateTime.Now;
        }

    }

    public class SatelliteMission : Mission
    {
        private bool _isOnOrbit; public bool IsOnOrbit { get => _isOnOrbit; }

        public SatelliteMission(int id, string name, List<(Person astronaut, string role)> astronauts, Spaceship spaceship, double totalCost, DateTime startDate, bool isOnOrbit=false, MissionStatus status = MissionStatus.Active)
            : base(id, name, astronauts, spaceship, totalCost, startDate, MissionStatus.Active)
        {
            _isOnOrbit = isOnOrbit;
        }
        public TimeSpan CalculateOrbitDecay(double dragCoefficient)
        {
            return TimeSpan.FromDays(100 / dragCoefficient);
        }

        public void DeploySatellite()
        {
            if (MissionStatus == MissionStatus.Active)
            {
                _isOnOrbit = !_isOnOrbit;
            }
            else throw new ArgumentException("Mission is not active!");
        }

        //логика работы на орбите

        public override IMissionDto GetStats()
        {
            return new SatelliteMissionDto
            {
                Name = this.Name,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                TravelTime = DateTime.Now - this.StartDate,
                IsOnOrbit = this._isOnOrbit,
                Status = this.MissionStatus,
                Astronauts = this.Astronauts.Select(a => new AstronautDto
                {
                    Name = a._astronaut.Name,
                    Role = a._role
                }).ToList(),
                Spaceship = new SpaceshipDto
                {
                    Model = this.Spaceship.Model,
                    MaxSpeed = this.Spaceship.MaxSpeed
                }
            };
        }
    }

    public class SatelliteMissionDto : IMissionDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan TravelTime { get; set; }
        public bool IsOnOrbit { get; set; }
        public MissionStatus Status { get; set; }


        public List<AstronautDto> Astronauts { get; set; }
        public SpaceshipDto Spaceship { get; set; }


        public string ToJson() => JsonSerializer.Serialize(this); //

    }

    public class ColonizationMission : Mission
    {
        private List<Colony> _сolonies; public List<Colony> Colonies { get => _сolonies; }

        public ColonizationMission(int id, string name, List<(Person astronaut, string role)> astronauts, Spaceship spaceship, double totalCost, DateTime startDate, MissionStatus status = MissionStatus.Active, params Colony[] colonies)
          : base(id, name, astronauts, spaceship, totalCost, startDate, MissionStatus.Active)
        {
            _сolonies = new List<Colony>(colonies ?? Array.Empty<Colony>());
        }

        public void DeployColony(string name, DateTime date)
        {
            if (name is null) throw new ArgumentNullException("name must be not null!");
            if (MissionStatus == MissionStatus.Active)
            {
                _сolonies.Add(new Colony(name, date));
            }
            else throw new ArgumentException("Mission is not active!");

        }

        public void DeployColony(Colony colony)
        {
            if (MissionStatus == MissionStatus.Active)
            {
                _сolonies.Add(colony);
            }
            else throw new ArgumentException("Mission is not active!");
        }

        public override IMissionDto GetStats()
        {
            return new ColonizationMissionDto
            {
                Name = this.Name,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                TravelTime = DateTime.Now - this.StartDate,
                Status = this.MissionStatus,
                Astronauts = this.Astronauts.Select(a => new AstronautDto
                {
                    Name = a._astronaut.Name,
                    Role = a._role
                }).ToList(),

                Colonies = this.Colonies.Select(a => new ColonyDto
                {
                    Name = a.Name,
                    Date = a.StartDate
                }).ToList(),

                Spaceship = new SpaceshipDto
                {
                    Model = this.Spaceship.Model,
                    MaxSpeed = this.Spaceship.MaxSpeed
                }
            };


        }

    }

    public class Colony //Класс под расширение
    {
        private readonly string _name; public string Name { get => _name; }
        private readonly DateTime _startDate; public DateTime StartDate { get => _startDate; }

        public Colony(string name, DateTime date)
        {
            if (name is null) throw new ArgumentNullException("name must be not null!");

            _name = name; _startDate = date;
        }
    }

    public class ColonyDto
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }

    public class ColonizationMissionDto : IMissionDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan TravelTime { get; set; }
        public MissionStatus Status { get; set; }


        public List<AstronautDto> Astronauts { get; set; }
        public SpaceshipDto Spaceship { get; set; }
        public List<ColonyDto> Colonies { get; set; }

        public string ToJson() => JsonSerializer.Serialize(this); //

    }

}










namespace SpaceMissionAnalyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Person p1 = new Person(1, "Ivanov");
            Person p2 = new Person(2, "Petrov");

            Spaceship sp = new Spaceship(1, "Rocket-121", 1000, 200, 10);

            PrepareMission someMission = new PrepareMission(202, "ColonizationMars2025", MissionType.Colonization, sp,null,null,1000); //Weight - 1000, budget - 1000

            Item expItm = new Item(10, "EXPENSIVE_ITEM", 100, 5000);
            Item bigItm = new Item(12, "BIG_ITEM", 10000, 500);
            Item Itm = new Item(13, "Some_item", 100, 100);

           // someMission.AddItem(expItm);
           // someMission.AddItem(bigItm);
            someMission.AddItem(Itm);
            someMission.AddAstronaut(p1, "Capitan");
            someMission.AddAstronaut(p2, "Helper");
            ColonizationMission clMs; ColonizationMissionDto dto1;
            try
            {
                clMs = (ColonizationMission)someMission.Launch();
                dto1 = (ColonizationMissionDto)clMs.GetStats();
                

            }
            catch (Exception ex)
            {
                throw new Exception("error1");
            }

            int a = 2;
            int b = 3; 
        }
    }
}
