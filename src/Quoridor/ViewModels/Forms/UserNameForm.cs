using System.ComponentModel.DataAnnotations;

namespace Quoridor.ViewModels.Forms;

public record UserNameForm
{
    [Required]
    public string UserName { get; set; } = string.Empty;
}