namespace IdentityService.Pages.Account.Register;

using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    [BindProperty]
    public RegisterViewModel Input { get; set; }
    
    [BindProperty]
    public bool RegisterSuccess { get; set; }

    public Index(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult OnGet(string returnUrl = null)
    {
        Input = new RegisterViewModel
        {
            ReturnUrl = returnUrl
        };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (Input.Button is not "register")
            return Redirect("~/");

        if (!ModelState.IsValid)
            return Page();

        if (Input.Password != Input.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Your passwords do not match");
            return Page();
        }

        
        var user = new ApplicationUser
        {
            UserName = Input.Username,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Could not register user");
            return Page();
        }
            
        await _userManager.AddClaimsAsync(user, new Claim[]
        {
            new(JwtClaimTypes.Name, Input.FullName),
        });
        RegisterSuccess = true;


        return Page();
    }
}