using PragueParkingClassLibrary;
namespace PragueParkingTests
{
    [TestClass]
    public class ConfigFileTest
    {
        public TestContext? TestContext { get; set; }
        [TestMethod]
        public void FileDoesExist()
        {
            
            ParkingGarage pg = new ParkingGarage(5, 5, 11);
            string fileName = @"C:\Users\Sebastian\source\repos\Prague Parking V2\Prague Parking V2\Config.txt";
            bool fromCall;

            TestContext?.WriteLine($"Checking for file: '{fileName}'");

            fromCall = pg.FileExists(fileName);
            Assert.IsTrue(fromCall);
        }

        [TestMethod]
        public void FileDoesNotExist()
        {
            string filePath = "../../../";
            ParkingGarage pg = new ParkingGarage(5, 5, 11);
            string fileName = filePath + "Config.txt";
            bool fromCall;

            TestContext?.WriteLine($"Checking for file: '{fileName}'");

            fromCall = pg.FileExists(fileName);
            Assert.IsFalse(fromCall);
        }
    }
}