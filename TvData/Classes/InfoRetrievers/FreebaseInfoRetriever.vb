Option Strict On

Imports TvData.Extensions

Public Class FreebaseInfoRetriever
    Implements IInfoRetriever

    Public Property ExtraEpisodes As IList(Of Episode) Implements IInfoRetriever.ExtraEpisodes
    Public Event StatusChanged(TypeOfStatus As StatusType, Message As String) Implements IInfoRetriever.StatusChanged

    Private Const FreebaseMqlQueryFormat As String = _
        "[{{" &
        "  ""mid"": ""{0}""," &
        "  ""/tv/tv_program/episodes"": [{{" &
        "      ""limit"": 10000," &
        "      ""mid"": null," &
        "      ""id"": null," &
        "      ""name"": null," &
        "      ""/common/topic/description"": []," &
        "      ""/tv/tv_series_episode/air_date"": null," &
        "      ""/tv/tv_series_episode/season_number"": null," &
        "      ""/tv/tv_series_episode/episode_number"": null," &
        "      ""/tv/tv_series_episode/production_number"": null," &
        "      ""/tv/tv_series_episode/season"": [{{" &
        "          ""/tv/tv_series_season/season_number"": null" &
        "        }}]," &
        "      ""count"": null" &
        "    }}]" &
        "}}]"

    Public Sub RetrieveInfo(FreebaseMid As String,
                            episodes As TrulyObservableCollection(Of Episode),
                            Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en) Implements IInfoRetriever.RetrieveInfo
        ExtraEpisodes = New List(Of Episode)
        Dim freebaseMqlUrl = "https://www.googleapis.com/freebase/v1/mqlread/?lang=/lang/en&key=AIzaSyAhixxV8Ms9I7ptRyZe_urI5SpWm9SzC0A&query=" &
                             String.Format(FreebaseMqlQueryFormat, FreebaseMid)
        Dim jsonResponse As String
        Using _client As New WebClientEx()
            jsonResponse = _client.DownloadString(New Uri(freebaseMqlUrl))
        End Using
        Dim fbEpisodes = jsonResponse.FromJSON(Of FreebaseShowEpisodes)()
        Dim fbEpisodesResult = fbEpisodes.Result.FirstOrDefault

        If fbEpisodesResult IsNot Nothing Then
            Dim seasons = (From eps In episodes Where eps.SeasonNumber <> 0 Select eps.SeasonNumber Distinct).ToList()
            For Each freebaseEpisode In fbEpisodesResult.Episodes
                If (freebaseEpisode.SeasonNumber.HasValue Or freebaseEpisode.SeasonNumberFromSeason.HasValue) AndAlso freebaseEpisode.EpisodeNumber.HasValue Then
                    Dim seasonNum As Integer
                    If freebaseEpisode.SeasonNumber.HasValue Then
                        seasonNum = freebaseEpisode.SeasonNumber.Value
                    Else
                        seasonNum = freebaseEpisode.SeasonNumberFromSeason.Value
                    End If

                    If seasons.Contains(CShort(seasonNum)) Then
                        Dim dataEpisode = (From e In episodes
                                           Where e.SeasonNumber = seasonNum AndAlso
                                                 e.EpisodeNumber = freebaseEpisode.EpisodeNumber).FirstOrDefault()
                        If dataEpisode IsNot Nothing Then
                            dataEpisode.Freebase_EpisodeName = freebaseEpisode.Name
                            dataEpisode.Freebase_AiredDate = freebaseEpisode.AirDate
                            dataEpisode.Freebase_Overview = freebaseEpisode.Overview
                            If freebaseEpisode.Id.StartsWith("/en/") Then
                                dataEpisode.FreebaseId = freebaseEpisode.Id
                            End If
                            dataEpisode.FreebaseMid = freebaseEpisode.Mid
                            If freebaseEpisode.ProductionNumber IsNot Nothing Then
                                dataEpisode.ProductionCode = freebaseEpisode.ProductionNumber
                            End If
                        Else
                            Dim extraEpisode As New Episode() With {
                                .SeasonNumber = CShort(seasonNum),
                                .EpisodeNumber = CShort(freebaseEpisode.EpisodeNumber.Value),
                                .Freebase_EpisodeName = freebaseEpisode.Name,
                                .Freebase_AiredDate = freebaseEpisode.AirDate,
                                .Freebase_Overview = freebaseEpisode.Overview
                            }

                            If freebaseEpisode.Id.StartsWith("/en/") Then
                                extraEpisode.FreebaseId = freebaseEpisode.Id
                            End If
                            extraEpisode.FreebaseMid = freebaseEpisode.Mid
                            If freebaseEpisode.ProductionNumber IsNot Nothing Then
                                extraEpisode.ProductionCode = freebaseEpisode.ProductionNumber
                            End If

                            ExtraEpisodes.Add(extraEpisode)
                        End If
                    End If
                End If
            Next
        End If

    End Sub

End Class
