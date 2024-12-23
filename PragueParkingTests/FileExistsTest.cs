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
            string filepath = "../../../";
            ParkingGarage pg = new ParkingGarage();
            string fileName = filepath + "Config.txt";
            bool fromCall;

            TestContext?.WriteLine($"Checking for file: '{fileName}'");

            fromCall = pg.FileExists(fileName);
            Assert.IsTrue(fromCall);
        }

        [TestMethod]
        public void FileDoesNotExist()
        {
            string filePath = "../../../";
            ParkingGarage pg = new ParkingGarage();
            string fileName = filePath + "HejHopp.txt";
            bool fromCall;

            TestContext?.WriteLine($"Checking for file: '{fileName}'");

            fromCall = pg.FileExists(fileName);
            Assert.IsFalse(fromCall);
        }
    }
}