using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace JPoke;

public class JObjectBuilder
{
    private readonly JsonNode _root;

    private JObjectBuilder(JsonNode root) => _root = root;

    public static JObjectBuilder CreateEmpty() => new(new JsonObject());

    public static JObjectBuilder FromObject(object o)
    {
        var node = ObjectToJsonNode(o);
        return new JObjectBuilder(node);
    }

    public JObjectBuilder Set(string path, object value) => Set(_root, PathParser.Parse(path), value);

    private static JsonArray EnsureContainerHasArrayAt(JsonObject container,
        string arrayName)

    {
        if (container.TryGetPropertyValue(arrayName, out var existingArray))
        {
            return existingArray as JsonArray ?? [];
        }

        var arr = new JsonArray();
        container.Add(arrayName, arr);
        return arr;
    }

    private static JsonObject EnsureContainerHasObjectAt(JsonObject container, string path)
    {
        if (container.TryGetPropertyValue(path, out var existingNode))
        {
            return existingNode as JsonObject ?? new JsonObject();
        }

        var child = new JsonObject();
        container.Add(path, child);
        return child;
    }

    private static void SetValueInArray<T>(JsonArray arr, JPathIndex index, Func<T> fill, T value)
        where T : JsonNode
    {
        var actualIndex = index.EffectiveIndex(arr.Count);
        while (arr.Count <= actualIndex) arr.Add(fill());
        arr[actualIndex] = value;
    }


    public JObjectBuilder Set(JsonNode current, JPath path, object value)
    {
        var container = current as JsonObject;
        var element = path.Elements.First();
        if (!path.IsTerminal)
        {
            if (element.IsIndex)
            {
                var array = EnsureContainerHasArrayAt(container, element.Name);
                var childContainer = new JsonObject();
                SetValueInArray(array, element.Index, () => null, childContainer);
                Set(childContainer, path.Descend(), value);
            }
            else
            {
                var child = EnsureContainerHasObjectAt(container, element.Name);
                Set(child, path.Descend(), value);
            }

            return this;
        }

        var newValue = ObjectToJsonNode(value);

        //if is terminal
        if (element.IsIndex)
        {
            var array = EnsureContainerHasArrayAt(container, element.Name);
            SetValueInArray(array, element.Index, () => null, newValue);
        }
        else
            container.Add(element.Name, newValue);

        return this;
    }

    private static JsonNode ObjectToJsonNode(object obj)
    {
        var txt = JsonSerializer.Serialize(obj);
        var tree = JsonNode.Parse(txt);
        return tree;
    }

    public string Serialize() =>
        _root.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        });
}