using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PinkoCommon.Utility;
using PinkoMocks;

namespace PinkoTests
{
    [TestClass]
    public class PinkoDictionaryTest
    {
        [TestMethod]
        public void TestReplaceConditionNull()
        {
            var dictionary = new PinkoDictionary<string, string>();

            dictionary["k1"] = "value1";
            dictionary["k2"] = "value2";

            var replacedValue = dictionary.ReplaceCondition("k1",
                                                            x => false,
                                                            x => "new value1");
            Assert.IsNull(replacedValue);
            Assert.IsTrue(dictionary["k1"].Equals("value1"));
            Assert.IsTrue(dictionary["k2"].Equals("value2"));
        }

        [TestMethod]
        public void TestReplaceCondition()
        {
            var dictionary = new PinkoDictionary<string, string>();

            dictionary["k1"] = "value1";
            dictionary["k2"] = "value2";

            var replacedValue = dictionary.ReplaceCondition("k1",
                                                            x => true,
                                                            x => "new value1");
            Assert.IsNotNull(replacedValue);
            Assert.IsTrue(dictionary["k1"].Equals("new value1"));
            Assert.IsTrue(dictionary["k2"].Equals("value2"));
        }


        [TestMethod]
        public void TestCount()
        {
            var dictionary = new PinkoDictionary<string, string>();

            dictionary["k1"] = "value1";
            dictionary["k2"] = "value2";

            Assert.IsTrue(dictionary.Count() == 2);
        }

        [TestMethod]
        public void TestEnum()
        {
            var dictionary = new PinkoDictionary<string, string>();

            dictionary["k1"] = "value1";
            dictionary["k2"] = "value2";

            Assert.IsTrue(dictionary.GetEnumerator().Count() == 2);
            Assert.IsTrue(dictionary.GetEnumerator().Count(x => x.Equals("value1")) == 1);
            Assert.IsTrue(dictionary.GetEnumerator().Count(x => x.Equals("value2")) == 1);
            Assert.IsTrue(dictionary.GetEnumerator().Count(x => x.Equals("value3")) == 0);
        }
        
        [TestMethod]
        public void TestIdexer()
        {
            var dictionary = new PinkoDictionary<string, string>();

            Assert.IsNull(dictionary["k1"]);
            Assert.IsNotNull(dictionary["k1"] = "value1");
            Assert.IsNotNull(dictionary["k1"]);

            Assert.IsNull(dictionary["k2"]);
        }

        [TestMethod]
        public void TestGet()
        {
            var dictionary = new PinkoDictionary<string, string>();

            Assert.IsNotNull(dictionary.Get("k1", () => "value1"));
            Assert.IsNull(dictionary.Get("k1", () => "value2"));
            Assert.IsNotNull(dictionary.Get("k2", () => "value1"));
        }

        [TestMethod]
        public void TestUpdate()
        {
            var dictionary = new PinkoDictionary<string, string>();

            Assert.IsFalse(dictionary.Update("k1", () => "value1"));
            Assert.IsTrue(dictionary.Update("k1", () => "value2"));
            Assert.IsFalse(dictionary.Update("k2", () => "value1"));
        }

        [TestMethod]
        public void TestRemove()
        {
            var dictionary = new PinkoDictionary<string, string>();

            Assert.IsFalse(dictionary.Update("k1", () => "value1"));
            Assert.IsNotNull(dictionary.Remove("k1"));
            Assert.IsNull(dictionary.Remove("k1"));
            Assert.IsNull(dictionary.Remove("k2"));
        }
    }
}
