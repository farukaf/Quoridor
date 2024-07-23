namespace Quoridor.ViewModels.Board;

public record BoardViewModel
{
    public BoardViewModel()
    {
        RowSize = 9;
        ColumnSize = 9;
        GenerateCorners();
        GenerateWalls();
        GenerateCells();
        GenerateGridAddress();
    }

    public short RowSize { get; set; }
    public short ColumnSize { get; set; }
    private List<WallViewModel> Walls { get; set; } = new();
    public Dictionary<GridAddress, GridElementViewModel> GridElements { get; set; } = new();
    public Dictionary<CellAddress, CellViewModel> Cells { get; set; } = new();
    public Dictionary<CornerAddress, CornerViewModel> Corners { get; set; } = new();

    public Func<Task>? BoardChanged { get; set; }

    public async Task CornerClicked(CornerViewModel clickedCorner)
    {
        switch (clickedCorner.State)
        {
            case CornerState.Enabled:
                await EnabledCornerClicked(clickedCorner);
                break;
            case CornerState.Selected:
                await SelectedCornerClicked(clickedCorner);
                break;
            case CornerState.Disabled:
                await DisabledCornerClicked(clickedCorner);
                break;
            case CornerState.Avaliable:
                await AvaliableCornerClicked(clickedCorner);
                break;
        }

        if (BoardChanged is not null)
            await BoardChanged.Invoke();
    }

    private Task EnabledCornerClicked(CornerViewModel clickedCorner)
    {
        clickedCorner.State = CornerState.Selected;

        foreach (var corner in Corners.Values)
        {
            if (corner == clickedCorner)
                continue;

            corner.State = CornerState.Disabled;

            if (corner.CornerAddress.X == clickedCorner.CornerAddress.X && corner.CornerAddress.Y == clickedCorner.CornerAddress.Y + 2)
            {
                //TODO: Check if colides with a wall
                corner.State = CornerState.Avaliable;
                continue;
            }

            if (corner.CornerAddress.Y == clickedCorner.CornerAddress.Y && corner.CornerAddress.X == clickedCorner.CornerAddress.X + 2)
            {
                //TODO: Check if colides with a wall
                corner.State = CornerState.Avaliable;
                continue;
            }
        }

        return Task.CompletedTask;
    }

    private async Task AvaliableCornerClicked(CornerViewModel clickedCorner)
    {
        throw new NotImplementedException();
    }

    private async Task DisabledCornerClicked(CornerViewModel clickedCorner)
    {
        throw new NotImplementedException();
    }

    private async Task SelectedCornerClicked(CornerViewModel clickedCorner)
    {
        throw new NotImplementedException();
    }

    private void GenerateCells()
    {
        for (short i = 0; i < RowSize; i++)
        {
            for (short j = 0; j < ColumnSize; j++)
            {
                var cellAddress = new CellAddress { Row = i, Column = j };

                var cell = new CellViewModel()
                {
                    Address = cellAddress,
                    TopWall = Walls.First(w => w.BottomCell == cellAddress),
                    LeftWall = Walls.First(w => w.RightCell == cellAddress),
                    RightWall = Walls.First(w => w.LeftCell == cellAddress),
                    BottomWall = Walls.First(w => w.TopCell == cellAddress),
                };
                Cells.Add(cellAddress, cell);
            }
        }
    }

