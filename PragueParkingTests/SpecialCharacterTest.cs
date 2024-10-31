using PragueParkingClassLibrary;
namespace PragueParkingTests
{
    [TestClass]
    public class SpecialCharacterTest
    {
        [TestMethod]
        public void StringDoesContainSpecialCharacter()
        {
            //Arrange
            var parkingGarage = new ParkingGarage(10, 10, 10);
            string stringToTest = "Test hej!";
            bool fromCall;

            //Act

            fromCall = parkingGarage.ContainsSpecialCharacters(stringToTest);

            //Assert
            Assert.IsTrue(fromCall);
        }
        [TestMethod]
        public void StringDoesNotContainSpecialCharacter()
        {
            //Arrange
            var parkingGarage = new ParkingGarage(10, 10, 10);
            string stringToTest = "HejHoppGummiSnopp";
            bool fromCall;

            //Act

            fromCall = parkingGarage.ContainsSpecialCharacters(stringToTest);

            //Assert
            Assert.IsFalse(fromCall);
        }
    }
}
