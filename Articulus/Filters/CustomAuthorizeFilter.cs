using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Articulus.Data;
using Articulus.DTOs.Users;
using Articulus.Data.Models;
using System.Linq;

namespace Articulus.Filters
{
    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly AppDbContext _dbContext;

        public CustomAuthorizeFilter(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // If the action/controller allows anonymous access, skip this filter.
            var endpointMetadata = context.ActionDescriptor?.EndpointMetadata;
            if (endpointMetadata != null && endpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            {
                return;
            }

            var principal = context.HttpContext.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userIdString = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Try FindAsync (PK lookup). If that fails, fall back to a predicate lookup.
            var dbUser = await _dbContext.Users.FindAsync(userId);

            if (dbUser == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userJwtClaims = new UserJwtClaims
            {
                UserId = userId,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                TimeZone = dbUser.TimeZone
            };
            context.HttpContext.Items["UserJwtClaims"] = userJwtClaims;
        }
    }
}


