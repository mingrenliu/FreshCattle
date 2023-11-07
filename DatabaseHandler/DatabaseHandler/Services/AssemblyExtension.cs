using System.Reflection;

namespace DatabaseHandler.Services;

public static class AssemblyExtension
{
    public static IEnumerable<TypeInfo> GetConstructibleTypes(this Assembly assembly)
    => assembly.GetLoadableDefinedTypes().Where(
        t => t is { IsAbstract: false, IsGenericTypeDefinition: false });
    public static IEnumerable<TypeInfo> GetLoadableDefinedTypes(this Assembly assembly)
    {
        try
        {
            return assembly.DefinedTypes;
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null).Select(IntrospectionExtensions.GetTypeInfo!);
        }
    }
}