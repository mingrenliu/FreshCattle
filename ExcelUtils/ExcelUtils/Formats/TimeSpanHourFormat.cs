﻿using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanHourFormat : ExcelStructConverter<TimeSpan>
{
    public TimeSpanHourFormat() : this(0)
    {

    }
    public TimeSpanHourFormat(int? precision) : base()
    {
        precision ??= 0;
        Format = precision <= 0 ? "0" : "0." + new string('0', precision.Value);
    }
    public TimeSpanHourFormat(string? format) : base()
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            var precision = 0;
            Format = precision <= 0 ? "0" : "0." + new string('0', precision);
        }
        else
        {
            Format = format;
        }
    }

    public override TimeSpan? Read(ICell cell)
    {
        if (CanConvert())
        {
            var value = cell.GetDouble();
            if (value.HasValue)
            {
                return TimeSpan.FromHours(value.Value);
            }
            return cell.GetTimeSpanFromHours();
        }
        return default;
    }

    protected override void WriteValue(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.TotalHours);
        }
    }
}