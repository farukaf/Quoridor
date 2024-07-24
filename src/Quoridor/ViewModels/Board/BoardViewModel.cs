namespace Quoridor.ViewModels.Board;

public record BoardViewModel: IDisposable
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
    public Func<Task>? WallPlacedEvent { get; set; }

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
        var avaliable = 0;
        clickedCorner.State = CornerState.Selected;
        foreach (var corner in Corners.Values)
        {
            if (corner == clickedCorner)
                continue;

            corner.State = CornerState.Disabled;

            if ((corner.CornerAddress.X == clickedCorner.CornerAddress.X && corner.CornerAddress.Y == clickedCorner.CornerAddress.Y + 2) ||
                (corner.CornerAddress.Y == clickedCorner.CornerAddress.Y && corner.CornerAddress.X == clickedCorner.CornerAddress.X + 2) ||
                (corner.CornerAddress.X == clickedCorner.CornerAddress.X && corner.CornerAddress.Y == clickedCorner.CornerAddress.Y - 2) ||
                (corner.CornerAddress.Y == clickedCorner.CornerAddress.Y && corner.CornerAddress.X == clickedCorner.CornerAddress.X - 2))
            {
                (_, var wall1, var wall2) = GetWallsFromCorners(clickedCorner, corner);
                if (wall1.IsPlaced || wall2.IsPlaced)
                    continue;
                corner.State = CornerState.Avaliable;
                avaliable++;
                continue;
            }
        }

        if (avaliable == 0)
        {
            foreach (var corner in Corners.Values)
                corner.State = CornerState.Enabled;
        }

        return Task.CompletedTask;
    }

    private async Task AvaliableCornerClicked(CornerViewModel clickedCorner)
    {
        var selectedCorner = Corners.Values.First(c => c.State == CornerState.Selected);

        (_, var wall1, var wall2) = GetWallsFromCorners(clickedCorner, selectedCorner);

        wall1.IsPlaced = true;
        wall2.IsPlaced = true;

        //Reset the corners
        foreach (var corner in Corners.Values)
        {
            corner.State = CornerState.Enabled;
        }

        //WallPlacedEvent
        if (WallPlacedEvent is not null)
            await WallPlacedEvent.Invoke(); 
    }

    private (IEnumerable<CornerViewModel> corners, WallViewModel wall1, WallViewModel wall2) GetWallsFromCorners(CornerViewModel corner1, CornerViewModel corner2)
    {
        //Find the corner in between selected and clicked
        var middleCorner = Corners.Values.First(c =>
        {
            if (corner1.CornerAddress.X == corner2.CornerAddress.X && corner1.CornerAddress.X == c.CornerAddress.X)
                return (c.CornerAddress.Y > corner1.CornerAddress.Y && c.CornerAddress.Y < corner2.CornerAddress.Y) ||
                    (c.CornerAddress.Y < corner1.CornerAddress.Y && c.CornerAddress.Y > corner2.CornerAddress.Y);

            if (corner1.CornerAddress.Y == corner2.CornerAddress.Y && corner1.CornerAddress.Y == c.CornerAddress.Y)
                return (c.CornerAddress.X > corner1.CornerAddress.X && c.CornerAddress.X < corner2.CornerAddress.X) ||
                    (c.CornerAddress.X < corner1.CornerAddress.X && c.CornerAddress.X > corner2.CornerAddress.X);

            return false;
        });

        var corners = (new[] { corner2, corner1, middleCorner })
            .Where(x => x is not null)
            .OrderBy(x => x.CornerAddress.X)
            .ThenBy(x => x.CornerAddress.Y);

        var wall1 = Walls.Single(w => w.From == corners.First().CornerAddress && w.To == corners.ElementAt(1).CornerAddress);
        var wall2 = Walls.Single(w => w.From == corners.ElementAt(1).CornerAddress && w.To == corners.Last().CornerAddress);

        return (corners, wall1, wall2);
    }

    private Task DisabledCornerClicked(CornerViewModel clickedCorner)
    {
        return Task.CompletedTask;
    }

    private Task SelectedCornerClicked(CornerViewModel clickedCorner)
    {
        foreach (var corner in Corners.Values)
        {
            //TODO: Check if colides with a wall
            corner.State = CornerState.Enabled;
        }

        return Task.CompletedTask;
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
                    TopWall = Walls.Single(w => w.BottomCell == cellAddress),
                    LeftWall = Walls.Single(w => w.RightCell == cellAddress),
                    RightWall = Walls.Single(w => w.LeftCell == cellAddress),
                    BottomWall = Walls.Single(w => w.TopCell == cellAddress),
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
                    Direction = Direction.Horizontal,
                };

                //genrate wall vertical and to the left the cell 
                var verLftWall = new WallViewModel
                {
                    LeftCell = j > 0 ? new CellAddress(i, (short)(j - 1)) : null,
                    RightCell = new CellAddress(i, j),
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

        //Resolve Corners First
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
            }
        }

        for (short i = 0; i < rows; i++)
        {
            for (short j = 0; j < columns; j++)
            {
                var gridAddress = new GridAddress(i, j);
                //even column and even row: corner 
                if (j % 2 == 0 && i % 2 == 0)
                {
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

                    //resolve corners
                    var cornerAddressFrom = gridAddress with
                    {
                        Column = (short)(gridAddress.Column - 1)
                    };
                    var cornerAddressTo = gridAddress with
                    {
                        Column = (short)(gridAddress.Column + 1)
                    };

                    var cornerFrom = GridElements[cornerAddressFrom] as CornerViewModel;
                    var cornerTo = GridElements[cornerAddressTo] as CornerViewModel;
                    wall.From = cornerFrom!.CornerAddress;
                    wall.To = cornerTo!.CornerAddress;

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

                    //resolve corners
                    var cornerAddressFrom = gridAddress with
                    {
                        Row = (short)(gridAddress.Row - 1)
                    };
                    var cornerAddressTo = gridAddress with
                    {
                        Row = (short)(gridAddress.Row + 1)
                    };

                    var cornerFrom = GridElements[cornerAddressFrom] as CornerViewModel;
                    var cornerTo = GridElements[cornerAddressTo] as CornerViewModel;
                    wall.From = cornerFrom!.CornerAddress;
                    wall.To = cornerTo!.CornerAddress;

                    continue;
                }
            }
        }
    }

    public void Dispose()
    {
        Walls.Clear();
        Walls.TrimExcess();
        GridElements.Clear();
        GridElements.TrimExcess();
        Cells.Clear();
        Cells.TrimExcess();
        Corners.Clear();
        Corners.TrimExcess();
    }
}
