using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;

namespace DbSetGenerator;

public class FileGenerateFactory
{
    private const string _dbUsing = "using Microsoft.EntityFrameworkCore;";

    public static string Generate(ClassInfo dbInfo, ImmutableArray<EntityInfo> entities)
    {
        var builder = new StringBuilder();
        builder.AppendLine(_dbUsing);
        foreach (var entity in entities.Select(x => x.NameSpace).Distinct())
        {
            if (entity != dbInfo.NameSpace && !string.IsNullOrEmpty(entity))
            {
                builder.AppendLine(GenerateUsing(entity));
            }
        }
        if (!string.IsNullOrEmpty(dbInfo.NameSpace))
        {
            builder.AppendLine(GenerateNameSpace(dbInfo.NameSpace));
        }
        builder.AppendLine(GenerateClass(dbInfo.Name));
        builder.AppendLine(GenerateBracket(true));
        var effectEntity = entities.Where(x => !string.IsNullOrEmpty(x.Name) && (x.DbContexts.Count == 0 || x.DbContexts.Contains(dbInfo.Name)));
        if (effectEntity.Any())
        {
            builder.AppendLine(DisableWarning());
            foreach (var item in entities)
            {
                builder.AppendLine(GenerateProperty(item.Name));
            }
            builder.AppendLine(RestoreWarning());
        }
        builder.AppendLine(GenerateBracket());
        return builder.ToString();
    }

    private static string DisableWarning()
    {
        return "# pragma warning disable IDE0051,CS8618";
    }

    private static string RestoreWarning()
    {
        return "# pragma warning restore IDE0051,CS8618";
    }

    private static string GenerateUsing(string name)
    {
        return $"using {name};";
    }

    private static string GenerateNameSpace(string name)
    {
        return $"namespace {name};";
    }

    private static string GenerateClass(string name)
    {
        return $"public partial class {name}";
    }

    private static string GenerateProperty(string name)
    {
        return $"    private DbSet<{name}> {name}" + "s { get; set; }";
    }

    private static string GenerateBracket(bool isLeft = false)
    {
        return isLeft ? "{" : "}";
    }
}