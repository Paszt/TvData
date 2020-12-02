Imports System.Runtime.Serialization.Json
Imports TvData.Extensions

Public Class TmdbApi
    'Implements IDisposable

#Region " Declarations "

    Public Const ApiKey As String = "03fab28c05313508eae7d69064ba1612"

    'Private rg As Infrastructure.RateGate = New Infrastructure.RateGate(38, TimeSpan.FromSeconds(10))

#End Region

#Region " Shared Functions "

#Region " TV Changes "

    Public Function GetTvChanges() As TmdbApi.TvPageSet
        Dim returnChanges As New TmdbApi.TvPageSet
        returnChanges.results = New List(Of TmdbApi.TvPageSet.Result)
        Dim pageNo As Integer = 1
        Dim totalpages As Integer = 0
        Do
            Dim changes As TmdbApi.TvPageSet = GetTvChangesPage(pageNo)
            If changes Is Nothing OrElse changes.results Is Nothing Then
                Return returnChanges
            End If
            For Each change In changes.results
                returnChanges.results.Add(change)
            Next
            returnChanges.total_results += changes.total_results
            totalpages = changes.total_pages
            pageNo += 1
        Loop Until (pageNo - 1) = totalpages
        Return returnChanges
    End Function

    Private Function GetTvChangesPage(pageNumber As Integer) As TmdbApi.TvPageSet
        Dim responseJsonString = GetApiResponse(Urls.TvChanges(pageNumber))
        Dim changes As New TmdbApi.TvPageSet()
        If Not String.IsNullOrEmpty(responseJsonString) Then
            Return responseJsonString.FromJSON(Of TmdbApi.TvPageSet)()
        End If
        Return changes
    End Function

#End Region

#Region " TV On Air (Next 7 days) "

    Public Function GetTvOnAir() As TmdbApi.TvPageSet
        Dim returnOnAir As New TvPageSet With {
            .results = New List(Of TvPageSet.Result)
        }
        Dim pageNo As Integer = 1
        Dim totalpages As Integer = 0
        Do
            Dim onAirShows As TvPageSet = GetTvOnAirPage(pageNo)
            If onAirShows Is Nothing OrElse onAirShows.results Is Nothing Then
                Return returnOnAir
            End If
            For Each onAirShow In onAirShows.results
                returnOnAir.results.Add(onAirShow)
            Next
            returnOnAir.total_results += onAirShows.total_results
            totalpages = onAirShows.total_pages
            pageNo += 1
        Loop Until (pageNo - 1) = totalpages
        Return returnOnAir
    End Function

    Private Function GetTvOnAirPage(pageNumber As Integer) As TvPageSet
        Dim responseJsonString = GetApiResponse(Urls.TvOnAir(pageNumber))
        Dim changes As New TvPageSet()
        If Not String.IsNullOrEmpty(responseJsonString) Then
            Return responseJsonString.FromJSON(Of TvPageSet)()
        End If
        Return changes
    End Function

