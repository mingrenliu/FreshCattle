﻿namespace InheritCore;

/// <summary>
/// 继承(继承所有字段)
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class InheritAttribute : Attribute
{
    /// <summary>
    /// 继承类名
    /// </summary>
    public string Name { get; }

    public InheritAttribute(string name)
    {
        Name = name;
    }
}