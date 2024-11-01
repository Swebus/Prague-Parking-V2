using System.Text.Json;
using PragueParkingClassLibrary;
using Spectre.Console;

namespace Prague_Parking_V2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filepath = "../../../";
            //int oldGarageSize;
            //var configValues = ReadConfigTxt();
            ParkingGarage pragueParking = new ParkingGarage();
            ParkingSpot[] parkingSpots = pragueParking.ReadParkingSpotsFromJson();
            parkingSpots = pragueParking.GarageSizeChange(parkingSpots);
            SaveParkingSpots();
            //ParkingSpot[] parkeringsPlatser;
            //if (File.Exists(filepath + "ParkingArray.json"))
            //{
            //    string parkingJsonString = File.ReadAllText(filepath + "ParkingArray.json");
            //    parkeringsPlatser = JsonSerializer.Deserialize<ParkingSpot[]>(parkingJsonString);
            //}
            //else
            //{
            //    DateTime testDateTime = DateTime.Now;
            //    parkeringsPlatser = new ParkingSpot[pragueParking.GarageSize];
            //    for (int i = 0; i < parkeringsPlatser.Length; i++)
            //    {
            //        parkeringsPlatser[i] = new ParkingSpot(0);
            //    }
            //    Car testCar = new Car("test123", testDateTime);
            //    Mc testMc = new Mc("test456", testDateTime);
            //    SaveParkingSpots();
            //}
            ////ReloadConfigFile();


            bool exit = false;
            while (!exit)
            {
                FigletPagrueParking();

                ShowParkingSpaces();

                TablePriceMeny();

                // Main menu selections
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(8)
                        .AddChoices(new[] {
            "Park Vehicle",
            "Get Vehicle",
            "Move Vehicle",
            "Find Vehicle",
            "Show Detailed Spaces",
            "Clear Garage",
            "Reload Config File",
            "Close Program",
                        }));

                // Selection switch
                switch (selection)
                {
                    case "Park Vehicle":
                        {
                            ParkVehicle();
                            break;
                        }
                    case "Get Vehicle":
                        {
                            GetVehicle();
                            break;
                        }
                    case "Move Vehicle":
                        {
                            MoveVehicle();
                            break;
                        }
                    case "Find Vehicle":
                        {
                            FindVehicle();
                            break;
                        }
                    case "Reload Config File":
                        {
                            ReloadConfigFile(); 
                            break;
                        }
                    case "Clear Garage":
                        {
                            ClearGarage();
                            break;
                        }
                    case "Show Detailed Spaces":
                        {
                            Console.Clear();
                            ShowDetailedParkingSpaces();
                            break;
                        }
                    case "Close Program":
                        {
                            exit = true;
                            break;
                        }
                }
                if (!exit)
                {
                    var table1 = new Table();
                    table1.AddColumn("[yellow]Press enter to return to Main Menu.[/]");
                    AnsiConsole.Write(table1);
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            
            
            void ParkVehicle()
            {
                int type = ChooseVehicleType();

                if (type == 1)      //type 1 = Car
                {
                    string regNumber = GetRegNumber();
                    if (regNumber == "error")
                    {
                        return;
                    }
                    DateTime parkingTime = DateTime.Now;
                    Car newCar = new Car(regNumber, parkingTime);

                    newCar.ParkVehicle(parkingSpots);
                    SaveParkingSpots();
                }
                else if (type == 2)     //type 2 = Mc
                {
                    string regNumber = GetRegNumber();
                    if (regNumber == "error")
                    {
                        return;
                    }
                    DateTime parkingTime = DateTime.Now;
                    Mc newMc = new Mc(regNumber, parkingTime);

                    newMc.ParkVehicle(parkingSpots);
                    SaveParkingSpots();
                }
            }
            int ChooseVehicleType()
            {
                int type = 0;
                var typeChoice = AnsiConsole.Prompt(
              new SelectionPrompt<string>()
                  .PageSize(4)
                  .AddChoices(new[] {
                 "Car",
                 "Mc",
                  }));
                if (typeChoice == "Car")
                {
                    type = 1;
                }
                else if (typeChoice == "Mc")
                {
                    type = 2;
                }
                return type;
            }
            string GetRegNumber()
            {

                while (true)
                {
                    Console.Write("Enter vehicle registration number: ");
                    string regNumber = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(regNumber) | regNumber.Length < 1 | regNumber.Length > 10 | pragueParking.ContainsSpecialCharacters(regNumber))
                    {
                        Console.WriteLine("\nInvalid, please try again.");
                        continue;
                    }
                    bool regNumberExists = parkingSpots.Any(spot => spot.ContainsVehicle(regNumber));

                    if (regNumberExists)
                    {
                        Console.WriteLine("Vehicle is already parked. Returning to main menu.");
                        return "error";
                    }
                    else
                    {
                        return regNumber;
                    }
                }
            }
            void GetVehicle()
            {
                string regNumber;
                // Be om registreringsnummer
                do
                {
                    Console.Write("Enter Registration Number:  ");
                    regNumber = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(regNumber))
                    {
                        var table2 = new Table();
                        table2.AddColumn("[yellow]Vehicle not found. Returning to main menu. [/]");
                        AnsiConsole.Write(table2);
                        return;
                    }
                } 
                while (string.IsNullOrEmpty(regNumber));

                // Leta upp fordonet i parkeringsplatserna
                ParkingSpot currentSpot = null;
                Vehicle vehicleToRemove = null;
                int currentSpotIndex = -1;

                for (int i = 1; i < parkingSpots.Length; i++)
                {
                    var spot = parkingSpots[i];
                    vehicleToRemove = spot.parkingSpot.FirstOrDefault(v => v.RegNumber == regNumber);

                    if (vehicleToRemove != null)
                    {
                        currentSpot = spot;
                        currentSpotIndex = i;
                        break;
                    }
                }

                if (currentSpot == null || vehicleToRemove == null)
                {
                    var table2 = new Table();
                    table2.AddColumn("[yellow]Vehicle not found. Returning to main menu. [/]");
                    AnsiConsole.Write(table2);
                    return;
                }

                // Beräkna parkeringstid och eventuella kostnader
                DateTime currentTime = DateTime.Now;
                TimeSpan parkingDuration = currentTime - vehicleToRemove.ParkingTime;

                double price = CalculateParkingCost(vehicleToRemove, parkingDuration);

                // Anta att de första 10 minuterna är gratis
                //double price = 0;
                //if (parkingDuration.TotalMinutes > 10)
                //{
                //    if (vehicleToRemove.Size == 4)
                //    {
                //        price = (parkingDuration.TotalMinutes - 10) * pragueParking.CarPrize / 60;
                //    }
                //    else if (vehicleToRemove.Size == 2)
                //    {
                //        price = (parkingDuration.TotalMinutes - 10) * pragueParking.McPrize / 60;
                //    }
                //}

                Console.WriteLine($"Parking duration: {parkingDuration.TotalMinutes:F1} minutes.");
                Console.WriteLine($"Parking cost: {price:F2}CZK");

                // Bekräfta om användaren vill ta bort fordonet
                Console.WriteLine("Do you want to retrieve and remove the vehicle?");
                var confirm = AnsiConsole.Prompt(new SelectionPrompt<string>()
                     .PageSize(4).AddChoices(new[] { "Yes", "No" }));
                if (confirm == "Yes")
                {
                    // Ta bort fordonet från nuvarande parkeringsplats
                    currentSpot.parkingSpot.Remove(vehicleToRemove);
                    currentSpot.CurrentSize -= vehicleToRemove.Size;

                    Console.WriteLine($"Vehicle {regNumber} has been retrieved from spot {currentSpotIndex}.");

                    // Spara uppdaterade parkeringsplatser till JSON-filen
                    SaveParkingSpots();
                }
            }
            void MoveVehicle()

            {
                string regNumber;

                do
                {
                    Console.Write("Enter registration number:  ");
                    regNumber = Console.ReadLine().Trim();
                    if (string.IsNullOrEmpty(regNumber))
                    {

                        var table2 = new Table();
                        table2.AddColumn("[yellow]Vehicle not found. Returning to main menu. [/]");
                        AnsiConsole.Write(table2);
                        return;
                    }
                } while (string.IsNullOrEmpty(regNumber));


                ParkingSpot currentSpot = null;
                Vehicle vehicleToMove = null;
                int currentSpotIndex = -1;

                // loop igenom för match regNum
                for (int i = 1; i < parkingSpots.Length; i++)
                {
                    var spot = parkingSpots[i];
                    vehicleToMove = spot.parkingSpot.FirstOrDefault(Vehicle => Vehicle.RegNumber == regNumber);
                    if (vehicleToMove != null)
                    {
                        currentSpot = spot;
                        currentSpotIndex = i;
                        break;
                    }
                }
                if (currentSpot == null)
                {
                    var table3 = new Table();
                    table3.AddColumn($"[yellow]Vehicle with registration nummber {regNumber} not found.[/]");
                    AnsiConsole.Write(table3);
                    return;
                }

                Console.WriteLine($"Vehicle '{regNumber}' is registered to spot '{currentSpotIndex}'");
                int newSpotIndex;

                bool isValidtoCheckOut = true;
                do
                {

                    Console.Write("Enter new parking spot number: ");

                    if (int.TryParse(Console.ReadLine(), out newSpotIndex) && newSpotIndex > 0 && newSpotIndex < parkingSpots.Length)
                    {
                        var newSpot = parkingSpots[newSpotIndex];

                        if (newSpot.CurrentSize + vehicleToMove.Size <= newSpot.MaxSize)
                        {
                            // To bort
                            currentSpot.parkingSpot.Remove(vehicleToMove);
                            currentSpot.CurrentSize -= vehicleToMove.Size;

                            // Flytta
                            newSpot.parkingSpot.Add(vehicleToMove);
                            newSpot.CurrentSize += vehicleToMove.Size;
                            Console.WriteLine($"Vehicle '{regNumber}' moved to spot '{newSpotIndex}'");

                            // Spara och update
                            SaveParkingSpots();
                            isValidtoCheckOut = false;
                        }
                        else
                        {
                            Console.WriteLine("Not enought space.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid parking spot number. Please try again.");

                    }
                } while (isValidtoCheckOut);
            }
            void FindVehicle()
            {
                Console.Write("Enter registration number: ");
                String regnumber = Console.ReadLine()?.Trim();
                bool found = false;
                for (int i = 1; i < parkingSpots.Length; i++)
                {
                    var Spot = parkingSpots[i];
                    var vehicle = Spot?.parkingSpot.FirstOrDefault(v => v.RegNumber == regnumber);
                    if (vehicle != null)
                    {
                        DateTime currentTime = DateTime.Now;
                        TimeSpan duration = currentTime - vehicle.ParkingTime;
                        double price = CalculateParkingCost(vehicle, duration);

                        Console.WriteLine($" Vehicle '{regnumber}' is registered to spot '{i}'");
                        Console.WriteLine($" Park Duration:{duration.TotalMinutes:F1} minutes");
                        Console.WriteLine($" Parking Cost {price:F2} CZK");
                        found = true;
                        break;
                    }

                }
                if (!found)
                {
                    Console.WriteLine("Vehicle not found.");
                }
            }
            double CalculateParkingCost(Vehicle vehicle, TimeSpan duration)
            {
                const double freetime = 10;
                double rate = 0;
                //const double hourlyRateCar = 20.0;
                //const double hourlyRateMc = 10.0;

                if (duration.TotalMinutes <= freetime)
                {
                    return 0;
                }
                else
                {
                    if (vehicle.Size == 2)
                    {
                        rate = pragueParking.McPrize;
                    }
                    else if (vehicle.Size == 4)
                    {
                        rate = pragueParking.CarPrize;
                    }
                }
                return ((duration.TotalMinutes - freetime) / 60) * rate;
            }
            void ShowParkingSpaces()
            {

                int emptyCount = -1;
                int halfFullCount = 0;
                int fullCunt = 0;

                foreach (var spot in parkingSpots)
                {
                    if (spot.CurrentSize == 0)
                    {
                        emptyCount++;
                    }
                    else if (spot.CurrentSize < spot.MaxSize)
                    {
                        halfFullCount++;
                    }
                    else if (spot.CurrentSize == spot.MaxSize)
                    {
                        fullCunt++;
                    }
                }

                var chart = new BreakdownChart()
                    .FullSize()
                    .AddItem("Empty", emptyCount, Color.Green)
                    .AddItem("Half Full", halfFullCount, Color.Yellow)
                    .AddItem("Full", fullCunt, Color.Red);
                AnsiConsole.Write(new Markup("[grey bold]Parking Space[/]\n"));
                AnsiConsole.Write(chart);
            }
            void ShowDetailedParkingSpaces()
            {
                FigletPagrueParking();
                TableStatusVehicle();
                int columns = 5;
                int rows = 1;
                int columnWide = 20;

                Console.WriteLine("Parking Overview: \n");

                for (int i = 1; i <= 100; i++)
                {
                    if (rows > columns)
                    {
                        Console.WriteLine();
                        rows = 1;
                    }


                    string status = $"{i}: Unknown\t";
                    ConsoleColor color = ConsoleColor.Gray;

                    if (parkingSpots[i].CurrentSize == 0)
                    {
                        status = $"{i}: Empty\t";
                        color = ConsoleColor.Green;
                    }
                    else
                    {
                    
                        int vechicleCount = parkingSpots[i].parkingSpot.Count;

                        if (vechicleCount == 2)
                        {
                            status = $"{i}: Occupied\t";
                            color = ConsoleColor.Red;
                        }
                        else if (vechicleCount == 1)
                        {
                            
                            // tagit från show Parking Space
                            if (parkingSpots[i].CurrentSize < parkingSpots[i].MaxSize && parkingSpots[i].CurrentSize > 00)
                            {
                                status = $"{i}: Half Full\t";
                                color = ConsoleColor.Yellow;
                            }
                            else
                            {
                                status = $"{i}: Occupied\t";
                                color = ConsoleColor.Red;
                            }
                        }

                    }
                    Console.ForegroundColor = color;
                    Console.Write(status.PadLeft(columnWide));
                    Console.ResetColor();
                    
                    rows++;
                }
                Console.WriteLine("\n");
            }
            void SaveParkingSpots()
            {
                string updatedParkingArrayJsonString = JsonSerializer.Serialize(parkingSpots, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filepath + "ParkingArray.json", updatedParkingArrayJsonString);
            }
            void ReloadConfigFile()
            {
                pragueParking.ReloadConfigTxt();
                parkingSpots = pragueParking.GarageSizeChange(parkingSpots);
                SaveParkingSpots();
                ////oldGarageSize = parkeringsPlatser.Length;
                //bool isEmpty = true;

                //for (int i = 0; i < parkeringsPlatser.Length; i++)
                //{
                //    if (parkeringsPlatser[i].CurrentSize > 0)
                //    {
                //        isEmpty = false;
                //        break;
                //    }
                //}

                //var newConfigValues = ReadConfigTxt();


                //if (newConfigValues.garageSize < parkeringsPlatser.Length && isEmpty == false)
                //{
                //    ParkingGarage pragueParking = new ParkingGarage(newConfigValues.mcPrize, configValues.carPrize, parkeringsPlatser.Length);
                //    Console.WriteLine("Garage not empty, number of spots remains the same. \n" +
                //        "Please empty the garage before decreasing its size.");
                //}
                //else if (newConfigValues.garageSize < parkeringsPlatser.Length && isEmpty == true)
                //{
                //    ParkingGarage newPragueParking = new ParkingGarage(newConfigValues.mcPrize, newConfigValues.carPrize, newConfigValues.garageSize);
                //    pragueParking = newPragueParking;
                //    parkeringsPlatser = new ParkingSpot[pragueParking.GarageSize];
                //    for (int i = 0; i < parkeringsPlatser.Length; i++)
                //    {
                //        parkeringsPlatser[i] = new ParkingSpot(0);
                //    }
                //}
                //else
                //{
                //    ParkingGarage newPragueParking = new ParkingGarage(newConfigValues.mcPrize, newConfigValues.carPrize, newConfigValues.garageSize);
                //    pragueParking = newPragueParking;
                //    ParkingSpot[] newParkeringsPlatser = new ParkingSpot[pragueParking.GarageSize];
                //    Array.Copy(parkeringsPlatser, newParkeringsPlatser, parkeringsPlatser.Length);
                //    for (int i = parkeringsPlatser.Length; i < newParkeringsPlatser.Length; i++)
                //    {
                //        newParkeringsPlatser[i] = new ParkingSpot(0);
                //    }
                //    parkeringsPlatser = newParkeringsPlatser;
                //}
                //SaveParkingSpots();
                //}
                //(int mcPrize, int carPrize, int garageSize) ReadConfigTxt()
                //{
                //    var configValues = new Dictionary<string, int>();

                //    foreach (var line in File.ReadLines(filepath + "Config.txt"))
                //    {
                //        if (string.IsNullOrEmpty(line) || line.TrimStart().StartsWith("#")) continue;

                //        var parts = line.Split(new[] { '=' }, 2);
                //        if (parts.Length == 2)
                //        {
                //            string key = parts[0].Trim();
                //            string value = parts[1].Trim().Split('#')[0].Trim();
                //            configValues[key] = int.Parse(value);
                //        }
                //    }

                //    configValues.TryGetValue("McPrize", out int mcPrize);
                //    configValues.TryGetValue("CarPrize", out int carPrize);
                //    configValues.TryGetValue("GarageSize", out int garageSize);

                //    return (mcPrize, carPrize, garageSize);
            }
            // Top MainMeny Design
            #region 
            void FigletPagrueParking()
            {
                AnsiConsole.Write(
                    new FigletText("Prague Parking")
                        .Centered()
                        .Color(Color.Red));
                Console.WriteLine("\n\n");
            }
            void TableStatusVehicle()
            {
                Table table = new Table();
                table.AddColumns("[green]EMPTY SPOT[/]",
                                        "[yellow]HALF FULL[/]",
                                        "[red]FULL SPOT[/]")
                                        .Collapse().Centered().Expand();
                AnsiConsole.Write(table);
            }
            void TablePriceMeny()
            {
                var table = new Table();
                table.AddColumn("Vehicle type: ");
                table.AddColumn(new TableColumn("Price/H: ").Centered());
                table.AddRow("Free", "10 min");
                table.AddRow("Mc", $"{pragueParking.McPrize} CZK/H");
                table.AddRow("Car", $"{pragueParking.CarPrize} CZK/H");

                AnsiConsole.Write(table.SimpleBorder().Alignment(Justify.Left));
            }
            #endregion

            void ClearGarage()
            {
                Console.WriteLine("Are you sure you want to empty the garage?");
                var confirm = AnsiConsole.Prompt(new SelectionPrompt<string>()
                     .PageSize(4).AddChoices(new[] { "Yes", "No" }));
                if (confirm == "Yes")
                {
                    for (int i = 1; i < parkingSpots.Length; i++)
                    {
                        parkingSpots[i].parkingSpot.Clear();
                        parkingSpots[i].CurrentSize = 0;
                    }
                    SaveParkingSpots();
                }
                else
                {
                    return;
                }
            }
        }
    }
}
