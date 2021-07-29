using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractFactoryLib;
using Xunit;

namespace AbstractFactoryTests
{
    public class AbstractFactoryTest
    {
        [Theory, ClassData(typeof(AbstractFactoryTestCars))]
        public void Should_create_a_Car_of_the_specified_type(IVechicleFactory vechicleFactory, Type expectedCarType)
        {
            ICar result = vechicleFactory.CreateCar();
            Assert.IsType(expectedCarType, result);
        }
    }
}
