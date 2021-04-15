using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore
{
    public class CyclicList<T> 
    {
        private List<T> _entireList = new List<T>();
        private List<T> _elementsLeft = new List<T>();

        public int Count { get { return _entireList.Count; } }
        public int CountLeft {  get { return _elementsLeft.Count; } }

        public CyclicList(List<T> elementsList) 
        {
            if (elementsList.Count < 2) 
                throw new System.Exception("Received list contains <2 elements.");

            _entireList.AddRange(elementsList);
        }

        public CyclicList(T[] elementsArray)
        {
            if (elementsArray.Length < 2) 
                throw new System.Exception("Received list contains <2 elements.");

            _entireList.AddRange(elementsArray);
            _elementsLeft = new List<T>();
        }

        public T Get(bool random = false, bool shuffleIfEmpty = false) 
        {
            if (_elementsLeft.Count <= 0)
            {
                _elementsLeft.AddRange(_entireList);

                if (shuffleIfEmpty)
                    _elementsLeft.Shuffle();
            }

            int indexTaken = random ? _elementsLeft.Count - 1 : Random.Range(0, _elementsLeft.Count);

            T returned = _elementsLeft[indexTaken];

            _elementsLeft.RemoveAt(indexTaken);

            return returned;
        }

        public void Add(T element) 
        {
            _entireList.Add(element);
            _elementsLeft.Add(element);
        }

        public void RemoveFromEntireList(T element)
        {
            if (_entireList.Count <= 2)
                throw new System.Exception("Can't remove elements from list.");

            _entireList.Remove(element);
        }

        public void RemoveFromElementsLeft(T element) 
        {
            _elementsLeft.Remove(element);
        }

        public void RemoveImmediately(T element)
        {
            RemoveFromElementsLeft(element);
            RemoveFromEntireList(element);
        }
    }
}
