using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarGameEngine;

namespace CarGameTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Car c = new Car();
            c.Name = "Volvo";
            c.ChangeDriveTrain(DriveTrainType.AllWheelDrive);
            PrintCarInfo(c);
            CarTools.SaveCarXml("test.xml", c);
            c = null;
            c = CarTools.LoadCarXml("test.xml");
            PrintCarInfo(c);
            Console.ReadLine();
        }




        static void PrintCarInfo(Car car)
        {
            Console.WriteLine("Printing info for car: {0}", car.Name);

            for (int i = 0; i < car.engine.maxRPM; i += 500)
            {
                Console.WriteLine("Rpm: {0}\tTorque(Nm): {1}\t\tPower(kW): {2}\t\tPower(HP): {3}", i, car.engine.GetTorqueNm(i), car.engine.GetPowerWatt(i) / 1000, car.engine.GetPowerHP(i));
            }
            Console.WriteLine();
            for (int i = 0; i < 290; i += 10)
            {
                Console.WriteLine("Speed(Km/h): {0}\t Drag(Nm): {1}", i, car.GetDrag(i));
            }
        }
    }
}
