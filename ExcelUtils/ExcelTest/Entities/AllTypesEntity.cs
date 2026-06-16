namespace ExcelTest.Entities;

/// <summary>
/// 全类型覆盖实体——测试所有内置转换器的导入导出。
/// </summary>
public class AllTypesEntity
{
    [ExcelColumn(Name = "字符串", Order = 0)]
    public string StringValue { get; set; } = "";

    [ExcelColumn(Name = "整数", Order = 1)]
    public int IntValue { get; set; }

    [ExcelColumn(Name = "长整数", Order = 2)]
    public long LongValue { get; set; }

    [ExcelColumn(Name = "短整数", Order = 3)]
    public short ShortValue { get; set; }

    [ExcelColumn(Name = "字节", Order = 4)]
    public byte ByteValue { get; set; }

    [ExcelColumn(Name = "浮点", Order = 5)]
    public float FloatValue { get; set; }

    [ExcelColumn(Name = "双精度", Order = 6)]
    public double DoubleValue { get; set; }

    [ExcelColumn(Name = "高精度", Order = 7)]
    public decimal DecimalValue { get; set; }

    [ExcelColumn(Name = "布尔", Order = 8)]
    public bool BoolValue { get; set; }

    [ExcelColumn(Name = "日期时间", Order = 9)]
    [ExcelConverter(typeof(LongDateTimeConverter))]
    public DateTime DateTimeValue { get; set; }

    [ExcelColumn(Name = "日期", Order = 10)]
    public DateOnly DateOnlyValue { get; set; }

    [ExcelColumn(Name = "时间", Order = 11)]
    public TimeOnly TimeOnlyValue { get; set; }

    [ExcelColumn(Name = "时间跨度", Order = 12)]
    [ExcelConverter(typeof(TimeSpanMinutesConverter))]
    public TimeSpan TimeSpanValue { get; set; }

    [ExcelColumn(Name = "时间偏移", Order = 13)]
    public DateTimeOffset DateTimeOffsetValue { get; set; }

    [ExcelColumn(Name = "GUID", Order = 14)]
    public Guid GuidValue { get; set; }

    [ExcelColumn(Name = "可空整数", Order = 15, Required = false)]
    public int? NullableInt { get; set; }

    [ExcelColumn(Name = "可空双精度", Order = 16, Required = false)]
    public double? NullableDouble { get; set; }

    [ExcelColumn(Name = "可空日期", Order = 17, Required = false)]
    public DateTime? NullableDateTime { get; set; }

    [ExcelColumn(Name = "可空布尔", Order = 18, Required = false)]
    public bool? NullableBool { get; set; }
}
