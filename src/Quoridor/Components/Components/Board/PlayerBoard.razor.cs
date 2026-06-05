using Microsoft.AspNetCore.Components;
using Quoridor.Services;
using Quoridor.ViewModels.Board;

namespace Quoridor.Components.Components.Board;

public partial class PlayerBoard
{
    [Parameter]
    public RoomViewModel Room { get; set; } = default!;

    protected Player? player;
    protected List<string> VictoryBoardShow = new();

    protected override async Task OnInitializedAsync()
    {
        player ??= await playerService.GetPlayer();
    }

    protected string ShowDatetime(DateTimeOffset date)
    {
        var timezone = player?.Timezone ?? TimeZoneInfo.Utc.Id;
        var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezone);
        var formatedDate = TimeZoneInfo.ConvertTime(date, timezoneInfo);

        return formatedDate.ToString();
    }
}
