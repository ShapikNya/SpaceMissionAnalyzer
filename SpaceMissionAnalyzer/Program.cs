using SpaceMissions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


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
            if (id < 0) throw new ArgumentException("id must be positive!");
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

            _id = id; _model = model; _maxWeight = maxWeight; _maxSpeed = maxSpeed; _fuelFlowRate = fuelFlowRate;
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
        public double Price { get { if (_price.HasValue == true) return _price.Value; else return 0; } }

        public Item(int id, string name, double weight, double? price)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
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
        private List<(Person _astronaut, string _role)> _astronauts;
        private MissionType? _missionType;

        public int Id => _id;
        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException(nameof(value), "Name must not be null!");
        }
        public double Budget
        {
            get => _budget;
            set
            {
                if (value <= 0) throw new ArgumentException("Budget must be greater than zero!", nameof(value));
                _budget = value;
            }
        }
        public Spaceship Spaceship
        {
            get => _spaceship;
            set => _spaceship = value ?? throw new ArgumentNullException(nameof(value), "Spaceship must not be null!");
        }
        public List<Item> Items
        {
            get => _items;
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value), "Items must not be null!");
                ValidateItems(value);
                _items = value;
            }
        }
        public List<(Person _astronaut, string _role)> Astronauts
        {
            get => _astronauts;
            set => _astronauts = value ?? throw new ArgumentNullException(nameof(value), "Astronauts must not be null!");
        }
        public MissionType? MissionType
        {
            get => _missionType;
            set => _missionType = value;
        }

        public PrepareMission(int id)
        {
            if (id < 0) throw new ArgumentException("Id must be positive!", nameof(id));
            _id = id;
            _items = new List<Item>();
            _astronauts = new List<(Person _astronaut, string _role)>();
        }

        public PrepareMission(int id, string name, MissionType? missionType, Spaceship spaceship = null,
                             List<Item> items = null, List<(Person _astronaut, string _role)> astronauts = null,
                             double budget = 0)
        {
            if (id < 0) throw new ArgumentException("Id must be positive!", nameof(id));
            if (name == null) throw new ArgumentNullException(nameof(name), "Name must not be null!");
            if (budget <= 0) throw new ArgumentException("Budget must be greater than zero!", nameof(budget));

            _id = id;
            _name = name;
            _budget = budget;
            _spaceship = spaceship;
            _items = items ?? new List<Item>();
            _astronauts = astronauts ?? new List<(Person _astronaut, string _role)>();
            _missionType = missionType;

            ValidateItems(_items);
        }

        public void AddItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item), "Item must not be null!");

            var tempItems = new List<Item>(_items) { item };
            ValidateItems(tempItems);
            _items.Add(item);
        }

        public void RemoveItem(int id)
        {
            if (id < 0) throw new ArgumentException("Id must be positive!", nameof(id));
            _items.RemoveAll(item => item.Id == id);
        }

        public void AddAstronaut(Person astronaut, string role)
        {
            if (astronaut == null) throw new ArgumentNullException(nameof(astronaut), "Astronaut must not be null!");
            if (role == null) throw new ArgumentNullException(nameof(role), "Role must not be null!");
            _astronauts.Add((astronaut, role));
        }

        public void RemoveAstronaut(int id)
        {
            if (id < 0) throw new ArgumentException("Id must be positive!", nameof(id));
            _astronauts.RemoveAll(astr => astr._astronaut.Id == id);
        }

        public double GetTotalWeight()
        {
            return _items.Sum(i => i.Weight);
        }

        public double GetTotalCost()
        {
            return _items.Sum(i => i.Price);
        }

        public Mission Launch()
        {
            ValidateLaunch();
            double totalCost = GetTotalCost();

            switch (_missionType)
            {
                case SpaceMissions.MissionType.Colonization:
                    return new ColonizationMission(_missionType.Value, _id, _name, _astronauts,
                        _spaceship, totalCost, DateTime.Now, MissionStatus.Active);
                case SpaceMissions.MissionType.Satellite:
                    return new SatelliteMission(_missionType.Value, _id, _name, _astronauts,
                        _spaceship, totalCost, DateTime.Now, false, MissionStatus.Active);
                default:
                    throw new InvalidOperationException("Mission type is not specified!");
            }
        }

        private void ValidateLaunch()
        {
            if (_name == null) throw new ArgumentNullException(nameof(_name), "Name must not be null!");
            if (_budget == 0) throw new ArgumentNullException(nameof(_budget), "Budget must not be null!");
            if (_spaceship == null) throw new ArgumentNullException(nameof(_spaceship), "Spaceship must not be null!");
            if (_items.Count == 0) throw new ArgumentNullException(nameof(_items), "Items must not be null!");
            if (_astronauts == null || _astronauts.Count == 0) throw new ArgumentNullException(nameof(_astronauts), "Astronauts must not be null!");
            if (_missionType == null) throw new ArgumentNullException(nameof(_missionType), "Mission type must not be null!");
            ValidateItems(_items);
        }

        private void ValidateItems(List<Item> items)
        {
            double itemsWeight = items.Sum(i => i.Weight);
            double itemsCost = items.Sum(i => i.Price);

            if (_spaceship != null && _spaceship.MaxWeight < itemsWeight)
                throw new WeightException();
            if (itemsCost > _budget)
                throw new BudgetException();
        }
    }


    public abstract class Mission
    {
        private readonly MissionType _missionType; public MissionType missionType { get => _missionType; }
        private readonly int _id; public int Id { get => _id; }
        private readonly string _name; public string Name { get => _name; }
        private readonly DateTime _startDate; public DateTime StartDate { get => _startDate; }
        private DateTime? _endDate; public DateTime? EndDate { get => _endDate; set { EnsureMissionIsActive(); _endDate = value; } }
        private readonly List<(Person _astronaut, string _role)> _astronauts; public List<(Person _astronaut, string _role)> Astronauts { get => _astronauts; }
        private readonly Spaceship _spaceship; public Spaceship Spaceship { get => _spaceship; }
        private readonly double? _totalCost; public double? TotalCost { get => _totalCost; }
        private MissionStatus _missionStatus; public MissionStatus MissionStatus { get => _missionStatus; set { EnsureMissionIsActive(); SetStatus(value); } }

        protected Mission(MissionType missionType, int id, string name, List<(Person astronaut, string role)> astronauts, Spaceship spaceship, double? totalCost, DateTime startDate, MissionStatus status = MissionStatus.Active)
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

            _missionType = missionType; _id = id; _name = name; _astronauts = astronauts; _astronauts = astronauts; _totalCost = totalCost; _missionStatus = status; _startDate = startDate; _spaceship = spaceship;
        }
        public abstract IMissionDto GetStats();

        public void SetStatus(MissionStatus status)
        {
            EnsureMissionIsActive();
            if (status == MissionStatus.Failed || status == MissionStatus.Finished) EndDate = DateTime.Now; _missionStatus = status;
        }

        protected void EnsureMissionIsActive()
        {
            if (MissionStatus == MissionStatus.Finished || MissionStatus == MissionStatus.Failed)
                throw new MissionStatusException();
        }

    }

    public class SatelliteMission : Mission
    {
        private bool _isOnOrbit; public bool IsOnOrbit { get => _isOnOrbit; }

        public SatelliteMission(MissionType missionType, int id, string name, List<(Person astronaut, string role)> astronauts, Spaceship spaceship, double? totalCost, DateTime startDate, bool isOnOrbit = false, MissionStatus status = MissionStatus.Active)
            : base(missionType, id, name, astronauts, spaceship, totalCost, startDate, MissionStatus.Active)
        {
            _isOnOrbit = isOnOrbit;
        }
        public TimeSpan CalculateOrbitDecay(double dragCoefficient)
        {
            if (dragCoefficient <= 0) throw new ArgumentException("Drag coefficient must be positive");
            return TimeSpan.FromDays(100 / dragCoefficient);
        }

        public async Task<TimeSpan> CalculateOrbitDecayAsync(double dragCoefficient, CancellationToken token = default)
        {
            if (dragCoefficient <= 0) throw new ArgumentException("Drag coefficient must be positive");

            await Task.Delay(10000, token); //CPU вычисления

            return await Task.Run(() => TimeSpan.FromDays(100 / dragCoefficient), token);
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
                missionType = this.missionType,
                Name = this.Name,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                TotalCost = this.TotalCost,
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
        public MissionType missionType { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan TravelTime { get; set; }
        public double? TotalCost { get; set; }
        public bool IsOnOrbit { get; set; }
        public MissionStatus Status { get; set; }



        public List<AstronautDto> Astronauts { get; set; }
        public SpaceshipDto Spaceship { get; set; }


        public string ToJson() => JsonSerializer.Serialize(this); //

    }

    public class ColonizationMission : Mission
    {
        private List<Colony> _сolonies; public List<Colony> Colonies { get => _сolonies; }

        public ColonizationMission(MissionType missionType, int id, string name, List<(Person astronaut, string role)> astronauts, Spaceship spaceship, double? totalCost, DateTime startDate, MissionStatus status = MissionStatus.Active, params Colony[] colonies)
          : base(missionType, id, name, astronauts, spaceship, totalCost, startDate, MissionStatus.Active)
        {
            _сolonies = new List<Colony>(colonies ?? Array.Empty<Colony>());
        }

        public void DeployColony(string name, DateTime date)
        {
            if (name is null) throw new ArgumentNullException("name must be not null!");
            if (MissionStatus == MissionStatus.Active)
            {
                if (date < StartDate) throw new ArgumentException("colony cannot be created before the mission is launched");
                _сolonies.Add(new Colony(name, date));
            }
            else throw new ArgumentException("Mission is not active!");

        }

        public void DeployColony(Colony colony)
        {
            if (MissionStatus == MissionStatus.Active)
            {
                if (colony.StartDate < StartDate) throw new ArgumentException("colony cannot be created before the mission is launched");
                _сolonies.Add(colony);
            }
            else throw new ArgumentException("Mission is not active!");
        }

        public override IMissionDto GetStats()
        {
            return new ColonizationMissionDto
            {
                missionType = this.missionType,
                Name = this.Name,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                TotalCost = this.TotalCost,
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
        public MissionType missionType { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? TotalCost { get; set; }

        public TimeSpan TravelTime { get; set; }
        public MissionStatus Status { get; set; }


        public List<AstronautDto> Astronauts { get; set; }
        public SpaceshipDto Spaceship { get; set; }
        public List<ColonyDto> Colonies { get; set; }

        public string ToJson() => JsonSerializer.Serialize(this); //

    }

    public class MissionAnalyzer
    {
        private List<Mission> _missions; public List<Mission> Missions { get => _missions; set => _missions = value; }

        public MissionAnalyzer()
        {
            _missions = new List<Mission>();
        }

        public MissionAnalyzer(List<Mission> missions)
        {
            if (missions == null) throw new ArgumentNullException("mission must be not null!");
            _missions = missions;
        }

        public void AddMissions(params Mission[] mission)
        {
            if (mission == null) throw new ArgumentNullException("missions must be not null!");
            _missions.AddRange(mission);
        }

        public void RemoveMission(int id)
        {
            _missions.RemoveAll(m => m.Id == id);
        }

        public double CalculateAllTotalCost()
        {
            return _missions.Sum(m => m.TotalCost).GetValueOrDefault();
        }

        public async Task<Double> CalculateAllTotalCostAsync(CancellationToken token = default)
        {
            await Task.Delay(10000, token); //CPU вычисления

            return await Task.Run(() => _missions.AsParallel().Sum(m => m.TotalCost).GetValueOrDefault(), token);
        }

        public IEnumerable<Mission> GetMissionByType(MissionType type)
        {
            switch (type)
            {
                case MissionType.Satellite:
                    return _missions.OfType<SatelliteMission>();
                case MissionType.Colonization:
                    return _missions.OfType<ColonizationMission>();
            }
            return new List<Mission>();
        }

        public IEnumerable<Mission> GetMissionByStatus(MissionStatus status)
        {
            return _missions.Where(m => m.MissionStatus == status);
        }


        public IEnumerable<SatelliteMission> GetSatellitesInOrbit()
        {
            return _missions.OfType<SatelliteMission>().Where(m => m.IsOnOrbit == true);
        }

        public IEnumerable<Colony> GetAllColony()
        {
            return _missions.OfType<ColonizationMission>().SelectMany(cm => cm.Colonies);
        }

        public Dictionary<int, Person> GetAllAstronauts()
        {
            return _missions
                .SelectMany(mission => mission.Astronauts)
                .GroupBy(astronautRole => astronautRole._astronaut.Id)
                .ToDictionary(
                    group => group.Key,
                    group => group.First()._astronaut
                );
        }

        public IEnumerable<Mission> GetMissionAfterDate(DateTime date)
        {
            return _missions.Where(m => m.StartDate >= date);
        }
        public Dictionary<Mission, TimeSpan> GetLongMission()
        {
            return _missions
                .Where(mission => mission.EndDate != null)
                .OrderByDescending(mission => mission.EndDate - mission.StartDate)
                .Take(5)
                .ToDictionary(
                    mission => mission,
                    mission => mission.EndDate.Value - mission.StartDate
                );
        }

        public async Task<Dictionary<Mission, TimeSpan>> GetLongMissionAsync(CancellationToken token = default)
        {
            await Task.Delay(10000, token); //CPU вычисления

            return await Task.Run(() => _missions.AsParallel()
                .Where(mission => mission.EndDate != null)
                .OrderByDescending(mission => mission.EndDate - mission.StartDate)
                .Take(5)
                .ToDictionary(
                    mission => mission,
                    mission => mission.EndDate.Value - mission.StartDate
                ), token);
        }

    }

    namespace SpaceMissionAnalyzer
    {
        internal class Program
        {
            static async Task Main(string[] args)
            {

                Person p1 = new Person(1, "Ivanov");
                Person p2 = new Person(2, "Petrov");

                Spaceship sp = new Spaceship(1, "Rocket-121", 1000, 200, 10);

                /*Item expItm = new Item(10, "EXPENSIVE_ITEM", 100, 5000);
                Item bigItm = new Item(12, "BIG_ITEM", 10000, 500);*/
                // someMission.AddItem(expItm);
                // someMission.AddItem(bigItm);
                Item Itm = new Item(13, "Some_item", 100, 100);
                Item Itm2 = new Item(13, "Some_item2", 450, 450);

                ColonizationMission clMs; SatelliteMission stMs; ColonizationMissionDto dto1;

                PrepareMission someMission = new PrepareMission(202, "ColonizationMars2025", MissionType.Colonization, sp, null, null, 1000); someMission.AddItem(Itm); someMission.AddAstronaut(p1, "Capitan"); someMission.AddAstronaut(p2, "Helper"); //Weight - 1000, budget - 1000 
                PrepareMission someMission2 = new PrepareMission(203, "SatilateMOON", MissionType.Satellite, sp, new List<Item> { Itm2 }, new List<(Person, string)> { (p2, "Capitan") }, 1000);


                clMs = (ColonizationMission)someMission.Launch(); stMs = (SatelliteMission)someMission2.Launch();

                clMs.DeployColony("MyColony", DateTime.Now);
                clMs.DeployColony("MyColony", DateTime.Now);
                clMs.DeployColony("MyColony", DateTime.Now);
                dto1 = (ColonizationMissionDto)clMs.GetStats();

                MissionAnalyzer myAnal = new MissionAnalyzer();

                myAnal.Missions.Add(clMs);
                myAnal.Missions.Add(stMs);



                ////
                double sum = myAnal.CalculateAllTotalCost();

                Console.WriteLine(sum.ToString());
                ////

                ////
                IEnumerable<Mission> FindMission = myAnal.GetMissionByType(MissionType.Satellite);

                Console.WriteLine(FindMission.First().Name.ToString());
                ////

                ////
                IEnumerable<Mission> ActiveMissions = myAnal.GetMissionByStatus(MissionStatus.Active);
                //clMs.SetStatus(MissionStatus.Paused);
                Console.WriteLine(ActiveMissions.Count());
                ////

                ////
                IEnumerable<Mission> MisionsOnOrbit = myAnal.GetSatellitesInOrbit();

                stMs.DeploySatellite();

                Console.WriteLine(MisionsOnOrbit.First().Name.ToString());
                ////

                //// 

                IEnumerable<Colony> MisionsColony = myAnal.GetAllColony();
                Console.WriteLine(MisionsColony.Count().ToString());
                ////

                //// 

                IEnumerable<Mission> DateMision = myAnal.GetMissionAfterDate(new DateTime(2000, 1, 1));
                Console.WriteLine(DateMision.Count().ToString());
                ////

                ////

                CancellationTokenSource tokenSource = new CancellationTokenSource();

                Console.WriteLine("Запуск асинхронной задачи");
                var task = stMs.CalculateOrbitDecayAsync(1, tokenSource.Token);

                Console.WriteLine("Какое-то действие");
                // tokenSource.Cancel();
                var res = await task;
                Console.WriteLine($"Результат: {res.TotalDays} дней)");

                ////

                Person p = new Person(-1, "str");
            }
        }
    }
}