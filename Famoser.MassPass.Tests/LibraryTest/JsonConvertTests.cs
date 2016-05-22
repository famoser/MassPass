using Famoser.MassPass.Tests.LibraryTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Famoser.MassPass.Tests.LibraryTest
{
    [TestClass]
    public class JsonConvertTests
    {
        [TestMethod]
        public void CanUseBaseClasses()
        {
            //arrange
            var model = new ModelWithProperty()
            {
                MyProperty = "hallo welt"
            };

            //act
            var res1 = JsonConvert.SerializeObject(model);
            var res2 = JsonConvert.SerializeObject((BaseModelWithoutProperty)model);

            //assert
            Assert.IsTrue(res1 == res2);
        }
    }
}
