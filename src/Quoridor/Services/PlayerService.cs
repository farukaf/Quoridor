using Blazored.LocalStorage;

namespace Quoridor.Services;

public interface IPlayerService
{
    EventHandler? PlayerChanged { get; set; }
    Task<Player?> GetPlayer();
    Task<Player> SetUser(string userName);
} 

public record Player(Guid Id, string UserName);

public class PlayerService(ILocalStorageService localStorage) : IPlayerService
{
    const string USER_KEY = "USER_KEY"; 

    public async Task<Player?> GetPlayer() => 
        await localStorage.GetItemAsync<Player>(USER_KEY);

    public async Task<Player> SetUser(string userName)
    {
        var player = new Player(Guid.NewGuid(), userName);
        await localStorage.SetItemAsync(USER_KEY, player);
        PlayerChanged?.Invoke(this, new());
        return player;
    }

    public EventHandler? PlayerChanged { get; set; }
}
