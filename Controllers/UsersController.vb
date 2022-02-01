Imports BackendAPI.Helper
Imports BackendAPI.Model
Imports BackendAPI.Services
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.Logging

Namespace WebApi.Controllers
    <ApiController>
    <Route("[controller]")>
    Public Class UsersController
        Inherits ControllerBase

        Private ReadOnly _UserService As IUserService
        Private ReadOnly _DB As ApplicationDbContext
        Private ReadOnly _Log As ILogger(Of UsersController)
        Private ReadOnly _Trace As Boolean
        Private ReadOnly _WithResult As Boolean


        Public Sub New(ByVal UserService As IUserService, DbContext As ApplicationDbContext, Logger As ILogger(Of UsersController), Configuration As IConfiguration)
            _UserService = UserService
            _DB = DbContext
            _Log = Logger
            _Trace = Configuration.GetValue(Of Boolean)("TraceAPI:Trace")
            _WithResult = Configuration.GetValue(Of Boolean)("TraceAPI:Trace")
        End Sub

        <HttpPost("authenticate")>
        Public Function Authenticate(ByVal Model As AuthenticateRequest) As IActionResult
            'Dim UserList = _DB.RawSqlQuery(Of ApplicationUser)("Select * from User", Function(X) New ApplicationUser With {.Username = X.Item("Name")})
            Dim Response = _UserService.Authenticate(Model)
            If Response Is Nothing Then Return BadRequest(New With {Key .message = "Username or password is incorrect"})
            Return Ok(Response)
        End Function

        <Jwt.Authorize>
        <HttpGet>
        Public Function GetAll() As IActionResult
            Dim Users = _UserService.GetAll
            Return Ok(Users)
        End Function
    End Class
End Namespace