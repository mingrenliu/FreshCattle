namespace ExcelUtileTest.Tests;

[TestFixture]
internal class LargeDataTests : TestBase
{
    [Test] public void Serialize_Deserialize_5000_Rows()
    {
        var count = 5000;
        var data = Enumerable.Range(1, count).Select(i => new Student
        {
            Name = $"学生_{i:D6}",
            Age = 18 + i % 30,
            Score = 60 + i % 40 + Math.Round(new Random(i).NextDouble(), 1),
            Graduated = i % 3 != 0,
            EnrollDate = new DateTime(2020, 1, 1).AddDays(i % 1000),
        }).ToList();

        var sw = Stopwatch.StartNew();
        var bytes = Excel.Serialize(data);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, count), bytes);
        sw.Stop();
        Console.WriteLine($"序列化 {count} 条: {sw.ElapsedMilliseconds}ms, 文件 {bytes.Length / 1024}KB");
        Assert.That(bytes.Length, Is.GreaterThan(1024));

        sw.Restart();
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<Student>(ms).ToList();
        sw.Stop();
        Console.WriteLine($"反序列化 {count} 条: {sw.ElapsedMilliseconds}ms");

        Assert.That(imported.Count, Is.EqualTo(count));
        Assert.That(imported[0].Name, Is.EqualTo(data[0].Name));
        Assert.That(imported[^1].Name, Is.EqualTo(data[^1].Name));
    }

    [Test] public void Serialize_40000_Rows_Smoke()
    {
        var data = Enumerable.Range(1, 40000).Select(i => new Student
        {
            Name = $"S_{i:D8}",
            Age = 18 + i % 30,
            Score = 60 + (i % 40),
            Graduated = i % 2 == 0,
            EnrollDate = DateTime.Today,
        }).ToList();

        var sw = Stopwatch.StartNew();
        var bytes = Excel.Serialize(data);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, 40000), bytes);
        sw.Stop();
        Console.WriteLine($"4万行序列化: {sw.ElapsedMilliseconds}ms, 文件 {bytes.Length / 1024}KB");
        Assert.That(bytes.Length, Is.GreaterThan(10240));
    }

    [Test] public void Serialize_AllTypes_10000_Rows()
    {
        var count = 10000;
        var data = Enumerable.Range(1, count).Select(i => new AllTypesEntity
        {
            StringValue = $"行_{i:D6}",
            IntValue = i,
            LongValue = i * 1000L,
            ShortValue = (short)(i % 100),
            ByteValue = (byte)(i % 256),
            FloatValue = i * 1.1f,
            DoubleValue = i * 2.2,
            DecimalValue = i * 3.3m,
            BoolValue = i % 2 == 0,
            DateTimeValue = DateTime.Today.AddDays(i % 365),
            DateOnlyValue = DateOnly.FromDateTime(DateTime.Today.AddDays(i % 365)),
            TimeOnlyValue = new TimeOnly(i % 24, i % 60),
            TimeSpanValue = TimeSpan.FromMinutes(i),
            DateTimeOffsetValue = DateTimeOffset.Now.AddDays(i % 100),
            GuidValue = Guid.NewGuid(),
            NullableInt = i % 3 == 0 ? null : i,
            NullableDouble = i % 5 == 0 ? null : i * 1.5,
            NullableDateTime = i % 7 == 0 ? null : DateTime.Today.AddDays(i),
            NullableBool = i % 4 == 0 ? null : true,
        }).ToList();

        var sw = Stopwatch.StartNew();
        var bytes = Excel.Serialize(data);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(19, count), bytes);
        sw.Stop();
        Console.WriteLine($"全类型{count}行序列化: {sw.ElapsedMilliseconds}ms, {bytes.Length / 1024}KB");

        sw.Restart();
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<AllTypesEntity>(ms).ToList();
        sw.Stop();
        Console.WriteLine($"全类型{count}行反序列化: {sw.ElapsedMilliseconds}ms");

        Assert.That(imported.Count, Is.EqualTo(count));
        Assert.That(imported[0].StringValue, Is.EqualTo(data[0].StringValue));
        Assert.That(imported[^1].StringValue, Is.EqualTo(data[^1].StringValue));
    }

    [Test] public void Serialize_ManySheets()
    {
        var sheets = new Dictionary<string, IEnumerable<Student>>();
        for (int i = 1; i <= 10; i++)
        {
            sheets[$"班级_{i}"] = new[] { new Student { Name = $"学生_{i}", Age = 15 + i, Score = 70 + i } };
        }

        var sw = Stopwatch.StartNew();
        var bytes = Excel.Serialize(sheets);
        sw.Stop();
        Console.WriteLine($"10个Sheet序列化: {sw.ElapsedMilliseconds}ms, {bytes.Length / 1024}KB");
        Assume.That(bytes.Length, Is.GreaterThan(500));
    }
}
