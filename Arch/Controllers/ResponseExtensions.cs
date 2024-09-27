using Hyperlinq;

namespace Arch.Controllers;

public static class ResponseExtensions
{
    public static IResult ToResult(this HElement he) =>
        Results.Content(he.ToString(), "text/html");
}
