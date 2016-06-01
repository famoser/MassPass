using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.LibraryTest
{
    [TestClass]
    public class ThreadSaveStackTests
    {
        [TestMethod]
        public async Task TestPerformance()
        {
            //todo: check vs normal stack
        }

        [TestMethod]
        public async Task TestCorrectness()
        {
            //todo: check what if pop on count == 0
        }
    }
}
