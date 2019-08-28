using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using TaxCash.LoanOrigination.Properties;

namespace TaxCash.LoanOrigination
{
    public class Session : ISession
    {
        private bool isInitialized;
        private string _token;
        private string _userName;

        public Session(IPrincipal principal)
        {
            Principal = principal;
        }

        public string Token
        {
            get
            {
                Initialize();
                return _token;
            }
        }

        public string UserName
        {
            get
            {
                Initialize();
                return _token;
            }
        }

        public IPrincipal Principal { get; }

        private void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            var claimsPrincipal = Principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                throw new InvalidOperationException(Resources.PrincipalMustBeAClaimsPrincipal);
            }
            var claim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.UserData);
            if (claim == null)
            {
                throw new SecurityTokenExpiredException(Resources.ThePrincipalIsMissinmgTheSessionTokenClaim);
            }
            _token = claim.Value;
            claim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            _userName = claim.Value;
            isInitialized = true;
        }
    }
}
