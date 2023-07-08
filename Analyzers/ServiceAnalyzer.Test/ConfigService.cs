using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ServiceAnalyzer.Test;

public partial class ConfigService : IConfigService

{
    /// <summary>
    /// 中国
    /// </summary>
    [StringLength(100)]
    public IEnumerable<DateTime> AllTime { get; set; }= new List<DateTime>();

    public void Display(string name)
    {
        Console.WriteLine(name);
    }

    public void Display1(string name)
    {
        Console.WriteLine(name);
    }
}

public interface IConfigService

{
    void Display(string name);

    void Display1(string name);
}