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
        /// <param name="dic"> </param>
        /// <param name="strict"> </param>
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
        public static IEnumerable<PropertyTypeInfo> GetTypeInfo(Type type)
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

        public static IEnumerable<PropertyTypeInfo> GetTypeInfo<T>()
        {
            return GetTypeInfo(typeof(T));
        }
    }
}