namespace Quoridor.ViewModels.Board;

public record struct CornerAddress
{
    public CornerAddress()
    { }
    public CornerAddress(short x, short y)
    {
        X = x;
        Y = y;
    }
    public short X { get; init; }
    public short Y { get; init; }
}
