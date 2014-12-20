using System;
using System.Collections.Generic;

namespace Chaow.Numeric.Sequence
{
    public sealed class RandomSequence : BaseSequence<int>
    {
        //fields
        int _maxValue;
        int _minValue;
        int _seed;

        //properties

        //constructors
        public RandomSequence() : this(MathExt.Random.Next(), 0, int.MaxValue)
        {
        }

        public RandomSequence(int maxValue) : this(MathExt.Random.Next(), 0, maxValue)
        {
        }

        public RandomSequence(int minValue, int maxValue) : this(MathExt.Random.Next(), minValue, maxValue)
        {
        }

        public RandomSequence(int seed, int minValue, int maxValue)
        {
            _seed = seed;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public int Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }

        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        //protected methods
        protected override IEnumerable<int> enumerate()
        {
            var random = new Random(_seed);
            while (true)
                yield return random.Next(_minValue, _maxValue);
        }

        //public methods
        public void Shuffle()
        {
            _seed = MathExt.Random.Next();
        }
    }
}