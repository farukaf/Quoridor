using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace Quoridor.Services;

public interface IPlayerService
{
    Func<Task>? PlayerChanged { get; set; }
    Task<Player?> GetPlayer();
    Task<Player> SetUser(string userName);
} 

public record Player(Guid Id, string UserName, string Timezone);

public class PlayerService(ILocalStorageService localStorage, IJSRuntime jsRuntime) : IPlayerService
{
    const string USER_KEY = "USER_KEY"; 

    public async Task<Player?> GetPlayer() => 
        await localStorage.GetItemAsync<Player>(USER_KEY);

    public async Task<Player> SetUser(string userName)
    {
        var timezone = await jsRuntime.InvokeAsync<string>("GetTimezone");
        var player = new Player(Guid.NewGuid(), userName, timezone);
        await localStorage.SetItemAsync(USER_KEY, player);
        PlayerChanged?.Invoke();
        return player;
    }

    public Func<Task>? PlayerChanged { get; set; }
}
