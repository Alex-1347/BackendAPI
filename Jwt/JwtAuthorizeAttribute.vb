Imports BackendAPI.Model
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc
Imports Microsoft.AspNetCore.Mvc.Filters
Imports System

Namespace Jwt

    <AttributeUsage(AttributeTargets.[Class] Or AttributeTargets.Method)>
    Public Class AuthorizeAttribute
        Inherits Attribute
        Implements IAuthorizationFilter

        Public Sub OnAuthorization(ByVal Context As AuthorizationFilterContext) Implements IAuthorizationFilter.OnAuthorization
            Dim CurUser = CType(Context.HttpContext.Items("User"), ApplicationUser)

            If CurUser Is Nothing Then
                Context.Result = New JsonResult(New With {
                                            Key .message = "Unauthorized"
            }) With {
                .StatusCode = StatusCodes.Status401Unauthorized
            }
            End If
        End Sub

    End Class

End Namespace