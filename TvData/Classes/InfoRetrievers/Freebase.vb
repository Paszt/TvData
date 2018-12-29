Option Strict On

Imports System.Runtime.Serialization
Imports TvData.Extensions

Public Class Freebase
    Implements IDisposable

    Private _client As WebClientEx

    Public Sub GetEpisodeMIds(FreebaseMId As String, episodes As IList(Of Episode))
        _client = New WebClientEx()
        Dim freebaseEpisodes = GetFreebaseEpisodes(FreebaseMId).Episodes
        For Each freebaseEpisode In freebaseEpisodes
            Dim epInfo = GetEpisodeInfo(freebaseEpisode.Id)
            If epInfo.SeasonNumber <> Nothing AndAlso epInfo.EpisodeNumber <> Nothing Then
                Dim dataEpisode = (From e In episodes
                                   Where e.SeasonNumber = epInfo.SeasonNumber AndAlso
                                         e.EpisodeNumber = epInfo.EpisodeNumber).FirstOrDefault
                If dataEpisode IsNot Nothing Then
                    dataEpisode.FreebaseMid = epInfo.Id
                End If
            End If
        Next
    End Sub

    Private Function GetFreebaseEpisodes(FreebaseMid As String) As FreebaseShow
        Dim episodesUrl = String.Format("https://www.googleapis.com/freebase/v1/topic{0}?filter=/tv/tv_program/episodes&limit=0&key=AIzaSyAhixxV8Ms9I7ptRyZe_urI5SpWm9SzC0A", FreebaseMid)
        Dim jsonResponse As String
        jsonResponse = _client.DownloadString(episodesUrl)
        Return jsonResponse.FromJSON(Of FreebaseShow)()
    End Function

    Private Function GetEpisodeInfo(EpisodeMId As String) As FreebaseEpisodeInfo
        Dim episodeInfoUrl = String.Format("https://www.googleapis.com/freebase/v1/topic{0}?filter=/tv/tv_series_episode/episode_number&filter=/tv/tv_series_episode/season_number&key=AIzaSyAhixxV8Ms9I7ptRyZe_urI5SpWm9SzC0A", EpisodeMId)
        Dim jsonResponse As String
        jsonResponse = _client.DownloadString(episodeInfoUrl)
        Return jsonResponse.FromJSON(Of FreebaseEpisodeInfo)()
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _client IsNot Nothing Then
                    _client.Dispose()
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class


#Region " Program Data Structures "

<DataContract>
Public Class EpisodeValue

    <DataMember(Name:="text")>
    Public Property Text As String

    <DataMember(Name:="lang")>
    Public Property Lang As String

    <DataMember(Name:="id")>
    Public Property Id As String

    <DataMember(Name:="creator")>
    Public Property Creator As String

End Class

<DataContract>
Public Class PropertyValue

    <DataMember(Name:="values")>
    Public Property Values As List(Of EpisodeValue)

End Class

<DataContract>
Public Class ProgramProperties

    <DataMember(Name:="/tv/tv_program/episodes")>
    Public Property Episodes As PropertyValue
End Class

<DataContract>
Public Class FreebaseShow

    <DataMember(Name:="id")>
    Public Property Id As String

    <DataMember(Name:="property")>
    Public Property Properties As ProgramProperties

    Public ReadOnly Property Episodes As List(Of EpisodeValue)
        Get
            If Properties IsNot Nothing Then
                Return Properties.Episodes.Values
            End If
            Return New List(Of EpisodeValue)
        End Get
    End Property

End Class

#End Region

#Region " Episode Data Structures "

<DataContract>
Public Class SeasonEpisodeValue

    <DataMember(Name:="value")>
    Public Property Value As Short

End Class

<DataContract>
Public Class Number

    <DataMember(Name:="values")>
    Public Property Values As List(Of SeasonEpisodeValue)

End Class

<DataContract>
Public Class EpisodeProperties

    <DataMember(Name:="/tv/tv_series_episode/episode_number")>
    Public Property EpisodeNumber As Number

    <DataMember(Name:="/tv/tv_series_episode/season_number")>
    Public Property SeasonNumber As Number

End Class

<DataContract>
Public Class FreebaseEpisodeInfo

    <DataMember(Name:="id")>
    Public Property Id As String

    <DataMember(Name:="property")>
    Public Property Properties As EpisodeProperties

    Public ReadOnly Property EpisodeNumber As Short
        Get
            If Properties.EpisodeNumber IsNot Nothing Then
                Return Properties.EpisodeNumber.Values.First.Value
            End If
            Return -1
        End Get
    End Property

    Public ReadOnly Property SeasonNumber As Short
        Get
            If Properties.SeasonNumber IsNot Nothing Then
                Return Properties.SeasonNumber.Values.First.Value
            End If
            Return -1
        End Get
    End Property

End Class

#End Region
