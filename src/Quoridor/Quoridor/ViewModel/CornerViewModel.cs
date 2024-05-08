namespace Quoridor.ViewModel;

public record CornerViewModel : GridElementViewModel
{
    public CornerAddress CornerAddress { get; set; }

    public override string CssClass()
    {
        return "corner";
    }
}
