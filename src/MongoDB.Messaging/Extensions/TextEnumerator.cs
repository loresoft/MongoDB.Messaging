using System;
using System.Collections;
using System.Collections.Generic;

namespace MongoDB.Messaging.Extensions
{
    internal sealed class TextEnumerator : IEnumerator<char>
    {
        private String _value;
        private int _index;
        private char _currentElement;

        internal TextEnumerator(String value)
        {
            _value = value;
            _index = -1;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool MoveNext()
        {
            if (_index < (_value.Length - 1))
            {
                _index++;
                _currentElement = _value[_index];
                return true;
            }

            _index = _value.Length;
            return false;
        }

        public void Dispose()
        {
            if (_value != null)
                _index = _value.Length;

            _value = null;
        }


        public char Current
        {
            get
            {
                if (_index == -1)
                    throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");

                if (_index >= _value.Length)
                    throw new InvalidOperationException("The enumeration has already completed.");

                return _currentElement;
            }
        }

        public void Reset()
        {
            _currentElement = (char)0;
            _index = -1;
        }

        object IEnumerator.Current => Current;
    }
}