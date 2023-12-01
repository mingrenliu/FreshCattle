using Microsoft.CodeAnalysis.Diagnostics;

namespace EFEntityAnalyzer
{
    public class AttributeOption
    {
        const string PropertyNameBase = "build_property.";
        const string PrecisionOfDecimalName = nameof(PrecisionOfDecimal);
        const string PrecisionOfDataTimeName = nameof(PrecisionOfDataTime);
        const string MaxLengthOfStringName = nameof(MaxLengthOfString);
        const string EnableDataTimePrecisionName = nameof(EnableDataTimePrecision);
        public int[] PrecisionOfDecimal { get; set; } = new[] { 18, 6 };
        public bool EnableDataTimePrecision { get; set; } = true;
        public int PrecisionOfDataTime { get; set; } = 0;
        public int MaxLengthOfString { get; set; } = 50;

        public static AttributeOption ParseOption(AnalyzerConfigOptions option)
        {
            var result = new AttributeOption();
            if (option.TryGetValue(PropertyNameBase + PrecisionOfDecimalName, out var decimalPrecision))
            {
                var temp = decimalPrecision.Split(',');
                if (temp.Length == 2 && int.TryParse(temp[0], out var precision) && int.TryParse(temp[1], out var scale))
                {
                    result.PrecisionOfDecimal = new[] { precision, scale };
                }
            }
            if (option.TryGetValue(PropertyNameBase + MaxLengthOfStringName, out var maxLength))
            {
                if (int.TryParse(maxLength, out var length))
                {
                    result.MaxLengthOfString = length;
                }
            }
            if (option.TryGetValue(PropertyNameBase + EnableDataTimePrecisionName, out var enableDataTimePrecision))
            {
                if (bool.TryParse(enableDataTimePrecision, out var enable))
                {
                    result.EnableDataTimePrecision = enable;
                }
            }
            if (option.TryGetValue(PropertyNameBase + PrecisionOfDataTimeName, out var dataTimePrecision))
            {
                if (int.TryParse(dataTimePrecision, out var precision))
                {
                    result.PrecisionOfDataTime = precision;
                }
            }
            return result;
        }
    }
}