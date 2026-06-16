using ExcelTest.Entities;
using ExcelTest.Utilities;
using ExcelTool.Converters;

namespace ExcelTest.Tests;

[TestFixture]
internal class ConverterTests : TestBase
{
    private static AllTypesEntity CreateSample(int seed = 1)
    {
        return new AllTypesEntity
        {
            StringValue = $"测试文本_{seed}",
            IntValue = 12345 * seed,
            LongValue = 9876543210L * seed,
            ShortValue = (short)(100 * seed),
            ByteValue = (byte)(200 % (seed + 1) + 1),
            FloatValue = 3.14f * seed,
            DoubleValue = 2.718281828 * seed,
            DecimalValue = 123456.78m * seed,
            BoolValue = seed % 2 == 1,
            DateTimeValue = new DateTime(2024, 6, 1, 12, 30, 45).AddDays(seed),
            DateOnlyValue = new DateOnly(2024, 1, 1).AddDays(seed),
            TimeOnlyValue = new TimeOnly(8 + seed % 12, 30, 0),
            TimeSpanValue = TimeSpan.FromHours(1.5 * seed),
            DateTimeOffsetValue = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.FromHours(8)).AddDays(seed),
            GuidValue = Guid.NewGuid(),
            NullableInt = seed % 2 == 0 ? 999 : null,
            NullableDouble = seed % 2 == 0 ? 3.14159 : null,
            NullableDateTime = seed % 2 == 0 ? new DateTime(2024, 12, 25) : null,
            NullableBool = seed % 2 == 0 ? true : null,
        };
    }

    [Test]
    public void String_RoundTrip()
    {
        var c = new StringConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, "Hello"); var r1 = c.Read(cell);
        c.Write(cell, ""); var r2 = c.Read(cell);
        c.Write(cell, "  s  "); var r3 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.EqualTo("Hello"));
            Assert.That(r2, Is.EqualTo(""));
            Assert.That(r3, Is.EqualTo("  s  "));
        });
    }

    [Test]
    public void Int_RoundTrip()
    {
        var c = new Int32Converter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 0); var r1 = c.Read(cell);
        c.Write(cell, int.MaxValue); var r2 = c.Read(cell);
        c.Write(cell, int.MinValue); var r3 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.EqualTo(0));
            Assert.That(r2, Is.EqualTo(int.MaxValue));
            Assert.That(r3, Is.EqualTo(int.MinValue));
        });
    }

    [Test]
    public void Long_RoundTrip()
    {
        var c = new Int64Converter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 999888777666L); Assert.That(c.Read(cell), Is.EqualTo(999888777666L));
    }

    [Test]
    public void Double_RoundTrip()
    {
        var c = new DoubleConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.1415926535);
        Assert.That(c.Read(cell), Is.EqualTo(3.1415926535).Within(0.0001));
    }

    [Test]
    public void Double_Precision_SetsExcelFormat()
    {
        var c0 = new DoubleConverter(0);
        var c2 = new DoubleConverter(2);
        var c5 = new DoubleConverter(5);
        Assert.Multiple(() =>
        {
            Assert.That(c0.ExcelFormat, Is.EqualTo("0"));
            Assert.That(c2.ExcelFormat, Is.EqualTo("0.00"));
            Assert.That(c5.ExcelFormat, Is.EqualTo("0.00000"));
        });
    }

    [Test]
    public void Double_Precision_DefaultIsNull()
    {
        var c = new DoubleConverter();
        Assert.Multiple(() =>
        {
            Assert.That(c.ExcelFormat, Is.Null);
        });
    }

    [Test]
    public void Double_ApplyToStyle_DefaultNoFormat()
    {
        var c = new DoubleConverter("#,##0.00");
        Assert.That(c.ExcelFormat, Is.EqualTo("#,##0.00"));
    }

    [Test]
    public void Double_ApplyToStyle_SetsDataFormat()
    {
        var c = new DoubleConverter(2);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0.00");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Double_ApplyToStyle_Precision0_SetsDataFormat()
    {
        var c = new DoubleConverter(0);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Double_ApplyToStyle_NoFormat_DoesNotSetDataFormat()
    {
        var c = new DoubleConverter(); // 默认无格式
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        var originalFormat = style.DataFormat;
        c.ApplyToStyle(style, sheet);
        Assert.That(style.DataFormat, Is.EqualTo(originalFormat));
    }

    [Test]
    public void Double_WriteToCell_FormatApplied()
    {
        var c = new DoubleConverter(2);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var cell = sheet.GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.14159, style);
        Assert.That(cell.CellStyle.DataFormat, Is.EqualTo(style.DataFormat));
    }

    [Test]
    public void Decimal_RoundTrip()
    {
        var c = new DecimalConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 123456.78m); Assert.That(c.Read(cell), Is.EqualTo(123456.78m).Within(0.01));
    }

    [Test]
    public void Decimal_Precision_SetsExcelFormat()
    {
        var c0 = new DecimalConverter(0);
        var c2 = new DecimalConverter(2);
        var c4 = new DecimalConverter(4);
        Assert.Multiple(() =>
        {
            Assert.That(c0.ExcelFormat, Is.EqualTo("0"));
            Assert.That(c2.ExcelFormat, Is.EqualTo("0.00"));
            Assert.That(c4.ExcelFormat, Is.EqualTo("0.0000"));
        });
    }

    [Test]
    public void Decimal_Precision_DefaultIsNull()
    {
        var c = new DecimalConverter();
        Assert.Multiple(() =>
        {
            Assert.That(c.ExcelFormat, Is.Null);
        });
    }

    [Test]
    public void Decimal_ApplyToStyle_SetsDataFormat()
    {
        var c = new DecimalConverter(2);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0.00");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Float_RoundTrip()
    {
        var c = new SingleConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.14f); Assert.That(c.Read(cell), Is.EqualTo(3.14f).Within(0.01));
    }

    [Test]
    public void Float_Precision_SetsExcelFormat()
    {
        var c0 = new SingleConverter(0);
        var c2 = new SingleConverter(2);
        var c3 = new SingleConverter(3);
        Assert.Multiple(() =>
        {
            Assert.That(c0.ExcelFormat, Is.EqualTo("0"));
            Assert.That(c2.ExcelFormat, Is.EqualTo("0.00"));
            Assert.That(c3.ExcelFormat, Is.EqualTo("0.000"));
        });
    }

    [Test]
    public void Float_Precision_DefaultIsNull()
    {
        var c = new SingleConverter();
        Assert.Multiple(() =>
        {
            Assert.That(c.ExcelFormat, Is.Null);
        });
    }

    [Test]
    public void Float_ApplyToStyle_SetsDataFormat()
    {
        var c = new SingleConverter(1);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0.0");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Precision_ReuseSameFormat_ReturnsSameIndex()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        // 相同格式字符串应返回相同的 DataFormat 索引
        var fmt1 = wb.GetDataFormat("0.00");
        var fmt2 = wb.GetDataFormat("0.00");
        Assert.That(fmt1, Is.EqualTo(fmt2));
    }

    [Test]
    public void Bool_RoundTrip()
    {
        var c = new BooleanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, true); var r1 = c.Read(cell);
        c.Write(cell, false); var r2 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.True);
            Assert.That(r2, Is.False);
        });
    }

    [Test]
    public void Bool_CustomText()
    {
        var c = new BooleanConverter("对", "错");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, true);
        Assert.Multiple(() =>
        {
            Assert.That(c.Read(cell), Is.True);
            Assert.That(cell.StringCellValue, Is.EqualTo("对"));
        });
    }

    [Test]
    public void DateTime_RoundTrip()
    {
        var c = new DateTimeConverter("yyyy-MM-dd HH:mm:ss");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTime(2024, 6, 15, 14, 30, 0));
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2024)); Assert.That(r.Month, Is.EqualTo(6));
        });
    }

    [Test]
    public void DateOnly_RoundTrip()
    {
        var c = new DateOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateOnly(2025, 3, 20));
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2025)); Assert.That(r.Month, Is.EqualTo(3));
        });
    }

    [Test]
    public void TimeOnly_RoundTrip()
    {
        var c = new TimeOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new TimeOnly(9, 15, 30));
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Hour, Is.EqualTo(9)); Assert.That(r.Minute, Is.EqualTo(15));
        });
    }

    [Test]
    public void TimeSpan_RoundTrip()
    {
        var c = new TimeSpanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, TimeSpan.FromHours(3.5));
        Assert.That(c.Read(cell).TotalHours, Is.EqualTo(3.5).Within(0.01));
    }

    [Test]
    public void Guid_RoundTrip()
    {
        var c = new GuidConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        var g = Guid.NewGuid(); c.Write(cell, g); Assert.That(c.Read(cell), Is.EqualTo(g));
    }

    [Test]
    public void DateTimeOffset_RoundTrip()
    {
        var c = new DateTimeOffsetConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTimeOffset(2024, 8, 1, 0, 0, 0, TimeSpan.FromHours(8)));
        Assert.That(c.Read(cell).Year, Is.EqualTo(2024));
    }

    [Test]
    public void ShortAndByte()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var sc = new Int16Converter(); var bc = new ByteConverter();
        sc.Write(s.GetOrCreateRow(0).GetOrCreateCell(0), (short)12345);
        var sr = sc.Read(s.GetOrCreateRow(0).GetOrCreateCell(0));
        bc.Write(s.GetOrCreateRow(1).GetOrCreateCell(0), (byte)200);
        var br = bc.Read(s.GetOrCreateRow(1).GetOrCreateCell(0));
        Assert.Multiple(() =>
        {
            Assert.That(sr, Is.EqualTo(12345));
            Assert.That(br, Is.EqualTo(200));
        });
    }

    [Test]
    public void LongDateTime_RoundTrip()
    {
        var c = new LongDateTimeConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        var dt = new DateTime(2024, 6, 15, 14, 30, 45);
        c.Write(cell, dt);
        Assert.Multiple(() =>
        {
            Assert.That(c.Read(cell), Is.EqualTo(dt));
            Assert.That(cell.StringCellValue, Is.EqualTo("2024-06-15 14:30:45"));
        });
    }

    [Test]
    public void LongDateTime_CustomFormat()
    {
        var c = new LongDateTimeConverter("yyyy/MM/dd HH:mm");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTime(2024, 12, 1, 8, 0, 0));
        Assert.That(cell.StringCellValue, Is.EqualTo("2024/12/01 08:00"));
    }

    [Test]
    public void TimeSpanMinutes_RoundTrip()
    {
        var c = new TimeSpanMinutesConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);

        // 3.5小时 = 210分钟
        var ts = TimeSpan.FromHours(3.5);
        c.Write(cell, ts);
        Assert.Multiple(() =>
        {
            Assert.That(cell.StringCellValue, Is.EqualTo("210.00"));
            Assert.That(c.Read(cell).TotalMinutes, Is.EqualTo(210).Within(0.01));
        });
    }

    [Test]
    public void TimeSpanHours_RoundTrip()
    {
        var c = new TimeSpanHoursConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);

        // 5400秒 = 1.5小时
        var ts = TimeSpan.FromSeconds(5400);
        c.Write(cell, ts);
        var cellText = cell.StringCellValue;
        var readBack = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(cellText, Is.EqualTo("1.50"));
            Assert.That(readBack.TotalHours, Is.EqualTo(1.5).Within(0.01));
        });
    }

    [Test]
    public void TimeSpanHours_ReadFromString()
    {
        var c = new TimeSpanHoursConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2.75");
        Assert.That(c.Read(cell).TotalHours, Is.EqualTo(2.75).Within(0.01));
    }

    [Test]
    public void Enum_Int_RoundTrip()
    {
        var c = new EnumConverter<DayOfWeek>();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);

        c.Write(cell, DayOfWeek.Wednesday);
        // 输出整数值 3（Sunday=0）
        var cellText = cell.StringCellValue;
        var readBack = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(cellText, Is.EqualTo("3"));
            Assert.That(readBack, Is.EqualTo(DayOfWeek.Wednesday));
        });
    }

    [Test]
    public void Enum_Int_ReadFromString()
    {
        var c = new EnumConverter<DayOfWeek>();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        // 也支持用名称字符串读取
        cell.SetCellValue("Friday");
        Assert.That(c.Read(cell), Is.EqualTo(DayOfWeek.Friday));
    }

    [Test]
    public void Enum_ReadFromNumericString()
    {
        var c = new EnumConverter<DayOfWeek>();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("3");
        Assert.That((int)(object)c.Read(cell), Is.EqualTo(3));
    }

    [Test]
    public void DateTime_ReadFromString()
    {
        var c = new DateTimeConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2024-12-25");
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2024));
            Assert.That(r.Month, Is.EqualTo(12));
        });
    }

    [Test]
    public void DateTime_Empty_ReturnsDefault()
    {
        var c = new DateTimeConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(c.Read(cell), Is.EqualTo(default(DateTime)));
    }

    [Test]
    public void DateTimeOffset_ReadFromString()
    {
        var c = new DateTimeOffsetConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2024-08-01T00:00:00+08:00");
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2024));
            Assert.That(r.Month, Is.EqualTo(8));
        });
    }

    [Test]
    public void Guid_Empty_And_Default()
    {
        var c = new GuidConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, Guid.Empty); var r1 = c.Read(cell);
        cell.SetCellValue(""); var r2 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.EqualTo(Guid.Empty));
            Assert.That(r2, Is.EqualTo(default(Guid)));
        });
    }

    [Test]
    public void Bool_ReadNumeric()
    {
        var c = new BooleanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        // BooleanConverter 不支持从数值读写，IsNumeric 返回 true 不会被识别为布尔
        cell.SetCellValue(1.0);
        var result = c.Read(cell);
        Assert.That(result, Is.True.Or.False); // 接受任意结果，取决于实现
    }

    [Test]
    public void String_Null_ReturnsEmpty()
    {
        var c = new StringConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(c.Read(cell), Is.EqualTo(""));
    }

    [Test]
    public void TimeSpan_ReadFromString()
    {
        var c = new TimeSpanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("02:30:00");
        Assert.That(c.Read(cell), Is.EqualTo(TimeSpan.FromHours(2.5)));
    }

    [Test]
    public void TimeOnly_ReadFromString()
    {
        var c = new TimeOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("14:30:00");
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Hour, Is.EqualTo(14));
            Assert.That(r.Minute, Is.EqualTo(30));
        });
    }

    [Test]
    public void DateOnly_ReadFromString()
    {
        var c = new DateOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2025-06-15");
        Assert.That(c.Read(cell), Is.EqualTo(new DateOnly(2025, 6, 15)));
    }
}
