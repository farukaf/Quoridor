namespace Quoridor.ViewModels.Board;

public record RoomConfigurationViewModel
{
    public int WallsPerPlayer { get; set; } = 10;
}
