using Quoridor.Helper;
using System;

namespace Quoridor.ViewModel;

public record RoowViewModel
{
    public RoowViewModel()
    {
        Id = Guid.NewGuid();
        Players = new Dictionary<CellAddress, PlayerViewModel>()
        {
            {
                new CellAddress(0, 4),
                new PlayerViewModel { Name = "Player 1", Color = Color.Blue }
            },
            {
                new CellAddress(8, 4),
                new PlayerViewModel { Name = "Player 2", Color = Color.Red }
            }
        };

        CurrentPlayer = Players.First().Value;
    }

    public EventHandler? RoomChanged { get; set; }

    public Guid Id { get; internal set; }

    public Dictionary<CellAddress, PlayerViewModel> Players { get; set; }

    public PlayerViewModel CurrentPlayer { get; set; } = new();

    public BoardViewModel Board { get; set; } = new();

    public PlayerViewModel? GetPlayer(CellAddress address)
    {
        return Players.TryGetValue(address, out var player) ? player : null;
    }

    public void MovePlayer(CellAddress from, CellAddress to)
    {
        if (Players.TryGetValue(from, out var player))
        {
            player.Address = to;
            RoomChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
