namespace InheritCore;

/// <summary>
/// 继承所有字段并生产关联的自定义类型
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class DeepInheritAttribute : Attribute
{
    /// <summary>
    /// 继承类名
    /// </summary>
    public string Name { get; }

    public DeepInheritAttribute(string name)
    {
        Name = name;
    }
}