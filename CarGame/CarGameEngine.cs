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
    [DataContract(Name = "DriveTrain")]
    public enum DriveTrainType
    {
        [EnumMember]
        FrontWheelDrive = 0,
        [EnumMember]
        RearWheelDrive = 1,
        [EnumMember]
        AllWheelDrive = 2
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
        private DriveTrainType _driveTrain;
        [DataMember]
        private Engine _engine;
        [DataMember]
        private string _name = String.Empty;

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
            _driveTrain = DriveTrainType.RearWheelDrive;
            _name = "Default";
            base.Mass = 1250;


        }
        public Car(string Name,DriveTrainType DriveTrain,Int32 Mass)
        {
            _engine = new Engine();
            _driveTrain = DriveTrain;
            _name = Name;
            base.Mass = Mass;


        }
        public float GetDrag(float Speed)
        {
            return base.GetDragForce(Speed);
        }
        public void ChangeDriveTrain(DriveTrainType driveTrain)
        {
            _driveTrain = driveTrain;
        }
        public string GetDriveTrainAsString()
        {
            return _driveTrain.ToString();
        }
        public string GetMassAsString()
        {
            return base.Mass.ToString();
        }
    }
    [DataContract]
    public class AerodynamicShape
    {
        private const float _dragCoefficient = 0.31f;
        private const float _rollingFrictionCoefficient = 0.015f;
        private const float _airDensity = 1.2f;
        private const Int32 _metersPerKiloMeter = 1000;
        private const Int32 _secondsPerHour = 3600;
        private float _frontalArea = 1.94f;
        private const Int32 _maxMass = 5000;
        [DataMember]
        private Int32 _mass = 1323;
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

    }
    [DataContract]
    public class Engine
    {
        [DataMember]
        private Int32[] _torqueCurve;


        public Int32 maxRPM
        {
            get
            {
                for (int i = _torqueCurve.Length - 1; i >= 0; i--)
                {
                    if (_torqueCurve[i] > 0) return Math.Min(i + 2, _torqueCurve.Length) * 500;
                }
                return 0;
            }
        }

        public Engine()
        {
            _torqueCurve = new Int32[] { 0, 80, 190, 210, 225, 230, 235, 238, 235, 230, 221, 200, 180, 120, 80, 0, 0, 0, 0, 0 };


        }
        public Int32 GetTorqueNm(Int32 rpm)
        {
            var index = rpm / 500;
            if (index >= 0 && index < _torqueCurve.Length)
            {
                return _torqueCurve[index];
            }
            else return 0;
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
