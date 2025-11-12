using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TaskMangementClientSide.Services;


namespace TaskMangementClientSide
{
    public class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<SprintService>();
            //builder.Services.AddAuthorizationCore();
            //builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();





            builder.Services.AddTransient<AuthHttpClientHandler>();

            builder.Services.AddHttpClient("AuthClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7218/"); // your API base
            }).AddHttpMessageHandler<AuthHttpClientHandler>();

            //builder.Services.AddScoped(sp => new HttpClient
            //{
            //    BaseAddress = new Uri("https://localhost:7218/")
            //});


            builder.Services.AddScoped(sp =>
             sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"));
            await builder.Build().RunAsync();
        }
    }
}
