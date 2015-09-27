using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;


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
        public static void SaveCarXml(Stream stream, Car carToSave)
        {
            DataContractSerializer ser =
                new DataContractSerializer(typeof(Car));
            ser.WriteObject(stream, carToSave);   
        }
        public static Car LoadCarXml(Stream stream)
        {
            DataContractSerializer ser =
                new DataContractSerializer(typeof(Car));
            Car carToLoad = (Car)ser.ReadObject(stream);
            return carToLoad;
        }
        public static void SaveCarZip(string filename,Car carToSave)
        {
            Stream fileStream = File.Open(filename, FileMode.Create);
            GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Compress);
            SaveCarXml(zipStream, carToSave);
            zipStream.Close();
            fileStream.Close();
        }
        public static Car LoadCarZip(string filename)
        {
            Stream fileStream = File.Open(filename, FileMode.Open);
            GZipStream zipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            Car carToLoad = LoadCarXml(zipStream);
            zipStream.Close();
            fileStream.Close();
            return carToLoad;
        }


    }
    [DataContract]
    public class ListOfCars
    {
        [DataMember]
        public List<Car> Cars = new List<Car>();
    }


    /// <summary>
    /// Describes a Car with engine, drivetrain, wheel inherited from AerodynamicShape
    /// </summary>
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
        private float _currentSpeed = 0;
        private Int32 _brakePosition = 0;

        public Int32 brakePosition
        {
            get { return _brakePosition; }
            set
            {
                if (value < 0)
                    _brakePosition = 0;
                else if (value > 100) _brakePosition = 100;
                else _brakePosition = value;
            }
        }
        public Int32 Gear
        {
            get
            {
                return _driveTrain.currentGear;
            }
        }
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
        /// <summary>
        /// Returns a new Car class.
        /// </summary>
        /// <param name="Name">Name of the Car</param>
        /// <param name="DriveTrain">DriveTrain </param>
        /// <param name="Mass">Mass of car in Kg</param>
        public Car(string Name, DriveTrainType DriveTrain, Int32 Mass)
        {
            _engine = new Engine();
            _driveTrainType = DriveTrain;
            _name = Name;
            base.Mass = Mass;


        }

        public float getForceOnCarX(double deltaTime)
        {
            var appliedForce = engine.GetTorqueNm() * _driveTrain.GetTotalGearRatio() / wheel.Radius;
            float netForce;
            if (_currentSpeed >= 0)
                netForce = appliedForce - GetDragForce(GetSpeed()) - GetRollingResistance() - brakePosition * 50;
            else
                netForce = appliedForce + GetDragForce(GetSpeed()) + GetRollingResistance() + brakePosition * 50;

#warning "Not correct. Divides time by 400f. should be 1000f(1 Second = 1000 ms"
            return netForce / (float)(base.Mass * (deltaTime / 400f));
        }

        /// <summary>
        /// Calculates Forces on the car and updates the rpm of the engine
        /// based on the information stored in the car class.
        /// </summary>
        /// <param name="deltaTime">Time in Milliseconds to use in the calculations.</param>
        public void Calculate(double deltaTime)
        {
            CalculateAccel(deltaTime);
            CalculateGravity(deltaTime);

        }
        private void CalculateAccel(double deltaTime)
        {

            var appliedForce = engine.GetTorqueNm() * _driveTrain.GetTotalGearRatio() / wheel.Radius;
            float netForce;
            if (_currentSpeed >= 0)
                netForce = appliedForce - GetDragForce(GetSpeed()) - GetRollingResistance() - brakePosition * 70;
            else
                netForce = appliedForce + GetDragForce(GetSpeed()) + GetRollingResistance() + brakePosition * 70;

#warning "Not correct. Divides time by 400f. should be 1000f(1 Second = 1000 ms"
            var acceleration = netForce / base.Mass * (deltaTime /400f);
            _currentSpeed = GetSpeed() + (float)acceleration / wheel.Radius;
            var wheelRPM = (_currentSpeed / (wheel.Radius * 2 * Math.PI) * 1000 / 60);
            var newRPM = wheelRPM * _driveTrain.GetTotalGearRatio();
            engine.currentRPM = (int)newRPM;
        }
        private void CalculateGravity(double deltaTime)
        {

        }
        /// <summary>
        /// Returns the speed in Km/h based on current rpm of engine and drivetrain gear ratio with current gear.
        /// </summary>
        /// <returns></returns>
        public float GetSpeed()
        {
            return _currentSpeed;
            //var t = _driveTrain.GetTotalGearRatio();
            //var wheelRPM = engine.currentRPM / _driveTrain.GetTotalGearRatio();
            //return (wheelRPM*wheel.Radius*2*(float)Math.PI)*60/1000;
        }
        public float GetDragResistance(float Speed)
        {
            return base.GetDragForce(Speed);
        }
        public new float GetRollingResistance()
        {
            return base.GetRollingResistance();
        }
        /// <summary>
        /// Puts in the next gear of the transmission.
        /// </summary>
        public void GearUp()
        {
            var speed = GetSpeed();
            if (_driveTrain.currentGear < 6)
                _driveTrain.currentGear++;
            var wheelRPM = (speed / (wheel.Radius * 2 * Math.PI) * 1000 / 60);
            engine.currentRPM = (int)(wheelRPM * _driveTrain.GetTotalGearRatio());

        }
        /// <summary>
        /// Puts in the previous gear of the transmission.
        /// </summary>
        public void GearDown()
        {
            var speed = GetSpeed();
            if (_driveTrain.currentGear - 1 > 0)
                _driveTrain.currentGear--;
            var wheelRPM = (speed / (wheel.Radius * 2 * Math.PI) * 1000 / 60);
            engine.currentRPM = (int)(wheelRPM * _driveTrain.GetTotalGearRatio());

        }
        public void GearReverse(bool reverse)
        {
            if (reverse)
                _driveTrain.currentGear = 7;
            else
                _driveTrain.currentGear = 1;
        }


    }
    [DataContract]
    public class Wheel
    {
        [DataMember]
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
        [DataMember]
        private float _finalGearRatio = 3.42f;
        //[DataMember]
        //private GearBoxType _gearBoxType = GearBoxType.Manual;
        [DataMember]
        private Dictionary<Int32, float> _gearBoxRatios = new Dictionary<int, float> { { 0, 0f }, { 1, 2.97f }, { 2, 2.07f }, { 3, 1.43f }, { 4, 1f }, { 5, 0.84f }, { 6, 0.56f }, { 7, -3.38f } };
        public Int32 currentGear = 1;
        public float GetTotalGearRatio()
        {

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
        [DataMember]
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
        public Dictionary<Int32, Int32> _torqueCurve;
        private Int32 _currentRPM = 850;
        private Int32 _throttlePosition;
        public Int32 throttlePosition
        {
            get
            {
                return _throttlePosition;
            }
            set
            {
                if (value > 100)
                    _throttlePosition = 100;
                else if (value < 0)
                    _throttlePosition = 0;
                else
                    _throttlePosition = value;
            }
        }
        public float Clutch
        {
            get
            {
                if (_currentRPM < 900)
                    return 0;
                else
                    return 1;
            }
        }
        public Int32 currentRPM
        {
            get
            {
                return _currentRPM;
            }
            set
            {
                if (value >= 850)
                    _currentRPM = value;
                else _currentRPM = 850;
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
                {0,30 },
                {500, 80 },
                {1000, 250 },
                {1500, 350 },
                {2000, 600 },
                {2500, 1200 },
                {3000, 1600 },
                {3500, 1600 },
                {4000, 1600 },
                {4500, 1600 },
                {5000, 1600 },
                {5500, 1600 },
                {6000, 1400 },
                {6500, 1200 },
                {7000, 900 },
                {7500,700 },
                {8000,400 },
                {8500,200 },
                {9000,0 }
            };


        }
        /// <summary>
        /// Returns max torque of engine at rpm in Nm.
        /// Only works if value of exact rpm is stored in _torqueCurve
        /// </summary>
        /// <param name="rpm">Rpm to return Torque of.</param>
        /// <returns></returns>
        public Int32 GetTorqueNmFast(Int32 rpm)
        {
            var index = rpm / 500;
            if (index >= 0 && index < _torqueCurve.Count)
            {
                return _torqueCurve[index] * (_throttlePosition / 100);
            }
            else return 0;
        }
        /// <summary>
        /// Returns max torque of engine att rpm.
        /// Interpolates if exact value is not found.
        /// </summary>
        /// <param name="rpm">Rpm to return Torque of.</param>
        /// <returns></returns>
        public float GetTorqueNm()
        {
            var firstKey = _torqueCurve.Keys.LastOrDefault(k => k < _currentRPM);
            var secondKey = _torqueCurve.Keys.FirstOrDefault(k => k > _currentRPM);

            if (_currentRPM <= firstKey) return _torqueCurve[firstKey];
            if (_currentRPM >= secondKey) return _torqueCurve[secondKey];

            var firstTorque = _torqueCurve[firstKey];
            var secondTorque = _torqueCurve[secondKey];
            float difference = secondTorque - firstTorque;
            float weight = (float)(_currentRPM - firstKey) / (float)(secondKey - firstKey);

            return ((float)firstTorque + difference * weight) * ((float)_throttlePosition / 100f);
        }
        /// <summary>
        /// Returns max Power in Watts at rpm
        /// </summary>
        /// <param name="rpm">Rpm</param>
        /// <returns></returns>
        public Int32 GetPowerWatt()
        {
            var torque = GetTorqueNm();
            return (int)Math.Round(_currentRPM / 60 * 2 * Math.PI * torque);

        }
        /// <summary>
        /// Returns max Power in HP at rpm
        /// </summary>
        /// <param name="rpm"></param>
        /// <returns></returns>
        public Int32 GetPowerHP()
        {
            var watt = GetPowerWatt();
            return (int)Math.Round(watt / 745.699872f);
        }
    }
}
