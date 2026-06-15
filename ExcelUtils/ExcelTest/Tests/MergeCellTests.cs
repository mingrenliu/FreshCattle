namespace ExcelUtileTest.Tests;

/// <summary>
/// 合并单元格测试：验证合并区域的写入、保存、读取一致性。
/// </summary>
[TestFixture]
internal class MergeCellTests : TestBase
{
    [Test]
    public void TopLeft_HasContent_Others_Empty()
    {
        var importPath = Path.Combine(LocationHelper.GetImportResourcesPath(), "Record_Template.xlsx");
        Assume.That(File.Exists(importPath), Is.True, "模板文件不存在");

        using var fs = new FileStream(importPath, FileMode.Open, FileAccess.Read);
        using var wb = ExcelFactory.CreateWorkBook(fs);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        var mergedRegions = reader.Sheet.MergedRegions;
        Assert.That(mergedRegions.Count, Is.GreaterThan(0), "模板应包含合并单元格");

        foreach (var region in mergedRegions)
        {
            var topLeftContent = reader.GetString(region.FirstRow, region.FirstColumn);
            Assert.That(topLeftContent, Is.Not.Null.And.Not.Empty,
                $"({region.FirstRow},{region.FirstColumn}) 左上角应有内容");

            for (var r = region.FirstRow; r <= region.LastRow; r++)
            for (var c = region.FirstColumn; c <= region.LastColumn; c++)
            {
                if (r == region.FirstRow && c == region.FirstColumn) continue;
                Assert.That(reader.GetString(r, c), Is.Null.Or.Empty,
                    $"({r},{c}) 非左上角内容应为空");
            }
        }
    }

    [Test]
    public void RoundTrip_WriteThenMerge()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet("合并测试"));

        writer.WriteAt(2, 0, "A");
        writer.WriteAt(2, 1, "B");
        writer.WriteAt(3, 0, "C");
        writer.WriteAt(3, 1, "D");
        writer.Merge(new MergeRegion(2, 3, 0, 1) { Value = "合并内容" });

        var bytes = wb.ToBytes();
        LocationHelper.SaveToFile(LocationHelper.ExportFileName(2, 2), bytes);

        using var ms = new MemoryStream(bytes);
        using var wb2 = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb2.GetSheetAt(0));

        Assert.That(reader.GetString(2, 0), Is.EqualTo("合并内容"), "左上角应为合并内容");
        Assert.That(reader.GetString(2, 1), Is.Null.Or.Empty, "(2,1) 内容应为空");
        Assert.That(reader.GetString(3, 0), Is.Null.Or.Empty, "(3,0) 内容应为空");
        Assert.That(reader.GetString(3, 1), Is.Null.Or.Empty, "(3,1) 内容应为空");
    }

    [Test]
    public void SingleCell_Merge_NoOp()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        // 单单元格合并（firstRow==lastRow && firstCol==lastCol）不应抛异常
        Assert.DoesNotThrow(() => writer.Merge(new MergeRegion(0, 0, 0, 0) { Value = "单格" }));
    }

    [Test]
    public void MultiRowMerge_ReadHeaders()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var writer = new ExcelSheetWriter(wb.CreateNewSheet());
        writer.Merge(new MergeRegion(0, 1, 0, 0) { Value = "跨行标题" });
        writer.MoveTo(0, 1).Write("列1");
        writer.MoveTo(0, 2).Write("列2");

        var bytes = wb.ToBytes();
        using var ms = new MemoryStream(bytes);
        using var wb2 = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb2.GetSheetAt(0));

        var headers = reader.ReadHeaders(0);
        Assert.That(headers.Values, Contains.Item("跨行标题"));
        Assert.That(headers.Values, Contains.Item("列1"));
        Assert.That(headers.Values, Contains.Item("列2"));
    }
}
