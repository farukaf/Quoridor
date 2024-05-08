namespace Quoridor.ViewModel;

public record CellViewModel : GridElementViewModel
{
    public WallViewModel TopWall { get; set; } = default!;
    public WallViewModel LeftWall { get; set; } = default!;
    public WallViewModel RightWall { get; set; } = default!;
    public WallViewModel BottomWall { get; set; } = default!;
    public CellAddress Address { get; set; }

    public override string CssClass()
    {
        return "cell";
    }

    public override string ToString()
    {
        return $"(r{Address.Row},c{Address.Column})";
    }

}
