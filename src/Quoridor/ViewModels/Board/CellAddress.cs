namespace Quoridor.ViewModels.Board;

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

    public static CellAddress Player1StartPosition = new(0, 4);
    public static CellAddress Player2StartPosition = new(8, 4);
}
