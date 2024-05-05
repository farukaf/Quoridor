using System.Collections.Concurrent;
using Quoridor.ViewModel;

namespace Quoridor.Services;

public class RoomService
{
    public RoomService()
    {
        Rooms = new();
    }

    private ConcurrentDictionary<Guid, RoowViewModel> Rooms { get; set; }

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
