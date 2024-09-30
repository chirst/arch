using Arch.Consts;
using Arch.Frags;
using Arch.Repositories;
using CSharpFunctionalExtensions;
using Hyperlinq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arch.Controllers;

[Controller]
public class IndexController() : ControllerBase
{
    [HttpGet]
    [Route(Routes.Root)]
    public Microsoft.AspNetCore.Http.IResult GetIndex() =>
        Frag.Layout(
                User,
                H.h2("Index"),
                H.a(a => a.href("/protected"), "Go to protected route")
            )
            .ToResult();

    [HttpGet]
    [Authorize]
    [Route("/protected")]
    public Microsoft.AspNetCore.Http.IResult GetProtected() =>
        Frag.Layout(User, H.h2("protected")).ToResult();
}
