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
    public class DiceSetTests
    {
        [TestMethod()]
        public void DiceValueTest()
        {
            Assert.AreEqual(new DiceSet(4, 5).Value, 54);
            Assert.AreEqual(new DiceSet(5, 4).Value, 54);
            Assert.AreEqual(new DiceSet(1, 3).Value, 31);
            Assert.AreEqual(new DiceSet(6, 6).Value, 66);
            Assert.AreEqual(new DiceSet(2, 2).Value, 22);
            Assert.AreEqual(new DiceSet(4, 1).Value, 41);
        }

        [TestMethod()]
        public void IsMeyerTest()
        {
            Assert.IsTrue(new DiceSet(1, 2).IsMeyer);
            Assert.IsTrue(new DiceSet(2, 1).IsMeyer);

            Assert.IsFalse(new DiceSet(1, 3).IsMeyer);
            Assert.IsFalse(new DiceSet(3, 1).IsMeyer);
            Assert.IsFalse(new DiceSet(4, 4).IsMeyer);
            Assert.IsFalse(new DiceSet(4, 2).IsMeyer);
        }

        [TestMethod()]
        public void IsBetterThanTest()
        {
            Assert.IsTrue(new DiceSet(12).IsBetterThan(new DiceSet(44)));
            Assert.IsTrue(new DiceSet(13).IsBetterThan(new DiceSet(44)));
            Assert.IsTrue(new DiceSet(55).IsBetterThan(new DiceSet(44)));
            

            Assert.IsFalse(new DiceSet(45).IsBetterThan(new DiceSet(44)));
            Assert.IsFalse(new DiceSet(56).IsBetterThan(new DiceSet(44)));
        }
    }
}