
namespace PragueParkingClassLibrary
{
    public class Car : Vehicle
    {
        private int size = 4;

        public override int Size
        {
            get { return size; }
        }
        public Car(string regNumber, DateTime parkingTime)
            : base(regNumber, parkingTime)
        {
            
        }
    }
}
