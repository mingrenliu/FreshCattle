using System.Collections.Generic;

namespace DbSetGenerator;

public class ClassInfo
{
    /// <summary>
    /// 类名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 类空间
    /// </summary>
    public string NameSpace { get; set; }
}
public class EntityInfo : ClassInfo
{
    /// <summary>
    /// 所属的数据库上下文
    /// </summary>
    public List<string> DbContexts { get; set; }=new List<string>();
}