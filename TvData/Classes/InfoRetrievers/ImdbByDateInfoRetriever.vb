Option Strict On
Imports System.Text.RegularExpressions
Imports CsQuery

Public Class ImdbByDateInfoRetriever
    Implements IInfoRetriever

    Public Event StatusChanged(TypeOfStatus As StatusType, Message As String) Implements IInfoRetriever.StatusChanged

    Public Property ExtraEpisodes As IList(Of Episode) Implements IInfoRetriever.ExtraEpisodes

    Private Shared Function ImdbUrl(ImdbSeriesId As String, year As Integer) As String
        Dim format As String = "http://www.imdb.com/title/{0}/episodes?year={1}"
        Return String.Format(format, ImdbSeriesId, year)
    End Function

    Public Sub RetrieveInfo(ImdbSeriesId As String,
                            episodes As TrulyObservableCollection(Of Episode),
                            Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en) Implements IInfoRetriever.RetrieveInfo
        ExtraEpisodes = New List(Of Episode)
        'Dim seasons = (From eps In episodes Where eps.SeasonNumber <> 0 Select eps.SeasonNumber Distinct).ToList()
        Dim years = (From eps In episodes Where eps.FirstAiredYear.HasValue Select eps.FirstAiredYear Distinct).ToList()
        Dim yearCounter As Integer = 1
        For Each airedYear In years
            Dim yearStatusFormat As String = "IMDB year {0} ({1} of {2}) - Downloading from IMDB"
            RaiseEvent StatusChanged(StatusType.Busy, String.Format(yearStatusFormat, airedYear, yearCounter, years.Count))
            Dim responseString As String = String.Empty
            Using wcx As New WebClientEx
                responseString = wcx.DownloadString(ImdbUrl(ImdbSeriesId, airedYear.Value))
            End Using

            ''responseString = My.Computer.FileSystem.ReadAllText("ImdbEpisodePage.html")

            If Not String.IsNullOrEmpty(responseString) Then
                RaiseEvent StatusChanged(StatusType.Busy, "IMDB - Preparing to parse HTML")
                Dim doc = CQ.CreateDocument(responseString)
                Dim episodeDivs = doc.Find("div.list_item")
                Dim episodeCounter As Integer = 1
                For Each episodeDiv In episodeDivs
                    Dim episodeStatusformat As String = "IMDB year {0} ({1} of {2}) - Parsing Episode {3} of {4}"
                    RaiseEvent StatusChanged(StatusType.Busy,
                                             String.Format(episodeStatusformat, New String() {CStr(airedYear),
                                                                                              CStr(yearCounter),
                                                                                              CStr(years.Count),
                                                                                              CStr(episodeCounter),
                                                                                              CStr(episodeDivs.Count)}))
                    'Dim SeasonEpisodeText As String = episodeDiv.Cq.Find("div.image div div").Text()
                    Dim Episodehref = episodeDiv.Cq.Find("div.info strong a").Attr("href")
                    Dim EpisodeName = episodeDiv.Cq.Find("div.info strong a").Text()
                    Dim DateString = episodeDiv.Cq.Find("div.info div.airdate").Text()
                    Dim PlotString = episodeDiv.Cq.Find("div.info div.item_description").Text()
                    Application.Current.Dispatcher.BeginInvoke(New AddDataDelegate(AddressOf AddData),
                                                               New Object() {
                                                                   Episodehref,
                                                                   EpisodeName,
                                                                   DateString,
                                                                   PlotString,
                                                                   episodes})
                    episodeCounter += 1
                Next
            End If
            yearCounter += 1
        Next
    End Sub

    Delegate Sub AddDataDelegate(EpisodeHref As String,
                                 EpisodeName As String,
                                 DateString As String,
                                 PlotString As String,
                                 Episodes As TrulyObservableCollection(Of Episode))

    Private Sub AddData(EpisodeHref As String,
                        EpisodeName As String,
                        DateString As String,
                        PlotString As String, Episodes As TrulyObservableCollection(Of Episode))
        'avoid null errors
        'If SeasonEpisodeText Is Nothing Then
        '    SeasonEpisodeText = String.Empty
        'End If
        If EpisodeHref Is Nothing Then
            EpisodeHref = String.Empty
        End If
        If EpisodeName Is Nothing Then
            EpisodeName = String.Empty
        End If
        If PlotString Is Nothing OrElse
            PlotString.Trim() = "Add a Plot" OrElse
            PlotString.Trim().StartsWith("Know what this is about?") Then
            PlotString = String.Empty
        End If
        ' Parse Season & Episode
        'Dim match As Match = Regex.Match(SeasonEpisodeText, "S(\d+),\s+Ep(\d+)")
        'Dim seasonNumber As Short
        'Dim episodeNumber As Short
        'If match.Success Then
        '    seasonNumber = CShort(match.Groups(1).Value)
        '    episodeNumber = CShort(match.Groups(2).Value)
        'Else
        '    Exit Sub
        'End If
        'Parse IMDB Id
        Dim ImdbId As String = String.Empty
        Dim match As Match = Regex.Match(EpisodeHref, "title\/(tt\d+)\/")
        If Match.Success Then
            ImdbId = Match.Groups(1).Value
        End If
        ' Parse Date
        If Not String.IsNullOrEmpty(DateString) Then
            Dim dte As Date
            If DateTime.TryParse(DateString, dte) Then
                DateString = dte.ToString("yyyy-MM-dd")
            End If
        End If

        Dim row = (From r In Episodes
                   Where r.FirstAired = DateString
                   Select r).FirstOrDefault()
        If row IsNot Nothing Then
            row.IMDB_ImdbId = ImdbId
            row.IMDB_AiredDate = DateString
            row.IMDB_Plot = PlotString.Trim
            row.IMDB_EpisodeName = EpisodeName.Trim
        Else
            Dim ep As New Episode() With {
                .IMDB_ImdbId = ImdbId,
                .IMDB_AiredDate = DateString,
                .IMDB_Plot = PlotString,
                .IMDB_EpisodeName = EpisodeName
            }
            ExtraEpisodes.Add(ep)
            'Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode),
            '                                           New Object() {Episodes, ep})
            'Episodes.Add(ep)
        End If
    End Sub

End Class
