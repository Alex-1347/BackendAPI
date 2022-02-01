Imports System.Text.Json.Serialization

Namespace Model
    Public Class ApplicationUser
        Public Property Id As Integer
        Public Property FirstName As String
        Public Property LastName As String
        Public Property Username As String
        <JsonIgnore>
        Public Property Password As String
    End Class
End Namespace
