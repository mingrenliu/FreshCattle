namespace ExcelUtileTest.Utilities;

internal static class LocationHelper
{
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

    internal static string GetExportResourcesPath()
    {
        return Path.Combine(GetBasePath(), "Resources", ".Exports");
    }
}