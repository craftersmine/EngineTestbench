using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EngineTestbench
{
    public class Testbench
    {
        private int currentTorqueToRotSpeedIndex = 0;

        /// <summary>
        /// Gets currently simulated engine
        /// </summary>
        public Engine CurrentEngine { get; private set; }
        /// <summary>
        /// Gets specified environment temperature
        /// </summary>
        public float EnvironmentTemperature { get; private set; }

        /// <summary>
        /// Initializes new Testbench instance with provided Engine and environment temperature
        /// </summary>
        /// <param name="engine">Engine to benchmark</param>
        /// <param name="environmentTemperature">Environment temperature</param>
        public Testbench(Engine engine, float environmentTemperature)
        {
            CurrentEngine = engine;
            EnvironmentTemperature = environmentTemperature;
            CurrentEngine.CurrentEngineTemperature = EnvironmentTemperature;
        }

        /// <summary>
        /// Tries to start the engine and bench it until torque is 0 or it is overheated
        /// </summary>
        public void Simulate()
        {
            Console.WriteLine("Engine starting...");
            if (!CurrentEngine.Start())
            {
                Console.WriteLine("Unable to start engine for simulation!");
                return;
            }
            Console.WriteLine("Engine started!");

            do
            {
                Iterate();
                Console.WriteLine(
                    "Torque: {0:F2} \t| CrankshaftRotSpeed: {1:F2} \t| EngineTemp: {2:F2} \t| IsOverheated: {3}",
                    CurrentEngine.CurrentEngineTorque, CurrentEngine.CurrentCrankshaftRotationalSpeed,
                    CurrentEngine.CurrentEngineTemperature, CurrentEngine.IsOverheated.ToString());
            } while (!CurrentEngine.IsOverheated && CurrentEngine.CurrentEngineTorque > 0.01f);

            if (CurrentEngine.IsOverheated)
            {
                Console.WriteLine("Engine Overheated! EngineTemp: {0:F2}",
                    CurrentEngine.CurrentEngineTemperature);
            }
        }

        /// <summary>
        /// Performs a single simulation iteration
        /// </summary>
        public void Iterate()
        {
            float a = CurrentEngine.CurrentEngineTorque / CurrentEngine.I;
            CurrentEngine.CurrentCrankshaftRotationalSpeed += a;
            CurrentEngine.CurrentEngineTorque = CurrentEngine.GetTorque(CurrentEngine.CurrentCrankshaftRotationalSpeed);

            float vh = CurrentEngine.CurrentEngineTorque * CurrentEngine.Hm +
                       CurrentEngine.CurrentCrankshaftRotationalSpeed * CurrentEngine.CurrentCrankshaftRotationalSpeed *
                       CurrentEngine.Hv;

            float vc = CurrentEngine.C * (EnvironmentTemperature - CurrentEngine.CurrentEngineTemperature);

            CurrentEngine.CurrentEngineTemperature += vh + vc;
        }
    }
}
