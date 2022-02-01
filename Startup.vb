Imports Microsoft.AspNetCore.Builder
Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports BackendAPI.Services
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.OpenApi.Models
Imports Microsoft.Extensions.Hosting
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.AspNetCore.HttpOverrides
Imports Microsoft.Extensions.Logging
Imports BackendAPI.Model

Public Class Startup
    Public Shared Property Environment As IWebHostEnvironment
    Public Shared Property LoggerFactory As ILoggerFactory
    Public ReadOnly Property Configuration As IConfiguration

    Public Sub New(ByVal StartupConfig As IConfiguration, ByVal StartupEnv As IWebHostEnvironment)
        Configuration = StartupConfig
        Environment = StartupEnv
    End Sub

    'add services to the DI container
    Public Sub ConfigureServices(ByVal Services As IServiceCollection)
        Services.AddCors
        Services.AddControllers(Sub(x) x.RespectBrowserAcceptHeader = True)

        Services.AddMvcCore(Sub(x) x.EnableEndpointRouting = False).
            SetCompatibilityVersion(CompatibilityVersion.Latest).
            AddFormatterMappings

        Services.AddSwaggerGen(Sub(x)
                                   x.SwaggerDoc("V2", New OpenApiInfo With {.Title = "Backend API", .Version = "V2"})
                               End Sub)

        Dim AES As New AesCryptor

        Services.AddDbContext(Of ApplicationDbContext)(Function(ByVal options As DbContextOptionsBuilder)
                                                           Return options.UseMySql(AES.DecryptSqlConnection(Configuration.GetConnectionString("DefaultConnection"), "XXXXXXXXXXXXX"),
                                                                                   ServerVersion.Parse("10.5.9-MariaDB-1:10.5.9+maria~xenial"), 'SHOW VARIABLES LIKE "%version%";
                                                                                   Sub(ByVal mySqlOption As Microsoft.EntityFrameworkCore.Infrastructure.MySqlDbContextOptionsBuilder)
                                                                                       mySqlOption.CommandTimeout(10)
                                                                                       mySqlOption.EnableRetryOnFailure(10)
                                                                                   End Sub)
                                                       End Function, ServiceLifetime.Transient, ServiceLifetime.Transient)

        ' configure strongly typed settings object
        Services.Configure(Of Jwt.JwtSettings)(Configuration.GetSection("JwtSetting"))

        'configure DI for application services
        Services.AddScoped(Of IUserService, UserService)
        Services.AddSingleton(Of IAesCryptor, AesCryptor)
    End Sub

    'configure the HTTP request pipeline
    Public Sub Configure(ByVal App As IApplicationBuilder, ByVal Env As IWebHostEnvironment, RequestLoggerFactory As ILoggerFactory)

        LoggerFactory = RequestLoggerFactory

        App.UseForwardedHeaders(New ForwardedHeadersOptions With {
        .ForwardedHeaders = ForwardedHeaders.XForwardedFor Or ForwardedHeaders.XForwardedProto
    })

        If Env.IsDevelopment() Then App.UseDeveloperExceptionPage()

        App.UseSwagger
        App.UseSwaggerUI(Sub(x)
                             x.SwaggerEndpoint("/swagger/V2/swagger.json", "Backend API V2") ' Notice the lack of / making it relative
                             'x.RoutePrefix = "CS" 'This Is the reverse proxy address
                         End Sub)
        App.UseRouting

        'global cors policy
        App.UseCors(Function(x)
                        Return x.
                        AllowAnyOrigin.
                        AllowAnyMethod.
                        AllowAnyHeader
                    End Function)

        'custom jwt auth middleware
        App.UseMiddleware(Of Jwt.JwtMiddleware)

        App.UseEndpoints(Function(x) x.MapControllers)
    End Sub
End Class
