using System;

namespace SimulatedDevice
{
    /// <summary>
    /// This class generates a series of data between a mimimum and a maximum value.
    /// It's used to mimic a physical sensor.
    /// </summary>
    class DataGenerator
    {
        private readonly double _minValue;
        private readonly double _maxValue;
        private double _lastValue;
        readonly Random _rand = new Random();

        public DataGenerator(double minValue, double maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _lastValue = _rand.Next((int)_minValue, (int)_maxValue);
        }

        public double GetNextValue()
        {
            double percentage = 5; // 5%

            // generate a new value based on the previous supplied value
            // The new value will be calculated to be within the threshold specified by the "percentage" variable from the original number.
            // The value will also always be within the the specified "min" and "max" values.
            var value = _lastValue * (1 + ((percentage / 100) * (2 * _rand.NextDouble() - 1)));

            value = Math.Max(value, _minValue);
            value = Math.Min(value, _maxValue);
            _lastValue = value;
            return _lastValue;
        }
    }
}
