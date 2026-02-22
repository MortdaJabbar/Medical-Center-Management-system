namespace MCMSAPI.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class OwnershipHandler
        : AuthorizationHandler<OwnershipRequirement, Guid>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnershipRequirement requirement,
            Guid resourcePersonId)
        {

            if (!context.User.Identity?.IsAuthenticated ?? true)
                return Task.CompletedTask;
            var claim = context.User.FindFirst("personId");

            if (claim == null)
                return Task.CompletedTask;

            if (!Guid.TryParse(claim.Value, out Guid authenticatedPersonId))
                return Task.CompletedTask;

            if (authenticatedPersonId == resourcePersonId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
