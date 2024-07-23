using System.Collections.Concurrent;
using Quoridor.ViewModels.Board;

namespace Quoridor.Services;

public interface IRoomService
{
    RoomViewModel GetRoom(Guid id);
}

public class RoomService : IRoomService
{
    private ConcurrentDictionary<Guid, RoomViewModel> Rooms { get; set; } = new();

    private RoomViewModel CreateRoom()
    {
        var room = new RoomViewModel();
        Rooms.TryAdd(room.Id, room);
        return room;
    }

    public RoomViewModel GetRoom(Guid id)
    {
        if (Rooms.TryGetValue(id, out var room))
            return room;
        room = CreateRoom();
        Rooms.TryAdd(id, room);
        return room;
    }
}
