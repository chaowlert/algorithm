using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;

namespace Chaow.Algorithms
{
    public class BinaryIndexedTree
    {
        readonly int[] _tree;
        readonly int _mid;

        public int[] Tree
        {
            get { return _tree; }
        }

        public BinaryIndexedTree(int[] values)
        {
            _tree = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                Add(i, values[i]);
            }

            _mid = 1;
            for (var num = values.Length - 1; num > 1; num >>= 1)
                _mid <<= 1;
            _mid--;
        }

        public int Sum(int left, int right)
        {
            return Sum(right) - Sum(left - 1);
        }

        public void Add(int k, int val)
        {
            for (int i = k; i < _tree.Length; i |= i + 1)
                _tree[i] += val;
        }

        public int Sum(int k)
        {
            int r = 0;
            for (int i = k; i >= 0; i = (i & (i + 1)) - 1)
                r += _tree[i];
            return r;
        }

        public int Get(int k)
        {
            int sum = _tree[k];
            int z = (k & (k + 1)) - 1;
            for (var i = k - 1; i != z; i = (i & (i + 1)) - 1)
            {
                sum -= _tree[i];
            }
            return sum;
        }

        public int Find(int sumValue)
        {
            int i = 0, c = -1;
            for (int mask = _mid, k = mask; ; mask >>= 1, k = i + mask)
            {
                if (k >= _tree.Length)
                    continue;
                else if (sumValue == _tree[k])
                    c = k;
                else if (sumValue > _tree[k])
                {
                    i = k + 1;
                    sumValue -= _tree[k];
                }
                if (mask == 0)
                    break;
            }
            return c >= 0 ? c : -i;
        }
    }
}
