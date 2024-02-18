using Microsoft.VisualStudio.TestTools.UnitTesting;
using Meyer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meyer.Tests
{
    [TestClass()]
    public class DiceTests
    {
        [TestMethod()]
        public void RollTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                int result = new Dice().Value;
                Console.WriteLine(result);
                Assert.IsTrue(result >= 1);
                Assert.IsTrue(result <= 6);
            }
        }
    }
}