using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Assignment;

public class Node<T> : IEnumerable<T>
{

    public T Value { get; set; }

    public Node<T> Next { get; private set; }

    // Constructor that takes a value
    public Node(T value)
    {
        Value = value;

        Next = this; // By default, the Next pointer refers back to itself


    }

    public override string? ToString()
    {
        if (Value == null) return null;
        return Value.ToString()!;

    }

    public void Append(T value)
    {
        if (Exists(value))
        {
            throw new ArgumentException("Value already exists");
        }
        else
        {
            Node<T> newNode = new(value)
            {

                Next = Next // New node points to the current node's next
            };
            Next = newNode; // Update current node's next to point to the new node

        }
    } 

        public void Clear()
        {
            Node<T> headNode = this;
            headNode.Next = this;

            Next = this;

        }

        public Boolean Exists(T value)

        {
            Node<T> headNode = this;
            do
            {
                if (headNode.Value!.Equals(value))

                {
                    return true;
                }
                headNode = headNode.Next;
            } while (headNode != this);

            return false;
        }

    public IEnumerator<T> GetEnumerator()
    {
        Node<T> currentNode = this;
        do
        {
            yield return currentNode.Value;
            currentNode = currentNode.Next;
        } while (currentNode != this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public IEnumerable<T> ChildItems(int maximum)
    {
        Node<T> currentNode = this;
        int count = 0;
           while(currentNode.Next!=this && count<maximum)
           {       
                yield return currentNode.Value;
                currentNode= currentNode.Next;
                count++;
            } 
        
    }

}

