namespace Quoridor.ViewModels.Board;

public abstract record GridElementViewModel
{
    public GridAddress GridAddress { get; set; }

    public abstract string CssClass();
}
