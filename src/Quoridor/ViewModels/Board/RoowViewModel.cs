using Quoridor.Helper;
using Quoridor.Services;

namespace Quoridor.ViewModels.Board;

public record RoowViewModel
{
    public RoowViewModel()
    {
        Id = Guid.NewGuid();
    }

    public Func<Task>? RoomChanged { get; set; }

    public Guid Id { get; internal set; }

    public Dictionary<CellAddress, PlayerViewModel> Players { get; set; } = new();

    public Dictionary<Guid, Player> Spectators { get; set; } = new();

    public PlayerViewModel? CurrentPlayer { get; set; }

    public BoardViewModel Board { get; set; } = new();

    public PlayerViewModel? GetPlayer(CellAddress address)
    {
        return Players.TryGetValue(address, out var player) ? player : null;
    }

    public IEnumerable<PlayerViewModel> GetPlayers() =>
        Players.Values;

    public IEnumerable<Player> GetSpectators() =>
        Spectators.Values;

    public KeyValuePair<CellAddress, PlayerViewModel>? GetPlayer(Player player) =>
        Players.FirstOrDefault(p => p.Value.Id == player.Id);

    public void MovePlayer(CellAddress from, CellAddress to)
    {
        if (Players.TryGetValue(from, out var player))
        {
            player.Address = to;
            RoomChanged?.Invoke();
        }
    }

    public void LeaveRoom(Player player)
    {
        Spectators.Remove(player.Id);
        RoomChanged?.Invoke();
    }

    public void EnterRoom(Player player)
    {
        PlayerViewModel? playerViewModel = EnterRoom_GetPlayer(player);

        if (playerViewModel is null)
        {
            Spectators.TryAdd(player.Id, player);
        }

        RoomChanged?.Invoke();
    }

    private PlayerViewModel? EnterRoom_GetPlayer(Player player)
    {
        var playerKv = GetPlayer(player);
        var playerViewModel = playerKv?.Value;

        if (playerViewModel is null)
        {
            if (Players.Count == 1)
            {
                //Add Player 2
                playerViewModel = new PlayerViewModel(player);
                playerViewModel.Address = CellAddress.Player2StartPosition;
                playerViewModel.Color = Color.Red;
                playerViewModel.Tags = [PlayerViewModel.Player2Tag];
                Players.Add(playerViewModel.Address, playerViewModel);
            }
            if (Players.Count == 0)
            {
                //Add Player 1 
                playerViewModel = new PlayerViewModel(player);
                playerViewModel.Address = CellAddress.Player1StartPosition;
                playerViewModel.Color = Color.Blue;
                playerViewModel.Tags = [PlayerViewModel.Player1Tag];
                Players.Add(playerViewModel.Address, playerViewModel);
                CurrentPlayer = playerViewModel;
            }
        }

        return playerViewModel;
    }
}