    private void GenerateCorners()
    {
        short i = 0, j = 0;
        for (; i < RowSize + 1; i++)
        {
            j = 0;
            for (; j < ColumnSize + 1; j++)
            {
                var cornerAddress = new CornerAddress { X = i, Y = j };
                var corner = new CornerViewModel { CornerAddress = cornerAddress };
                corner.Clicked += CornerClicked;
                Corners.Add(cornerAddress, corner);
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
                //Generatel walls horizontal and above the cell 
                var horSupWall = new WallViewModel
                {
                    TopCell = i > 0 ? new CellAddress((short)(i - 1), j) : null,
                    BottomCell = new CellAddress(i, j),
                    From = new CornerAddress { X = i, Y = j },
                    To = new CornerAddress { X = (short)(i + 1), Y = j },
                    Direction = Direction.Horizontal,
                };

                //genrate wall vertical and to the left the cell 
                var verLftWall = new WallViewModel
                {
                    LeftCell = j > 0 ? new CellAddress(i, (short)(j - 1)) : null,
                    RightCell = new CellAddress(i, j),
                    From = new CornerAddress { X = i, Y = j },
                    To = new CornerAddress { X = i, Y = (short)(j + 1) },
                    Direction = Direction.Vertical,
                };

                Walls.Add(horSupWall);
                Walls.Add(verLftWall);
            }

            //Generate wall vertical and to the right the cell
            var verRgtWall = new WallViewModel
            {
                LeftCell = new CellAddress(i, (short)(j - 1)),
                From = new CornerAddress { X = i, Y = j },
                To = new CornerAddress { X = i, Y = (short)(j + 1) },
                Direction = Direction.Vertical,
            };
            Walls.Add(verRgtWall);
        }

        j = 0;
        for (; j < ColumnSize; j++)
        {
            //Generate walls horizontal and under the cell
            var horInfWall = new WallViewModel
            {
                TopCell = new CellAddress((short)(i - 1), j),
                From = new CornerAddress { X = i, Y = j },
                To = new CornerAddress { X = (short)(i + 1), Y = j },
                Direction = Direction.Horizontal,
            };
            Walls.Add(horInfWall);
        }
    }

    public void GenerateGridAddress()
    {
        var rows = RowSize * 2 + 1;
        var columns = ColumnSize * 2 + 1;
        for (short i = 0; i < rows; i++)
        {
            for (short j = 0; j < columns; j++)
            {
                var gridAddress = new GridAddress(i, j);
                //even column and even row: corner 
                if (j % 2 == 0 && i % 2 == 0)
                {
                    var cornerAddress = new CornerAddress((short)(i / 2), (short)(j / 2));
                    var corner = Corners[cornerAddress];
                    corner.GridAddress = gridAddress;
                    GridElements.Add(gridAddress, corner);
                    continue;
                }

                //odd column and odd row: cell
                if (j % 2 != 0 && i % 2 != 0)
                {
                    var cellAddress = new CellAddress((short)(i / 2), (short)(j / 2));
                    var cell = Cells[cellAddress];
                    cell.GridAddress = gridAddress;
                    GridElements.Add(gridAddress, cell);
                    continue;
                }

                //odd column and even row: horizontal wall
                if (j % 2 != 0 && i % 2 == 0)
                {
                    var cellAddress = new CellAddress((short)(i / 2), (short)(j / 2));
                    WallViewModel? wall = null;
                    if (Cells.ContainsKey(cellAddress))
                    {
                        var cell = Cells[cellAddress];
                        wall = cell.TopWall;
                    }
                    else
                    {
                        //this will handle the last row
                        var cell = Cells[new CellAddress((short)(cellAddress.Row - 1), cellAddress.Column)];
                        wall = cell.BottomWall;
                    }
                    wall.GridAddress = gridAddress;
                    GridElements.Add(gridAddress, wall);
                    continue;
                }

                //even column and odd row: vertical wall
                if (j % 2 == 0 && i % 2 != 0)
                {
                    var cellAddress = new CellAddress((short)(i / 2), (short)(j / 2));
                    WallViewModel? wall = null;
                    if (Cells.ContainsKey(cellAddress))
                    {
                        var cell = Cells[cellAddress];
                        wall = cell.LeftWall;
                    }
                    else
                    {
                        //this will handle the last column
                        var cell = Cells[new CellAddress(cellAddress.Row, (short)(cellAddress.Column - 1))];
                        wall = cell.RightWall;
                    }
                    wall.GridAddress = gridAddress;
                    GridElements.Add(gridAddress, wall);
                    continue;
                }
            }
        }
    }
}
