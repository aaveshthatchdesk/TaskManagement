using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

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
            

            try
            {
                var token = await _localStorage.GetItemAsStringAsync("authToken");
                Console.WriteLine($"[AuthState] Token read: {(string.IsNullOrEmpty(token) ? "NULL" : "FOUND")}");

                if (string.IsNullOrWhiteSpace(token))
                {
                    
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                // Extract all claims (including Role)
                var claims = jwt.Claims.ToList();

                // IMPORTANT: Convert "role" claim to ClaimTypes.Role (needed by IsInRole)
                claims = FixRoleClaims(claims);

                var identity = new ClaimsIdentity(claims, "jwtAuthType");
                var user = new ClaimsPrincipal(identity);
                Console.WriteLine($"[AuthState] User authenticated: {user.Identity?.IsAuthenticated}");
                return new AuthenticationState(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _localStorage.RemoveItemAsync("authToken");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void RefreshAuthState()
        {

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }


        //public void NotifyUserAuthentication(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwt = handler.ReadJwtToken(token);

        //    var claims = jwt.Claims.ToList();
        //    claims = FixRoleClaims(claims);

        //    var identity = new ClaimsIdentity(claims, "jwtAuthType");
        //    var user = new ClaimsPrincipal(identity);

        //    NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(user)));
        //}
        public async System.Threading.Tasks.Task NotifyUserAuthenticationAsync()
        {
            await System.Threading.Tasks.Task.Delay(150);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
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
