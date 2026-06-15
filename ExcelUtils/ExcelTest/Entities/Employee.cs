namespace ExcelTest.Entities;

/// <summary>
/// opt-out 模式实体（配合 AutoInclude=true）：
/// 所有 public 属性默认都导出，[ExcelIgnore] 的排除。
/// 使用独立的 [ExcelConverter]。
/// </summary>
public class Employee
{
    /// <summary>员工姓名</summary>
    [ExcelColumn(Order = 0, Width = 12)]
    public string Name { get; set; } = string.Empty;

    /// <summary>工号（自动包含，列名=属性名）</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>部门</summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>工资（带格式）</summary>
    public decimal Salary { get; set; }

    /// <summary>入职日期</summary>
    public DateTime HireDate { get; set; }

    /// <summary>是否在职</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>内部备注——不导出</summary>
    [ExcelIgnore]
    public string? InternalNote { get; set; }

    /// <summary>密码哈希——不导出</summary>
    [ExcelIgnore]
    public string? PasswordHash { get; set; }
}
