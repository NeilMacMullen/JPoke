# JPoke

JPoke is a a simple library that allows construction or manipulation of JSON/YAML structured objects from the C# domain.  For example:

```CSharp
var builder = JObjectBuilder.CreateEmpty();
builder.Set("settings[0].size",new {x=100,y=200});
builder.Copy("settings[0].size.x","config.initial_width");
Console.WriteLine(builder.ToJson());
```

```Json
{
  "config": {
	"initial_width": 100
  },
  "settings" : [ 
                  { 
                      "size": { 
                                  "x" : 100, 
                                  "y" : 200
                       }
                  
                ]
}
```
JPoke is able to construct intermediate nodes and elements as required.

## Rationale
JPoke was written to support the VegaSharp library where the underlying Vega-Lite Json is too variable to allow simple mapping to C# structure equivalents and too deeply nested for dynamic objects to be useful.

## Usage

### Custom serialisation

JPoke does not support serialisation of custom types; however you can construct a JObjectBuilder from a serialised string and then use that as a value...

```CSharp
var jsonText = CustomSerializer.Deserialise(customObject);
var objectToBeInserted = JObjectBuilder.FromJson(jsonText);
builder.Set("complexNode",objectToBeInserted);


```
### Construction
All object manipulation is done via a JObjectBuilder.

```CSharp
var builder = JObjectBuilder.CreateEmpty();
var builder = JObjectBuilder.FromJson(jsonString);
var builder = JObjectBuilder.FromObject(object); 
```

### Writing
Values are set using "dotted" syntax to indicated structure and square brackets for array indices.

Values may be primitive types or structured objects.  It is also possible set "serialised"

Examples:

var 

builder.Set("settings[0].size",new {x=100,y=200});



##### Indexer syntax
[]
[1] 
[+1]
[-1]

### Policies
ExtendArrays/NoExtend
ArrayFillNull/ArrayFillEmpty/ArrayFillCopy/ArrayFillDefault (for primitives)
OverwriteAllowed 
AutoConstruct

### Conventions...

```CSharp
var builder = JObjectBuilder.FromJson();
var builder = JObjectBuilder.FromObject(object/JObjectBuilder/JsonNode)

var json = builder.ToJson(); //Serialize()
var object = builder.ToJsonNode();
var object = builder.Deserialize<T>();



```

### Mutability

JObjectBuilder is mutable.
All value objects/trees are cloned when setting nodes.

## YAML
...to come
