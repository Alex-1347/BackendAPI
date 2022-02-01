Namespace Model
    Public Class AuthenticateResponse
        Public Property Id As Integer
        Public Property FirstName As String
        Public Property LastName As String
        Public Property Username As String
        Public Property Token As String

        Public Sub New(ByVal CrUser As ApplicationUser, ByVal CrToken As String)
            Id = CrUser.Id
            FirstName = CrUser.FirstName
            LastName = CrUser.LastName
            Username = CrUser.Username
            Token = CrToken
        End Sub
    End Class
End Namespace
