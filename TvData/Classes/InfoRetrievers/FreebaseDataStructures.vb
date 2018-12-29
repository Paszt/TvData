Imports System.Runtime.Serialization

<DataContract>
Public Class FreebaseTvProgramEpisode

    <DataMember(Name:="name")>
    Public Property Name As String

    <DataMember(Name:="/tv/tv_series_episode/season_number")>
    Public Property SeasonNumber As Integer?

    <DataMember(Name:="/tv/tv_series_episode/season")>
    Private Property Season As List(Of FreebaseTvSeriesEpisodeSeason)

    Public ReadOnly Property SeasonNumberFromSeason As Integer?
        Get
            If Season.FirstOrDefault IsNot Nothing Then
                If Season.FirstOrDefault.SeasonNumber.HasValue Then
                    Return Season.FirstOrDefault.SeasonNumber
                End If
            End If
            Return Nothing
        End Get
    End Property

    <DataMember(Name:="/tv/tv_series_episode/episode_number")>
    Public Property EpisodeNumber As Integer?

    <DataMember(Name:="/common/topic/description")>
    Private Property CommonTopicDescriptions As List(Of String)

    ''' <summary>
    ''' The first /common/topic/description
    ''' </summary>
    Public ReadOnly Property Overview As String
        Get
            Return CommonTopicDescriptions.FirstOrDefault()
        End Get
    End Property

    <DataMember(Name:="/tv/tv_series_episode/air_date")>
    Public Property AirDate As String

    <DataMember(Name:="/tv/tv_series_episode/production_number")>
    Public Property ProductionNumber As String

    <DataMember(Name:="id")>
    Public Property Id As String

    <DataMember(Name:="mid")>
    Public Property Mid As String

    <DataMember(Name:="count")>
    Public Property Count As Integer

End Class

<DataContract>
Public Class FreebaseTvSeriesEpisodeSeason

    <DataMember(Name:="/tv/tv_series_season/season_number")>
    Public Property SeasonNumber As Integer?

End Class

<DataContract>
Public Class FreebaseShowEpisodesResult

    <DataMember(Name:="mid")>
    Public Property Mid As String

    <DataMember(Name:="/tv/tv_program/episodes")>
    Public Property Episodes As List(Of FreebaseTvProgramEpisode)

End Class

<DataContract>
Public Class FreebaseShowEpisodes

    <DataMember(Name:="result")>
    Public Property Result As FreebaseShowEpisodesResult()

End Class