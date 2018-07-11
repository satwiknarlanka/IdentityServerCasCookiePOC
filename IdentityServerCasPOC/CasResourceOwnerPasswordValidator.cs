using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace IdentityServerCasPOC
{
    public class CasResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (context.UserName != null)
            {
                context.Result = new GrantValidationResult(context.UserName, authenticationMethod: "custom");
            }
            else
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
        }
    }
}
