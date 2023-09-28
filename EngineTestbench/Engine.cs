using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using static System.Net.Mime.MediaTypeNames;

namespace EngineTestbench
{
    public class Engine
    {
        /// <summary>
        /// Gets or sets engine moment of inertia (kg*m^2)
        /// </summary>
        [JsonProperty("I")]
        public float I { get; set; }
        /// <summary>
        /// Gets or sets torques (N*m)
        /// </summary>
        [JsonProperty("M")]
        public float[] M { get; set; }
        /// <summary>
        /// Gets or sets rotational speeds of crankshafts (rad/s)
        /// </summary>
        [JsonProperty("V")]
        public float[] V { get; set; }
        /// <summary>
        /// Gets or sets engine overheat temperature (T - C*)
        /// </summary>
        [JsonProperty("Toverheat")]
        public float TOverheat { get; set; }
        /// <summary>
        /// Gets or sets factor of relation of warmup speed to torque (T / (N * m * seconds))
        /// </summary>
        [JsonProperty("Hm")]
        public float Hm { get; set; }
        /// <summary>
        /// Gets or sets factor of relation of warmup speed to rotational speed of crankshaft ((T * seconds) / rad^2)
        /// </summary>
        [JsonProperty("Hv")]
        public float Hv { get; set; }
        /// <summary>
        /// Gets or sets factor of engine cooldown speed to environmental temperature (1/seconds)
        /// </summary>
        [JsonProperty("C")]
        public float C { get; set; }

        /// <summary>
        /// Current engine temperature
        /// </summary>
        [JsonIgnore]
        public float CurrentEngineTemperature { get; set; }
        /// <summary>
        /// Current engine torque
        /// </summary>
        [JsonIgnore]
        public float CurrentEngineTorque { get; set; }
        /// <summary>
        /// Current crankshaft rotational speed
        /// </summary>
        [JsonIgnore]
        public float CurrentCrankshaftRotationalSpeed { get; set; }
        /// <summary>
        /// Gets true if engine is overheated
        /// </summary>
        [JsonIgnore]
        public bool IsOverheated => CurrentEngineTemperature >= TOverheat;
        /// <summary>
        /// Gets true if engine is running in bench
        /// </summary>
        [JsonIgnore]
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Tries to start the engine and returns true if started, otherwise false
        /// </summary>
        /// <returns>True if engine is started, false if engine unable to start (overheated)</returns>
        public bool Start()
        {
            if (IsOverheated)
            {
                IsRunning = false;
                Console.WriteLine("Engine is overheated! Unable to start! Engine Temperature: {0:F2}", CurrentEngineTemperature);
                return false;
            }

            if (CurrentEngineTorque <= 0)
            {
                CurrentEngineTorque = GetMinTorque();
            }
            IsRunning = true;
            return true;
        }

        /// <summary>
        /// Stops the engine
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Gets torque for specified crankshaft rotational speed
        /// </summary>
        /// <param name="crankshaftRotationalSpeed">Rotational speed of Crankshaft</param>
        /// <returns></returns>
        public float GetTorque(float crankshaftRotationalSpeed)
        {
            var closest = V.Aggregate((cur, next) =>
            {
                return crankshaftRotationalSpeed <= next && crankshaftRotationalSpeed >= cur ? cur : next;
            });

            int ind = V.ToList().IndexOf(closest);
            float torqueLow = M[ind];
            float torqueHigh = 0;
            float cRSLow = V[ind];
            float cRSHigh = 0;
            float cRSValue = 0f;

            if (ind + 1 != M.Length)
            {
                torqueHigh = M[ind + 1];
                cRSHigh = V[ind + 1];
            }

            cRSValue = (crankshaftRotationalSpeed - cRSLow) / (cRSHigh - cRSLow);

            return Helper.Lerp(torqueLow, torqueHigh, cRSValue);
        }

        /// <summary>
        /// Gets minimal torque
        /// </summary>
        /// <returns></returns>
        public float GetMinTorque()
        {
            return M.Min();
        }
    }
}
