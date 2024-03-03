using System.Collections.Immutable;

namespace JPoke;

public readonly record struct JPath(ImmutableArray<JPathElement> Elements)
{
    public static readonly JPath Empty = new(ImmutableArray<JPathElement>.Empty);

    /// <summary>
    ///     Indicates that the path is terminal, i.e. that we can't descend further into it.
    /// </summary>
    public bool IsTerminal => Elements.Length <= 1;

    /// <summary>
    ///     Indicates that the path is "root", ie we have no name or index
    /// </summary>
    public bool IsRoot => Elements.Length == 0;

    public int Length => Elements.Length;
    public JPathElement Top => Length > 0 ? Elements[0] : JPathElement.Empty;
    public JPathElement Leaf => Elements.Last();

    public JPath Descend() => new(Elements.Slice(1, Elements.Length - 1));

    public JPath Append(JPathElement el) => new(Elements.Append(el).ToImmutableArray());

    public JPath LeafParent() => new(Elements.Take(Length - 1).ToImmutableArray());
}