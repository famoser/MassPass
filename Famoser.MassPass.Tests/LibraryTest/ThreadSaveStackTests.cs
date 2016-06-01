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
            var fastThreadSaveStack = new FastThreadSafeStack<byte>();
            var stack = new Stack<byte>();

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

            var start3 = DateTime.Now;
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    fastThreadSaveStack.Push(t1);
                }
            }
            foreach (byte t in items)
            {
                foreach (byte t1 in items)
                {
                    fastThreadSaveStack.Pop();
                }
            }
            var stop3 = DateTime.Now;

            //assert
            var time = stop - start;
            var time1 = stop1 - start1;
            var time2 = stop2 - start2;
            var time3 = stop3 - start3;
            Assert.IsTrue(time3.Ticks < time1.Ticks, "very slow fast stack!");
        }

        [TestMethod]
        public async Task ThreadSaveStackCorrectness()
        {
            var stack = new ThreadSafeStack<byte>();
            Assert.IsTrue(await stack.TryPeek() == default(byte));
            Assert.IsTrue(await stack.TryPop() == default(byte));
        }

        [TestMethod]
        public void FastThreadSaveStack()
        {
            //prepare
            var stack = new FastThreadSafeStack<byte>();
            byte item1 = 2;
            byte item2 = 3;

            //act
            stack.Push(item1);
            stack.Push(item2);

            //assert
            Assert.IsTrue(stack.Pop() == item2);
            Assert.IsTrue(stack.Pop() == item1);
            Assert.IsTrue(stack.Pop() == default(byte));
            Assert.IsTrue(stack.Pop() == default(byte));
            Assert.IsTrue(stack.Pop() == default(byte));
        }
    }
}
