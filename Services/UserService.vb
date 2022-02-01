Imports System.IdentityModel.Tokens.Jwt
Imports System.Security.Claims
Imports System.Text
Imports BackendAPI.Model
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.Options
Imports Microsoft.IdentityModel.Tokens
Imports BackendAPI.Helper

Namespace Services

    Public Class UserService
        Implements IUserService


        Private ReadOnly _JwtSettings As Jwt.JwtSettings
        Private ReadOnly _DB As ApplicationDbContext
        Private ReadOnly _Log As ILogger(Of UserService)

        Public Sub New(ByVal AppSettings As IOptions(Of Jwt.JwtSettings), DbContext As ApplicationDbContext, Logger As ILogger(Of UserService))
            _JwtSettings = AppSettings.Value
            _DB = DbContext
            _Log = Logger
        End Sub

        Public Function Authenticate(ByVal Model As AuthenticateRequest) As AuthenticateResponse Implements IUserService.Authenticate
            Dim User = Users.UsersList.SingleOrDefault(Function(x) x.Username = Model.Username AndAlso x.Password = Model.Password)

            'return null if user not found
            If User Is Nothing Then Return Nothing

            'authentication successful so generate jwt token
            Dim Token = GenerateJwtToken(User)
            Return New AuthenticateResponse(User, Token)
        End Function

        Public Function GetAll() As IEnumerable(Of ApplicationUser) Implements IUserService.GetAll
            Return Users.UsersList
        End Function

        Public Function GetById(ByVal id As Integer) As ApplicationUser Implements IUserService.GetById
            'Dim UserList = _DB.RawSqlQuery(Of ApplicationUser)("Select * from User", Function(X) New ApplicationUser With {.Username = X.Item("Name")})
            Return Users.UsersList.FirstOrDefault(Function(x) x.Id = id)
        End Function

        'helper methods
        Private Function GenerateJwtToken(ByVal CrUser As ApplicationUser) As String
            'generate token that is valid for 7 days
            Dim TokenHandler = New JwtSecurityTokenHandler()
            Dim Key = Encoding.ASCII.GetBytes(_JwtSettings.Key)
            Dim TokenDescriptor = New SecurityTokenDescriptor With {
                .Subject = New ClaimsIdentity({New Claim("id", CrUser.Id.ToString())}),
                .Expires = DateTime.UtcNow.AddDays(7),
                .SigningCredentials = New SigningCredentials(New SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature),
                .Issuer = _JwtSettings.Issuer
            }
            Dim Token = TokenHandler.CreateToken(TokenDescriptor)
            Return TokenHandler.WriteToken(Token)
        End Function

    End Class

End Namespace