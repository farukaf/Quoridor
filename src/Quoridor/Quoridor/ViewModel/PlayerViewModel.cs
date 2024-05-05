using Quoridor.Helper;

namespace Quoridor.ViewModel;

public record PlayerViewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Color Color { get; set; }
    public CellAddress Address { get; set; } = new();
    public string Name { get; set; } = string.Empty;
}
