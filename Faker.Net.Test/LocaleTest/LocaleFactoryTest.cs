using Faker.Locales;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Faker.Net.Test.LocaleTest
{
    [TestClass]
    public class LocaleFactoryTest
    {
        [TestMethod]
        public void TestCreate()
        {
            Assert.AreEqual(typeof(Locales.En), LocaleFactory.Create(LocaleType.en).GetType());
        }
    }
}
