namespace InheritCore;

/// <summary>
/// 浅继承(只继承系统类型属性)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ShallowInheritAttribute : Attribute
{
    /// <summary>
    /// 继承类名
    /// </summary>
    public string Name { get; }

    public ShallowInheritAttribute(string name)
    {
        Name = name;
    }
}