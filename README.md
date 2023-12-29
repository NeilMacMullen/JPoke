# JPoke

JPoke is a a simple library that allows construction or manipulation of JSON/YAML structured objects from the C# domain.  For example:

```CSharp
var builder = JObjectBuilder.CreateEmpty();
builder.Set("flags.enable[0]",false);
Console.WriteLine(builder.ToJson());
```
```Json
{
    "flags" : { 
                "enable": [ false }
                }
        }
}
```
JPoke is able to construct intermediate nodes and elements as required.

## Rationale
JPoke was written to support the VegaSharp library where the underlying Vega-Lite Json is too variable to allow simple mapping to C# structure equivalents and too deeply nested for dynamic objects to be useful.

## Usage

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



###

## YAML
...to come
