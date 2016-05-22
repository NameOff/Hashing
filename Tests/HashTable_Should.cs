using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using HashTable;
using LinearHashing;

namespace Tests
{
    [TestFixture]
    public class HashTable_Should
    {
        private static HashTable<int, string> hashTable;

        [SetUp]
        public void SetUp()
        {
            hashTable = new HashTable<int, string>();
        }

        [Test]
        public void HaveOneElement_AfterAddOneElement()
        {
            hashTable[10] = "10";
            hashTable[10].Should().Be("10");
        }

        [Test]
        public void HaveTenThousandElements_AfterAddTenThousandElements()
        {
            for (var i = 0; i < 10000; i++)
                hashTable[i] = "Hello";
            hashTable.Count.Should().Be(10000);
        }

        [Test]
        public void HaveChangedKeyValue_AfterChangeKeyValue()
        {
            hashTable[15] = "Test1";
            hashTable[15] = "Test2";
            hashTable[15].Should().Be("Test2");
        }

        [Test]
        public void Empty_AfterRemoveAllElements()
        {
            for (var i = 0; i < 100; i++)
                hashTable[i] = i.ToString();
            for (var i = 0; i < 100; i++)
                hashTable.Remove(i);
            hashTable.Count.Should().Be(0);
        }

        [Test]
        public void ThrowKeyNotFoundException_AfterFindRemovedKey()
        {
            hashTable[100] = "Hello";
            hashTable.Remove(100);
            Assert.Throws<KeyNotFoundException>(() => Console.WriteLine(hashTable[100]));
        }

        [Test]
        public void ThrowKeyNotFoundException_AfterRemoveNonexistentKey()
        {
            Assert.Throws<KeyNotFoundException>(() => hashTable.Remove(10));
        }
    }
}
