namespace InheritCore;

/// <summary>
/// 继承
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class InheritAttribute : Attribute
{
    /// <summary>
    /// 继承类名
    /// </summary>
    public string Name { get;}
    /// <summary>
    /// 是否递归
    /// </summary>
    public bool Recursion { get; }
    public InheritAttribute(string name, bool recursion = true)
    {
        Name = name;
        Recursion = recursion;
    }
}
