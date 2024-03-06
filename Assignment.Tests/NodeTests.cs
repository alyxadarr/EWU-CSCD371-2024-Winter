using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Assignment.Tests;

    public class NodeTests
    {
        [Fact]
        public void Node_SingleNode_PointsToSelf()
        {
            Node<string> newNode = new Node<string>("hola");


            Assert.NotNull(newNode.Next); // Ensure Next property is not null
            Assert.Equal(newNode, newNode.Next); // Ensure Next property points to itself
        }
        [Fact]
        public void Append_AddsNodeAfterFirst_Success()
        {
            Node<string> headNode = new("start");
            headNode.Append("second");

            Assert.NotNull(headNode.Next);
            Assert.Equal("second", headNode.Next.Value);
            Assert.Equal("start", headNode.Next.Next.Value); //checks that it is circular and links back to the first node
        }

        [Fact]
        public void Append_AddDuplicateNode_Failure()
        {
            Node<string> headNode = new("start");
            headNode.Append("second");

            Assert.Throws<ArgumentException>(() => headNode.Append("second"));
        }

        [Fact]
        public void Exists_ValueExists_ReturnsTrue()
        {
            Node<string> node = new Node<string>("Hello");

            bool exists = node.Exists("Hello");

            Assert.True(exists);
        }


        [Fact]
        public void Exists_ValueDoesNotExist_ReturnsFalse()
        {
            Node<string> node = new Node<string>("Hello");

            bool exists = node.Exists("World");

            Assert.False(exists);
        }

        [Fact]
        public void ToString_StringMatch_ReturnsCorrectStringRepresentation()
        {
            Node<int> node = new Node<int>(42);

            string result = node.ToString()!;

            Assert.Equal("42", result);
        }

        [Fact]
        public void ToString_HeadsNext_ReturnsCorrectStringRepresentation()
        {
            Node<int> node = new Node<int>(42);
            node.Append(16);

            string result = node.Next.ToString()!;

            Assert.Equal("16", result);
        }

        [Fact]
        public void Clear_OnlyCurrentNodeRemainsinList_Success()
        {
            Node<string> headNode = new("start");
            headNode.Append("second");
            headNode.Append("third");
            headNode.Append("fourth");
            headNode.Clear();
            Assert.Equal("start", headNode.Next.Value);
            Assert.False(headNode.Exists("second"));
            Assert.False(headNode.Exists("third"));
            Assert.False(headNode.Exists("fourth"));
        }

        [Fact]
        public void GetEnumerator_Returns_Successful()
        {
            // Arrange: Create a circular linked list with some sample values
            Node<int> headNode = new(4);

            headNode.Append(1);
            headNode.Append(2);
            headNode.Append(3);

            // Act: Get the enumerator and collect the values
            var values = new List<int>();
            foreach (var value in headNode)
            {
                values.Add(value);
            }

            // Assert: Check if the yielded values match our expectations
            Assert.Equal(new[] { 4, 3, 2, 1 }, values);
        }

        [Fact]
        public void GetEnumerator_ReturnsString_Successful()
        {
            // Arrange: Create a circular linked list with some sample values
            Node<string> headNode = new("one");

            headNode.Append("two");
            headNode.Next.Append("three");
            headNode.Next.Next.Append("four");

            // Act: Get the enumerator and collect the values
            var values = new List<string>();
            foreach (var value in headNode)
            {
                values.Add(value);
            }

            // Assert: Check if the yielded values match our expectations
            Assert.Equal(new[] { "one", "two", "three", "four" }, values);
        }
}


