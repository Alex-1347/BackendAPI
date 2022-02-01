
Imports System.Data
Imports System.Data.Common
Imports BackendAPI.Model
Imports Microsoft.EntityFrameworkCore

Namespace Helper
    Public Module RawSqlQuery
        <Runtime.CompilerServices.Extension>
        Public Function RawSqlQuery(Of T)(Context As ApplicationDbContext, ByVal SqlQuery As String, ByVal Map As Func(Of DbDataReader, T)) As List(Of T)

            Using Command = Context.Database.GetDbConnection().CreateCommand()
                Command.CommandText = SqlQuery
                Command.CommandType = CommandType.Text
                Context.Database.OpenConnection()

                Using RDR = Command.ExecuteReader()
                    Dim ResultList = New List(Of T)()

                    While RDR.Read()
                        ResultList.Add(Map(RDR))
                    End While

                    Return ResultList
                End Using
            End Using

        End Function

    End Module
End Namespace