using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ExcelUtileTest")]

namespace ExcelUtile;

internal static class PropertyUtil
{
    internal static Dictionary<string, PropertyInfos> ParsePropertys(Type type)
    {
        var results = new Dictionary<string, PropertyInfos>();
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                results[attribute.Name] = new PropertyInfos(property, attribute);
            }
        }
        return results;
    }

    internal static Dictionary<string, PropertyInfos> ParsePropertys<T>() where T : class
    {
        return ParsePropertys(typeof(T));
    }

    internal static List<PropertyInfos> Propertys(Type type)
    {
        var results = new List<PropertyInfos>();
        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                results.Add(new PropertyInfos(property, attribute));
            }
        }
        return results.OrderBy(x => x.AttributeInfo.Order).ToList();
    }

    internal static List<PropertyInfos> Propertys<T>()
    {
        return Propertys(typeof(T));
    }
}

internal static class PropertyExtension
{
    internal static Dictionary<string, PropertyInfos> ParsePropertys(this Type type)
    {
        return PropertyUtil.ParsePropertys(type);
    }

    internal static List<PropertyInfos> Propertys(this Type type)
    {
        return PropertyUtil.Propertys(type);
    }
}