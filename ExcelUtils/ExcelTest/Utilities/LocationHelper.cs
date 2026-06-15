namespace ExcelUtileTest.Utilities;

internal static class LocationHelper
{
    private static readonly bool OutputToFiles = true;

    public static string GetTestPath()
    {
        return TestContext.CurrentContext.TestDirectory;
    }

    internal static string GetBasePath()
    {
        var path = GetTestPath();
        var directionInfo = new DirectoryInfo(path);
        while (!string.Equals(directionInfo!.Name, "bin", StringComparison.OrdinalIgnoreCase))
        {
            directionInfo = directionInfo.Parent;
        }
        return directionInfo.Parent!.FullName;
    }

    internal static string GetImportResourcesPath()
    {
        return Path.Combine(GetBasePath(), "Resources", ".Imports");
    }

    public static string GetExportResourcesPath()
    {
        return Path.Combine(GetBasePath(), "Resources", ".Exports");
    }

    /// <summary>保存导出文件，自动创建目录。</summary>
    public static void SaveToFile(string fileName, byte[] content)
    {
        if (!OutputToFiles) return;
        var dir = GetExportResourcesPath();
        if (Directory.Exists(dir) is false)
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllBytes(Path.Combine(dir, fileName), content);
    }

    /// <summary>生成标准导出文件名：测试名_列数_行数.xlsx。</summary>
    public static string ExportFileName(int cols, int rows)
    {
        var testName = TestContext.CurrentContext.Test.Name;
        return $"{testName}_{cols}cols_{rows}rows.xlsx";
    }
}