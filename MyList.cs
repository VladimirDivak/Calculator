using System;
using System.Collections;
using System.Collections.Generic;

namespace Calculator
{
    public class MyList<T> : IList<T>
    {
        private T[] _elements = new T[8];
        private int _count;

        public MyList() => _count = 0;

        public T this[int index]
        {
            get
            {
                return _elements[index];
            }
            set
            {
                _elements[index] = value;
            }
        }

        public int Count => _count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            bool done = false;

            for(int i = 0; i < _elements.Length; i++)
            {
                if(_elements[i].Equals(default(T)))
                {
                    _elements[i] = item;
                    done = true;
                    break;
                }
            }
            if(!done)
            {
                var newElementsArray = new T[_elements.Length * 2];
                for(int i = 0; i < _elements.Length; i++)
                    newElementsArray[i] = _elements[i];

                _elements = new T[newElementsArray.Length];
                for(int i = 0; i < _elements.Length; i++)
                    _elements[i] = newElementsArray[i];
                
                _elements[_count] = item;   
            }

            _count++;
        }

        public void Clear()
        {
            _elements = new T[8];
            _count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (item.Equals(_elements[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < _count; i++)
            {
                array.SetValue(_elements[i], arrayIndex++);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (item.Equals(_elements[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            if(index < _elements.Length && index > 0)
            {
                var arrayCopy = new T[_count];
                for (int i = 0; i < _count; i++)
                    arrayCopy[i] = _elements[i];

                _elements = new T[_count + 1];
                for (int i = 0; i < _count; i++)
                    _elements[i] = arrayCopy[i];

                int counter = _elements.Length - 1;

                while(counter != index)
                {
                    _elements[counter] = arrayCopy[counter - 1];
                    counter--;
                }

                _elements[index] = item;
                _count++;
            }
            else throw new IndexOutOfRangeException();
        }

        public bool Remove(T item)
        {
            for(int i = 0; i < _elements.Length; i++)
            {
                if(item.Equals(_elements[i]))
                {
                    _elements[i] = default(T);
                    _count--;
                    return true;
                }
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if(index < _elements.Length && index > 0)
            {
                if(index == _elements.Length - 1)
                {
                    _elements[index] = default(T);
                }
                else
                {
                    int counter = index;
                    while(counter != _elements.Length - 1)
                    {
                        _elements[index] = _elements[counter + 1];
                        counter++;
                    }
                    _elements[_elements.Length - 1] = default(T);
                }

                _count--;
            }
            else
                throw new IndexOutOfRangeException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}