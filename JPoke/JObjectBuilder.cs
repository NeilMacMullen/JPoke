using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;

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

    public JObjectBuilder Set(string path, object value) => Set2(_root, PathParser.Parse(path), value);

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


    public readonly record struct MatchResult(MatchType Result, JsonNode Node,JPath unmatched);


    public enum MatchType
    {
        MissingObjectProperty,
        Object,
        IncompleteArray,
        IncompatibleType,
        RanOutOfPath
    }
    public MatchResult Descend(JsonNode currentParent, JPath path)
    {
        if (path.Elements.Length == 0)
            return new MatchResult(MatchType.RanOutOfPath, currentParent,JPath.Empty);
        var top = path.Elements.First();
        
        switch (currentParent)
        {
            case JsonObject obj:
                if (!obj.TryGetPropertyValue(top.Name, out var p))
                    return new MatchResult(MatchType.MissingObjectProperty, obj,path);
                //we have property
                if (path.IsTerminal)
                {
                    return top.IsIndex 
                        ? Descend(p,path) //special case for indexers   
                        : new MatchResult(MatchType.Object, p,path);
                }

                return Descend(p, path.Descend());
            case JsonArray arr:
                if (!top.IsIndex)
                    return new MatchResult(MatchType.IncompatibleType, currentParent,path);
                if (top.Index.EffectiveIndex(arr.Count)>=arr.Count)
                    return new MatchResult(MatchType.IncompleteArray, arr,path);
                var indexedItem = arr.ElementAt(top.Index.EffectiveIndex(arr.Count));
                return path.IsTerminal 
                    ? new MatchResult(MatchType.Object, indexedItem,path) 
                    : Descend(indexedItem, path.Descend());

            default:
                return new MatchResult(MatchType.IncompatibleType, currentParent,path);
        }
    }

    public JObjectBuilder Set2(JsonNode node,JPath  jpath, object value)
    {
        int n = 10;
        while (n-- > 0)
        {
            var match = Descend(node, jpath);
         
            var firstUnmatched = match.unmatched.Elements.First();
            switch (match.Result)
            {
                case MatchType.MissingObjectProperty:
                    var parent = match.Node as JsonObject;
                    if (match.unmatched.IsTerminal)
                    {
                        parent.Add(firstUnmatched.Name, ObjectToJsonNode(value));
                        return this;
                    }
                    else
                    {
                        if (firstUnmatched.IsIndex)
                            EnsureContainerHasArrayAt(parent, firstUnmatched.Name);
                        else
                            EnsureContainerHasObjectAt(parent, firstUnmatched.Name);
                    }

                    break;
                case MatchType.Object:
                    throw new InvalidOperationException("object");
                    break;
                case MatchType.IncompleteArray:
                    throw new InvalidOperationException("incomplete array");
                    break;
                case MatchType.IncompatibleType:
                    throw new InvalidOperationException("incompatibleType");
                    break;
                case MatchType.RanOutOfPath:
                    throw new InvalidOperationException("ran out of path");
                    break;
                default:
                    Console.WriteLine($"unhandled case {match.Result}");
                    break;
            }
        }

        return this;
    }
    
    public MatchResult Match(string path)
    {
        var jpath = PathParser.Parse(path);
        return Descend(_root, jpath);
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
