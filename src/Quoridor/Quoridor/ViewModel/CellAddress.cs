namespace Quoridor.ViewModel;

public record struct CellAddress
{
    public CellAddress()
    { }
    public CellAddress(short row, short column)
    {
        Row = row;
        Column = column;
    }
    public short Row { get; init; }
    public short Column { get; init; }
}
