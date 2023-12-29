﻿using System.Collections.Immutable;

namespace JPoke;

public readonly record struct JPath(ImmutableArray<JPathElement> Elements)
{
    public static readonly JPath Empty = new(ImmutableArray<JPathElement>.Empty);
    public bool IsTerminal => Elements.Length == 1;
    public int Length => Elements.Length;

    public JPath Descend() => new(Elements.Slice(1, Elements.Length - 1));

    public JPath Append(JPathElement el) => new(Elements.Append(el).ToImmutableArray());
}