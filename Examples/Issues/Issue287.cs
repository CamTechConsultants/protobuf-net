using NUnit.Framework;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.Issues
{
    [TestFixture]
    public class Issue287
    {
        // List item class to use in testing
        [ProtoContract(SkipConstructor = true)]
        class ListItem
        {
            [ProtoMember(1)]
            public string String;

            public ListItem(string s) { String = s; }
        }

        /*
         * Check that exceptions are thrown using default behaviour with simple nested lists
         */

        [ProtoContract]
        class SomeList : List<ListItem> { }

        [ProtoContract]
        class NestedList : List<SomeList> { }

       [Test, ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Nested or jagged lists and arrays are not supported")]
       public void SerializeNestedList()
       {
           var list1 = new SomeList() { new ListItem("aaa"), new ListItem("bbb") };
           var list2 = new SomeList() { new ListItem("ccc"), new ListItem("ddd") };
           var nestedList = new NestedList();
           nestedList.Add(list1);
           nestedList.Add(list2);

           Serializer.Serialize(Stream.Null, nestedList);
       }

       [Test, ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Nested or jagged lists and arrays are not supported")]
       public void DeserializeNestedList()
       {
           Serializer.Deserialize<NestedList>(Stream.Null);
       }

        /*
         * Check that setting Attribute IgnoreListHandling to true stops the exception being thrown.
         */

       [ProtoContract(IgnoreListHandling = true)]
       class SomeIgnoreList : List<ListItem> { }

       [ProtoContract]
       class NestedIgnoreList : List<SomeIgnoreList> { }

       [Test]
       public void SerializeNestedIgnoreList()
       {
           var list1 = new SomeIgnoreList() { new ListItem("aaa"), new ListItem("bbb") };
           var list2 = new SomeIgnoreList() { new ListItem("ccc"), new ListItem("ddd") };
           var nestedList = new NestedIgnoreList();
           nestedList.Add(list1);
           nestedList.Add(list2);
           
           Serializer.Serialize(Stream.Null, nestedList);
       }

       [Test]
       public void DeserializeNestedIgnoreList()
       {
           Serializer.Deserialize<NestedIgnoreList>(Stream.Null);
       }

       // Cloning doesn't copy list items as they are currently being ignored.
       [Test]
       public void ExecuteNestedIgnoreList()
       {
           var typeModel = TypeModel.Create();
           typeModel.AutoCompile = true;
           var list1 = new SomeIgnoreList() { new ListItem("aaa"), new ListItem("bbb") };
           var list2 = new SomeIgnoreList() { new ListItem("ccc"), new ListItem("ddd") };
           var nestedList = new NestedIgnoreList();
           nestedList.Add(list1);
           nestedList.Add(list2);

           var clone = (NestedIgnoreList)typeModel.DeepClone(nestedList);
           Assert.AreEqual(clone.Count, 2);
           Assert.AreEqual(clone[0].Count, 0); //IgnoreListHandling causes contents of nested lists to be lost
           Assert.AreEqual(clone[1].Count, 0);
       }

        /*
         * Using a property as a wrapper for the whole class provides a way to successfully serialize the contents of nested lists
         */

       [ProtoContract(IgnoreListHandling = true)]
       class IgnoreListWithMember : List<ListItem>
       {
           [ProtoMember(1)]
           private List<ListItem> Items
           {
               get { return this; }
               set { this.AddRange(value); }
           }
       }

       [ProtoContract]
       class NestedIgnoreListWithMember : List<IgnoreListWithMember> { }


       [Test]
       public void ExecuteNestedIgnoreListWithMember()
       {
           var typeModel = TypeModel.Create();
           typeModel.AutoCompile = true;
           var list1 = new IgnoreListWithMember() { new ListItem("aaa"), new ListItem("bbb") };
           var list2 = new IgnoreListWithMember() { new ListItem("ccc"), new ListItem("ddd") };
           var nestedList = new NestedIgnoreListWithMember();
           nestedList.Add(list1);
           nestedList.Add(list2);

           var clone = (NestedIgnoreListWithMember)typeModel.DeepClone(nestedList);
           Assert.AreEqual(clone.Count, 2);
           Assert.AreEqual(clone[0].Count, 2);
           Assert.AreEqual(clone[1].Count, 2);
           Assert.AreEqual(clone[0][0].String, "aaa"); // Contents of nested lists is recovered via the wrapper property.
           Assert.AreEqual(clone[1][1].String, "ddd");
       }
    }
}
