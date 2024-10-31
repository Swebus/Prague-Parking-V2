
using System.Text.RegularExpressions;

namespace PragueParkingClassLibrary
{
    public class ParkingGarage
    {

        public int McPrize;
        public int CarPrize;
        public int GarageSize;

        public ParkingGarage(int mcPrize, int carPrize, int garageSize)
        {
            McPrize = mcPrize;
            CarPrize = carPrize;
            GarageSize = garageSize;
        }

        public bool FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            return File.Exists(fileName);
        }
        public bool ContainsSpecialCharacters(string regNumber)
        {
            return Regex.IsMatch(regNumber, @"[^\p{L}\p{N}]");
        }
    } 
}
