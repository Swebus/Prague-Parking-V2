
namespace PragueParkingClassLibrary
{
    public class Vehicle 
    {
        public string RegNumber { get; set; }
        public DateTime ParkingTime { get; set; }
        //public string Index { get; set; }
        public virtual int Size { get; set; }
        public Vehicle(string regNumber, DateTime parkingTime)
        {
            RegNumber = regNumber;
            ParkingTime = parkingTime;
        }
        public bool ParkVehicle(ParkingSpot[] parkingSpots)
        {
            for (int i = 1; i < parkingSpots.Length; i++)
            { 
                if (parkingSpots[i].TakeVehicle(this))             //Provar att skicka fordonet till en parkeringsplats. Skickar tillbaka true om det gick bra
                {
                    Console.WriteLine("Vehicle '{0}' registered to spot '{1}'", this.RegNumber, i);
                    return true;
                }
            }
            return false;
        }
    }
}
