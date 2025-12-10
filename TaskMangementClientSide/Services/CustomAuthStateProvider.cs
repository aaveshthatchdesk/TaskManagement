using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TaskMangementClientSide.Services
{
    public class CustomAuthStateProvider:AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                // No token → user anonymous
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // Extract all claims (including Role)
                var claims = jwt.Claims.ToList();

                // IMPORTANT: Convert "role" claim to ClaimTypes.Role (needed by IsInRole)
                claims = FixRoleClaims(claims);

                var identity = new ClaimsIdentity(claims, "jwtAuthType");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                // Token invalid → remove it
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthentication(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var claims = jwt.Claims.ToList();
            claims = FixRoleClaims(claims);

            var identity = new ClaimsIdentity(claims, "jwtAuthType");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(anonymous)));
        }

        private List<Claim> FixRoleClaims(List<Claim> claims)
        {
            var fixedClaims = new List<Claim>();

            foreach (var claim in claims)
            {
                if (claim.Type == "role" ||
                    claim.Type == ClaimTypes.Role ||
                    claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                {
                    fixedClaims.Add(new Claim(ClaimTypes.Role, claim.Value));
                }
                else
                {
                    fixedClaims.Add(claim);
                }
            }

            return fixedClaims;
        }
    }
}
