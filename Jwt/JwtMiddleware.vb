Imports BackendAPI.Services
Imports Microsoft.AspNetCore.Http
Imports Microsoft.Extensions.Options
Imports Microsoft.IdentityModel.Tokens
Imports System
Imports System.IdentityModel.Tokens.Jwt
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks


Namespace Jwt
    Public Class JwtMiddleware
        Private ReadOnly _Next As RequestDelegate
        Private ReadOnly _JwtSettings As JwtSettings

        Public Sub New(ByVal NextDelegate As RequestDelegate, ByVal JwtSettings As IOptions(Of JwtSettings))
            _Next = NextDelegate
            _JwtSettings = JwtSettings.Value
        End Sub

        Public Async Function Invoke(ByVal Context As HttpContext, ByVal UserService As IUserService) As Task
            Dim token = Context.Request.Headers("Authorization").FirstOrDefault?.Split(" ").Last
            If token IsNot Nothing Then AttachUserToContext(Context, UserService, token)
            Await _Next(Context)
        End Function

        Private Sub AttachUserToContext(ByVal Context As HttpContext, ByVal UserService As IUserService, ByVal Token As String)
            Dim ValidatedToken As SecurityToken = Nothing

            Try
                Dim TokenHandler = New JwtSecurityTokenHandler
                Dim Key = Encoding.ASCII.GetBytes(_JwtSettings.Key)
                TokenHandler.ValidateToken(Token, New TokenValidationParameters With {
                    .ValidateIssuerSigningKey = True,
                    .IssuerSigningKey = New SymmetricSecurityKey(Key),
                    .ValidateIssuer = True,
                    .ValidateAudience = False,
                    .ValidIssuer = _JwtSettings.Issuer,
                    .ClockSkew = TimeSpan.Zero 'set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                }, ValidatedToken)
                Dim JwtToken = CType(ValidatedToken, JwtSecurityToken)
                Dim UserId = CInt(JwtToken.Claims.First(Function(x) x.Type = "id").Value)
                'attach user to context on successful jwt validation
                Context.Items("User") = UserService.GetById(UserId)
            Catch
                'do nothing if jwt validation fails
                'user Is Not attached to context so request won't have access to secure routes

            End Try
        End Sub
    End Class
End Namespace
