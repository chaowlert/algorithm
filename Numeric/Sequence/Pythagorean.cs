using System;
using System.Collections.Generic;

namespace Chaow.Numeric.Sequence
{
    public sealed class Pythagorean : BaseSequence<Pythagorean.Triple>
    {
        //protect methods
        protected override IEnumerable<Triple> enumerate()
        {
            for (var m = 2;; m++)
            {
                for (var n = (m % 2 == 0) ? 1 : 2; n < m; n += 2)
                {
                    if (MathExt.Gcd(m, n) == 1)
                        yield return new Triple(m, n);
                }
            }
        }

        //child classes
        public struct Triple : IEquatable<Triple>
        {
            //fields
            readonly int _seed1;
            readonly int _seed2;
            readonly int _sideA;
            readonly int _sideB;
            readonly int _sideC;
            readonly int _size;

            public Triple(int seed1, int seed2) : this(seed1, seed2, 1)
            {
            }

            public Triple(int seed1, int seed2, int size)
            {
                _seed1 = seed1;
                _seed2 = seed2;
                _size = size;

                //calculate triple
                checked
                {
                    var seed1pow2 = _seed1 * _seed1;
                    var seed2pow2 = _seed2 * _seed2;

                    _sideA = seed1pow2 - seed2pow2;
                    _sideB = 2 * _seed1 * _seed2;
                    _sideC = seed1pow2 + seed2pow2;

                    if (_size != 1)
                    {
                        _sideA *= _size;
                        _sideB *= _size;
                        _sideC *= _size;
                    }
                }
            }

            //properties
            public int Seed1
            {
                get { return _seed1; }
            }

            public int Seed2
            {
                get { return _seed2; }
            }

            public int Size
            {
                get { return _size; }
            }

            public int SideA
            {
                get { return _sideA; }
            }

            public int SideB
            {
                get { return _sideB; }
            }

            public int SideC
            {
                get { return _sideC; }
            }

            public int Perimeter
            {
                get { return _sideA + _sideB + _sideC; }
            }

            //constructors

            public bool Equals(Triple other)
            {
                return _seed1 == other._seed1 && _seed2 == other._seed2 && _size == other._size;
            }

            public override bool Equals(object obj)
            {
                return obj is Triple && Equals((Triple)obj);
            }

            public override int GetHashCode()
            {
                return _sideA;
            }

            public override string ToString()
            {
                return string.Format("({0}, {1}, {2})", _sideA, _sideB, _sideC);
            }

            public Triple ToSize(int size)
            {
                return new Triple(_seed1, _seed2, size);
            }
        }
    }
}