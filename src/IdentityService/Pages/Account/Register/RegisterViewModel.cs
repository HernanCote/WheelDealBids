namespace IdentityService.Pages.Account.Register;

using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string ConfirmPassword { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    public string FullName { get; set; }
    public string ReturnUrl { get; set; } = null!;
    public string Button { get; set; }
}