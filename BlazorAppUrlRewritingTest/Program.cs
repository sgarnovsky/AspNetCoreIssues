using BlazorAppUrlRewritingTest.Components;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;

namespace BlazorAppUrlRewritingTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents();

            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddReverseProxy()
              .LoadFromConfig(configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var rewriteOptions = new RewriteOptions()
                // static cache folder rewrite rule
                .Add(UrlRewritingUtility.RewriteCachedStaticFilePathRequests);
            app.UseRewriter(rewriteOptions);

            app.UseStaticFiles();

            // https://github.com/microsoft/reverse-proxy/issues/2393
            // if a routing is not set up then the StaticFiles mapping doesn't work together with the used Forwarder
            app.UseRouting();

            app.UseAntiforgery();

            app.MapRazorComponents<App>();

            app.MapReverseProxy();

            app.Run();
        }
    }
}
