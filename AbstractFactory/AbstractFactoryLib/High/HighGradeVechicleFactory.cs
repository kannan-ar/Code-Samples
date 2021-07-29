using System.ComponentModel.Composition;

namespace AbstractFactoryLib.High
{
    [Export(nameof(VechileType.High), typeof(IVechicleFactory))]
    public class HighGradeVechicleFactory : IVechicleFactory
    {
        public ICar CreateCar() => new HighGradeCar();
        public IBike CreateBike() => new HighGradeBike();
    }
}
