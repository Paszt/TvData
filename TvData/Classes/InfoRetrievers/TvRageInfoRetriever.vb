Option Strict On

Imports CsQuery
Imports System.Text.RegularExpressions

Public Class TvRageInfoRetriever
    Implements IInfoRetriever

    Public Event StatusChanged(TypeOfStatus As StatusType, Message As String) Implements IInfoRetriever.StatusChanged

    Public Property ExtraEpisodes As IList(Of Episode) Implements IInfoRetriever.ExtraEpisodes

    '' http://www.tvrage.com/shows/id-{TvRageId}/episode_guide/{SeasonNumber}
    Private Const seasonEpisodeGuideFormat As String = "http://www.tvrage.com/shows/id-{0}/episode_guide/{1}"

    Public Sub RetrieveInfo(TvRageSeriesId As String,
                            episodes As TrulyObservableCollection(Of Episode),
                            Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en) Implements IInfoRetriever.RetrieveInfo
        ExtraEpisodes = New List(Of Episode)
        Dim seasons = (From eps In episodes Where CInt(eps.SeasonNumber) <> 0 Select eps.SeasonNumber Distinct).ToList()
        Dim seasonCounter As Integer = 1
        Using wcx As New WebClientEx
            For Each season In seasons
                Dim seasonStatusFormat As String = "TvRage Season {0} ({1} of {2}) - Downloading from TvRage"
                RaiseEvent StatusChanged(StatusType.Busy, String.Format(seasonStatusFormat, season, seasonCounter, seasons.Count))
                Dim responseString As String = String.Empty
                Dim seasonUrl = String.Format(seasonEpisodeGuideFormat, TvRageSeriesId, season)
                Try
                    responseString = wcx.DownloadString(seasonUrl)
                    'Catch wex As Net.WebException
                    '    MessageWindow.ShowDialog(wex.Message, "Error")
                Catch ex As Exception
                    responseString = TextWindow.GetText("Error downloading from TV Rage", "Unable to download from TV Rage. Please paste the source code of the following webpage:", seasonUrl)
                End Try

                If Not String.IsNullOrEmpty(responseString) Then
                    RaiseEvent StatusChanged(StatusType.Busy, "TvRage - Preparing to parse HTML, Season " & season)
                    Dim dom = CQ.Create(responseString)
                    Dim rageEpisodes = dom("div.grid_7_5.box.margin_top_bottom.left")
                    Dim episodeCounter As Integer = 1
                    For Each rageEpisodeDiv In rageEpisodes
                        Dim episodeStatusformat As String = "TvRage Season {0} ({1} of {2}) - Parsing Episode {3} of {4}"
                        RaiseEvent StatusChanged(StatusType.Busy,
                                                 String.Format(episodeStatusformat, New String() {CStr(season),
                                                                                                  CStr(seasonCounter),
                                                                                                  CStr(seasons.Count),
                                                                                                  CStr(episodeCounter),
                                                                                                  CStr(rageEpisodes.Count)}))

                        Dim titleLink = rageEpisodeDiv.Cq.Find("a.wlink")
                        Dim tvRageIdMatch = Regex.Match(titleLink.Attr("href"), "episodes\/(\d+)")
                        Dim tvRageId As String = String.Empty
                        If tvRageIdMatch.Success Then
                            tvRageId = tvRageIdMatch.Groups(1).Value
                        End If
                        Dim titleText = titleLink.Text()
                        Dim titleMatches = Regex.Match(titleText, "(?<SeasonNo>\d+)x(?<EpisodeNo>\d+) - (?<Title>.+)")
                        If titleMatches.Success Then
                            Dim SeasonNo = CInt(titleMatches.Groups("SeasonNo").Value)
                            Dim EpisodeNo = CInt(titleMatches.Groups("EpisodeNo").Value)
                            Dim Title = titleMatches.Groups("Title").Value
                            Dim Plot = rageEpisodeDiv.Cq.Find("div.margin_bottom").Text().Trim()
                            If Plot = "• No Summary (Add Here)" Then
                                Plot = String.Empty
                            End If
                            Dim AiredDate = rageEpisodeDiv.Cq.Find("i").Text()
                            Dim dte As Date
                            If DateTime.TryParse(AiredDate, dte) Then
                                AiredDate = dte.ToString("yyyy-MM-dd")
                            End If

                            Dim row = (From r In episodes
                                       Where r.SeasonNumber = SeasonNo AndAlso
                                             r.EpisodeNumber = EpisodeNo
                                       Select r).FirstOrDefault()
                            If row IsNot Nothing Then
                                row.TvRage_EpisodeName = Title.Trim()
                                row.TvRage_AiredDate = AiredDate
                                row.TvRage_Plot = Plot
                                row.TvRageId = tvRageId
                            Else
                                Dim ep As New Episode() With {
                                    .SeasonNumber = CShort(SeasonNo),
                                    .EpisodeNumber = CShort(EpisodeNo),
                                    .TvRage_EpisodeName = Title.Trim(),
                                    .TvRage_AiredDate = AiredDate,
                                    .TvRage_Plot = Plot,
                                    .TvRageId = tvRageId
                                }
                                ExtraEpisodes.Add(ep)
                                'Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Episodes, ep})
                                'Episodes.Add(ep)
                            End If
                        End If
                    Next
                End If
            Next

        End Using


    End Sub

End Class
