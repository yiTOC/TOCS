using System;

namespace TOCS
{
    public abstract class ValueRule<T>
    {
        public T MinValue { get; protected set; }
        public T MaxValue { get; protected set; }
        public T Step { get; protected set; }

        public ValueRule(T minValue, T maxValue, T step)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Step = step;
        }
        public ValueRule((T, T, T) tuple)
        : this(tuple.Item1, tuple.Item2, tuple.Item3)
        { }

        public abstract int RepeatIndex(int value);
        public abstract T GetValueByIndex(int index);
        public abstract int GetNearestIndex(T num);
    }
    public class IntegerValueRule : ValueRule<int>
    {
        public IntegerValueRule(int minValue, int maxValue, int step)
        : base(minValue, maxValue, step) { }
        public IntegerValueRule((int, int, int) tuple)
        : base(tuple) { }

        public static implicit operator IntegerValueRule((int, int, int) tuple)
            => new(tuple);

        public override int RepeatIndex(int value)
        {
            int MaxIndex = (MaxValue - MinValue) / Step;
            value = value % (MaxIndex + 1);
            if (value < 0) value = MaxIndex;
            return value;
        }

        public override int GetValueByIndex(int index)
            => RepeatIndex(index) * Step + MinValue;

        public override int GetNearestIndex(int num)
        {
            return (int)Math.Round((num - MinValue) / (float)Step);
        }
    }
}