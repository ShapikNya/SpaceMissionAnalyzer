using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpaceMissions
{
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

    public enum MissionType
    {
        Planetary = 1,
        Satellite = 2
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
            if (price == null) _price = 0;

            _id = id; _name = name; _weight = weight; 
        }
    }


    public class PrepareMission
    {
        private readonly int _id;
        private string _name;
        private double? _budget;
        private Spaceship _spaceship;
        private List<Item> _items;
        private List<(Person _astronaut, string _role)> _astronauts;
        private MissionType? _missionType;

        public string Name { get => _name; set { if (value is null) throw new ArgumentException("name must be not null!"); } }
        public double? Budget { get => _budget; set { if (value <= 0) throw new ArgumentException("weight must be greater than zero!"); } }
        public Spaceship Spaceship { get => _spaceship; set { _spaceship = value; } }
        public List<Item> Items { get => _items; set {

                if (value is null) throw new ArgumentNullException("items must not null!");

                double curentWeight = 0;
                foreach (var itm in _items)
                {
                    curentWeight += itm.Weight;
                }

                double itemsWeight = 0;
                foreach (var itm in value)
                {
                    itemsWeight += itm.Weight;
                }

                if (curentWeight < itemsWeight) throw new WeightException();



                double curentCost = 0;
                foreach (var itm in _items)
                {
                    curentCost += itm.Price;
                }

                double itemsCost = 0;
                foreach (var itm in value)
                {
                    itemsCost += itm.Price;
                }

                if (_budget-curentCost < itemsWeight) throw new BudgetException();

                _items = value;
            }
        }
        public List<(Person _astronaut, string _role)> Astronauts { get => _astronauts; set { _astronauts = value; } }
        public MissionType? MissionType { get => _missionType; set { _missionType = value; } }

        public PrepareMission(int id)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
            _id = id;
        }

        public PrepareMission(int id, double? budget, Spaceship spaceship, List<Item> items, List<(Person _astronaut, string _role)> astronauts, MissionType? missionType)
        {
            if (id < 0) throw new ArgumentException("id must be positive!");
            if (budget == null) budget = 0;

            _id = id; _spaceship = spaceship; _items = items; _astronauts = astronauts; _missionType = missionType;
        }

        public void AddItem(Item item)
        {
            if (item is null) throw new ArgumentNullException("item must not null!");

            double curentWeight = 0;
            foreach (var itm in _items)
            {
                curentWeight += itm.Weight;
            }
            if (curentWeight + item.Weight > _spaceship.MaxWeight) throw new WeightException();

            double curentCost = 0;
            foreach (var itm in _items)
            {
                curentCost += itm.Price;
            }
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

        public void Launch()
        {
            if (_name is null) throw new ArgumentNullException("name must be not null!");
            if (_budget == 0) throw new ArgumentNullException("budget must be not null!");
            if (Spaceship is null) throw new ArgumentNullException("spaceship must be not null!");
            if (_items.Count == 0) throw new ArgumentNullException("items must be not null!");
            if (_astronauts.Count == 0) throw new ArgumentNullException("astronauts must be not null!");
            if (_missionType is null) throw new ArgumentNullException("missiontype must be not null!");


            //СОЗДАНИЕ МИССИЙ


        }



    }





}







namespace SpaceMissionAnalyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
