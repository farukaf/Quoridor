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
    public CellAddress(int row, int column)
    {
        Row = (short)row;
        Column = (short)column;
    }
    public short Row { get; init; }
    public short Column { get; init; }

    public readonly static CellAddress Player1StartPosition = new(0, 4);
    public readonly static CellAddress Player2StartPosition = new(8, 4);

    public readonly static CellAddress[] Player1VictoryCells = [new(8, 0), new(8, 1), new(8, 2), new(8, 3), new(8, 4), new(8, 5), new(8, 6), new(8, 7), new(8, 8)];
    public readonly static CellAddress[] Player2VictoryCells = [new(0, 0), new(0, 1), new(0, 2), new(0, 3), new(0, 4), new(0, 5), new(0, 6), new(0, 7), new(0, 8)];
}
