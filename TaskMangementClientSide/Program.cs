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
            builder.Services.AddSingleton<SpinnerService>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddScoped<CustomAuthStateProvider>();


        //builder.Services.AddAuthorizationCore();
        //builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();




            builder.Services.AddScoped<AuthHttpClientHandler>();

            builder.Services.AddHttpClient("AuthClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7218/");
            }).AddHttpMessageHandler<AuthHttpClientHandler>();

            //builder.Services.AddHttpClient("AuthClient", client =>
            //{
            //    client.BaseAddress = new Uri("http://aavtan04-001-site1.stempurl.com/");
            //}).AddHttpMessageHandler<AuthHttpClientHandler>();



            builder.Services.AddScoped(sp =>
             sp.GetRequiredService<IHttpClientFactory>().CreateClient("AuthClient"));
            await builder.Build().RunAsync();
        }
    }
}
