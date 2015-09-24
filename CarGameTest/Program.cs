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
            //for(int i=50;i<7496;i+=96)
            //Console.WriteLine("Rpm: {0}\tkWatt: {1}",i,c.engine.GetPowerWatt(i)/1000);
            c.engine.currentRPM = 7000;
            Console.WriteLine("Speed: {0}", c.GetSpeed());
            //ListOfCars cars = new ListOfCars();
            //Random rand = new Random();
            //for (int i = 0; i++ < 10;)
            //{
            //    cars.Cars.Add(new Car(Guid.NewGuid().ToString(), (DriveTrainType)(rand.Next(0,3)),rand.Next(1000,2000)));
            //}
            ////foreach (Car c in cars.Cars) Console.WriteLine("Name: {0}\tDriveTrain: {1}\tMass:{2}\tMaxRPM:{3}", c.Name, c.GetDriveTrainAsString(), c.GetMassAsString(),c.engine.maxRPM);



            //CarTools.SaveCarsXml("test.xml", cars);
            //ListOfCars newCars;
            //newCars = CarTools.LoadCarsXml("test.xml");
            //foreach (Car c in newCars.Cars) Console.WriteLine("Name: {0}\tDriveTrain: {1}\tMass:{2}",c.Name,c.GetDriveTrainAsString(),c.GetMassAsString());
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
