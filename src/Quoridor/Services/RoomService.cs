using System.Collections.Concurrent;
using Quoridor.ViewModels.Board;

namespace Quoridor.Services;

public interface IRoomService
{
    RoowViewModel GetRoom(Guid id);
}

public class RoomService : IRoomService
{
    private ConcurrentDictionary<Guid, RoowViewModel> Rooms { get; set; } = new();

    private RoowViewModel CreateRoom()
    {
        var room = new RoowViewModel();
        Rooms.TryAdd(room.Id, room);
        return room;
    }

    public RoowViewModel GetRoom(Guid id)
    {
        if (Rooms.TryGetValue(id, out var room))
            return room;
        room = CreateRoom();
        Rooms.TryAdd(id, room);
        return room;
    }
}
