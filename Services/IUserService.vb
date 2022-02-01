Imports BackendAPI.Model

Namespace Services
    Public Interface IUserService
        Function Authenticate(ByVal Model As AuthenticateRequest) As AuthenticateResponse
        Function GetAll() As IEnumerable(Of ApplicationUser)
        Function GetById(ByVal id As Integer) As ApplicationUser
    End Interface

End Namespace


