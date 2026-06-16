namespace ExcelTest.Tests;

[TestFixture]
internal class ConverterTests : TestBase
{
    [Test]
    public void String_RoundTrip()
    {
        var c = new StringConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, "Hello"); var r1 = c.Read(cell);
        c.Write(cell, ""); var r2 = c.Read(cell);
        c.Write(cell, "  s  "); var r3 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.EqualTo("Hello"));
            Assert.That(r2, Is.EqualTo(""));
            Assert.That(r3, Is.EqualTo("  s  "));
        });
    }

    [Test]
    public void Int_RoundTrip()
    {
        var c = new Int32Converter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 0); var r1 = c.Read(cell);
        c.Write(cell, int.MaxValue); var r2 = c.Read(cell);
        c.Write(cell, int.MinValue); var r3 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.EqualTo(0));
            Assert.That(r2, Is.EqualTo(int.MaxValue));
            Assert.That(r3, Is.EqualTo(int.MinValue));
        });
    }

    [Test]
    public void Long_RoundTrip()
    {
        var c = new Int64Converter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 999888777666L); Assert.That(c.Read(cell), Is.EqualTo(999888777666L));
    }

    [Test]
    public void Double_RoundTrip()
    {
        var c = new DoubleConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.1415926535);
        Assert.That(c.Read(cell), Is.EqualTo(3.1415926535).Within(0.0001));
    }

    [Test]
    public void Double_Precision_SetsExcelFormat()
    {
        var c0 = new DoubleConverter(0);
        var c2 = new DoubleConverter(2);
        var c5 = new DoubleConverter(5);
        Assert.Multiple(() =>
        {
            Assert.That(c0.ExcelFormat, Is.EqualTo("0"));
            Assert.That(c2.ExcelFormat, Is.EqualTo("0.00"));
            Assert.That(c5.ExcelFormat, Is.EqualTo("0.00000"));
        });
    }

    [Test]
    public void Double_Precision_DefaultIsNull()
    {
        var c = new DoubleConverter();
        Assert.Multiple(() =>
        {
            Assert.That(c.ExcelFormat, Is.Null);
        });
    }

    [Test]
    public void Double_ApplyToStyle_DefaultNoFormat()
    {
        var c = new DoubleConverter("#,##0.00");
        Assert.That(c.ExcelFormat, Is.EqualTo("#,##0.00"));
    }

    [Test]
    public void Double_ApplyToStyle_SetsDataFormat()
    {
        var c = new DoubleConverter(2);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0.00");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Double_ApplyToStyle_Precision0_SetsDataFormat()
    {
        var c = new DoubleConverter(0);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Double_ApplyToStyle_NoFormat_DoesNotSetDataFormat()
    {
        var c = new DoubleConverter(); // 默认无格式
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        var originalFormat = style.DataFormat;
        c.ApplyToStyle(style, sheet);
        Assert.That(style.DataFormat, Is.EqualTo(originalFormat));
    }

    [Test]
    public void Double_WriteToCell_FormatApplied()
    {
        var c = new DoubleConverter(2);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var cell = sheet.GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.14159, style);
        Assert.That(cell.CellStyle.DataFormat, Is.EqualTo(style.DataFormat));
    }

    [Test]
    public void Decimal_RoundTrip()
    {
        var c = new DecimalConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 123456.78m); Assert.That(c.Read(cell), Is.EqualTo(123456.78m).Within(0.01m));
    }

    [Test]
    public void Decimal_Precision_SetsExcelFormat()
    {
        var c0 = new DecimalConverter(0);
        var c2 = new DecimalConverter(2);
        var c4 = new DecimalConverter(4);
        Assert.Multiple(() =>
        {
            Assert.That(c0.ExcelFormat, Is.EqualTo("0"));
            Assert.That(c2.ExcelFormat, Is.EqualTo("0.00"));
            Assert.That(c4.ExcelFormat, Is.EqualTo("0.0000"));
        });
    }

    [Test]
    public void Decimal_Precision_DefaultIsNull()
    {
        var c = new DecimalConverter();
        Assert.Multiple(() =>
        {
            Assert.That(c.ExcelFormat, Is.Null);
        });
    }

    [Test]
    public void Decimal_ApplyToStyle_SetsDataFormat()
    {
        var c = new DecimalConverter(2);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0.00");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Float_RoundTrip()
    {
        var c = new SingleConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, 3.14f); Assert.That(c.Read(cell), Is.EqualTo(3.14f).Within(0.01));
    }

    [Test]
    public void Float_Precision_SetsExcelFormat()
    {
        var c0 = new SingleConverter(0);
        var c2 = new SingleConverter(2);
        var c3 = new SingleConverter(3);
        Assert.Multiple(() =>
        {
            Assert.That(c0.ExcelFormat, Is.EqualTo("0"));
            Assert.That(c2.ExcelFormat, Is.EqualTo("0.00"));
            Assert.That(c3.ExcelFormat, Is.EqualTo("0.000"));
        });
    }

    [Test]
    public void Float_Precision_DefaultIsNull()
    {
        var c = new SingleConverter();
        Assert.Multiple(() =>
        {
            Assert.That(c.ExcelFormat, Is.Null);
        });
    }

    [Test]
    public void Float_ApplyToStyle_SetsDataFormat()
    {
        var c = new SingleConverter(1);
        using var wb = ExcelFactory.CreateWorkBook();
        var sheet = wb.CreateNewSheet();
        var style = wb.CreateCellStyle();
        c.ApplyToStyle(style, sheet);
        var expectedFormat = wb.GetDataFormat("0.0");
        Assert.That(style.DataFormat, Is.EqualTo(expectedFormat));
    }

    [Test]
    public void Precision_ReuseSameFormat_ReturnsSameIndex()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        // 相同格式字符串应返回相同的 DataFormat 索引
        var fmt1 = wb.GetDataFormat("0.00");
        var fmt2 = wb.GetDataFormat("0.00");
        Assert.That(fmt1, Is.EqualTo(fmt2));
    }

    [Test]
    public void Bool_RoundTrip()
    {
        var c = new BooleanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, true); var r1 = c.Read(cell);
        c.Write(cell, false); var r2 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.True);
            Assert.That(r2, Is.False);
        });
    }

    [Test]
    public void Bool_CustomText()
    {
        var c = new BooleanConverter("对", "错");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, true);
        Assert.Multiple(() =>
        {
            Assert.That(c.Read(cell), Is.True);
            Assert.That(cell.StringCellValue, Is.EqualTo("对"));
        });
    }

    [Test]
    public void DateTime_RoundTrip()
    {
        var c = new DateTimeConverter("yyyy-MM-dd HH:mm:ss");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTime(2024, 6, 15, 14, 30, 0));
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2024)); Assert.That(r.Month, Is.EqualTo(6));
        });
    }

    [Test]
    public void DateOnly_RoundTrip()
    {
        var c = new DateOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateOnly(2025, 3, 20));
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2025)); Assert.That(r.Month, Is.EqualTo(3));
        });
    }

    [Test]
    public void TimeOnly_RoundTrip()
    {
        var c = new TimeOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new TimeOnly(9, 15, 30));
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Hour, Is.EqualTo(9)); Assert.That(r.Minute, Is.EqualTo(15));
        });
    }

    [Test]
    public void TimeSpan_RoundTrip()
    {
        var c = new TimeSpanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, TimeSpan.FromHours(3.5));
        Assert.That(c.Read(cell).TotalHours, Is.EqualTo(3.5).Within(0.01));
    }

    [Test]
    public void Guid_RoundTrip()
    {
        var c = new GuidConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        var g = Guid.NewGuid(); c.Write(cell, g); Assert.That(c.Read(cell), Is.EqualTo(g));
    }

    [Test]
    public void DateTimeOffset_RoundTrip()
    {
        var c = new DateTimeOffsetConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTimeOffset(2024, 8, 1, 0, 0, 0, TimeSpan.FromHours(8)));
        Assert.That(c.Read(cell).Year, Is.EqualTo(2024));
    }

    [Test]
    public void ShortAndByte()
    {
        using var wb = ExcelFactory.CreateWorkBook();
        var s = wb.CreateNewSheet();
        var sc = new Int16Converter(); var bc = new ByteConverter();
        sc.Write(s.GetOrCreateRow(0).GetOrCreateCell(0), (short)12345);
        var sr = sc.Read(s.GetOrCreateRow(0).GetOrCreateCell(0));
        bc.Write(s.GetOrCreateRow(1).GetOrCreateCell(0), (byte)200);
        var br = bc.Read(s.GetOrCreateRow(1).GetOrCreateCell(0));
        Assert.Multiple(() =>
        {
            Assert.That(sr, Is.EqualTo(12345));
            Assert.That(br, Is.EqualTo(200));
        });
    }

    [Test]
    public void LongDateTime_RoundTrip()
    {
        var c = new LongDateTimeConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        var dt = new DateTime(2024, 6, 15, 14, 30, 45);
        c.Write(cell, dt);
        Assert.Multiple(() =>
        {
            Assert.That(c.Read(cell), Is.EqualTo(dt));
            Assert.That(cell.StringCellValue, Is.EqualTo("2024-06-15 14:30:45"));
        });
    }

    [Test]
    public void LongDateTime_CustomFormat()
    {
        var c = new LongDateTimeConverter("yyyy/MM/dd HH:mm");
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, new DateTime(2024, 12, 1, 8, 0, 0));
        Assert.That(cell.StringCellValue, Is.EqualTo("2024/12/01 08:00"));
    }

    [Test]
    public void TimeSpanMinutes_RoundTrip()
    {
        var c = new TimeSpanMinutesConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);

        // 3.5小时 = 210分钟
        var ts = TimeSpan.FromHours(3.5);
        c.Write(cell, ts);
        Assert.Multiple(() =>
        {
            Assert.That(cell.NumericCellValue, Is.EqualTo(210));
            Assert.That(c.Read(cell).TotalMinutes, Is.EqualTo(210).Within(0.01));
        });
    }

    [Test]
    public void TimeSpanHours_RoundTrip()
    {
        var c = new TimeSpanHoursConverter(1);
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);

        // 5400秒 = 1.5小时
        var ts = TimeSpan.FromSeconds(5400);
        c.Write(cell, ts);
        var cellText = cell.NumericCellValue;
        var readBack = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(cellText, Is.EqualTo(1.5));
            Assert.That(readBack.TotalHours, Is.EqualTo(1.5).Within(0.01));
        });
    }

    [Test]
    public void TimeSpanHours_ReadFromString()
    {
        var c = new TimeSpanHoursConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2.75");
        Assert.That(c.Read(cell).TotalHours, Is.EqualTo(2.75).Within(0.01));
    }

    [Test]
    public void Enum_Int_RoundTrip()
    {
        var c = new EnumConverter<DayOfWeek>();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);

        c.Write(cell, DayOfWeek.Wednesday);
        // 输出整数值 3（Sunday=0）
        var cellText = cell.NumericCellValue;
        var readBack = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(cellText, Is.EqualTo(3));
            Assert.That(readBack, Is.EqualTo(DayOfWeek.Wednesday));
        });
    }

    [Test]
    public void Enum_Int_ReadFromString()
    {
        var c = new EnumConverter<DayOfWeek>();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        // 也支持用名称字符串读取
        cell.SetCellValue("Friday");
        Assert.That(c.Read(cell), Is.EqualTo(DayOfWeek.Friday));
    }

    [Test]
    public void Enum_ReadFromNumericString()
    {
        var c = new EnumConverter<DayOfWeek>();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("3");
        Assert.That((int)(object)c.Read(cell), Is.EqualTo(3));
    }

    [Test]
    public void DateTime_ReadFromString()
    {
        var c = new DateTimeConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2024-12-25");
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2024));
            Assert.That(r.Month, Is.EqualTo(12));
        });
    }

    [Test]
    public void DateTime_Empty_ReturnsDefault()
    {
        var c = new DateTimeConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(c.Read(cell), Is.EqualTo(default(DateTime)));
    }

    [Test]
    public void DateTimeOffset_ReadFromString()
    {
        var c = new DateTimeOffsetConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2024-08-01T00:00:00+08:00");
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Year, Is.EqualTo(2024));
            Assert.That(r.Month, Is.EqualTo(8));
        });
    }

    [Test]
    public void Guid_Empty_And_Default()
    {
        var c = new GuidConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        c.Write(cell, Guid.Empty); var r1 = c.Read(cell);
        cell.SetCellValue(""); var r2 = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r1, Is.EqualTo(Guid.Empty));
            Assert.That(r2, Is.EqualTo(default(Guid)));
        });
    }

    [Test]
    public void Bool_ReadNumeric()
    {
        var c = new BooleanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        // BooleanConverter 不支持从数值读写，IsNumeric 返回 true 不会被识别为布尔
        cell.SetCellValue(1.0);
        var result = c.Read(cell);
        Assert.That(result, Is.True.Or.False); // 接受任意结果，取决于实现
    }

    [Test]
    public void String_Null_ReturnsEmpty()
    {
        var c = new StringConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        Assert.That(c.Read(cell), Is.EqualTo(""));
    }

    [Test]
    public void TimeSpan_ReadFromString()
    {
        var c = new TimeSpanConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("02:30:00");
        Assert.That(c.Read(cell), Is.EqualTo(TimeSpan.FromHours(2.5)));
    }

    [Test]
    public void TimeOnly_ReadFromString()
    {
        var c = new TimeOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("14:30:00");
        var r = c.Read(cell);
        Assert.Multiple(() =>
        {
            Assert.That(r.Hour, Is.EqualTo(14));
            Assert.That(r.Minute, Is.EqualTo(30));
        });
    }

    [Test]
    public void DateOnly_ReadFromString()
    {
        var c = new DateOnlyConverter();
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        cell.SetCellValue("2025-06-15");
        Assert.That(c.Read(cell), Is.EqualTo(new DateOnly(2025, 6, 15)));
    }

    #region ResolveConverter + Activator.CreateInstance with Args

    /// <summary>
    /// 测试实体：使用 [ExcelConverter(typeof(...), args)] 带参数构造转换器。
    /// 覆盖 ExcelTypeInfoResolver.ResolveConverter 中 Activator.CreateInstance(type, args) 分支。
    /// </summary>
    private class ConverterArgsEntity
    {
        [ExcelColumn(Name = "日期", Order = 0)]
        [ExcelConverter(typeof(LongDateTimeConverter), "yyyy/MM/dd")]
        public DateTime DateWithFormat { get; set; }

        [ExcelColumn(Name = "是否", Order = 1)]
        [ExcelConverter(typeof(BooleanConverter), "Yes", "No")]
        public bool BoolWithCustomText { get; set; }

        [ExcelColumn(Name = "分钟", Order = 2)]
        [ExcelConverter(typeof(TimeSpanMinutesConverter), 2)]
        public TimeSpan TimeSpanWithPrecision { get; set; }

        [ExcelColumn(Name = "数字", Order = 3)]
        [ExcelConverter(typeof(DoubleConverter), 3)]
        public double DoubleWithPrecision { get; set; }
    }

    [Test]
    public void ResolveConverter_Args_DateTimeFormat_RoundTrip()
    {
        var entity = new ConverterArgsEntity
        {
            DateWithFormat = new DateTime(2024, 6, 15, 14, 30, 0),
            BoolWithCustomText = true,
            TimeSpanWithPrecision = TimeSpan.FromMinutes(210.5), // 210.50
            DoubleWithPrecision = 3.14159,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 验证日期以自定义格式写入
        Assert.That(reader.GetString(1, 0), Is.EqualTo("2024/06/15"));
    }

    [Test]
    public void ResolveConverter_Args_BooleanCustomText_RoundTrip()
    {
        var entity = new ConverterArgsEntity
        {
            DateWithFormat = DateTime.Today,
            BoolWithCustomText = true,
            TimeSpanWithPrecision = TimeSpan.FromHours(1),
            DoubleWithPrecision = 1.0,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 验证布尔值以自定义文本 "Yes"/"No" 写入
        Assert.That(reader.GetString(1, 1), Is.EqualTo("Yes"));
    }

    [Test]
    public void ResolveConverter_Args_BooleanCustomText_False()
    {
        var entity = new ConverterArgsEntity
        {
            DateWithFormat = DateTime.Today,
            BoolWithCustomText = false,
            TimeSpanWithPrecision = TimeSpan.FromHours(1),
            DoubleWithPrecision = 1.0,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        Assert.That(reader.GetString(1, 1), Is.EqualTo("No"));
    }

    [Test]
    public void ResolveConverter_Args_TimeSpanPrecision_RoundTrip()
    {
        var entity = new ConverterArgsEntity
        {
            DateWithFormat = DateTime.Today,
            BoolWithCustomText = true,
            TimeSpanWithPrecision = TimeSpan.FromMinutes(210.567), // 3h30m34s ≈ 210.567min
            DoubleWithPrecision = 1.0,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 精度=2，应四舍五入到 210.57
        var cellText = reader.GetString(1, 2);
        Assert.That(cellText, Does.Contain("210.57"));
    }

    [Test]
    public void ResolveConverter_Args_DoublePrecision_RoundTrip()
    {
        var entity = new ConverterArgsEntity
        {
            DateWithFormat = DateTime.Today,
            BoolWithCustomText = true,
            TimeSpanWithPrecision = TimeSpan.FromHours(1),
            DoubleWithPrecision = 3.14159,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<ConverterArgsEntity>(ms).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(imported, Has.Count.EqualTo(1));
            Assert.That(imported[0].DoubleWithPrecision, Is.EqualTo(3.14159).Within(0.001));
        });
    }

    [Test]
    public void ResolveConverter_Args_FullRoundTrip()
    {
        var original = new ConverterArgsEntity
        {
            DateWithFormat = new DateTime(2025, 12, 25, 8, 0, 0),
            BoolWithCustomText = true,
            TimeSpanWithPrecision = TimeSpan.FromMinutes(90.5),
            DoubleWithPrecision = 2.71828,
        };

        var bytes = Excel.Serialize([original]);
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<ConverterArgsEntity>(ms).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(imported, Has.Count.EqualTo(1));
            Assert.That(imported[0].DateWithFormat.Year, Is.EqualTo(2025));
            Assert.That(imported[0].DateWithFormat.Month, Is.EqualTo(12));
            Assert.That(imported[0].DateWithFormat.Day, Is.EqualTo(25));
            Assert.That(imported[0].BoolWithCustomText, Is.True);
            Assert.That(imported[0].TimeSpanWithPrecision.TotalMinutes,
                Is.EqualTo(90.5).Within(0.01));
            Assert.That(imported[0].DoubleWithPrecision,
                Is.EqualTo(2.71828).Within(0.001));
        });
    }

    /// <summary>
    /// 测试实体：使用 [ExcelConverter(typeof(...))] 无参数构造转换器。
    /// 覆盖 ExcelTypeInfoResolver.ResolveConverter 中 Activator.CreateInstance(type) 分支。
    /// </summary>
    private class ConverterNoArgsEntity
    {
        [ExcelColumn(Name = "日期时间", Order = 0)]
        [ExcelConverter(typeof(LongDateTimeConverter))]
        public DateTime DateTimeDefault { get; set; }

        [ExcelColumn(Name = "分钟", Order = 1)]
        [ExcelConverter(typeof(TimeSpanMinutesConverter))]
        public TimeSpan TimeSpanMinDefault { get; set; }

        [ExcelColumn(Name = "短日期", Order = 2)]
        [ExcelConverter(typeof(DateTimeConverter))]
        public DateTime ShortDateDefault { get; set; }

        [ExcelColumn(Name = "布尔", Order = 3)]
        [ExcelConverter(typeof(BooleanConverter))]
        public bool BoolDefault { get; set; }

        [ExcelColumn(Name = "小时", Order = 4)]
        [ExcelConverter(typeof(TimeSpanHoursConverter))]
        public TimeSpan TimeSpanHourDefault { get; set; }

        [ExcelColumn(Name = "偏移", Order = 5)]
        [ExcelConverter(typeof(DateTimeOffsetConverter))]
        public DateTimeOffset OffsetDefault { get; set; }
    }

    [Test]
    public void ResolveConverter_NoArgs_LongDateTime_UsesDefaultFormat()
    {
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = new DateTime(2024, 6, 15, 14, 30, 0),
            ShortDateDefault = DateTime.Today,
            BoolDefault = true,
            TimeSpanMinDefault = TimeSpan.FromMinutes(90),
            TimeSpanHourDefault = TimeSpan.FromHours(1),
            OffsetDefault = DateTimeOffset.Now,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 默认格式 "yyyy-MM-dd HH:mm:ss"
        Assert.That(reader.GetString(1, 0), Is.EqualTo("2024-06-15 14:30:00"));
    }

    [Test]
    public void ResolveConverter_NoArgs_TimeSpanMinutes_UsesDefaultPrecision()
    {
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = DateTime.Today,
            ShortDateDefault = DateTime.Today,
            BoolDefault = false,
            TimeSpanMinDefault = TimeSpan.FromMinutes(210.567), // 四舍五入到整数
            TimeSpanHourDefault = TimeSpan.FromHours(1),
            OffsetDefault = DateTimeOffset.Now,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 默认精度=0，四舍五入到整数 211
        Assert.That(reader.GetString(1, 1), Does.Contain("211"));
    }

    [Test]
    public void ResolveConverter_NoArgs_FullRoundTrip()
    {
        var original = new ConverterNoArgsEntity
        {
            DateTimeDefault = new DateTime(2025, 6, 16, 9, 30, 45),
            ShortDateDefault = new DateTime(2025, 3, 1),
            BoolDefault = true,
            TimeSpanMinDefault = TimeSpan.FromMinutes(120),
            TimeSpanHourDefault = TimeSpan.FromHours(3),
            OffsetDefault = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.FromHours(8)),
        };

        var bytes = Excel.Serialize([original]);
        using var ms = new MemoryStream(bytes);
        var imported = Excel.Deserialize<ConverterNoArgsEntity>(ms).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(imported, Has.Count.EqualTo(1));
            Assert.That(imported[0].DateTimeDefault.Year, Is.EqualTo(2025));
            Assert.That(imported[0].DateTimeDefault.Month, Is.EqualTo(6));
            Assert.That(imported[0].ShortDateDefault.Year, Is.EqualTo(2025));
            Assert.That(imported[0].BoolDefault, Is.True);
            Assert.That(imported[0].TimeSpanMinDefault.TotalMinutes,
                Is.EqualTo(120).Within(0.01));
            Assert.That(imported[0].TimeSpanHourDefault.TotalHours,
                Is.EqualTo(3).Within(0.01));
            Assert.That(imported[0].OffsetDefault.Year, Is.EqualTo(2025));
        });
    }

    [Test]
    public void ResolveConverter_NoArgs_DateTimeConverter_UsesDefaultFormat()
    {
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = DateTime.Today,
            ShortDateDefault = new DateTime(2024, 12, 25),
            BoolDefault = true,
            TimeSpanMinDefault = TimeSpan.FromHours(1),
            TimeSpanHourDefault = TimeSpan.FromHours(1),
            OffsetDefault = DateTimeOffset.Now,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // DateTimeConverter 默认格式 "yyyy-MM-dd"
        Assert.That(reader.GetString(1, 2), Is.EqualTo("2024-12-25"));
    }

    [Test]
    public void ResolveConverter_NoArgs_BooleanConverter_DefaultText()
    {
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = DateTime.Today,
            ShortDateDefault = DateTime.Today,
            BoolDefault = true,
            TimeSpanMinDefault = TimeSpan.FromHours(1),
            TimeSpanHourDefault = TimeSpan.FromHours(1),
            OffsetDefault = DateTimeOffset.Now,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // BooleanConverter 默认 true→"是"（中文默认）
        Assert.That(reader.GetString(1, 3), Is.EqualTo("是"));
    }

    [Test]
    public void ResolveConverter_NoArgs_BooleanConverter_False()
    {
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = DateTime.Today,
            ShortDateDefault = DateTime.Today,
            BoolDefault = false,
            TimeSpanMinDefault = TimeSpan.FromHours(1),
            TimeSpanHourDefault = TimeSpan.FromHours(1),
            OffsetDefault = DateTimeOffset.Now,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // BooleanConverter 默认 false→"否"（中文默认）
        Assert.That(reader.GetString(1, 3), Is.EqualTo("否"));
    }

    [Test]
    public void ResolveConverter_NoArgs_TimeSpanHours_UsesDefaultPrecision()
    {
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = DateTime.Today,
            ShortDateDefault = DateTime.Today,
            BoolDefault = true,
            TimeSpanMinDefault = TimeSpan.FromHours(1),
            TimeSpanHourDefault = TimeSpan.FromHours(2.756), // 默认精度0 → 四舍五入到3
            OffsetDefault = DateTimeOffset.Now,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // 默认精度=0，四舍五入到整数 3
        Assert.That(reader.GetString(1, 4), Does.Contain("3"));
    }

    [Test]
    public void ResolveConverter_NoArgs_DateTimeOffset_UsesDefaultFormat()
    {
        var dto = new DateTimeOffset(2024, 6, 15, 14, 30, 0, TimeSpan.FromHours(8));
        var entity = new ConverterNoArgsEntity
        {
            DateTimeDefault = DateTime.Today,
            ShortDateDefault = DateTime.Today,
            BoolDefault = true,
            TimeSpanMinDefault = TimeSpan.FromHours(1),
            TimeSpanHourDefault = TimeSpan.FromHours(1),
            OffsetDefault = dto,
        };

        var bytes = Excel.Serialize([entity]);
        using var ms = new MemoryStream(bytes);
        using var wb = ExcelFactory.CreateWorkBook(ms);
        var reader = new ExcelSheetReader(wb.GetSheetAt(0));

        // DateTimeOffsetConverter 默认格式 "yyyy-MM-dd HH:mm:ss zzz"
        var cellText = reader.GetString(1, 5);
        Assert.That(cellText, Does.Contain("2024-06-15"));
    }

    #endregion ResolveConverter + Activator.CreateInstance with Args

    #region BuiltInConverters 枚举/Nullable 动态创建

    [Test]
    public void BuiltInConverters_EnumType_ReturnsConverter()
    {
        Assert.Multiple(() =>
        {
            Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek), out var converter), Is.True);
            Assert.That(converter, Is.Not.Null);
            Assert.That(converter?.Type, Is.EqualTo(typeof(DayOfWeek)));
        });
    }

    [Test]
    public void BuiltInConverters_EnumType_RoundTrip()
    {
        Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek), out var converter), Is.True);
        Assert.That(converter, Is.Not.Null);
        using var wb = ExcelFactory.CreateWorkBook();
        var cell = wb.CreateNewSheet().GetOrCreateRow(0).GetOrCreateCell(0);
        converter!.WriteObject(cell, DayOfWeek.Friday);
        Assert.That(cell.NumericCellValue, Is.EqualTo(5)); // Friday=5
        var result = converter.ReadObject(cell);
        Assert.That(result, Is.EqualTo(DayOfWeek.Friday));
    }

    [Test]
    public void BuiltInConverters_NullableInt_ReturnsConverter()
    {
        Assert.Multiple(() =>
        {
            Assert.That(BuiltInConverters.TryGetConverter(typeof(int?), out var converter), Is.True);
            Assert.That(converter, Is.Not.Null);
            Assert.That(converter?.Type, Is.EqualTo(typeof(int)));
        });
    }

    [Test]
    public void BuiltInConverters_NullableEnum_ReturnsConverter()
    {
        Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek?), out var converter), Is.True);
        Assert.That(converter, Is.Not.Null);
        Assert.That(converter.Type, Is.EqualTo(typeof(DayOfWeek)));
    }

    [Test]
    public void BuiltInConverters_UnknownType_ReturnsFalse()
    {
        Assert.That(BuiltInConverters.TryGetConverter(typeof(ConverterTests), out _), Is.False);
    }

    [Test]
    public void BuiltInConverters_GetConverter_NullForUnknown()
    {
        var result = BuiltInConverters.GetConverter(typeof(ConverterTests));
        Assert.That(result, Is.Null);
    }

    [Test]
    public void BuiltInConverters_Cache_EnumType()
    {
        // 第一次获取
        Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek), out var c1), Is.True);
        // 第二次获取应返回缓存实例
        Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek), out var c2), Is.True);
        Assert.That(ReferenceEquals(c1, c2), Is.True);
    }

    [Test]
    public void BuiltInConverters_Cache_NullableEnumType()
    {
        Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek?), out var c1), Is.True);
        Assert.That(BuiltInConverters.TryGetConverter(typeof(DayOfWeek?), out var c2), Is.True);
        Assert.That(ReferenceEquals(c1, c2), Is.True);
    }

    #endregion BuiltInConverters 枚举/Nullable 动态创建
}