﻿
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PragueParkingClassLibrary
{
    public class ParkingGarage
    {

        public int McPrize { get; set; }
        public int CarPrize { get; set; }
        public int GarageSize { get; set; }


        public ParkingGarage()                              //Läser in configfilen och ger sig själv sina värden utifrån det den läser in
        {
            string filepath = "../../../";
            var configValues = new Dictionary<string, int>();

            foreach (var line in File.ReadLines(filepath + "Config.txt"))
            {
                if (string.IsNullOrEmpty(line) || line.TrimStart().StartsWith("#")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim().Split('#')[0].Trim();
                    configValues[key] = int.Parse(value);
                }
            }

            configValues.TryGetValue("McPrize", out int configMcPrize);
            configValues.TryGetValue("CarPrize", out int configCarPrize);
            configValues.TryGetValue("GarageSize", out int configGarageSize);

            this.McPrize = configMcPrize;
            this.CarPrize = configCarPrize;
            this.GarageSize = configGarageSize;

        }
        public void ReloadConfigTxt()              //Metod för att ladda om config värden medan programmet är igång
        {
            string filepath = "../../../";
            var configValues = new Dictionary<string, int>();

            foreach (var line in File.ReadLines(filepath + "Config.txt"))
            {
                if (string.IsNullOrEmpty(line) || line.TrimStart().StartsWith("#")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim().Split('#')[0].Trim();
                    configValues[key] = int.Parse(value);
                }
            }
            configValues.TryGetValue("McPrize", out int configMcPrize);
            configValues.TryGetValue("CarPrize", out int configCarPrize);
            configValues.TryGetValue("GarageSize", out int configGarageSize);

            this.McPrize = configMcPrize;
            this.CarPrize = configCarPrize;
            this.GarageSize = configGarageSize;
        }
        public ParkingSpot[] GarageSizeChange(ParkingSpot[] input)                           //Körs efter ReloadConfigTxt för att se om det skett en förändring i garagets storlek
                                                                                             //Om det har det så gör den arrayen större om alla platser är tomma, annars inte
                                                                                             //Det är den existerande arrayen som skickas in i metoden, så skickar den den nya tillbaka
        {
            bool isEmpty = true;
            ParkingSpot[] output;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].CurrentSize > 0)
                {
                    isEmpty = false;
                    break;
                }
            }
            if (this.GarageSize < input.Length && isEmpty == false) 
            {
                Console.WriteLine("Garage not empty, number of spots remains the same. \n" +
                        "Please empty the garage before decreasing its size.");
                return input;
            }
            else if (this.GarageSize < input.Length && isEmpty == true)
            {
                output = new ParkingSpot[this.GarageSize];
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = new ParkingSpot(0);
                }
            }
            else
            {
                output = new ParkingSpot[this.GarageSize];
                Array.Copy(input, output, input.Length);
                for (int i = input.Length; i < output.Length; i++)
                {
                    output[i] = new ParkingSpot(0);
                }
            }
            return output;
        }
        public ParkingSpot[] ReadParkingSpotsFromJson()                                                     //kopierar jsonfilen till arrayen, körs i början av programmet
        {
            string filepath = "../../../";
            ParkingSpot[] parkingSpots;
            if (File.Exists(filepath + "ParkingArray.json"))
            {
                string parkingJsonString = File.ReadAllText(filepath + "ParkingArray.json");
                parkingSpots = JsonSerializer.Deserialize<ParkingSpot[]>(parkingJsonString);
            }
            else
            {
                DateTime testDateTime = DateTime.Now;
                parkingSpots = new ParkingSpot[this.GarageSize];
                for (int i = 0; i < parkingSpots.Length; i++)
                {
                    parkingSpots[i] = new ParkingSpot(0);
                }
                Car testCar = new Car("test123", testDateTime);
                Mc testMc = new Mc("test456", testDateTime);
                //SaveParkingSpots();
            }
            return parkingSpots;
        }
        public bool ContainsSpecialCharacters(string regNumber)                                              //inputvalidering
        {
            return Regex.IsMatch(regNumber, @"[^\p{L}\p{N}]");
        }

        public bool FileExists(string fileName)                                                                 //testmetod
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            return File.Exists(fileName);
        }
    }
}
