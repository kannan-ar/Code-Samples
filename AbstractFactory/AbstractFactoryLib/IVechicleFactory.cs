namespace AbstractFactoryLib
{
    public interface IVechicleFactory
    {
        ICar CreateCar();
        IBike CreateBike();
    }
}
