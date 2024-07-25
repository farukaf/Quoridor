namespace Quoridor.ViewModels.Board;

public record struct Victory(DateTimeOffset Date, string PlayerName, Guid PlayerId);