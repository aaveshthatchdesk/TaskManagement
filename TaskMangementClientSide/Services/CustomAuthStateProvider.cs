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
            var identity = new ClaimsIdentity();

            if (!string.IsNullOrWhiteSpace(token))
            {
                identity = new ClaimsIdentity(new[]
              {
                    new Claim(ClaimTypes.Name, "User")
                }, "jwtAuthType");
                //try
                //{
                //    var handler = new JwtSecurityTokenHandler();
                //    var jwt = handler.ReadJwtToken(token);

                //    var claims = jwt.Claims.ToList();
                //    identity = new ClaimsIdentity(claims, "jwtAuthType");
                //}
                //catch
                //{
                //    await _localStorage.RemoveItemAsync("authToken");
                //}
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwtAuthType");

            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(System.Threading.Tasks.Task.FromResult(new AuthenticationState(anonymous)));
        }
    }
}
