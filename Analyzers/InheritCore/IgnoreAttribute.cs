namespace InheritCore;

/// <summary>
/// 忽略
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class IgnoreAttribute : Attribute
{
    public IgnoreAttribute()
    {
    }
}