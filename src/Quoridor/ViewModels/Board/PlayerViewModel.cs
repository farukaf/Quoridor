using Quoridor.Helper;
using Quoridor.Services;

namespace Quoridor.ViewModels.Board;

public record PlayerViewModel
{ 
    public PlayerViewModel(Player player)
    {
        Id = player.Id;
        Name = player.UserName; 
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Color Color { get; set; }
    public CellAddress Address { get; set; } = new();
    public string Name { get; set; } = string.Empty;
    public string[] Tags { get;set; } = Array.Empty<string>();
    public int WallCount { get; internal set; }
}
