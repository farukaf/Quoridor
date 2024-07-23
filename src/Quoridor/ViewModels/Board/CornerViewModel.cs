namespace Quoridor.ViewModels.Board;

public record CornerViewModel : GridElementViewModel
{
    public CornerAddress CornerAddress { get; set; }

    public override string CssClass()
    {
        return "corner";
    }
}
