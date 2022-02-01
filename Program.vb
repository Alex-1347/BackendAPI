Imports Microsoft.AspNetCore.Hosting
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging

Public Class Program
    Public Shared Sub Main(ByVal args As String())
        CreateHostBuilder(args).
            Build.
            Run
    End Sub

    Public Shared Function CreateHostBuilder(ByVal args As String()) As IHostBuilder
        Return Host.
                CreateDefaultBuilder(args).
                ConfigureLogging(Sub(hostingContext, logging)
                                     logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"))
                                     logging.AddConsole()
                                     logging.AddDebug()
                                     logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug)
                                     logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug)
                                 End Sub).
                ConfigureWebHostDefaults(Sub(webBuilder)
                                             webBuilder.
                                                UseStartup(Of Startup).
                                                UseUrls("http://localhost:4000")
                                         End Sub)
    End Function
End Class






