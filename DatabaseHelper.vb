Imports System
Imports System.Web
Imports System.Configuration

''' <summary>
''' Helper para obtener la cadena de conexión correcta según la empresa seleccionada en sesión
''' </summary>
Public Class DatabaseHelper

    ''' <summary>
    ''' Obtiene la cadena de conexión basada en la empresa seleccionada en la sesión
    ''' Si no hay empresa seleccionada, usa la conexión por defecto (DaytonaDB)
    ''' </summary>
    Public Shared Function GetConnectionString() As String
        ' Intentar obtener el nombre de la connection string desde la sesión
        Dim connStringName As String = TryCast(HttpContext.Current.Session("ConnectionStringName"), String)

        ' Si no hay empresa seleccionada, usar la conexión por defecto
        If String.IsNullOrWhiteSpace(connStringName) Then
            connStringName = "DaytonaDB"
        End If

        ' Obtener la connection string del Web.config
        Dim cs = ConfigurationManager.ConnectionStrings(connStringName)

        If cs Is Nothing Then
            ' Fallback a la conexión por defecto si no se encuentra la configurada
            cs = ConfigurationManager.ConnectionStrings("DaytonaDB")
        End If

        If cs Is Nothing Then
            Throw New Exception("No se encontró ninguna cadena de conexión configurada en Web.config")
        End If

        Return cs.ConnectionString
    End Function

    ''' <summary>
    ''' Obtiene el nombre de la empresa seleccionada en la sesión
    ''' </summary>
    Public Shared Function GetEmpresaSeleccionada() As String
        Dim empresa As String = TryCast(HttpContext.Current.Session("EmpresaSeleccionada"), String)

        If String.IsNullOrWhiteSpace(empresa) Then
            Return "INBURSA" ' Default
        End If

        Return empresa
    End Function

    ''' <summary>
    ''' Verifica si el usuario ha seleccionado una empresa
    ''' </summary>
    Public Shared Function TieneEmpresaSeleccionada() As Boolean
        Return Not String.IsNullOrWhiteSpace(TryCast(HttpContext.Current.Session("ConnectionStringName"), String))
    End Function

End Class
