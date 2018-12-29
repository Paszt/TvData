Option Strict On

Imports TvData.Extensions

Public Class FreebaseMQL


    Private Shared FreebaseMqlQueryFormat As String = _
        "[{{" &
        "  ""mid"": ""{0}""," &
        "  ""/tv/tv_program/episodes"": [{{" &
        "      ""limit"": 10000," &
        "      ""mid"": null," &
        "      ""id"": null," &
        "      ""name"": null," &
        "      ""/common/topic/description"": []," &
        "      ""/tv/tv_series_episode/season_number"": null," &
        "      ""/tv/tv_series_episode/episode_number"": null," &
        "      ""/tv/tv_series_episode/production_number"": null," &
        "      ""/tv/tv_series_episode/season"": [{{" &
        "          ""/tv/tv_series_season/season_number"": null" &
        "        }}]," &
        "      ""count"": null" &
        "    }}]" &
        "}}]"

    Public Shared Sub GetEpisodeMIds(FreebaseMid As String, episodes As IList(Of Episode))
        Dim freebaseMqlUrl = "https://www.googleapis.com/freebase/v1/mqlread/?lang=/lang/en&key=AIzaSyAhixxV8Ms9I7ptRyZe_urI5SpWm9SzC0A&query=" & _
                             String.Format(FreebaseMqlQueryFormat, FreebaseMid)
        'freebaseMqlUrl = Web.HttpUtility.UrlEncode(freebaseMqlUrl)
        Dim jsonResponse As String
        Using _client As New WebClientEx()
            jsonResponse = _client.DownloadString(New Uri(freebaseMqlUrl))
        End Using
        Dim fbEpisodes = jsonResponse.FromJSON(Of FreebaseShowEpisodes)()
        Dim fbEpisodesResult = fbEpisodes.Result.FirstOrDefault

        If fbEpisodesResult IsNot Nothing Then
            For Each epi In fbEpisodesResult.Episodes
                If (epi.SeasonNumber.HasValue Or epi.SeasonNumberFromSeason.HasValue) AndAlso epi.EpisodeNumber.HasValue Then
                    Dim seasonNum As Integer
                    If epi.SeasonNumber.HasValue Then
                        seasonNum = epi.SeasonNumber.Value
                    Else
                        seasonNum = epi.SeasonNumberFromSeason.Value
                    End If
                    Dim dataEpisode = (From e In episodes
                                       Where e.SeasonNumber = seasonNum AndAlso
                                             e.EpisodeNumber = epi.EpisodeNumber).FirstOrDefault
                    If dataEpisode IsNot Nothing Then
                        If epi.Id.StartsWith("/en/") Then
                            dataEpisode.FreebaseId = epi.Id
                        End If
                        dataEpisode.FreebaseMid = epi.Mid
                        If epi.ProductionNumber IsNot Nothing Then
                            dataEpisode.ProductionCode = epi.ProductionNumber
                        End If
                    End If
                End If
            Next
        End If

    End Sub

End Class
