using System.Security.Claims;
using RemoteProject.Shared.Abstractions.Exceptions;

namespace RemoteProject.Shared.Abstractions.Helpers;

public static class ClaimsPrincipalExtensions
{
    private const string SubjectClaimType = "sub";
    private const string EmailClaimType = "email";
    private const string NameClaimType = "name";
    private const string PictureClaimType = "picture";
    private const string RoleClaimType = "role";
    private const string RolesClaimType = "roles";

    public static Guid GetUserId(
        this ClaimsPrincipal claimsPrincipal
    )
    {
        var subject = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == SubjectClaimType)
            // And to support Asp.net's incoming claim mapping.
            ?? claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier); 

        if (subject is null || !Guid.TryParse(subject.Value, out var userId))
        {
            throw new KnownUnauthorizedException();
        }

        return userId;
    }

    public static string GetEmail(
        this ClaimsPrincipal principal
    )
    {
        var subject = principal.Claims.FirstOrDefault(c => c.Type == EmailClaimType);

        if (subject is null)
        {
            throw new KnownUnauthorizedException();
        }

        return subject.Value;
    }

    // public static string[] GetRoles(
    //     this ClaimsPrincipal principal
    // )
    // {
    //     var subject = principal.Claims.FirstOrDefault(c => c.Type == RoleClaimType)
    //         ?? principal.Claims.FirstOrDefault(c => c.Type == RolesClaimType);
    //
    //     if (subject is null)
    //     {
    //         throw new MateUnauthorizedException();
    //     }
    //
    //     return subject.Value;
    // }

    public static string GetName(
        this ClaimsPrincipal principal
    )
    {
        var subject = principal.Claims.FirstOrDefault(c => c.Type == NameClaimType);

        if (subject is null)
        {
            throw new KnownUnauthorizedException();
        }

        return subject.Value;
    }

    public static string GetPicture(
        this ClaimsPrincipal principal
    )
    {
        var subject = principal.Claims.FirstOrDefault(c => c.Type == PictureClaimType);

        if (subject is null)
        {
            throw new KnownUnauthorizedException();
        }

        return subject.Value;
    }
}
