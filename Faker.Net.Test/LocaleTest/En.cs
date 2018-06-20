using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Faker.Net.Test.LocaleTest
{
    [TestClass]
    public class En
    {
        [TestMethod]
        public void TestCurrencyRead()
        {
            Locales.En en = new Locales.En();
            var dic = en.Currency;
            var result = dic["UAE Dirham"];
            Assert.AreEqual("AED", result.code);
        }
    }
}
