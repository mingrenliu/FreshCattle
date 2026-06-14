namespace ExcelUtileTest.Tests;

[TestFixture]
internal class SerializerRoundTripTests : TestBase
{
    [Test] public void AllTypes_RoundTrip()
    {
        var original = new List<AllTypesEntity>
        {
            new() { StringValue = "A", IntValue = 1, LongValue = 2, ShortValue = 3, ByteValue = 4,
                FloatValue = 1.1f, DoubleValue = 2.2, DecimalValue = 3.3m, BoolValue = true,
                DateTimeValue = new DateTime(2024,1,1), DateOnlyValue = new DateOnly(2024,1,1),
                TimeOnlyValue = new TimeOnly(8,0), TimeSpanValue = TimeSpan.FromHours(2),
                DateTimeOffsetValue = DateTimeOffset.Now, GuidValue = Guid.NewGuid(),
                NullableInt = 99, NullableDouble = 1.23, NullableDateTime = new DateTime(2024,12,25), NullableBool = true },
            new() { StringValue = "B", IntValue = 100, LongValue = 200, ShortValue = 30, ByteValue = 40,
                FloatValue = 10.1f, DoubleValue = 20.2, DecimalValue = 30.3m, BoolValue = false,
                DateTimeValue = new DateTime(2023,6,15), NullableInt = null, NullableDouble = null, NullableDateTime = null, NullableBool = null },
        };

        var bytes = ExcelSerializer.Serialize(original);
        using var ms = new MemoryStream(bytes);
        var imported = ExcelSerializer.Deserialize<AllTypesEntity>(ms).ToList();

        Assert.That(imported.Count, Is.EqualTo(2));
        for (int i = 0; i < 2; i++)
        {
            var o = original[i]; var r = imported[i];
            Assert.That(r.StringValue, Is.EqualTo(o.StringValue));
            Assert.That(r.IntValue, Is.EqualTo(o.IntValue));
            Assert.That(r.LongValue, Is.EqualTo(o.LongValue));
            Assert.That(r.DoubleValue, Is.EqualTo(o.DoubleValue).Within(0.01));
            Assert.That(r.DecimalValue, Is.EqualTo(o.DecimalValue).Within(0.1));
            Assert.That(r.BoolValue, Is.EqualTo(o.BoolValue));
            Assert.That(r.GuidValue, Is.EqualTo(o.GuidValue));
            Assert.That(r.DateTimeValue.Year, Is.EqualTo(o.DateTimeValue.Year));
            Assert.That(r.DateOnlyValue, Is.EqualTo(o.DateOnlyValue));
            Assert.That(r.NullableInt, Is.EqualTo(o.NullableInt));
            Assert.That(r.NullableBool, Is.EqualTo(o.NullableBool));
        }
    }

    [Test] public void NullValues_RoundTrip()
    {
        var e = new AllTypesEntity { StringValue = "", IntValue = 0, NullableInt = null, NullableDouble = null, NullableDateTime = null, NullableBool = null, GuidValue = Guid.Empty };
        var bytes = ExcelSerializer.Serialize(new[] { e });
        using var ms = new MemoryStream(bytes);
        var r = ExcelSerializer.Deserialize<AllTypesEntity>(ms).First();
        Assert.That(r.NullableInt, Is.Null);
        Assert.That(r.NullableDouble, Is.Null);
        Assert.That(r.NullableDateTime, Is.Null);
        Assert.That(r.NullableBool, Is.Null);
    }

    [Test] public void MaxMinValues_RoundTrip()
    {
        var e = new AllTypesEntity { StringValue = "边界", IntValue = int.MaxValue, LongValue = long.MaxValue, ShortValue = short.MaxValue, ByteValue = byte.MaxValue, DoubleValue = double.MaxValue, DecimalValue = 999999999.99m, BoolValue = true };
        var bytes = ExcelSerializer.Serialize(new[] { e });
        using var ms = new MemoryStream(bytes);
        var r = ExcelSerializer.Deserialize<AllTypesEntity>(ms).First();
        Assert.That(r.IntValue, Is.EqualTo(int.MaxValue));
        Assert.That(r.LongValue, Is.EqualTo(long.MaxValue));
        Assert.That(r.ShortValue, Is.EqualTo(short.MaxValue));
        Assert.That(r.ByteValue, Is.EqualTo(byte.MaxValue));
    }

    [Test] public void MultiSheet_RoundTrip()
    {
        var sheets = new Dictionary<string, IEnumerable<Student>>
        {
            ["一班"] = new[] { new Student { Name = "A1", Age = 15, Score = 80 } },
            ["二班"] = new[] { new Student { Name = "B1", Age = 16, Score = 90 } },
        };
        var bytes = ExcelSerializer.Serialize(sheets);
        using var ms = new MemoryStream(bytes);
        var r = ExcelSerializer.DeserializeAll<Student>(ms);
        Assert.That(r.Count, Is.EqualTo(2));
        Assert.That(r["一班"].First().Name, Is.EqualTo("A1"));
        Assert.That(r["二班"].First().Name, Is.EqualTo("B1"));
    }

    [Test] public void Template_OnlyHeader()
    {
        var bytes = ExcelSerializer.CreateTemplate<Student>();
        using var ms = new MemoryStream(bytes);
        var r = ExcelSerializer.Deserialize<Student>(ms).ToList();
        Assert.That(r, Is.Empty);
    }
}
