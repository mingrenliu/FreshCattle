namespace ExcelTest.Tests;

[TestFixture]
internal class CellTypeTests : TestBase
{
    [Test]
    public void String_Writes_As_String()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var c = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.SetCellValue("text");
        Assert.Multiple(() =>
        {
            Assert.That(c.CellType, Is.EqualTo(CellType.String));
            Assert.That(c.IsString(), Is.True);
            Assert.That(c.GetString(), Is.EqualTo("text"));
        });
    }

    [Test]
    public void Numeric_Writes_As_Numeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var c = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.SetCellValue(123.456);
        Assert.Multiple(() =>
        {
            Assert.That(c.CellType, Is.EqualTo(CellType.Numeric));
            Assert.That(c.IsNumeric(), Is.True);
            Assert.That(c.GetDouble(), Is.EqualTo(123.456).Within(0.001));
        });
    }

    [Test]
    public void Boolean_Writes_As_Boolean()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var c = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.SetCellValue(true);
        Assert.Multiple(() =>
        {
            Assert.That(c.CellType, Is.EqualTo(CellType.Boolean));
            Assert.That(c.IsBoolean(), Is.True);
            Assert.That(c.GetBoolean(), Is.True);
        });
    }

    [Test]
    public void Blank_Is_Invalid()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var c = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.Multiple(() =>
        {
            Assert.That(c.CellType, Is.EqualTo(CellType.Blank).Or.EqualTo(CellType.Unknown));
            Assert.That(c.IsInValid(), Is.True);
        });
    }

    [Test]
    public void Formula_Numeric_CachedAsNumeric()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var r = s.GetOrCreateRow(0);
        r.GetOrCreateCell(0).SetCellValue(10);
        r.GetOrCreateCell(1).SetCellValue(20);
        var c2 = r.GetOrCreateCell(2);
        c2.SetCellFormula("A1+B1");
        _ = wb.GetCreationHelper().CreateFormulaEvaluator().EvaluateFormulaCell(c2);
        Assert.Multiple(() =>
        {
            Assert.That(c2.CellType, Is.EqualTo(CellType.Formula));
            Assert.That(c2.CachedFormulaResultType, Is.EqualTo(CellType.Numeric));
            Assert.That(c2.GetDouble(), Is.EqualTo(30).Within(0.001));
        });
    }

    [Test]
    public void Formula_String_CachedAsString()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var r = s.GetOrCreateRow(0);
        r.GetOrCreateCell(0).SetCellValue("Hello");
        r.GetOrCreateCell(1).SetCellValue(" World");
        var c2 = r.GetOrCreateCell(2);
        c2.SetCellFormula("A1&B1");
        _ = wb.GetCreationHelper().CreateFormulaEvaluator().EvaluateFormulaCell(c2);
        Assert.Multiple(() =>
        {
            Assert.That(c2.CachedFormulaResultType, Is.EqualTo(CellType.String));
            Assert.That(c2.GetString(), Is.EqualTo("Hello World"));
        });
    }

    [Test]
    public void Numeric_Zero_Is_Not_Invalid()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var c = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.SetCellValue(0.0);
        Assert.Multiple(() =>
        {
            Assert.That(c.CellType, Is.EqualTo(CellType.Numeric));
            Assert.That(c.IsInValid(), Is.False);
            Assert.That(c.GetDouble(), Is.EqualTo(0));
        });
    }

    [Test]
    public void Empty_String_Is_Not_Invalid()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var c = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.SetCellValue("");
        Assert.Multiple(() =>
        {
            Assert.That(c.CellType, Is.EqualTo(CellType.String));
            Assert.That(c.IsInValid(), Is.False);
        });
    }
}
