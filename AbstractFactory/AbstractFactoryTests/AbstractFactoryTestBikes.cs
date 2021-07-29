using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstractFactoryLib.Low;
using AbstractFactoryLib.High;

namespace AbstractFactoryTests
{
    public class AbstractFactoryTestBikes : AbstractFactoryBaseTestData
    {
        public AbstractFactoryTestBikes()
        {
            AddTestData<LowGradeVechicleFactory, LowGradeBike>();
            AddTestData<HighGradeVechicleFactory, HighGradeBike>();
        }
    }
}
