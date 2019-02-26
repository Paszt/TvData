Imports System.IO
Imports System.Text
Imports System.Runtime.Serialization.Json

Namespace Extensions

    Public Module JsonExtensions

        ''' <summary>
        ''' Creates an object from JSON
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="json"></param>
        ''' <returns></returns>
        <System.Runtime.CompilerServices.Extension>
        Public Function FromJSON(Of T)(json As String) As T
            If String.IsNullOrEmpty(json) Then
                Return Nothing
            End If

            Try
                Using ms = New MemoryStream(Encoding.UTF8.GetBytes(json.ToCharArray()))
                    Dim ser = New DataContractJsonSerializer(GetType(T))
                    Return DirectCast(ser.ReadObject(ms), T)
                End Using
            Catch generatedExceptionName As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Creates a list based on a JSON Array
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="jsonArray"></param>
        ''' <returns></returns>
        <System.Runtime.CompilerServices.Extension>
        Public Function FromJSONArray(Of T)(jsonArray As String) As IEnumerable(Of T)
            If String.IsNullOrEmpty(jsonArray) Then
                Return New List(Of T)()
            End If

            Try
                Using ms = New MemoryStream(Encoding.UTF8.GetBytes(jsonArray))
                    Dim ser = New DataContractJsonSerializer(GetType(IEnumerable(Of T)))
                    Dim result = DirectCast(ser.ReadObject(ms), IEnumerable(Of T))

                    If result Is Nothing Then
                        Return New List(Of T)()
                    Else
                        Return result
                    End If
                End Using
            Catch generatedExceptionName As Exception
                Return New List(Of T)()
            End Try
        End Function

        ''' <summary>
        ''' Creates a JSON Array based on a list
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="list"></param>
        ''' <returns></returns>
        <System.Runtime.CompilerServices.Extension>
        Public Function ToJSONArray(Of T)(ByVal list As IEnumerable(Of T)) As String
            Dim s As DataContractJsonSerializer = New DataContractJsonSerializer(GetType(IEnumerable(Of T)))
            Dim ms As MemoryStream = New MemoryStream()
            s.WriteObject(ms, list)
            Return Encoding.UTF8.GetString(ms.ToArray())
        End Function

    End Module

End Namespace
