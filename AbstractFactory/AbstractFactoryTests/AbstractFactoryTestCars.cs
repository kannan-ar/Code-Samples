using AbstractFactoryLib.High;
using AbstractFactoryLib.Low;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactoryTests
{
    public class AbstractFactoryTestCars : AbstractFactoryBaseTestData
    {
        public AbstractFactoryTestCars()
        {
            AddTestData<LowGradeVechicleFactory, LowGradeCar>();
            AddTestData<HighGradeVechicleFactory, HighGradeCar>();
        }
    }
}
