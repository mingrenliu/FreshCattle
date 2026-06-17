namespace ExcelTest.Tests;

[TestFixture]
internal class CellExtensionTests : TestBase
{
    [Test]
    public void IsInValid_Blank()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(cell.IsInValid(), Is.True);
    }

    [Test]
    public void GetString_From_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(123.45);
        Assert.Multiple(() =>
        {
            Assert.That(cell.IsNumeric(), Is.True);
            Assert.That(cell.GetString(), Is.EqualTo("123.45"));
        });
    }

    [Test]
    public void GetDouble_From_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("3.14");
        Assert.Multiple(() =>
        {
            Assert.That(cell.IsString(), Is.True);
            Assert.That(cell.GetDouble(), Is.EqualTo(3.14).Within(0.001));
        });
    }

    [Test]
    public void GetInt_From_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("42");
        Assert.That(cell.GetInt(), Is.EqualTo(42));
    }

    [Test]
    public void GetBoolean_Chinese()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("是");
        var isTrue = cell.GetBoolean();
        cell.SetCellValue("否");
        var isFalse = cell.GetBoolean();
        Assert.Multiple(() =>
        {
            Assert.That(isTrue, Is.True);
            Assert.That(isFalse, Is.False);
        });
    }

    [Test]
    public void GetDateTime_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(new DateTime(2024, 6, 15));
        Assert.Multiple(() =>
        {
            Assert.That(cell.IsNumeric(), Is.True);
            Assert.That(cell.GetDateTime()!.Value.Year, Is.EqualTo(2024));
        });
    }

    [Test]
    public void GetDateTime_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2024-06-15");
        Assert.That(cell.GetDateTime()!.Value.Year, Is.EqualTo(2024));
    }

    [Test]
    public void GetTimeSpan_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(0.125); // 3h
        Assert.That(cell.GetTimeSpan()!.Value.TotalHours, Is.EqualTo(3).Within(0.01));
    }

    [Test]
    public void GetObject_CorrectTypes()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var r = s.GetOrCreateRow(0);
        r.GetOrCreateCell(0).SetCellValue("hello");
        r.GetOrCreateCell(1).SetCellValue(42.0);
        r.GetOrCreateCell(2).SetCellValue(true);
        Assert.Multiple(() =>
        {
            Assert.That(r.GetOrCreateCell(0).GetObject(), Is.EqualTo("hello"));
            Assert.That(r.GetOrCreateCell(1).GetObject(), Is.EqualTo(42.0));
            Assert.That(r.GetOrCreateCell(2).GetObject(), Is.EqualTo(true));
        });
    }

    [Test]
    public void GetDouble_From_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(true);
        Assert.That(cell.GetDouble(), Is.Null);
    }

    [Test]
    public void GetInt_From_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(false);
        Assert.That(cell.GetInt(), Is.Null);
    }

    [Test]
    public void GetBoolean_From_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(1);
        var r1 = cell.GetBoolean();
        cell.SetCellValue(0);
        var r2 = cell.GetBoolean();
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.Null);
            Assert.That(r2, Is.Null);
        });
    }

    [Test]
    public void GetBoolean_English()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("true");
        var isTrue = cell.GetBoolean();
        cell.SetCellValue("false");
        var isFalse = cell.GetBoolean();
        Assert.Multiple(() =>
        {
            Assert.That(isTrue, Is.True);
            Assert.That(isFalse, Is.False);
        });
    }

    [Test]
    public void GetString_From_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(true);
        Assert.That(cell.GetString(), Is.EqualTo("TRUE"));
    }

    [Test]
    public void GetString_Blank()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(cell.GetString(), Is.Null);
    }

    [Test]
    public void GetBoolean_Invalid()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        var blank = cell.GetBoolean();
        cell.SetCellValue("not a bool");
        var notBool = cell.GetBoolean();
        Assert.Multiple(() =>
        {
            Assert.That(blank, Is.Null);
            Assert.That(notBool, Is.Null);
        });
    }

    [Test]
    public void IsNumeric_And_IsString()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue(123);
        var isNum = cell.IsNumeric();
        cell.SetCellValue("abc");
        var isStr = cell.IsString();
        Assert.Multiple(() =>
        {
            Assert.That(isNum, Is.True);
            Assert.That(isStr, Is.True);
        });
    }
}
