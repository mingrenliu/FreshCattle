namespace CenterClient;

public class JsonDocumentParser
{
    private readonly Dictionary<string, string?> _data;

    public JsonDocumentParser()
    {
        _data = new();
    }

    private static string GetPrefix(string parent, int current)
    {
        parent = string.IsNullOrEmpty(parent) ? string.Empty : parent + ConfigurationPath.KeyDelimiter;
        return parent + current;
    }

    private static string GetPrefix(string parent, string current)
    {
        parent = string.IsNullOrEmpty(parent) ? string.Empty : parent + ConfigurationPath.KeyDelimiter;
        return parent + current;
    }

    public Dictionary<string, string?> Parse(JsonDocument doc)
    {
        Parse(doc.RootElement, string.Empty);
        return _data;
    }

    private void Parse(JsonElement doc, string parent)
    {
        switch (doc.ValueKind)
        {
            case JsonValueKind.Object:
                ParseObject(doc, parent);
                break;

            case JsonValueKind.Array:
                ParseArray(doc, parent);
                break;

            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
                ParseBaseType(doc, parent);
                break;

            case JsonValueKind.Null:
                ParseNull(doc, parent);
                break;

            default:
                break;
        }
    }

    private void ParseArray(JsonElement doc, string parent)
    {
        int index = 0;
        foreach (var item in doc.EnumerateArray())
        {
            Parse(item, GetPrefix(parent, index++));
        }
    }

    private void ParseObject(JsonElement doc, string parent)
    {
        foreach (var item in doc.EnumerateObject())
        {
            Parse(item.Value, GetPrefix(parent, item.Name));
        }
    }

    private void ParseBaseType(JsonElement doc, string parent)
    {
        _data.TryAdd(parent, doc.GetRawText() ?? string.Empty);
    }

    private void ParseNull(JsonElement doc, string parent)
    {
        _data.TryAdd(parent, doc.GetRawText());
        return;
    }
}