using System.ComponentModel.Composition;

namespace AbstractFactoryLib.Low
{
    [Export(nameof(VechileType.Low), typeof(IVechicleFactory))]
    public class LowGradeVechicleFactory : IVechicleFactory
    {
        public ICar CreateCar() => new LowGradeCar();
        public IBike CreateBike() => new LowGradeBike();
    }
}
