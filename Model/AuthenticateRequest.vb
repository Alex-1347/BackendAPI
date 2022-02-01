Imports System.ComponentModel.DataAnnotations

Namespace Model
    Public Class AuthenticateRequest
        <Required>
        Public Property Username As String
        <Required>
        Public Property Password As String
    End Class
End Namespace