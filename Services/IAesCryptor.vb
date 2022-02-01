Namespace Services
    Public Interface IAesCryptor
        Function EncryptString(ByVal Text As String, ByVal KeyString As String) As String
        Function DecryptString(ByVal CipherText As String, ByVal KeyString As String) As String
        Function DecryptSqlConnection(CN As String, MaskedPass As String) As String
    End Interface

End Namespace
