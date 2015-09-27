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
            Car car = new Car();
            car.Name = "Epa";
            CarTools.SaveCarZip("test.zip", car);
            Car newCar = CarTools.LoadCarZip("test.zip");
            Console.WriteLine("Name: {0}", newCar.Name);
            Console.ReadLine();

        }
    }
}
