using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace EnvInject
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                LaunchFromJson();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("An error occurred:");
                Console.Error.WriteLine(ex.Message);
            }
        }

        static void LaunchFromJson()
        {
            string jsonPath = "config.json";
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("config.json not found.");

            var json = File.ReadAllText(jsonPath);
            JObject config = JObject.Parse(json);

            var launch = config["launch"];
            if (launch == null)
                throw new Exception("Missing 'launch' section.");

            string executable = launch["executable"]?.ToString() ?? throw new Exception("Missing 'launch.executable'.");
            string arguments = launch["arguments"]?.ToString() ?? "";
            string workingDirectory = launch["workingdirectory"]?.ToString() ?? Directory.GetCurrentDirectory();

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false
            };

            var environment = config["environment"] as JObject;
            if (environment != null)
            {
                foreach (var kvp in environment)
                {
                    startInfo.EnvironmentVariables[kvp.Key] = kvp.Value?.ToString() ?? "";
                }
            }

            Process.Start(startInfo);
        }
    }
}

