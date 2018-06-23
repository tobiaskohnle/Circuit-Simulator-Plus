using System;

namespace CircuitSimulatorPlus
{
    public class DropOutStack<T>
    {
        T[] elements;
        int index;
        int count;

        public DropOutStack(int bufferSize)
        {
            elements = new T[bufferSize];
        }

        public void Push(T item)
        {
            if (count < elements.Length)
                count++;

            elements[index] = item;

            if (++index >= elements.Length)
                index = 0;
        }

        public T Pop()
        {
            if (--index < 0)
                index = elements.Length - 1;

            return Peek();
        }

        public T Peek()
        {
            if (count > 0)
            {
                count--;
                return elements[index];
            }
            throw new InvalidOperationException("The stack is empty.");
        }

        public void Clear()
        {
            count = 0;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }
    }
}