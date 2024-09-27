using Arch.Consts;
using Arch.Frags;
using Arch.Repositories;
using CSharpFunctionalExtensions;
using Hyperlinq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arch.Controllers;

[Controller]
public class IndexController(UserRepository userRepository) : ControllerBase
{
    [HttpGet]
    [Route(Routes.Root)]
    public Task<Microsoft.AspNetCore.Http.IResult> GetIndex() =>
        userRepository
            .GetUsers()
            .Map(us => us.Select(u => H.li($"{u.Id} {u.UserName}")))
            .Map(users => Frag.Layout(User, H.h2("Index"), users).ToResult())
            .Finally(result => result.Value);

    [HttpGet]
    [Authorize]
    [Route("/protected")]
    public Microsoft.AspNetCore.Http.IResult GetProtected() =>
        Frag.Layout(User, H.h2("protected")).ToResult();
}
