using Arch.Consts;
using Arch.Frags;
using CSharpFunctionalExtensions;
using Hyperlinq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Arch.Controllers;

[Controller]
public class AccountController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager
) : ControllerBase
{
    [HttpGet]
    [Route(Routes.Login)]
    public Microsoft.AspNetCore.Http.IResult GetLogin() =>
        Frag.Layout(
                User,
                H.h2("login"),
                H.form(
                    f => f.method("POST").action(Routes.Login),
                    H.input(a => a.type("text").name("username")),
                    H.input(a => a.type("password").name("password")),
                    H.input(a => a.type("submit").value("Login"))
                )
            )
            .ToHtmlResponse();

    [HttpPost]
    [Route(Routes.Login)]
    public async Task<IActionResult> OnLoginAsync(string? returnUrl = null) =>
        (
            await signInManager.PasswordSignInAsync(
                Request.Form.Single(e => e.Key == "username").Value.ToString(),
                Request.Form.Single(e => e.Key == "password").Value.ToString(),
                false,
                false
            )
        ).Succeeded
            ? RedirectPermanent(returnUrl ?? Routes.Root)
            : throw new Exception($"failed to login");

    [HttpGet]
    [Route(Routes.Register)]
    public Microsoft.AspNetCore.Http.IResult GetRegister() =>
        Frag.Layout(
                User,
                H.h2("register"),
                H.form(
                    f => f.method("POST").action(Routes.Register),
                    H.input(a => a.type("text").name("username")),
                    H.input(a => a.type("password").name("password")),
                    H.input(a => a.type("submit").value("Register"))
                )
            )
            .ToHtmlResponse();

    [HttpPost]
    [Route(Routes.Register)]
    public async Task<IActionResult> OnRegisterAsync()
    {
        var res = await userManager.CreateAsync(
            new IdentityUser(
                Request.Form.Single(e => e.Key == "username").Value.ToString()
            ),
            Request.Form.Single(e => e.Key == "password").Value.ToString()
        );
        return res.Succeeded
            ? Redirect(Routes.Login)
            : throw new Exception(
                string.Join(", ", res.Errors.Select(e => e.Description))
            );
    }

    [HttpPost]
    [Route(Routes.Logout)]
    public async Task<IActionResult> OnLogoutAsync()
    {
        await signInManager.SignOutAsync();
        return LocalRedirect(Routes.Root);
    }
}
