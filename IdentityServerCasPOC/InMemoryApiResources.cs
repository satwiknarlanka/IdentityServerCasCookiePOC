using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdentityServerCasPOC
{
    public class InMemoryApiResources
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("MyRestApi")
                {
                    Name = "MyRestApi",
                    ApiSecrets = { new Secret("secret".Sha256()) }
                }
            };
        }
    }
}
