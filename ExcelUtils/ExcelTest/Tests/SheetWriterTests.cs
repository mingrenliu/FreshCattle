namespace ExcelUtileTest.Tests;

[TestFixture]
internal class SheetWriterTests : TestBase
{
    [Test] public void Merge_Style_WriteAt_RoundTrip()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet("低层测试"));

        writer.Style("h", s => { s.FillForegroundColor = 22; s.FillPattern = FillPattern.SolidForeground; });

        writer.MoveTo(0, 0);
        writer.Merge(new MergeRegion(0, 0, 0, 2) { Value = "标题行" });
        writer.MoveTo(1, 0);
        writer.Write("列A", writer.Style("h")); writer.Width(15);
        writer.Write("列B", writer.Style("h")); writer.Width(15);
        writer.NextRow();
        writer.Write("值1"); writer.Write(999);
        writer.NextRow();
        writer.Write("值2"); writer.Write(888);

        var bytes = wb.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(2, 3), bytes);
        using var ms = new MemoryStream(bytes);
        using var wb2 = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb2.GetSheetAt(0));

        Assert.That(reader.ReadHeaders(1)[0], Is.EqualTo("列A"));
        Assert.That(reader.GetString(2, 0), Is.EqualTo("值1"));
        Assert.That(reader.GetString(3, 1), Is.EqualTo("888"));
    }

    [Test] public void Write_Row_Then_Header_Order()
    {
        var students = new List<Student> {
            new() { Name = "张三", Age = 20, Score = 88.5 },
            new() { Name = "李四", Age = 22, Score = 92.0 },
        };

        var bytes = ExcelSerializer.Serialize(students);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 表头在第0行，数据从第1行开始
        Assert.That(reader.ReadHeaders(0)[0], Is.EqualTo("姓名"));
        Assert.That(reader.GetString(1, 0), Is.EqualTo("张三"));
        Assert.That(reader.GetString(2, 0), Is.EqualTo("李四"));
    }

    [Test] public void WriteAt_Absolute_Position()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());

        writer.WriteAt(5, 3, "角落值");
        writer.WriteAt(0, 0, "起始");

        var bytes = wb.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(1, 2), bytes);
        using var ms = new MemoryStream(bytes);
        using var wb2 = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb2.GetSheetAt(0));

        Assert.That(reader.GetString(0, 0), Is.EqualTo("起始"));
        Assert.That(reader.GetString(5, 3), Is.EqualTo("角落值"));
    }

    [Test] public void Width_Auto_And_Fixed()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());

        writer.MoveTo(0, 0).Write("固定列"); writer.Width(20);
        writer.Write("自适应"); writer.Width(-1);

        // 没有抛异常即可
        Assert.Pass();
    }

    [Test] public void NamedStyle_Reuse_Same_Instance()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());

        var s1 = writer.Style("h", s => s.BorderBottom = BorderStyle.Double);
        var s2 = writer.Style("h");

        Assert.That(ReferenceEquals(s1, s2), Is.True);
        Assert.That(writer.HasStyle("h"), Is.True);
        Assert.That(writer.HasStyle("nonexistent"), Is.False);
    }

    [Test] public void CustomLayout_WithMerge()
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet("自定义报表");
        var writer = new ExcelSheetWriter(sheet);

        writer.MoveTo(0, 0);
        writer.Write("日报表");
        writer.Merge(new MergeRegion(0, 0, 0, 3) { Value = "2024年1月日报表" });

        writer.MoveTo(1, 0);
        foreach (var h in new[] { "序号", "项目", "金额", "备注" })
        {
            writer.Write(h);
            writer.Width(12);
        }

        writer.MoveTo(2, 0);
        writer.Write(1); writer.Write("销售收入"); writer.Write(10000.50); writer.Write("已到账");
        writer.NextRow();
        writer.Write(2); writer.Write("采购支出"); writer.Write(3500.00); writer.Write("待审批");

        var bytes = workbook.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(4, 3), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));
    }

    [Test] public void ReadHeadersAndRows()
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet("读取测试");
        var writer = new ExcelSheetWriter(sheet);
        writer.MoveTo(0, 0).Write("姓名"); writer.Write("年龄");
        writer.NextRow(); writer.Write("张三"); writer.Write(25);
        writer.NextRow(); writer.Write("李四"); writer.Write(30);

        var reader = new ExcelSheetReader(sheet);
        Assert.That(reader.ReadHeaders(0)[0], Is.EqualTo("姓名"));
        var row = reader.ReadRowAsText(1, new[] { 0, 1 });
        Assert.That(row[0], Is.EqualTo("张三"));
        Assert.That(row[1], Is.EqualTo("25"));
    }
}
