Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Namespace Services


    Public Class AesCryptor
        Implements IAesCryptor

        Public Function EncryptString(Text As String, KeyString As String) As String Implements IAesCryptor.EncryptString
            Dim Key As Byte() = Encoding.UTF8.GetBytes(KeyString)
            ReDim Preserve Key(31)
            Using AesAlg = Aes.Create()
                Using Encryptor = AesAlg.CreateEncryptor(Key, AesAlg.IV)
                    Using MsEncrypt = New MemoryStream()
                        Using CsEncrypt = New CryptoStream(MsEncrypt, Encryptor, CryptoStreamMode.Write)
                            Using SwEncrypt = New StreamWriter(CsEncrypt)
                                SwEncrypt.Write(Text)
                            End Using
                        End Using
                        Dim IV = AesAlg.IV
                        Dim DecryptedContent = MsEncrypt.ToArray()
                        Dim Result = New Byte(IV.Length + DecryptedContent.Length - 1) {}
                        Buffer.BlockCopy(IV, 0, Result, 0, IV.Length)
                        Buffer.BlockCopy(DecryptedContent, 0, Result, IV.Length, DecryptedContent.Length)
                        Return Convert.ToBase64String(Result)
                    End Using
                End Using
            End Using
        End Function

        Public Function DecryptString(CipherText As String, KeyString As String) As String Implements IAesCryptor.DecryptString
            Try
                Dim FullCipher = Convert.FromBase64String(CipherText)
                Dim IV = New Byte(15) {}
                Dim Cipher = New Byte(15) {}
                Buffer.BlockCopy(FullCipher, 0, IV, 0, IV.Length)
                Buffer.BlockCopy(FullCipher, IV.Length, Cipher, 0, IV.Length)
                Dim Key = Encoding.UTF8.GetBytes(KeyString)
                ReDim Preserve Key(31)
                Using AesAlg = Aes.Create()
                    Using Decryptor = AesAlg.CreateDecryptor(Key, IV)
                        Dim Result As String
                        Using MsDecrypt = New MemoryStream(Cipher)
                            Using CsDecrypt = New CryptoStream(MsDecrypt, Decryptor, CryptoStreamMode.Read)
                                Using SrDecrypt = New StreamReader(CsDecrypt)
                                    Result = SrDecrypt.ReadToEnd()
                                End Using
                            End Using
                        End Using
                        Return Result
                    End Using
                End Using
            Catch ex As CryptographicException
                Return "Wrong password"
            End Try
        End Function

        Friend Function DecryptSqlConnection(CN As String, MaskedPass As String) As String Implements IAesCryptor.DecryptSqlConnection
            Dim Dercryptor As New AesCryptor
            Dim Bytes = System.Convert.FromBase64String(MaskedPass)
            Dim TmpPass = Text.Encoding.UTF8.GetString(Bytes)
            Dim ConnectionStringPart As String() = CN.Split(";")
            For i As Integer = 0 To ConnectionStringPart.Length - 1
                If ConnectionStringPart(i).StartsWith("password") Then
                    Dim EncryptedMySQLPass = ConnectionStringPart(i).Replace("password=", "") 'password need to last parm to avoid finishing char - ";"
                    Dim RealDatabasePass As String = DecryptString(EncryptedMySQLPass, TmpPass)
                    ConnectionStringPart(i) = $"password={RealDatabasePass}"
                End If
            Next
            Return String.Join(";", ConnectionStringPart)
        End Function

    End Class

End Namespace