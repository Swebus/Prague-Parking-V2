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
            ParkingGarage pragueParking = new ParkingGarage();
            ParkingSpot[] parkingSpots = pragueParking.ReadParkingSpotsFromJson();
            parkingSpots = pragueParking.GarageSizeChange(parkingSpots);
            SaveParkingSpots();
            bool exit = false;
            while (!exit)
            {
                FigletPagrueParking();

                ShowParkingSpaces();

                TablePriceMeny();

                // Main menu selections
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .PageSize(7)
                        .AddChoices(new[] {
            "Park Vehicle",
            "Get Vehicle",
            "Move Vehicle",
            "Find Vehicle",
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

                    bool tryPark = newCar.ParkVehicle(parkingSpots);                        //Prövar att parkera fordonet
                    if (tryPark == false)                                                   //Om det är fullt
                    {
                        Console.WriteLine("Parking Garage at capacity!");
                    }
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

                    bool tryPark = newMc.ParkVehicle(parkingSpots);                        //Prövar att parkera fordonet
                    if (tryPark == false)                                                   //Om det är fullt
                    {
                        Console.WriteLine("Parking Garage at capacity!");
                    }
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

                    //inputvalidering
                    if (string.IsNullOrEmpty(regNumber) | regNumber.Length < 1 | regNumber.Length > 10 | pragueParking.ContainsSpecialCharacters(regNumber))
                    {
                        Console.WriteLine("\nInvalid, please try again.");
                        continue;
                    }

                    //skickar regnummret för att se om det redan finns
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
