Option Strict On

Imports System.Net
Imports System.Runtime.Serialization.Json
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports TvData.Extensions
Imports TvData

Public Class TmdbInfoRetriever
    Implements IInfoRetriever

    Public Event StatusChanged(TypeOfStatus As StatusType, Message As String) Implements IInfoRetriever.StatusChanged

    Public Property ExtraEpisodes As IList(Of Episode) Implements IInfoRetriever.ExtraEpisodes

    Private _language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en

    Private tmdb As TmdbApi

    Public Sub New(tmdb As TmdbApi)
        Me.tmdb = tmdb
    End Sub

    '' url to get imdb_id, input with id = imdb_id
    ''https://www.themoviedb.org/tv/1406/season/2/episode/12/edit
    'Private Const WebUrlFormat As String = "https://www.themoviedb.org/tv/{0}/season/{1}/episode/12/edit"

    Public Overloads Sub RetrieveInfo(TMDbSeriesId As String,
                                      Episodes As TrulyObservableCollection(Of Episode),
                                      Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en) Implements IInfoRetriever.RetrieveInfo
        _language = Language
        ExtraEpisodes = New List(Of Episode)
        Dim seasonNumbers = (From eps In Episodes Select eps.SeasonNumber Distinct).ToList()
        Dim seasonCounter As Integer = 1
        For Each seasonNo In seasonNumbers
            Dim seasonStatus = RaiseSeasonStatus(seasonNumbers.Count, seasonCounter, seasonNo)

            Dim tmdbSeason = tmdb.GetTvSeason(TMDbSeriesId, seasonNo, _language)

            If tmdbSeason IsNot Nothing Then
                Dim episodeCounter As Integer = 1
                For Each episode As TmdbApi.TvEpisode In tmdbSeason.episodes
                    'fix html encoding in overview
                    episode.overview = WebUtility.HtmlDecode(episode.overview.Trim())

                    RaiseEpisodeStatus(seasonStatus, episodeCounter, tmdbSeason.episodes.Count)

                    ' Dim epFull = tmdb.GetTvEpisode(TMDbSeriesId, episode.season_number, episode.episode_number)
                    Dim epFull = tmdb.GetTvEpisode(TMDbSeriesId, tmdbSeason.season_number, episode.episode_number)
                    If epFull IsNot Nothing Then
                        episode.external_ids = epFull.external_ids
                    End If

                    Dim existingEpisode = (From r In Episodes
                                           Where r.SeasonNumber = seasonNo AndAlso
                                                 r.EpisodeNumber = episode.episode_number
                                           Select r).FirstOrDefault()
                    If existingEpisode IsNot Nothing Then
                        existingEpisode.TMDB_AiredDate = episode.air_date
                        existingEpisode.TMDB_Overview = episode.overview
                        existingEpisode.TMDB_EpisodeName = episode.name?.Trim()
                        existingEpisode.TMDB_ImdbId = episode.external_ids.imdb_id
                        If episode.production_code IsNot Nothing Then
                            existingEpisode.TMDB_ProductionCode = episode.production_code
                        End If
                        If String.IsNullOrWhiteSpace(existingEpisode.TvRageId) AndAlso episode.external_ids.tvrage_id.HasValue Then
                            existingEpisode.TvRageId = episode.external_ids.tvrage_id.ToString()
                        End If
                        If String.IsNullOrWhiteSpace(existingEpisode.FreebaseMid) AndAlso Not String.IsNullOrEmpty(episode.external_ids.freebase_mid) Then
                            existingEpisode.FreebaseMid = episode.external_ids.freebase_mid
                        End If
                        If String.IsNullOrWhiteSpace(existingEpisode.FreebaseId) AndAlso Not String.IsNullOrEmpty(episode.external_ids.freebase_id) Then
                            existingEpisode.FreebaseId = episode.external_ids.freebase_id
                        End If
                    Else
                        Dim ep As New Episode() With {
                            .SeasonNumber = seasonNo,
                            .EpisodeNumber = CShort(episode.episode_number),
                            .TMDB_EpisodeName = episode.name,
                            .TMDB_AiredDate = episode.air_date,
                            .TMDB_Overview = episode.overview
                        }

                        If episode.production_code IsNot Nothing Then
                            ep.TMDB_ProductionCode = episode.production_code
                        End If
                        ExtraEpisodes.Add(ep)
                        'Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Episodes, ep})
                        ''Episodes.Add(ep)
                    End If
                    episodeCounter += 1
                Next 'episode
            End If
            seasonCounter += 1
        Next 'Season
    End Sub

    Private Function RaiseSeasonStatus(seasonNumbersCount As Integer, seasonCounter As Integer, seasonNo As Short) As String
        Dim seasonStatusFormat As String = "Downloading TMDb Season {0} ({1} of {2})"
        Dim seasonStatus = String.Format(seasonStatusFormat, seasonNo, seasonCounter, seasonNumbersCount)
        RaiseEvent StatusChanged(StatusType.Busy, seasonStatus)
        Return seasonStatus
    End Function

    Private Sub RaiseEpisodeStatus(seasonStatus As String, episodeCounter As Integer, seasonEpisodesCount As Integer)
        Dim episodeStatusFormat As String = ", episode {0} of {1}"
        Dim episodeStatus = seasonStatus & String.Format(episodeStatusFormat, episodeCounter, seasonEpisodesCount)
        RaiseEvent StatusChanged(StatusType.Busy, episodeStatus)
    End Sub

    'Public Overloads Sub RetrieveInfo(TMDbSeriesId As String, Episodes As TrulyObservableCollection(Of Episode), Language As MainViewModel.Iso639_1)
    '    _language = Language
    '    RetrieveInfo(TMDbSeriesId, Episodes)
    'End Sub

End Class
