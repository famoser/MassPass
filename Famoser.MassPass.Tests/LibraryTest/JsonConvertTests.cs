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

        [TestMethod]
        public void CanSerializeByteArrays()
        {
            //arrange
            var model = new ModelWithProperty()
            {
                ByteArray = new byte[] { 12, 23, 1, 41, 2 }
            };

            //act
            var res1 = JsonConvert.SerializeObject(model);
            var model2 = JsonConvert.DeserializeObject<ModelWithProperty>(res1);
            var res2 = JsonConvert.SerializeObject(model2);

            //assert
            Assert.IsTrue(model2.ByteArray[0] == 12, res1 + "   " + res2);
            Assert.IsTrue(model2.ByteArray[1] == 23, res1 + "   " + res2);
            Assert.IsTrue(model2.ByteArray[2] == 1, res1 + "   " + res2);
            Assert.IsTrue(model2.ByteArray[3] == 41, res1 + "   " + res2);
            Assert.IsTrue(model2.ByteArray[4] == 2, res1 + "   " + res2);
        }
    }
}
