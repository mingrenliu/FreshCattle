namespace ExcelUtileTest.Tests;

[TestFixture]
internal class CellExtensionTests : TestBase
{
    [Test] public void IsInValid_Blank()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(cell.IsInValid(), Is.True);
    }

    [Test] public void GetString_From_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(123.45);
        Assert.That(cell.IsNumeric(), Is.True);
        Assert.That(cell.GetString(), Is.EqualTo("123.45"));
    }

    [Test] public void GetDouble_From_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("3.14");
        Assert.That(cell.IsString(), Is.True);
        Assert.That(cell.GetDouble(), Is.EqualTo(3.14).Within(0.001));
    }

    [Test] public void GetInt_From_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("42");
        Assert.That(cell.GetInt(), Is.EqualTo(42));
    }

    [Test] public void GetBoolean_Chinese()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("是"); Assert.That(cell.GetBoolean(), Is.True);
        cell.SetCellValue("否"); Assert.That(cell.GetBoolean(), Is.False);
    }

    [Test] public void GetDateTime_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(new DateTime(2024, 6, 15));
        Assert.That(cell.IsNumeric(), Is.True);
        Assert.That(cell.GetDateTime()!.Value.Year, Is.EqualTo(2024));
    }

    [Test] public void GetDateTime_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2024-06-15");
        Assert.That(cell.GetDateTime()!.Value.Year, Is.EqualTo(2024));
    }

    [Test] public void GetTimeSpan_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(0.125); // 3h
        Assert.That(cell.GetTimeSpan()!.Value.TotalHours, Is.EqualTo(3).Within(0.01));
    }

    [Test] public void GetObject_CorrectTypes()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var r = s.GetOrCreateRow(0);
        r.GetOrCreateCell(0).SetCellValue("hello"); Assert.That(r.GetOrCreateCell(0).GetObject(), Is.EqualTo("hello"));
        r.GetOrCreateCell(1).SetCellValue(42.0);     Assert.That(r.GetOrCreateCell(1).GetObject(), Is.EqualTo(42.0));
        r.GetOrCreateCell(2).SetCellValue(true);     Assert.That(r.GetOrCreateCell(2).GetObject(), Is.EqualTo(true));
    }

    [Test] public void GetDouble_From_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(true);
        // Boolean 类型不被 IsNumeric 识别，GetDouble 返回 null
        Assert.That(cell.GetDouble(), Is.Null);
    }

    [Test] public void GetInt_From_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(false);
        Assert.That(cell.GetInt(), Is.Null);
    }

    [Test] public void GetBoolean_From_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        // GetBoolean 不支持从数值 1/0 转换，返回 null
        cell.SetCellValue(1); Assert.That(cell.GetBoolean(), Is.Null);
        cell.SetCellValue(0); Assert.That(cell.GetBoolean(), Is.Null);
    }

    [Test] public void GetBoolean_English()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("true"); Assert.That(cell.GetBoolean(), Is.True);
        cell.SetCellValue("false"); Assert.That(cell.GetBoolean(), Is.False);
    }

    [Test] public void GetString_From_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(true);
        Assert.That(cell.GetString(), Is.EqualTo("TRUE"));
    }

    [Test] public void GetString_Blank()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(cell.GetString(), Is.Null);
    }

    [Test] public void GetBoolean_Invalid()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(cell.GetBoolean(), Is.Null); // 空白返回 null
        cell.SetCellValue("notabool");
        Assert.That(cell.GetBoolean(), Is.Null);
    }

    [Test] public void IsNumeric_And_IsString()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(123); Assert.That(cell.IsNumeric(), Is.True);
        cell.SetCellValue("abc"); Assert.That(cell.IsString(), Is.True);
    }
}
