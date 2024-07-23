namespace Quoridor.ViewModels.Board;

public record WallViewModel : GridElementViewModel
{
    public bool IsPlaced { get; set; } = false;
    public bool CanBePlaced { get; set; } = true;
    public bool CanBeSelected { get; set; } = true;
    public bool IsDisabled { get => !CanBeSelected; }
    public bool IsSelected { get; set; } = false;

    public CellAddress? TopCell { get; set; }
    public CellAddress? BottomCell { get; set; }
    public CellAddress? LeftCell { get; set; }
    public CellAddress? RightCell { get; set; }

    public Direction Direction { get; set; }

    public CornerAddress To { get; set; }
    public CornerAddress From { get; set; }

    public override string CssClass()
    {
        return "wall " + Direction.ToString().ToLower();
    }
}

public enum Direction
{
    Horizontal,
    Vertical
}