using System.Reflection;

namespace ExcelUtils
{
    internal class PropertyInfos
    {
        public string Name { get; set; }
        public PropertyInfo Info { get; set; }
        public DisplayAttribute AttributeInfo { get; set; }

        public PropertyInfos(PropertyInfo info, DisplayAttribute attribute)
        {
            Info = info;
            AttributeInfo = attribute;
            Name = attribute.Name;
        }
    }
}