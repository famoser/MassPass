using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Famoser.MassPass.Tests.LibraryTest
{
    [TestClass]
    public class ThreadSaveStackTests
    {
        [TestMethod]
        public async Task ThreadSaveStackPerformance()
        {
            //prepare
            var items = new byte[255];
            for (byte i = 0; i < items.Length; i++)
            {
                items[i] = i;
            }
            var theradSafeStack = new ThreadSafeStack<byte>();
            var stack = new Stack<byte>();

            //act
            var start = DateTime.Now;
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    await theradSafeStack.Push(t1);
                }
            }
            var stop = DateTime.Now;

            //act
            var start1 = DateTime.Now;
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    await theradSafeStack.Push(t1);
                }
            }
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    await theradSafeStack.TryPop();
                }
            }
            var stop1 = DateTime.Now;

            var start2 = DateTime.Now;
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    stack.Push(t1);
                }
            }
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    stack.Pop();
                }
            }
            var stop2 = DateTime.Now;

            //assert
            var time = stop - start;
            var time1 = stop - start;
            var time2 = stop2 - start2;
            var difference = time2 - time1;
            //Assert.IsTrue(time1.Ticks*0.3 > time1.Ticks, "very slow stack, more than 30% :(");
        }

        [TestMethod]
        public async Task ThreadSaveStackCorrectness()
        {
            var stack = new ThreadSafeStack<byte>();
            Assert.IsTrue(await stack.TryPeek() == default(byte));
            Assert.IsTrue(await stack.TryPop() == default(byte));
        }
    }
}
