using System;

namespace GameCore {
    public class RingBuffer<T> {
        readonly T[] _buffer;
        int _head = -1; // First valid element in the queue; -1 means empty
        int _tail; // Last valid element in the queue

        public int Capacity => _buffer.Length;

        public int Length {
            get {
                if (_head == -1)
                    return 0;

                // Head = 0
                // Tail = 0

                if (_head < _tail)
                    return _tail - _head;
                else
                    return _buffer.Length - (_head - _tail);
            }
        }

        public RingBuffer(int capacity) {
            _buffer = new T[capacity];
        }

        public bool IsFull => _tail == _head;

        public void Enqueue(T o) {
            if (_tail == _head)
                throw new InvalidOperationException("full");

            if (_head == -1) {
                _head = 0;
            }

            _buffer[_tail] = o;
            _tail = (_tail + 1) % _buffer.Length;
        }

        public T Dequeue() {
            if (_head == -1)
                throw new InvalidOperationException("empty");

            var result = _buffer[_head];
            _buffer[_head] = default;
            _head = (_head + 1) % _buffer.Length;
            if (_head == _tail) {
                _head = -1;
            }
            return result;
        }

        public T[] ToArray() {
            if (_head == -1)
                return Array.Empty<T>();

            if (_head >= _tail) {
                var result = new T[Length];
                // Copy(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length);
                Array.Copy(_buffer, _head, result, 0, _buffer.Length - _head);
                Array.Copy(_buffer, 0, result, _buffer.Length - _head, _tail);
                return result;
            }

            return _buffer[_head.._tail];
        }
    }
}