using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace EngineTestbench
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Read environment temperature
            Console.WriteLine("Enter environmental temperature (C*) (default: 25.0): ");
            string input = Console.ReadLine();
            float temp = 25.0f;

            if (!float.TryParse(input, out temp))
            {
                temp = 25.0f;
                Console.WriteLine("Invalid input! Will be used default value of 25.0 degrees Celsius");
            }

            // Load engine specification from JSON file and write to console
            Engine eng = JsonConvert.DeserializeObject<Engine>(File.ReadAllText("engineSpec.json"));
            Console.WriteLine("Loaded engine specification file: \"engineSpec.json\"");
            Console.WriteLine("I = {0}", eng.I);
            Console.WriteLine("M = [ {0} ]", string.Join(", ", eng.M));
            Console.WriteLine("V = [ {0} ]", string.Join(", ", eng.V));
            Console.WriteLine("Toverheat = {0}", eng.TOverheat);
            Console.WriteLine("Hm = {0}", eng.Hm);
            Console.WriteLine("Hv = {0}", eng.Hv);
            Console.WriteLine("C = {0}", eng.C);

            // Initialize testbench
            Console.WriteLine("Creating engine testbench...");
            Testbench bench = new Testbench(eng, temp);

            Console.WriteLine("Environmental temperature is {0:F2}", bench.EnvironmentTemperature);

            Console.WriteLine("Press any key to start engine bench...");
            Console.ReadLine();

            // Simulate!
            bench.Simulate();

            Console.WriteLine("Testbench stopped, engine stopped!");

            // Relaunch if needed
            Console.WriteLine("Press any key to start engine bench...");
            Console.ReadLine();

            bench.Simulate();
        }
    }
}
