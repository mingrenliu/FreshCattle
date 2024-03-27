﻿using ExcelUtile.ExcelCore;

namespace ExcelUtile.Formats;

public class TimeSpanFormat : ExcelStructConverter<TimeSpan>
{
    public override TimeSpan? Read(ICell cell)
    {
        if (CanConvert())
        {
            return cell.GetTimeSpan();
        }
        return default;
    }

    public override void Write(ICell cell, TimeSpan? value)
    {
        if (value.HasValue)
        {
            cell.SetCellValue(value.Value.ToString("d\\.hh\\:mm\\:ss"));
        }
    }
}