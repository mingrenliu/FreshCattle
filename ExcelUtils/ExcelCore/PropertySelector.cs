using System.Reflection;

namespace ExcelUtile.ExcelCore
{
    public class MapOverridePropertySelector
    {
        private readonly Dictionary<string, string> _dic;
        private readonly bool _strict = false;
        /// <summary>
        /// 映射覆盖属性选择器
        /// </summary>
        /// <param name="dic">Dictionary used for replacing the names specified in the DisplayAttribute,key is Property Name,value is display name when export or import</param>
        /// <param name="strict">true:Properties that have a DisplayAttribute and whose property name exists in the dictionary will be selected; false: All properties that have a DisplayAttribute will be selected</param>
        public MapOverridePropertySelector(Dictionary<string, string> dic, bool strict = false)
        {
            _dic = dic;
            _strict = strict;
        }

        public IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (_dic.TryGetValue(property.Name, out var name) || !_strict)
                {
                    var attribute = property.GetCustomAttribute<DisplayAttribute>();
                    if (attribute != null)
                    {
                        yield return new DefaultPropertyInfo(property, attribute, name);
                    }
                }
            }
        }
    }

    public static class DefaultPropertySelector
    {
        internal static IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                var attribute = property.GetCustomAttribute<DisplayAttribute>();
                if (attribute != null)
                {
                    yield return new DefaultPropertyInfo(property, attribute);
                }
            }
        }
    }
}