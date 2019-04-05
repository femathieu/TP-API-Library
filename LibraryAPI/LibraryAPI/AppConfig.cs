using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI
{
    public class AppConfig
    {
        private readonly SymmetricSecurityKey _SecretPassPhrase
            = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ceci est une passphrase securisee personne ne pourra la trouver"));

        public SymmetricSecurityKey getSecretPassPhrase()
        {
            return _SecretPassPhrase;
        }

        public static string getTokenClaims(HttpContext httpContext, String claimName)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            string ret = null;
            if (identity != null)
            {
                ret = identity.FindFirst(claimName).Value;
            }
            return ret;
        }
    }
}
