namespace Quoridor.ViewModel;

public abstract record GridElementViewModel
{
    public GridAddress GridAddress { get; set; }

    public abstract string CssClass();
}
