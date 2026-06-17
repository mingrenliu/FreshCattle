namespace ExcelTest.Tests;

[TestFixture]
internal class SheetWriterTests : TestBase
{
    [Test]
    public void Merge_Style_WriteAt_RoundTrip()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet("低层测试"));

        writer.Style("h", s => s.SetBackgroundColor(22));
        var mergeR = writer.Style("merge");
        mergeR.SetBackgroundColor(48);
        mergeR.Alignment = HorizontalAlignment.Center;
        mergeR.VerticalAlignment = VerticalAlignment.Center;
        writer.MoveTo(0, 0);
        writer.Merge(new MergeRegion(0, 0, 0, 2) { Value = "标题行", PostAction = c => c.CellStyle = mergeR });
        writer.MoveTo(1, 0);
        writer.Write("列A", writer.Style("h")); writer.PrevWidth(15);
        writer.Write("列B", writer.Style("h")); writer.PrevWidth(15);
        writer.NextRow();
        writer.Write("值1"); writer.Write(999);
        writer.NextRow();
        writer.Write("值2"); writer.Write(888);

        var bytes = wb.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(2, 2), bytes);
        using var ms = new MemoryStream(bytes);
        using var wb2 = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb2.GetSheetAt(0));

        Assert.Multiple(() =>
        {
            Assert.That(reader.ReadHeaders(1)[0], Is.EqualTo("列A"));
            Assert.That(reader.GetString(2, 0), Is.EqualTo("值1"));
            Assert.That(reader.GetString(3, 1), Is.EqualTo("888"));
        });
    }

    [Test]
    public void Write_Row_Then_Header_Order()
    {
        var students = new List<Student> {
            new() { Name = "张三", Age = 20, Score = 88.5 },
            new() { Name = "李四", Age = 22, Score = 92.0,EnrollDate=DateTime.Now },
        };

        var bytes = Excel.Serialize(students);
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(5, students.Count), bytes);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 表头在第0行，数据从第1行开始
        Assert.Multiple(() =>
        {
            Assert.That(reader.ReadHeaders(0)[0], Is.EqualTo("姓名"));
            Assert.That(reader.GetString(1, 0), Is.EqualTo("张三"));
            Assert.That(reader.GetString(2, 0), Is.EqualTo("李四"));
        });
    }

    [Test]
    public void WriteAt_Absolute_Position()
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

        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0, 0), Is.EqualTo("起始"));
            Assert.That(reader.GetString(5, 3), Is.EqualTo("角落值"));
        });
    }

    [Test]
    public void Width_Auto_And_Fixed()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());

        writer.MoveTo(0, 0).Write("固定列"); writer.PrevWidth(20);
        writer.Write("自适应"); writer.PrevWidth(-1);

        // 没有抛异常即可
        Assert.Pass();
    }

    [Test]
    public void NamedStyle_Reuse_Same_Instance()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());

        var s1 = writer.Style("h", s => s.BorderBottom = BorderStyle.Double);
        var s2 = writer.Style("h");

        Assert.Multiple(() =>
        {
            Assert.That(ReferenceEquals(s1, s2), Is.True);
            Assert.That(writer.HasStyle("h"), Is.True);
            Assert.That(writer.HasStyle("nonexistent"), Is.False);
        });
    }

    [Test]
    public void CustomLayout_WithMerge()
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
            writer.PrevWidth(12);
        }

        writer.MoveTo(2, 0);
        writer.Write(1); writer.Write("销售收入"); writer.Write(10000.50); writer.Write("已到账");
        writer.NextRow();
        writer.Write(2); writer.Write("采购支出"); writer.Write(3500.00); writer.Write("待审批");

        var bytes = workbook.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(4, 2), bytes);
        Assume.That(bytes.Length, Is.AtLeast(30));
    }

    [Test]
    public void ReadHeadersAndRows()
    {
        using var workbook = ExcelFactory.CreateWorkBook();
        var sheet = workbook.CreateNewSheet("读取测试");
        var writer = new ExcelSheetWriter(sheet);
        writer.MoveTo(0, 0).Write("姓名"); writer.Write("年龄");
        writer.NextRow(); writer.Write("张三"); writer.Write(25);
        writer.NextRow(); writer.Write("李四"); writer.Write(30);

        var reader = new ExcelSheetReader(sheet);
        var header = reader.ReadHeaders(0)[0];
        var row = reader.ReadRowAsText(1, [0, 1]);
        Assert.Multiple(() =>
        {
            Assert.That(header, Is.EqualTo("姓名"));
            Assert.That(row[0], Is.EqualTo("张三"));
            Assert.That(row[1], Is.EqualTo("25"));
        });
    }

    [Test]
    public void Cursor_MoveTo_NextRow_NextCol()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());

        writer.MoveTo(0, 0);
        var r0 = writer.RowIndex; var c0 = writer.ColIndex;
        writer.NextCol();
        var c1 = writer.ColIndex;
        writer.NextCol(3);
        var c4 = writer.ColIndex;
        writer.NextRow();
        var r1 = writer.RowIndex; var c0b = writer.ColIndex;
        writer.MoveTo(5, 2);
        var r5 = writer.RowIndex; var c2 = writer.ColIndex;

        Assert.Multiple(() =>
        {
            Assert.That(r0, Is.EqualTo(0)); Assert.That(c0, Is.EqualTo(0));
            Assert.That(c1, Is.EqualTo(1));
            Assert.That(c4, Is.EqualTo(4));
            Assert.That(r1, Is.EqualTo(1)); Assert.That(c0b, Is.EqualTo(0));
            Assert.That(r5, Is.EqualTo(5)); Assert.That(c2, Is.EqualTo(2));
        });
    }

    [Test]
    public void Write_VariousTypes()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        writer.MoveTo(0, 0);
        writer.Write("文本"); writer.Write(123); writer.Write(45.67); writer.Write(true);
        writer.NextRow();
        writer.Write(new DateTime(2024, 1, 1)); writer.Write(Guid.NewGuid());

        var reader = new ExcelSheetReader(wb.GetSheetAt(0));
        Assert.Multiple(() =>
        {
            Assert.That(reader.GetString(0, 0), Is.EqualTo("文本"));
            Assert.That(reader.GetString(0, 1), Is.EqualTo("123"));
            Assert.That(reader.GetString(0, 2), Does.Contain("45.67"));
            Assert.That(reader.GetString(0, 3), Is.EqualTo("是")); // bool 默认中文输出
        });
    }

    [Test]
    public void WriteAt_WithConverter()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        var converter = new DateTimeConverter("yyyy-MM-dd");
        writer.WriteAt(2, 2, new DateTime(2025, 6, 15), converter);

        var reader = new ExcelSheetReader(wb.GetSheetAt(0));
        Assert.That(reader.GetString(2, 2), Is.EqualTo("2025-06-15"));
    }

    [Test]
    public void UseSheet_SwitchAndBack()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet("SheetA"));
        writer.Write("A1");
        writer.UseSheet("SheetB").Write("B1");

        var readerA = new ExcelSheetReader(wb.GetSheet("SheetA"));
        var readerB = new ExcelSheetReader(wb.GetSheet("SheetB"));
        Assert.Multiple(() =>
        {
            Assert.That(readerA.GetString(0, 0), Is.EqualTo("A1"));
            Assert.That(readerB.GetString(0, 0), Is.EqualTo("B1"));
        });
    }

    [Test]
    public void DefaultStyle_IsCached()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        var s1 = writer.DefaultStyle;
        var s2 = writer.DefaultStyle;
        Assert.That(ReferenceEquals(s1, s2), Is.True);
    }

    [Test]
    public void WidthAt_ExplicitColumn()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        // 不抛异常即可
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => writer.WidthAt(0, 20));
            Assert.DoesNotThrow(() => writer.WidthAt(1, -1));
            Assert.DoesNotThrow(() => writer.WidthAt(2, 0));
        });
    }

    [Test]
    public void Merge_EmptyRegion_NoCrash()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => writer.Merge((IEnumerable<MergeRegion>?)null));
            Assert.DoesNotThrow(() => writer.Merge([]));
        });
    }
}