using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace CarGameEngine
{
    [DataContract(Name = "DriveTrainType")]
    public enum DriveTrainType
    {
        [EnumMember]
        FrontWheelDrive = 0,
        [EnumMember]
        RearWheelDrive = 1,
        [EnumMember]
        AllWheelDrive = 2
    };
    [DataContract(Name = "GearBoxType")]
    public enum GearBoxType
    {
        [EnumMember]
        Manual = 0,
        [EnumMember]
        Automatic = 1,
        [EnumMember]
        Sequential = 2
    };


    public static class CarTools
    {
        public static void SaveCarXml(string filename, Car carToSave)
        {
            Stream stream = File.Open(filename, FileMode.Append);
            DataContractSerializer ser =
                new DataContractSerializer(typeof(Car));
            ser.WriteObject(stream, carToSave);
            stream.Close();
        }
        public static Car LoadCarXml(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            DataContractSerializer ser =
                new DataContractSerializer(typeof(Car));
            Car carToLoad = (Car)ser.ReadObject(stream);
            stream.Close();
            return carToLoad;
        }
        public static void SaveCarsXml(string filename, ListOfCars carsToSave)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            DataContractSerializer ser =
                new DataContractSerializer(typeof(ListOfCars));
            ser.WriteObject(stream, carsToSave);
            stream.Close();
        }
        public static ListOfCars LoadCarsXml(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            DataContractSerializer ser =
                new DataContractSerializer(typeof(ListOfCars));
            ListOfCars listOfCars = new ListOfCars();
            listOfCars = (ListOfCars)ser.ReadObject(stream);

            stream.Close();
            return listOfCars;
        }


    }
    [DataContract]
    public class ListOfCars
    {
        [DataMember]
        public List<Car> Cars = new List<Car>();
    }

    [DataContract]
    public class Car : AerodynamicShape
    {
        [DataMember]
        private DriveTrainType _driveTrainType;
        [DataMember]
        private Engine _engine;
        [DataMember]
        private string _name = String.Empty;
        [DataMember]
        private DriveTrain _driveTrain = new DriveTrain();
        [DataMember]
        private Wheel wheel = new Wheel();

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    _name = "Default";
                else
                    _name = value;
            }
        }
        public Engine engine
        {
            get
            {
                return _engine;
            }
        }

        public Car()
        {
            _engine = new Engine();
            _driveTrainType = DriveTrainType.RearWheelDrive;
            _name = "Default";


        }
        public Car(string Name, DriveTrainType DriveTrain, Int32 Mass)
        {
            _engine = new Engine();
            _driveTrainType = DriveTrain;
            _name = Name;
            base.Mass = Mass;


        }
        public float GetAcceleration(float DeltaTime)
        {
           var appliedForce = engine.GetTorqueNm(engine.currentRPM)*_driveTrain.GetTotalGearRatio()/wheel.Radius;
            var netForce = appliedForce - GetDragForce(GetSpeed())-GetRollingResistance();
            return netForce/base.Mass*DeltaTime;
        }
        public float GetSpeed()
        {
            var t = _driveTrain.GetTotalGearRatio();
            var wheelRPM = engine.currentRPM / _driveTrain.GetTotalGearRatio();
            return (wheelRPM*wheel.Radius*2*(float)Math.PI)*60/1000;
        }
        public float GetDrag(float Speed)
        {
            return base.GetDragForce(Speed);
        }
        public float GetRoll()
        {
            return base.GetRollingResistance();
        }
        public void ChangeDriveTrain(DriveTrainType driveTrain)
        {
            _driveTrainType = driveTrain;
        }
        public string GetDriveTrainAsString()
        {
            return _driveTrainType.ToString();
        }
        public string GetMassAsString()
        {
            return base.Mass.ToString();
        }

    }
    [DataContract]
    public class Wheel
    {
        private float _radius = 0.37f;
        public float Radius
        {
            get
            {
                return _radius;
            }
        }
    }
    [DataContract]
    public class DriveTrain
    {
        private float _finalGearRatio = 3.42f;
        private GearBoxType _gearBoxType = GearBoxType.Manual;
        private Dictionary<Int32, float> _gearBoxRatios = new Dictionary<int, float>{ { 0, 0f }, { 1, 2.97f }, { 2, 2.07f }, { 3, 1.43f }, { 4, 1f }, { 5, 0.84f }, { 6, 0.56f }, { 7, -3.38f } };
        public Int32 currentGear = 6;
        public float GetTotalGearRatio(){
            
            return _finalGearRatio * _gearBoxRatios[_gearBoxRatios.Keys.FirstOrDefault(k => k == currentGear)];
        }
    }
    [DataContract]
    public class AerodynamicShape
    {
        private const float _dragCoefficient = 0.31f;
        private const float _rollingResistanceCoefficient = 0.01f;
        private const float _airDensity = 1.2f;
        private const Int32 _metersPerKiloMeter = 1000;
        private const Int32 _secondsPerHour = 3600;
        private float _frontalArea = 1.94f;
        private const Int32 _maxMass = 5000;
        private const float _gravityConstant = 9.81f;
        [DataMember]
        private Int32 _mass = 1000;
        protected Int32 Mass
        {
            get { return _mass; }
            set { if (value > 0 && value < _maxMass) _mass = value; }
        }
        protected float GetDragForce(float SpeedInKmPerHour)
        {
            float speedInMeterPerSecond = SpeedInKmPerHour * _metersPerKiloMeter / _secondsPerHour;
            return 1f / 2f * (_dragCoefficient * _airDensity * _frontalArea * speedInMeterPerSecond * speedInMeterPerSecond);


        }
        protected float GetRollingResistance()
        {
            return (float)_mass * _gravityConstant * _rollingResistanceCoefficient;
        }

    }
    [DataContract]
    public class Engine
    {
        [DataMember]
        private Dictionary<Int32, Int32> _torqueCurve;
        private Int32 _currentRPM=850;
        public Int32 currentRPM
        {
            get
            {
                return _currentRPM;
            }
            set
            {
                _currentRPM = value;
            }
        }


        public Int32 maxRPM
        {
            get
            {
                for (int i = _torqueCurve.Count - 1; i >= 0; i--)
                {
                    if (_torqueCurve[i] > 0) return Math.Min(i + 2, _torqueCurve.Count) * 500;
                }
                return 0;
            }
        }

        public Engine()
        {
            _torqueCurve = new Dictionary<Int32, Int32>{
                { 0, 0 },
                { 500, 80 },
                {1000, 190 },
                {1500, 210 },
                {2000, 225 },
                {2500, 230 },
                {3000, 235 },
                {3500, 238 },
                {4000, 235 },
                {4500, 230 },
                {5000, 221 },
                {5500, 200 },
                {6000, 180 },
                {6500, 120 },
                {7000, 80 },
                {7500, 0 }
            };


        }
        public Int32 GetTorqueNmFast(Int32 rpm)
        {
            var index = rpm / 500;
            if (index >= 0 && index < _torqueCurve.Count)
            {
                return _torqueCurve[index];
            }
            else return 0;
        }
        public float GetTorqueNm(Int32 rpm)
        {
            var firstKey = _torqueCurve.Keys.LastOrDefault(k => k < rpm);
            var secondKey = _torqueCurve.Keys.FirstOrDefault(k => k > rpm);

            if (rpm <= firstKey) return _torqueCurve[firstKey];
            if (rpm >= secondKey) return _torqueCurve[secondKey];

            var firstTorque = _torqueCurve[firstKey];
            var secondTorque = _torqueCurve[secondKey];
            float difference = secondTorque - firstTorque;
            float weight =  (float)(rpm - firstKey)/(float)(secondKey - firstKey);
            
            return (float)firstTorque+difference*weight;
        }
        public Int32 GetPowerWatt(Int32 rpm)
        {
            var torque = GetTorqueNm(rpm);
            return (int)Math.Round(rpm / 60 * 2 * Math.PI * torque);

        }
        public Int32 GetPowerHP(Int32 rpm)
        {
            var watt = GetPowerWatt(rpm);
            return (int)Math.Round(watt / 745.699872f);
        }
    }
}
