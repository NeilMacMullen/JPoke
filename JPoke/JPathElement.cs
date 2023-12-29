namespace JPoke;

public readonly record struct JPathElement(string Name, JPathIndex Index)
{
    public bool IsIndex => Index.IndexType != JPathIndexType.NotIndex;
}