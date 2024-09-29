using System.Security.Claims;
using Arch.Consts;
using Hyperlinq;

namespace Arch.Frags;

public static partial class Frag
{
    public static HElement Layout(
        ClaimsPrincipal claims,
        params object[] children
    ) =>
        H.html(
            H.head(H.link(a => a.rel("stylesheet").href("/index.css"))),
            H.body(
                H.span(
                    a => a.css("header"),
                    H.a(a => a.href(Routes.Root), H.h1("Arch")),
                    H.span(
                        a => a.css("header-right"),
                        claims.Identity?.IsAuthenticated ?? false
                            ? H.form(
                                a => a.action(Routes.Logout).method("POST"),
                                H.input(a => a.type("submit").value("Logout"))
                            )
                            : new[]
                            {
                                H.a(a => a.href(Routes.Login), "login"),
                                H.a(a => a.href(Routes.Register), "register"),
                            }
                    )
                ),
                H.span(children)
            )
        );
}
