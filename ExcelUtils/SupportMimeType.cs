namespace ExcelUtile;

public static class SupportMimeType
{
    public const string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public static bool ValidMimeType(string contentType)
    {
        return contentType.Equals(XLSX, StringComparison.OrdinalIgnoreCase);
    }
}