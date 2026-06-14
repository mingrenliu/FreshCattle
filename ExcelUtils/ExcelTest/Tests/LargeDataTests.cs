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
        var bytes = ExcelSerializer.Serialize(data);
        sw.Stop();
        Console.WriteLine($"序列化 {count} 条: {sw.ElapsedMilliseconds}ms, 文件 {bytes.Length / 1024}KB");
        Assert.That(bytes.Length, Is.GreaterThan(1024));

        sw.Restart();
        using var ms = new MemoryStream(bytes);
        var imported = ExcelSerializer.Deserialize<Student>(ms).ToList();
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
        var bytes = ExcelSerializer.Serialize(data);
        sw.Stop();
        Console.WriteLine($"4万行序列化: {sw.ElapsedMilliseconds}ms, 文件 {bytes.Length / 1024}KB");
        Assert.That(bytes.Length, Is.GreaterThan(10240)); // >10KB
    }
}
