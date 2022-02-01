Imports Microsoft.EntityFrameworkCore

Namespace Model

    Public Class ApplicationDbContext
        Inherits DbContext

        Public Sub New(options As DbContextOptions(Of ApplicationDbContext))
            MyBase.New(options)
        End Sub

        'Public Property Servers As DbSet(Of Servers)

    End Class
End Namespace