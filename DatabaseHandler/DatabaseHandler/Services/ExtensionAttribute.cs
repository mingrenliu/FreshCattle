namespace DatabaseHandler.Services;
/// <summary>
/// provider extension attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ExtensionAttribute : Attribute
{
    public string ExtensionName { get; set; }
    public ExtensionAttribute(string extensionName)
    {
        ExtensionName = extensionName;
    }

}
