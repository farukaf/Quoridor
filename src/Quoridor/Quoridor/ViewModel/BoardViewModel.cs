
namespace Quoridor.ViewModel;

public record BoardViewModel
{
    public BoardViewModel()
    {
        RowSize = 9;
        ColumnSize = 9;
        GenerateWalls();
        for (short i = 0; i < RowSize; i++)
        {
            for (short j = 0; j < ColumnSize; j++)
            {
                var cellAddress = new CellAddress { Row = i, Column = j };

                var cell = new CellViewModel()
                {
                    Address = cellAddress,
                    BottomWall = Walls.First(w => w.TopCell == cellAddress),
                    TopWall = Walls.First(w => w.BottomCell == cellAddress),
                    LeftWall = Walls.First(w => w.RightCell == cellAddress),
                    RightWall = Walls.First(w => w.LeftCell == cellAddress),
                };
                Cells.Add(cellAddress, cell);
            }
        }
    }

    private void GenerateWalls()
    {
        Walls.Clear();
        short i = 0, j = 0;
        for (; i < RowSize; i++)
        {
            j = 0;
            for (; j < ColumnSize; j++)
            {
                //Gerar paredes horizontais superior a celula
                var horSupWall = new WallViewModel
                {
                    TopCell = i > 0 ? new CellAddress((short)(i - 1), j) : null,
                    BottomCell = new CellAddress(i, j),
                };

                //Gerar paredes verticais a esquerda da celula
                var verLftWall = new WallViewModel
                {
                    LeftCell = j > 0 ? new CellAddress(i, (short)(j - 1)) : null,
                    RightCell = new CellAddress(i, j),
                };

                Walls.Add(horSupWall);
                Walls.Add(verLftWall);
            }

            //Gerar paredes verticais a direita da celula
            var verRgtWall = new WallViewModel
            {
                LeftCell = new CellAddress(i, (short)(j - 1)),
            };
            Walls.Add(verRgtWall);
        }

        j = 0;
        for (; j < ColumnSize; j++)
        {
            //Gerar paredes horizontais inferior a celula
            var horInfWall = new WallViewModel
            {
                TopCell = new CellAddress((short)(i - 1), j),
            };
            Walls.Add(horInfWall);
        }
    }

    private List<WallViewModel> Walls { get; set; } = new();

    public Dictionary<CellAddress, CellViewModel> Cells { get; set; } = new();

    public short RowSize { get; set; }
    public short ColumnSize { get; set; }
}
