namespace Quoridor.ViewModel;

public record CellViewModel
{
    public WallViewModel TopWall { get; set; }
    public WallViewModel BottomWall { get; set; }
    public WallViewModel LeftWall { get; set; }
    public WallViewModel RightWall { get; set; }
    public CellAddress Address { get; set; }

    public override string ToString()
    {
        return $"(r{Address.Row},c{Address.Column})";
    }
}
