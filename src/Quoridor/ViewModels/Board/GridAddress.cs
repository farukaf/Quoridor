namespace Quoridor.ViewModels.Board;

public record struct GridAddress
{
    public GridAddress()
    { }
    public GridAddress(short row, short column)
    {
        Row = row;
        Column = column;
    }
    public short Row { get; init; }
    public short Column { get; init; }
}
