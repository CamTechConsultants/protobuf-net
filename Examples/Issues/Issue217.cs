using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Examples.Issues
{
    [TestFixture]
    public class Issue217
    {
        [ProtoContract]
        public class Test
        {
            [ProtoMember(1, SupportNull = true)]
            public List<string> Strings { get; set; }
        }
        [Test]
        public void Execute()
        {
            var typeModel = TypeModel.Create();
            typeModel.AutoCompile = true;
            var obj = new Test() { Strings = new List<string> { "foo", null, "bar" } };

            var clone = (Test)typeModel.DeepClone(obj);
            Assert.AreEqual(clone.Strings[0], "foo");
            Assert.IsNull(clone.Strings[1]);
            Assert.AreEqual(clone.Strings[2], "bar");
        }
    }
}
