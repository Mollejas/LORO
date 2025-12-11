Imports System
Imports System.Web
Imports System.Configuration
Imports System.Data.SqlClient

''' <summary>
''' Helper para obtener la cadena de conexión correcta según la empresa seleccionada en sesión
''' </summary>
Public Class DatabaseHelper

    ''' <summary>
    ''' Obtiene la cadena de conexión basada en la empresa seleccionada en la sesión
    ''' Modifica dinámicamente el Initial Catalog según la empresa:
    ''' - QUALITAS → LORONUEVOQUA
    ''' - INBURSA → LORONUEVO (default)
    ''' - EXTERNOS → EXTERNO
    ''' </summary>
    Public Shared Function GetConnectionString() As String
        ' Obtener la connection string base del Web.config
        Dim cs = ConfigurationManager.ConnectionStrings("DaytonaDB")

        If cs Is Nothing Then
            Throw New Exception("No se encontró la cadena de conexión 'DaytonaDB' en Web.config")
        End If

        ' Obtener la empresa seleccionada de la sesión
        Dim empresa As String = GetEmpresaSeleccionada()

        ' Construir el connection string con el catálogo correcto
        Dim builder As New SqlConnectionStringBuilder(cs.ConnectionString)

        Select Case empresa.ToUpperInvariant()
            Case "QUALITAS"
                builder.InitialCatalog = "LORONUEVOQUA"
            Case "INBURSA"
                builder.InitialCatalog = "LORONUEVO"
            Case "EXTERNOS"
                builder.InitialCatalog = "EXTERNO"
            Case Else
                ' Default: INBURSA
                builder.InitialCatalog = "LORONUEVO"
        End Select

        Return builder.ConnectionString
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
        Return Not String.IsNullOrWhiteSpace(TryCast(HttpContext.Current.Session("EmpresaSeleccionada"), String))
    End Function

End Class
