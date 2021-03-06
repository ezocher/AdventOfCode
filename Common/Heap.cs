using System;
using System.Runtime.CompilerServices;
namespace AdventOfCode.Common {
    public sealed class Heap<T> where T : IComparable<T> {
        private int size;
        private T[] nodes;

        public Heap() : this(1000) { }
        public Heap(int maxNodes) {
            size = 0;
            nodes = new T[maxNodes];
        }
        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return size; }
        }
        public void Clear() {
            size = 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(T node) {
            if (size == nodes.Length) {
                Resize();
            }

            int current = size;
            nodes[size++] = node;

            while (current > 0) {
                int parent = (current - 1) >> 1;
                if (node.CompareTo(nodes[parent]) >= 0) { break; }

                nodes[current] = nodes[parent];
                nodes[parent] = node;
                current = parent;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public T Dequeue() {
            T result = nodes[0];
            int current = 0;
            nodes[0] = nodes[--size];

            do {
                int last = current;
                int left = (current << 1) + 1;
                int right = left + 1;
                if (size > left && nodes[current].CompareTo(nodes[left]) > 0) {
                    current = left;
                }
                if (size > right && nodes[current].CompareTo(nodes[right]) > 0) {
                    current = right;
                }

                if (current == last) { break; }

                T temp = nodes[current];
                nodes[current] = nodes[last];
                nodes[last] = temp;
            } while (true);

            return result;
        }
        private void Resize() {
            T[] newNodes = new T[(int)(nodes.Length * 1.5)];
            for (int i = 0; i < nodes.Length; i++) {
                newNodes[i] = nodes[i];
            }
            nodes = newNodes;
        }

        public override string ToString() {
            return $"Count = {Count}";
        }
    }
}