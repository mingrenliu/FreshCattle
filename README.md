# .NetTest
一、excel导入导出
1、定义导出对象：例如： internal class Record { /// <summary> /// 创建时间 /// </summary> [Display("创建时间", Order = 4, Width = 20)] [DataFormat(typeof(LongTimeFormat))] public DateTime CreatedTime { get; set; } /// <summary> /// 计量名称 /// </summary> [Display("计量名称", Order = 0)] public string? Name { get; set; }

/// <summary>
/// 计量类别
/// </summary>
[Display("顺序", Order = 1)]
public int? Order { get; set; }
} 2、导入导出：例如 var lst = ExcelHelper.Import<Record>(workBook); var bytes = ExcelHelper.Export(persons); 3、可覆盖导出名称： var dic = new Dictionary<string, string>() { ["DataKey1"] = "液位", ["Value"] = "体积" }; var bytes = ExcelHelper.ExportTemplate<BoundImportModel>(CreateOption(dic));

private static ExcelSerializeOptions CreateOption(Dictionary<string,string> map,bool strict = true)
{
    var result = new ExcelSerializeOptions();
    result.SetProperty(type => new MapOverridePropertySelector(map, strict).GetTypeInfo(type));
    return result;
}