#End Region

    Public Function GetTvShow(id As Integer) As TvShow
        Dim responseJsonString = GetApiResponse(Urls.TvShow(id))
        If Not String.IsNullOrEmpty(responseJsonString) Then
            Return responseJsonString.FromJSON(Of TvShow)()
        End If
        Return New TvShow()
    End Function

    Public Function GetTvSeason(TMDbSeriesId As String, seasonNo As Integer, language As MainViewModel.Iso639_1) As TvSeason
        Dim responseJsonString As String = GetApiResponse(Urls.TvSeason(TMDbSeriesId, seasonNo, language))
        If Not String.IsNullOrEmpty(responseJsonString) Then
            Return responseJsonString.FromJSON(Of TvSeason)
        End If
        Return Nothing
    End Function

    Public Function GetTvEpisode(TMDBSeriesId As String, seasonNo As Integer, episodeNo As Integer) As TvEpisode
        Dim responseJsonString As String = GetApiResponse(Urls.TvEpisode(TMDBSeriesId, seasonNo, episodeNo))

        'Dim jsonSerializer As New DataContractJsonSerializer(GetType(TvEpisode))

        'Dim results As New TvEpisode
        'Using stream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseJsonString))
        '    results = CType(jsonSerializer.ReadObject(stream), TvEpisode)
        'End Using

        'Return results

        If Not String.IsNullOrWhiteSpace(responseJsonString) Then
            Return responseJsonString.FromJSON(Of TvEpisode)
        End If
        Return Nothing
    End Function

    Public Function GetLatestTvShow() As TvShow
        Dim responseJsonString = GetApiResponse(Urls.TvShowLatest())
        If Not String.IsNullOrWhiteSpace(responseJsonString) Then
            Return responseJsonString.FromJSON(Of TvShow)()
        End If
        Return Nothing
    End Function

    Public Function Search(Query As String, FirstAired As String) As SearchPage
        Dim apiUrl As String = Urls.TvSearch(Query) '  String.Format(ApiSearchUrlFormat, System.Uri.EscapeDataString(Query))
        Dim dte As Date
        If Date.TryParse(FirstAired, dte) Then
            apiUrl &= "&first_air_date_year=" & dte.Year
        End If

        Dim responseJsonString = GetApiResponse(apiUrl)

        If Not String.IsNullOrWhiteSpace(responseJsonString) Then
            Return responseJsonString.FromJSON(Of SearchPage)
        Else
            Return New SearchPage
        End If

        'Dim results As New TmdbApi.SearchPage()
        'Using stream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseJsonString))
        '    Dim jsonSerializer As New DataContractJsonSerializer(GetType(TmdbApi.SearchPage))
        '    results = CType(jsonSerializer.ReadObject(stream), TmdbApi.SearchPage)
        'End Using
        'Return results
    End Function

    Public Function GetTmdbIdFromTvdbId(tvdbId As String) As Integer?
        Dim responseJsonString = GetApiResponse(Urls.TmdbIdFromTvdbId(tvdbId))

        Dim jsonSerializer As New DataContractJsonSerializer(GetType(FindResults))

        Dim results As New FindResults
        Using stream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseJsonString))
            results = CType(jsonSerializer.ReadObject(stream), FindResults)
        End Using

        If results.tv_results IsNot Nothing AndAlso results.tv_results.Count > 0 Then
            Return results.tv_results.First.id
        End If

        Return Nothing
    End Function

    Public Function GetTvShowExternalIds(id As String) As TvShowExternalIds
        Dim responseJsonString = GetApiResponse(Urls.TvShowExternalIds(id))
        Dim externalIds As New TvShowExternalIds()
        If Not String.IsNullOrEmpty(responseJsonString) Then
            externalIds = responseJsonString.FromJSON(Of TvShowExternalIds)
            'Dim jsonSerializer As New DataContractJsonSerializer(GetType(TmdbApi.TvShowExternalIds))
            'Using stream As New IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(responseJsonString))
            '    externalIds = CType(jsonSerializer.ReadObject(stream), TmdbApi.TvShowExternalIds)
            'End Using
        End If
        Return externalIds
    End Function

#End Region

    Private Function GetApiResponse(url As String) As String
        Dim responseJsonString As String = String.Empty
        'rg.WaitToProceed()
        Using wce As New WebClientEx
            Try
                responseJsonString = wce.DownloadString(url)
            Catch ex As Exception
            End Try
        End Using
        Return responseJsonString
    End Function

#Region "IDisposable Support"
    'Private disposedValue As Boolean ' To detect redundant calls

    '' IDisposable
    'Protected Overridable Sub Dispose(disposing As Boolean)
    '    If Not disposedValue Then
    '        If disposing Then
    '            ' TODO: dispose managed state (managed objects).
    '            If rg IsNot Nothing Then
    '                rg.Dispose()
    '            End If
    '        End If

    '        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
    '        ' TODO: set large fields to null.
    '    End If
    '    disposedValue = True
    'End Sub

    '' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    ''Protected Overrides Sub Finalize()
    ''    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    ''    Dispose(False)
    ''    MyBase.Finalize()
    ''End Sub

    '' This code added by Visual Basic to correctly implement the disposable pattern.
    'Public Sub Dispose() Implements IDisposable.Dispose
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(True)
    '    ' TODO: uncomment the following line if Finalize() is overridden above.
    '    ' GC.SuppressFinalize(Me)
    'End Sub
#End Region

End Class
