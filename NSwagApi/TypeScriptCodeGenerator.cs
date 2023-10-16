using NJsonSchema;
using NSwag;
using System.Text;

namespace NSwagApi;

/// <summary>
/// api generator
/// </summary>
public class TypeScriptCodeGenerator
{
    /// <summary>
    /// generator file string
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
    public static string GenerateFile(OpenApiDocument document)
    {
        var stringBuilder = new StringBuilder();
        var schemas = new Dictionary<JsonSchema, string>();
        foreach (var item in document.Definitions)
        {
            schemas[item.Value] = item.Key;
        }
        stringBuilder.AppendLine("//#region \"type\"");
        foreach (var item in document.Definitions)
        {
            if (item.Value.IsEnumeration)
            {
                stringBuilder.Append(Start(item.Key, "enum"));
                stringBuilder.AppendLine(Open);
                foreach (var name in item.Value.EnumerationNames)
                {
                    stringBuilder.AppendLine($"    {name},");
                }
            }
            else
            {
                stringBuilder.Append(Start(item.Key));
                if (item.Value.InheritedSchema?.ActualSchema != null && schemas.TryGetValue(item.Value.InheritedSchema.ActualSchema, out var extendedName))
                {
                    var overrides = item.Value.ActualProperties.Keys.Intersect(item.Value.InheritedSchema.ActualSchema.ActualProperties.Keys);
                    var str = overrides.Any() ? $" extends Omit<{extendedName},{string.Join('|', overrides.Select(x => "\"" + x + "\""))}> " : $" extends {extendedName} ";
                    stringBuilder.Append(str);
                }
                stringBuilder.AppendLine(Open);
                foreach (var prop in item.Value.ActualProperties)
                {
                    stringBuilder.AppendLine(Property(prop.Key, Type(prop.Value.ActualSchema, schemas), prop.Value.IsRequired));
                }
            }
            stringBuilder.AppendLine(Close);
        }
        stringBuilder.AppendLine("//#endregion \"type\"");
        if (document.Paths.Any())
        {
            stringBuilder.AppendLine("//#region \"api\"");
            stringBuilder.AppendLine("import { Result, request } from \"./request\"\r\nconst pattern: RegExp = /\\(w+\\)/g");
            foreach (var pathItem in document.Paths)
            {
                if (pathItem.Value.ActualPathItem.Count > 0)
                {
                    var value = pathItem.Value.ActualPathItem.First();
                    var summary = value.Value.Summary;
                    var methodName = value.Value.OperationId;
                    var paras = new List<Parameter>();
                    foreach (var param in value.Value.ActualParameters.Where(x => (x.Kind & (OpenApiParameterKind.Query | OpenApiParameterKind.Body | OpenApiParameterKind.Path | OpenApiParameterKind.FormData)) != 0))
                    {
                        paras.Add(new Parameter() { Name = param.Name, Type = param.Kind == OpenApiParameterKind.FormData ? "FormData" : Type(param.ActualSchema, schemas, TypeModel.Response), Kind = param.Kind, IsRequired = param.IsRequired });
                    }
                    var response = value.Value.ActualResponses.First().Value.ActualResponse;
                    var resultType = response.Schema?.ActualSchema == null ? "void" : Type(response.Schema.ActualSchema, schemas, TypeModel.Response);
                    if (!string.IsNullOrWhiteSpace(value.Value.Summary))
                    {
                        stringBuilder.AppendLine("//" + value.Value.Summary);
                    }
                    stringBuilder.AppendLine(Function(methodName, resultType, paras));
                    stringBuilder.AppendLine(Tab + Const("method", value.Key));
                    var paths = paras.Where(x => x.Kind == OpenApiParameterKind.Path);
                    if (paths.Any())
                    {
                        stringBuilder.AppendLine(Tab + Const("url", pathItem.Key, "let"));
                        stringBuilder.AppendLine(Tab + Map(paths));
                    }
                    else
                    {
                        stringBuilder.AppendLine(Tab + Const("url", pathItem.Key));
                    }
                    var queries = paras.Where(x => x.Kind == OpenApiParameterKind.Query);
                    var query = queries.Any() ? Query(queries) : "null";
                    var body = paras.FirstOrDefault(x => x.Kind == OpenApiParameterKind.Body || x.Kind == OpenApiParameterKind.FormData);
                    stringBuilder.AppendLine(Tab + Request(query, body, resultType));
                    stringBuilder.AppendLine(Close);
                }
            }
            stringBuilder.AppendLine("//#endregion \"api\"");
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    ///
    /// </summary>
    private class Parameter
    {
        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否为必须的
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public OpenApiParameterKind Kind { get; set; }
    }

    public enum TypeModel
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        Request,

        /// <summary>
        /// 返回值
        /// </summary>
        Response
    }

    private static string Type(JsonSchema schema, Dictionary<JsonSchema, string> schemas, TypeModel model = TypeModel.Request)
    {
        if (schemas.TryGetValue(schema, out var type))
        {
            return type;
        }
        if (schema.Type == JsonObjectType.None && schema.Reference != null)
        {
            return Type(schema.Reference.ActualSchema, schemas, model);
        }
        if (schema.Type == JsonObjectType.String)
        {
            return "string";
        }
        if (schema.Type == JsonObjectType.File)
        {
            return model == TypeModel.Request ? "string" : "Blob";
        }
        if (schema.Type == JsonObjectType.Integer || schema.Type == JsonObjectType.Number)
        {
            return "number";
        }
        if (schema.Type == JsonObjectType.Boolean)
        {
            return "boolean";
        }
        if (schema.IsTuple)
        {
            return "[]";
        }
        if (schema.IsArray)
        {
            return Type(schema.Item.ActualSchema, schemas, model) + "[]";
        }
        if (schema.IsDictionary || schema.IsObject)
        {
            return "object";
        }
        return "any";
    }

    private static string Const(string name, string value, string keyword = "const")
    {
        return $"{keyword} {name} = \"{value}\"";
    }

    private static string Property(string name, string type, bool nullable) => $"    {name}{(nullable ? "?" : "")}: {type}";

    private static string Function(string function, string returnType, IEnumerable<Parameter> para)
    {
        string p = string.Join(",", para.Select(x => $"{x.Name}{RequiredMark(x.IsRequired)}: {x.Type}"));
        return $"export async function {function}({p}) : Promise<Result<{returnType}>>" + Open;
    }

    private static string Request(string query, Parameter body, string returnType)
    {
        if (body == null)
        {
            return $"return await request<{returnType}>(url, method, {query})";
        }
        else if (body.Kind == OpenApiParameterKind.FormData)
        {
            return $"return await request<{returnType}>(url, method, {query}, {body.Name} ,{{ \"Content-Type\": \"multipart/form-data\" }})";
        }
        else
        {
            return $"return await request<{returnType}>(url, method, {query}, {body.Name})";
        }
    }

    private static string Query(IEnumerable<Parameter> paras)
    {
        return Open + string.Join(',', paras.Select(x => x.Name)) + Close;
    }

    private static string Map(IEnumerable<Parameter> paras)
    {
        return "url = url.format(pattern,new Map<string, any>([" + string.Join(',', paras.Select(x => $"[\"{x.Name}\", {x.Name}]")) + "]))";
    }

    private static string RequiredMark(bool isRequired)
    {
        return isRequired ? "" : "?";
    }

    private static string Start(string name, string type = "interface") => $"export {type} {name}";

    private static readonly string Open = "{";

    private static readonly string Close = "}";

    private static readonly string Tab = "    ";
}