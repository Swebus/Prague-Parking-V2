
namespace PragueParkingClassLibrary
{
    public class ParkingSpot
    {
        public List<Vehicle> parkingSpot { get; set; }
        public int MaxSize { get; }
        public int CurrentSize { get; set; }
        public ParkingSpot(int currentSize) 
        {
            MaxSize = 4;
            parkingSpot = new List<Vehicle>();
            CurrentSize = currentSize;
        }
        public bool TakeVehicle(Vehicle vehicle)               //Kollar om fordonet de tar emot får plats. Om ja så läggs den in, och skickar tillbaka true
        {
            if (CurrentSize + vehicle.Size <= MaxSize) 
            { 
                parkingSpot.Add(vehicle);
                CurrentSize += vehicle.Size;
                return true; 
            }
            return false;
        }

        public bool ContainsVehicle(string regNumber)            //Kollar om regNummret man skickar in redan finns i listan
        {
            return parkingSpot.Any(vehicle => vehicle.RegNumber == regNumber);
        }
    }
}
