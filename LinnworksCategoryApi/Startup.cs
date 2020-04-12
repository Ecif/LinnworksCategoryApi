using System.Net.Http;
using LinnworksCategoryApi.Config;
using LinnworksCategoryApi.Headers;
using LinnworksCategoryApi.HttpClients;
using LinnworksCategoryApi.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LinnworksCategoryApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddControllers(options =>
            {
                options.Filters.Add<OperationCancelledExceptionFilter>();
            }).AddJsonOptions(options =>
            {
                // Use the default property (Pascal) casing.
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddCors(options =>
                {
                    options.AddPolicy(name: "MyAllowedSpecificOrigins",
                        builder =>
                        {
                            builder.WithOrigins("http://localhost:8080", "https://localhost:8080")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                }
            );

            services.AddHttpClient();

            services.AddTransient<ICategoryApiClient, CategoryApiClient>();
            services.AddTransient<IDashboardApiClient, DashboardApiClient>();

            services.AddSingleton<IConfigurator<HttpContext, HttpRequestMessage>, RequestHeadersConfigurator>();
            services.AddSingleton<IConfigurator<HttpResponseMessage, HttpResponse>, ResponseHeadersConfigurator>();

            services.Configure<CategoryApiConfig>(Configuration.GetSection("CategoryApi"));
            services.Configure<DashboardApiConfig>(Configuration.GetSection("DashboardApi"));
            services.Configure<AuthConfig>(Configuration.GetSection("Authorization"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //TODO self-authorization token.

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error-Develop");
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("MyAllowedSpecificOrigins");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
