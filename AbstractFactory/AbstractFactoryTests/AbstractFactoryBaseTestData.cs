using AbstractFactoryLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AbstractFactoryTests
{
    public abstract class AbstractFactoryBaseTestData : IEnumerable<object[]>
    {
        private readonly TheoryData<IVechicleFactory, Type> _data = new TheoryData<IVechicleFactory, Type>();

        protected void AddTestData<TConcreteFactory, TExpectedVechicle>()
            where TConcreteFactory : IVechicleFactory, new()
        {
            _data.Add(new TConcreteFactory(), typeof(TExpectedVechicle));
        }

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
    }
}
