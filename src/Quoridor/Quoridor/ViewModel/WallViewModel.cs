namespace Quoridor.ViewModel;

public record WallViewModel
{
    public bool IsPlaced { get; set; } = false;
    public bool CanBePlaced { get; set; } = true;
    public bool CanBeSelected { get; set; } = true;
    public bool IsDisabled { get => !CanBeSelected; }
    public bool IsSelected { get; set; } = false;

    public event Func<WallViewModel, Task>? SelectedEvent;

    public CellAddress? TopCell { get; set; }
    public CellAddress? BottomCell { get; set; }
    public CellAddress? LeftCell { get; set; }
    public CellAddress? RightCell { get; set; }

    public async Task Select()
    {
        IsSelected = true;
        if (SelectedEvent is not null)
            await SelectedEvent.Invoke(this);
    }
}
