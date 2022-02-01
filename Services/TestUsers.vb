Imports BackendAPI.Model

Namespace Services
    Public Class Users
        Friend Shared ReadOnly UsersList As New List(Of ApplicationUser) From {
                                                            New ApplicationUser With {
                                                                    .Id = 1,
                                                                    .FirstName = "Test",
                                                                    .LastName = "User",
                                                                    .Username = "test",
                                                                    .Password = "test"
                                                            }}
    End Class
End Namespace