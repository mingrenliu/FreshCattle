namespace DatabaseHandler.Services;

/// <summary>
/// provider extension attribute
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ProviderAttribute : Attribute
{
    public string ProviderName { get; set; }

    public ProviderAttribute(string extensionName)
    {
        ProviderName = extensionName;
    }
}