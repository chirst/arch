using System.Security.Policy;
using Arch.Consts;
using Arch.Frags;
using CSharpFunctionalExtensions;
using Hyperlinq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualBasic;

namespace Arch.Controllers;

[Controller]
public class AccountController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager
) : ControllerBase
{
    [HttpGet]
    [Route(Routes.Login)]
    public Microsoft.AspNetCore.Http.IResult GetLogin(int? e = null) =>
        Frag.Layout(
                User,
                H.h2("login"),
                H.form(
                    f => f.method("POST").action(Routes.Login),
                    H.input(a =>
                        a.type("text").name("username").required(true)
                    ),
                    H.input(a =>
                        a.type("password").name("password").required(true)
                    ),
                    H.input(a => a.type("submit").value("Login"))
                ),
                e switch
                {
                    1 => H.div("User is locked out"),
                    2 => H.div("User is not allowed to sign in"),
                    _ => H.div("Username or password is incorrect"),
                }
            )
            .ToHtmlResponse();

    [HttpPost]
    [Route(Routes.Login)]
    public async Task<IActionResult> OnLoginAsync(string? returnUrl = null)
    {
        var res = await signInManager.PasswordSignInAsync(
            Request.Form.Single(e => e.Key == "username").Value.ToString(),
            Request.Form.Single(e => e.Key == "password").Value.ToString(),
            isPersistent: true,
            lockoutOnFailure: true
        );
        if (res.Succeeded)
            return RedirectPermanent(returnUrl ?? Routes.Root);
        return RedirectPermanent(
            QueryHelpers.AddQueryString(
                Routes.Login,
                "e",
                res switch
                {
                    { IsLockedOut: true } => "1",
                    { IsNotAllowed: true } => "2",
                    _ => "3",
                }
            )
        );
    }

    [HttpGet]
    [Route(Routes.Register)]
    public Microsoft.AspNetCore.Http.IResult GetRegister(
        string? error = null
    ) =>
        Frag.Layout(
                User,
                H.h2("register"),
                H.form(
                    f => f.method("POST").action(Routes.Register),
                    H.input(a =>
                        a.type("text").name("username").required(true)
                    ),
                    H.input(a =>
                        a.type("password").name("password").required(true)
                    ),
                    H.input(a => a.type("submit").value("Register"))
                ),
                error is null ? H.span() : H.div(error)
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
            ? RedirectPermanent(Routes.Login)
            : RedirectPermanent(
                QueryHelpers.AddQueryString(
                    Routes.Register,
                    "error",
                    res.Errors.First().Description
                )
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
