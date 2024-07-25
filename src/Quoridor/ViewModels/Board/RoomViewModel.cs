using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Quoridor.Helper;
using Quoridor.Services;

namespace Quoridor.ViewModels.Board;

public record RoomViewModel : IDisposable
{
    public RoomViewModel()
    {
        Board = new BoardViewModel(this);
        Board.WallPlacedEvent += WallPlaced_Event;
        PlayerMovedEvent += PlayerMovedEventHandler;
        VictoryEvent += VictoryEventHandler;
    }

    private async Task PlayerMovedEventHandler(PlayerViewModel player, CellAddress oldAddress, CellAddress newAddress)
    {
        var victory = ValidateVictory(player, newAddress);
        if (victory)
        {
            if (VictoryEvent is not null)
                await VictoryEvent.Invoke(player);            
            return;
        }
        ChangePlayerTurn();
    }

    private async Task VictoryEventHandler(PlayerViewModel player)
    {
        Victories.Add(new(DateTimeOffset.UtcNow, player.Name, player.Id));
        ResetBoard();
        if (VictoryFinishedEvent is not null)
            await VictoryFinishedEvent.Invoke();
    }

    public Func<Task>? RoomChanged { get; set; }
    public Func<PlayerViewModel, Task>? VictoryEvent { get; set; }
    public Func<Task>? VictoryFinishedEvent { get; set; }

    /// <summary>
    /// Player, Old Address, New Address
    /// </summary>
    public Func<PlayerViewModel, CellAddress, CellAddress, Task>? PlayerMovedEvent { get; set; }

    public Guid Id { get; internal set; } = Guid.NewGuid();

    public RoomConfigurationViewModel Configuration { get; set; } = new(10);

    public List<Victory> Victories { get; set; } = new();

    public PlayerViewModel? Player1 { get; private set; }
    public PlayerViewModel? Player2 { get; private set; }

    public Dictionary<CellAddress, PlayerViewModel> Players { get; set; } = new();

    public Dictionary<Guid, Player> Spectators { get; set; } = new();

    public BoardViewModel Board { get; set; }

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

    //TODO: Send this to boardvm
    public async Task MoveCurrentPlayer(CellAddress newAddress)
    {
        if (Board.CurrentPlayer is null)
            return;

        var oldAddress = Board.CurrentPlayer.Address;

        Players.Remove(Board.CurrentPlayer.Address);
        Players.Add(newAddress, Board.CurrentPlayer);

        Board.CurrentPlayer.Address = newAddress;

        if (PlayerMovedEvent is not null)
            await PlayerMovedEvent.Invoke(Board.CurrentPlayer, oldAddress, newAddress);
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
                playerViewModel.Color = ColorHelper.Player2Color;
                playerViewModel.WallCount = Configuration.WallsPerPlayer;
                Players.Add(playerViewModel.Address, playerViewModel);
                Player2 = playerViewModel;
            }
            if (Players.Count == 0)
            {
                //Add Player 1 
                playerViewModel = new PlayerViewModel(player);
                playerViewModel.Address = CellAddress.Player1StartPosition;
                playerViewModel.Color = ColorHelper.Player1Color;
                playerViewModel.WallCount = Configuration.WallsPerPlayer;
                Players.Add(playerViewModel.Address, playerViewModel);
                Board.CurrentPlayer = playerViewModel;
                Player1 = playerViewModel;
            }
        }

        return playerViewModel;
    }

    private void ResetBoard()
    {
        Board.Reset();
        Players.Clear();

        if (Player1 is not null)
        {
            Player1.Address = CellAddress.Player1StartPosition;
            Player1.WallCount = Configuration.WallsPerPlayer;
            Players.Add(Player1.Address, Player1);
        }

        if (Player2 is not null)
        {
            Player2.Address = CellAddress.Player2StartPosition;
            Player2.WallCount = Configuration.WallsPerPlayer;
            Players.Add(Player2.Address, Player2);
        }

        Board.CurrentPlayer = Player1;
    }

    private bool ValidateVictory(PlayerViewModel player, CellAddress newAddress)
    {
        var cell = Board.GetCell(newAddress);
        return cell is not null && cell.VictoryCondition.PlayerColors.Contains(player.Color);        
    }

    private void ChangePlayerTurn()
    {
        if (Board.CurrentPlayer is null)
            return;

        Board.CurrentPlayer = GetOponentFromCurrent();
    }

    //TODO: Change this when make for more than 2 players
    private PlayerViewModel? GetOponentFromCurrent()
    {
        switch (Board.CurrentPlayer)
        {
            case { } player when player == Player1:
                return Player2;
            case { } player when player == Player2:
                return Player1;
        }

        return null;
    }

    private Task WallPlaced_Event()
    {
        if (Board.CurrentPlayer is null)
            return Task.CompletedTask;

        Board.CurrentPlayer.WallCount--;
        ChangePlayerTurn();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Board.WallPlacedEvent -= WallPlaced_Event;
        Players?.Clear();
        Players?.TrimExcess();
        Spectators?.Clear();
        Spectators?.TrimExcess();
    }
}
