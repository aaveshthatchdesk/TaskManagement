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

        //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        //{


        //    try
        //    {
        //        var token = await _localStorage.GetItemAsStringAsync("authToken");


        //        if (string.IsNullOrWhiteSpace(token))
        //        {

        //            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //        }

        //        var handler = new JwtSecurityTokenHandler();
        //        var jwt = handler.ReadJwtToken(token);


        //        var claims = jwt.Claims.ToList();

        //        claims = FixRoleClaims(claims);

        //        var identity = new ClaimsIdentity(claims, "jwtAuthType");
        //        var user = new ClaimsPrincipal(identity);

        //        return new AuthenticationState(user);
        //    }
        //    catch(Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        await _localStorage.RemoveItemAsync("authToken");
        //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //    }
        //}
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return Anonymous();
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var claims = new List<Claim>();

                foreach (var claim in jwt.Claims)
                {
                    if (claim.Type == "role" ||
                        claim.Type == ClaimTypes.Role ||
                        claim.Type.EndsWith("/role"))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, claim.Value));
                    }
                    else if (claim.Type == "name" || claim.Type == ClaimTypes.Name)
                    {
                        claims.Add(new Claim(ClaimTypes.Name, claim.Value));
                    }
                    else
                    {
                        claims.Add(claim);
                    }
                }

                // 🔥 ENSURE Name exists
                if (!claims.Any(c => c.Type == ClaimTypes.Name))
                {
                    var email = claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
                    if (email != null)
                        claims.Add(new Claim(ClaimTypes.Name, email));
                }

                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch
            {
                await _localStorage.RemoveItemAsync("authToken");
                return Anonymous();
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
        //public async System.Threading.Tasks.Task NotifyUserAuthenticationAsync()
        //{


        //    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        //}
        public void NotifyUserAuthentication()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout()
        {
            //var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            //NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(anonymous)));
            NotifyAuthenticationStateChanged(System.Threading .Tasks.Task.FromResult(Anonymous()));
        }
        private AuthenticationState Anonymous()
       => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
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
