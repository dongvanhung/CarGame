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
            bool exit = false;
            int cnt = 2;
            Car c = new Car("Volvo", DriveTrainType.RearWheelDrive, 1000);
            c.engine.throttlePosition = 100;
            
            while (!exit)
            {
                c.Calculate(1000);
                Console.Clear();
                Console.SetCursorPosition(0, 10);
                Console.Write("Speed: {0}\nRpm: {1}\nTime: {2}s", c.GetSpeed(), c.engine.currentRPM,cnt++);
                switch (Console.ReadLine().ToLower())
                {
                    case "a":
                        exit = true;
                        break;
                    case "i":
                        c.GearUp();
                        break;
                    case "k":
                        c.GearDown();
                        break;

                }
            }



            //ConsoleKeyInfo key = new ConsoleKeyInfo();
            //bool exit = false;
            //long startTime = DateTime.Now.Ticks;
            //Car c = new Car("Test", DriveTrainType.RearWheelDrive, 1000);
            //long startLoopTime = 0;
            //Console.CursorVisible = false;
            //c.Start();
            //do
            //{
            //    startLoopTime = DateTime.Now.Ticks;
            //    c.Calculate();
            //    //c.AddAcceleration(100);
            //    //c.Throttle = 1;
            //    if (Console.KeyAvailable)
            //    {
            //        key = Console.ReadKey(false);
            //        switch (key.Key)
            //        {
            //            case ConsoleKey.Escape:
            //                exit = true;
            //                break;
            //            case ConsoleKey.UpArrow:
            //                c.Throttle = 100;
            //                break;
            //            case ConsoleKey.DownArrow:
            //                c.Throttle = 10;
            //                break;

            //        }
            //    }

            //    Console.Clear();
            //    Console.SetCursorPosition(1, 10);
            //    Console.Write("Speed: {0}\nRpm: {1}", c.GetSpeed(),c.engine.currentRPM);
            //    while (DateTime.Now.Ticks - startLoopTime < 100000) { }
            //} while (!exit);






            //for(int i=50;i<7496;i+=96)
            //Console.WriteLine("Rpm: {0}\tkWatt: {1}",i,c.engine.GetPowerWatt(i)/1000);
            //c.engine.currentRPM = 7000;
            //Console.WriteLine("Speed: {0}", c.GetSpeed());
            //ListOfCars cars = new ListOfCars();
            //Random rand = new Random();
            //for (int i = 0; i++ < 10;)
            //{
            //    cars.Cars.Add(new Car(Guid.NewGuid().ToString(), (DriveTrainType)(rand.Next(0,3)),rand.Next(1000,2000)));
            //}
            //foreach (Car c in cars.Cars) Console.WriteLine("Name: {0}\tDriveTrain: {1}\tMass:{2}\tMaxRPM:{3}", c.Name, c.GetDriveTrainAsString(), c.GetMassAsString(),c.engine.maxRPM);
            //CarTools.SaveCarsXml("test.xml", cars);
            //ListOfCars newCars;
            //newCars = CarTools.LoadCarsXml("test.xml");
            //foreach (Car c in newCars.Cars) Console.WriteLine("Name: {0}\tDriveTrain: {1}\tMass:{2}",c.Name,c.GetDriveTrainAsString(),c.GetMassAsString());
            //Console.ReadLine();
        }





    }
}
