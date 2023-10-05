using System;

namespace TheLostHopeEngine.EngineCode.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class RangeAttribute : Attribute
    {
        public float MinValue { get; }
        public float MaxValue { get; }

        public RangeAttribute(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
