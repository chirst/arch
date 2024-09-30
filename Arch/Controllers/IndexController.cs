using Arch.Consts;
using Arch.Frags;
using Arch.Repositories;
using CSharpFunctionalExtensions;
using Hyperlinq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Arch.Controllers;

[Controller]
public class IndexController(
    UserRepository userRepository,
    UserManager<IdentityUser> userManager
) : ControllerBase
{
    [HttpGet]
    [Route(Routes.Root)]
    public Microsoft.AspNetCore.Http.IResult GetIndex() =>
        Frag.Layout(
                User,
                H.h2("Index"),
                H.a(a => a.href("/protected"), "Go to protected route")
            )
            .ToHtmlResponse();

    [HttpGet]
    [Authorize]
    [Route("/protected")]
    public async Task<Microsoft.AspNetCore.Http.IResult> GetProtected(
        CancellationToken cancellationToken
    ) =>
        await userRepository
            .FindByIdAsync(
                Maybe.From(userManager.GetUserId(User)).GetValueOrThrow(),
                cancellationToken
            )
            .ToResultAsync("user not found")
            .Map(user =>
                Frag.Layout(
                    User,
                    H.h2("protected"),
                    H.div($"ID: {user.Id}"),
                    H.div($"Username: {user.UserName}"),
                    H.div($"Normalized Username: {user.NormalizedUserName}"),
                    H.div($"Password Hash: {user.PasswordHash}")
                )
            )
            .Finally(r =>
                r.IsSuccess
                    ? r.Value.ToHtmlResponse()
                    : Frag.Layout(User, H.h2("protected"), H.div(r.Error))
                        .ToHtmlResponse()
            );
}
