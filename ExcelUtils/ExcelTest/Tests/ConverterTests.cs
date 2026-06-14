using ExcelUtile.Converters;

namespace ExcelUtileTest.Tests;

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

    [Test] public void String_RoundTrip()
    {
        var c = new StringConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, "Hello"); Assert.That(c.Read(cell), Is.EqualTo("Hello"));
        c.Write(cell, ""); Assert.That(c.Read(cell), Is.EqualTo(""));
        c.Write(cell, "  s  "); Assert.That(c.Read(cell), Is.EqualTo("  s  "));
    }

    [Test] public void Int_RoundTrip()
    {
        var c = new Int32Converter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 0); Assert.That(c.Read(cell), Is.EqualTo(0));
        c.Write(cell, int.MaxValue); Assert.That(c.Read(cell), Is.EqualTo(int.MaxValue));
        c.Write(cell, int.MinValue); Assert.That(c.Read(cell), Is.EqualTo(int.MinValue));
    }

    [Test] public void Long_RoundTrip()
    {
        var c = new Int64Converter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 999888777666L); Assert.That(c.Read(cell), Is.EqualTo(999888777666L));
    }

    [Test] public void Double_RoundTrip()
    {
        var c = new DoubleConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.1415926535);
        Assert.That(c.Read(cell), Is.EqualTo(3.1415926535).Within(0.0001));
    }

    [Test] public void Decimal_RoundTrip()
    {
        var c = new DecimalConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 123456.78m); Assert.That(c.Read(cell), Is.EqualTo(123456.78m).Within(0.01));
    }

    [Test] public void Float_RoundTrip()
    {
        var c = new SingleConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.14f); Assert.That(c.Read(cell), Is.EqualTo(3.14f).Within(0.01));
    }

    [Test] public void Bool_RoundTrip()
    {
        var c = new BooleanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, true); Assert.That(c.Read(cell), Is.True);
        c.Write(cell, false); Assert.That(c.Read(cell), Is.False);
    }

    [Test] public void Bool_CustomText()
    {
        var c = new BooleanConverter("对", "错");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, true); Assert.That(c.Read(cell), Is.True);
        Assert.That(cell.StringCellValue, Is.EqualTo("对"));
    }

    [Test] public void DateTime_RoundTrip()
    {
        var c = new DateTimeConverter("yyyy-MM-dd HH:mm:ss");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTime(2024, 6, 15, 14, 30, 0));
        var r = c.Read(cell);
        Assert.That(r.Year, Is.EqualTo(2024)); Assert.That(r.Month, Is.EqualTo(6));
    }

    [Test] public void DateOnly_RoundTrip()
    {
        var c = new DateOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateOnly(2025, 3, 20));
        var r = c.Read(cell);
        Assert.That(r.Year, Is.EqualTo(2025)); Assert.That(r.Month, Is.EqualTo(3));
    }

    [Test] public void TimeOnly_RoundTrip()
    {
        var c = new TimeOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new TimeOnly(9, 15, 30));
        var r = c.Read(cell);
        Assert.That(r.Hour, Is.EqualTo(9)); Assert.That(r.Minute, Is.EqualTo(15));
    }

    [Test] public void TimeSpan_RoundTrip()
    {
        var c = new TimeSpanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, TimeSpan.FromHours(3.5));
        Assert.That(c.Read(cell).TotalHours, Is.EqualTo(3.5).Within(0.01));
    }

    [Test] public void Guid_RoundTrip()
    {
        var c = new GuidConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        var g = Guid.NewGuid(); c.Write(cell, g); Assert.That(c.Read(cell), Is.EqualTo(g));
    }

    [Test] public void DateTimeOffset_RoundTrip()
    {
        var c = new DateTimeOffsetConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTimeOffset(2024, 8, 1, 0, 0, 0, TimeSpan.FromHours(8)));
        Assert.That(c.Read(cell).Year, Is.EqualTo(2024));
    }

    [Test] public void Short_Byte_RoundTrip()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var sc = new Int16Converter(); var bc = new ByteConverter();
        sc.Write(s.GetOrCreateRow(0).GetOrCreateCell(0), (short)12345);
        Assert.That(sc.Read(s.GetOrCreateRow(0).GetOrCreateCell(0)), Is.EqualTo(12345));
        bc.Write(s.GetOrCreateRow(1).GetOrCreateCell(0), (byte)200);
        Assert.That(bc.Read(s.GetOrCreateRow(1).GetOrCreateCell(0)), Is.EqualTo(200));
    }
}
