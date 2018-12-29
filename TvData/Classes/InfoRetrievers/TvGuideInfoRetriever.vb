Option Strict On
Imports System.Text.RegularExpressions
Imports CsQuery

Public Class TvGuideInfoRetriever
    Implements IInfoRetriever

    Public Event StatusChanged(TypeOfStatus As StatusType, Message As String) Implements IInfoRetriever.StatusChanged

    Public Property ExtraEpisodes As IList(Of Episode) Implements IInfoRetriever.ExtraEpisodes

    Private m_seriesName As String

    Public Sub New(SeriesName As String)
        m_seriesName = SeriesName
    End Sub

    Public Sub RetrieveInfo(TvGuideSeriesId As String,
                            episodes As TrulyObservableCollection(Of Episode),
                            Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en) Implements IInfoRetriever.RetrieveInfo
        ExtraEpisodes = New List(Of Episode)
        Dim SeasonOnSeparatePages As Boolean = True
        Dim seasons = (From eps In episodes Where CInt(eps.SeasonNumber) <> 0 Select eps.SeasonNumber Distinct).ToList()
        Dim seasonCounter As Integer = 1
        For Each season In seasons
            Dim sesonStatusFormat As String = "TV Guide Season {0} ({1} of {2}) - Downloading..."
            RaiseEvent StatusChanged(StatusType.Busy, String.Format(sesonStatusFormat, season, seasonCounter, seasons.Count))
            Dim responseString As String = String.Empty
            ParseSeason(TvGuideSeriesId, season, seasonCounter, seasons, episodes, 1)
            seasonCounter += 1
        Next

        'If SeasonOnSeparatePages = False Then
        '    ' all episodes are listed on the same page, probably no episode overview included
        '    ' this happens for less popular or older TV shows where the data isn't in the newest format
        '    RaiseEvent StatusChanged(StatusType.Busy, "Downloading from TV Guide")
        '    Dim responseString As String = String.Empty
        '    Using wcx As New WebClientEx
        '        responseString = wcx.DownloadString(TvGuideUrl(TvGuideSeriesId))
        '    End Using
        '    If Not String.IsNullOrEmpty(responseString) Then
        '        Dim htmlDoc As New HtmlAgilityPack.HtmlDocument()
        '        htmlDoc.LoadHtml(responseString)
        '        'Dim episodeDivs = htmlDoc.DocumentNode.Descendants.Where(Function(div) div.HasAttributes AndAlso
        '        '                                                                       div.Attributes("class") IsNot Nothing AndAlso
        '        '                                                                       div.Attributes("class").Value.Contains("news-content-inner-w"))

        '        Dim episodeTds = htmlDoc.DocumentNode.Descendants.Where(Function(div) div.HasAttributes AndAlso
        '                                                                              div.Attributes("class") IsNot Nothing AndAlso
        '                                                                              div.Attributes("class").Value.Contains("articleCell"))
        '        Dim episodeCounter As Integer = 1
        '        For Each episodeTd In episodeTds
        '            Dim episodeStatusformat As String = "All Seasons - Parsing Episode {0} of {1}"
        '            RaiseEvent StatusChanged(StatusType.Busy, String.Format(episodeStatusformat, New String() {CStr(episodeCounter),
        '                                                                                                                    CStr(episodeTds.Count)}))
        '            Dim seasonEpisodeText As String = episodeTd.SelectNodes("text()").Last().InnerText
        '            If Not String.IsNullOrWhiteSpace(seasonEpisodeText) Then
        '                Dim episodeName As String = String.Empty
        '                Dim airedDate As String = String.Empty
        '                Dim overView As String = String.Empty
        '                Dim episodeLink = episodeTd.SelectSingleNode("a")
        '                If episodeLink IsNot Nothing Then
        '                    Dim DateNameText As String = episodeLink.InnerText
        '                    Dim match As Match = Regex.Match(DateNameText, "(\d+\/\d+\/\d{4}): (.+)")
        '                    If match.Success Then
        '                        airedDate = match.Groups(1).Value
        '                        episodeName = match.Groups(2).Value
        '                    End If
        '                    overView = GetEpisodeOverView("http://www.tvguide.com" & episodeLink.Attributes("href").Value)
        '                End If
        '                AddData(seasonEpisodeText, episodeName, airedDate, overView, episodes)
        '            End If
        '            episodeCounter += 1
        '        Next
        '    End If
        'End If
    End Sub

    Private Sub ParseSeason(TvGuideSeriesId As String, season As Short, seasonCounter As Integer,
                            seasons As List(Of Short), episodes As TrulyObservableCollection(Of Episode), pageNumber As Integer)
        Dim responseString As String = String.Empty
        Dim url = TvGuideUrl(TvGuideSeriesId, season)
        If pageNumber > 1 Then
            url &= pageNumber & "/?ref=load-more"
        End If
        Using wcx As New WebClientEx
            Dim refererUrl = TvGuideRefererUrl(TvGuideSeriesId, season)
            wcx.DownloadString(refererUrl)
            wcx.Referer = refererUrl
            responseString = wcx.DownloadString(url)
        End Using
        If Not String.IsNullOrEmpty(responseString) Then
            Dim cqDoc = CQ.CreateDocument(responseString)
            Dim episodeDivs = cqDoc("div.tvobject-episode")
            If episodeDivs.Length > 0 Then
                episodeDivs.Each(
                    Sub(index As Integer, div As IDomObject)
                        Dim episodeStatusformat As String = "TV Guide Season {0} ({1} of {2}) - Parsing Episode {3} of {4}"
                        RaiseEvent StatusChanged(StatusType.Busy,
                                                String.Format(episodeStatusformat, New String() {CStr(season),
                                                                                                 CStr(seasonCounter),
                                                                                                 CStr(seasons.Count),
                                                                                                 CStr(index),
                                                                                                 CStr(episodeDivs.Count)}))
                        Dim episodeName As String = div.Cq.Find("h4.tvobject-episode-title a").Text()
                        Dim airedDate As String = div.Cq.Find("p.tvobject-episode-airdate:first").Text()
                        Dim overView As String = div.Cq.Find("p.tvobject-episode-description").Text()
                        Dim SeasonEpisodeText As String = div.Cq.Find("p.tvobject-episode-episodic-info").Text()
                        AddData(SeasonEpisodeText, episodeName, airedDate, overView, episodes)
                    End Sub)
            Else

            End If

            If cqDoc("[data-title='Show More']").Length > 0 Then
                ParseSeason(TvGuideSeriesId, season, seasonCounter, seasons, episodes, pageNumber + 1)
            End If



            'Dim episodeDivs = cqDoc(".row.tvobject-full-episodes")
            'If episodeDivs.Length > 0 Then
            '    episodeDivs.Each(
            '        Sub(index As Integer, div As IDomObject)
            '            Dim episodeStatusformat As String = "TV Guide Season {0} ({1} of {2}) - Parsing Episode {3} of {4}"
            '            RaiseEvent StatusChanged(StatusType.Busy,
            '                                    String.Format(episodeStatusformat, New String() {CStr(season),
            '                                                                                     CStr(seasonCounter),
            '                                                                                     CStr(seasons.Count),
            '                                                                                     CStr(index),
            '                                                                                     CStr(episodeDivs.Count)}))
            '            Dim episodeName As String = div.Cq.Find(".tvobject-full-episode-title").Text()
            '            Dim airedDate As String = div.Cq.Find(".tvobject-full-episode-date").Text()
            '            Dim overView As String = div.Cq.Find(".tvobject-full-episode-description").Text()
            '            Dim SeasonEpisodeText As String = div.Cq.Find(".tvobject-full-episode-number").Text()
            '            AddData(SeasonEpisodeText, episodeName, airedDate, overView, episodes)
            '        End Sub)
            'Else

            'End If
        End If
    End Sub

#Region " Old Code, before site update (2015-02-01)"

    'Public Sub RetrieveInfo(TvGuideSeriesId As String, episodes As IList(Of Episode)) Implements IInfoRetriever.RetrieveInfo
    '    ExtraEpisodes = New List(Of Episode)
    '    Dim SeasonOnSeparatePages As Boolean = True
    '    Dim seasons = (From eps In episodes Where CInt(eps.SeasonNumber) <> 0 Select eps.SeasonNumber Distinct).ToList()
    '    Dim seasonCounter As Integer = 1
    '    For Each season In seasons
    '        Dim sesonStatusFormat As String = "TV Guide Season {0} ({1} of {2}) - Downloading..."
    '        RaiseEvent StatusChanged(StatusType.Busy, String.Format(sesonStatusFormat, season, seasonCounter, seasons.Count))
    '        Dim responseString As String = String.Empty
    '        Using wcx As New WebClientEx
    '            responseString = wcx.DownloadString(TvGuideUrl(TvGuideSeriesId, CInt(season)))
    '        End Using
    '        If Not String.IsNullOrEmpty(responseString) Then
    '            Dim htmlDoc As New HtmlAgilityPack.HtmlDocument()
    '            htmlDoc.LoadHtml(responseString)
    '            Dim episodeDivs = htmlDoc.DocumentNode.Descendants.Where(Function(div) div.HasAttributes AndAlso
    '                                                                                   div.Attributes("class") IsNot Nothing AndAlso
    '                                                                                   div.Attributes("class").Value.Contains("news-content-inner-w"))
    '            Dim episodeCounter As Integer = 1
    '            If episodeDivs.Count > 0 Then
    '                For Each episodeDiv In episodeDivs
    '                    Dim episodeStatusformat As String = "TV Guide Season {0} ({1} of {2}) - Parsing Episode {3} of {4}"
    '                    RaiseEvent StatusChanged(StatusType.Busy, String.Format(episodeStatusformat, New String() {CStr(season),
    '                                                                                                                            CStr(seasonCounter),
    '                                                                                                                            CStr(seasons.Count),
    '                                                                                                                            CStr(episodeCounter),
    '                                                                                                                            CStr(episodeDivs.Count)}))
    '                    Dim SeasonEpisodeNode As HtmlNode = episodeDiv.SelectSingleNode("h2/text()")
    '                    Dim episodeName As String
    '                    Dim airedDate As String
    '                    Dim overView As String = String.Empty
    '                    If SeasonEpisodeNode IsNot Nothing Then
    '                        Dim SeasonEpisodeText As String = SeasonEpisodeNode.InnerText.Trim()
    '                        episodeName = episodeDiv.SelectSingleNode("h2/a").Text.Trim()
    '                        airedDate = episodeDiv.SelectSingleNode("ul/li").InnerHtml.Trim()
    '                        'overView = episodeDiv.SelectSingleNode("p").SelectSingleNode("text()").InnerText
    '                        Dim paragraphs = episodeDiv.SelectNodes("p")
    '                        If paragraphs.Count = 1 Then
    '                            If paragraphs(0).SelectNodes("p") IsNot Nothing Then
    '                                For Each p As HtmlNode In paragraphs(0).SelectNodes("p")
    '                                    overView += p.TextExceptClass("rednav") & Environment.NewLine
    '                                Next
    '                            Else
    '                                overView = paragraphs(0).SelectSingleNode("text()").InnerText
    '                            End If
    '                        Else
    '                            For Each paragraph As HtmlNode In episodeDiv.SelectNodes("p")
    '                                overView += paragraph.TextExceptClass("rednav") & Environment.NewLine
    '                            Next
    '                        End If
    '                        AddData(SeasonEpisodeText, episodeName, airedDate, overView, episodes)
    '                    End If
    '                Next
    '                episodeCounter += 1
    '            Else
    '                ' the episodes are listed all on one page in a different format, 
    '                '  happens for older or less popular shows where data isn't in newest form
    '                SeasonOnSeparatePages = False
    '                Exit For
    '            End If
    '        End If
    '        seasonCounter += 1
    '    Next

    '    If SeasonOnSeparatePages = False Then
    '        ' all episodes are listed on the same page, probably no episode overview included
    '        ' this happens for less popular or older TV shows where the data isn't in the newest format
    '        RaiseEvent StatusChanged(StatusType.Busy, "Downloading from TV Guide")
    '        Dim responseString As String = String.Empty
    '        Using wcx As New WebClientEx
    '            responseString = wcx.DownloadString(TvGuideUrl(TvGuideSeriesId))
    '        End Using
    '        If Not String.IsNullOrEmpty(responseString) Then
    '            Dim htmlDoc As New HtmlAgilityPack.HtmlDocument()
    '            htmlDoc.LoadHtml(responseString)
    '            'Dim episodeDivs = htmlDoc.DocumentNode.Descendants.Where(Function(div) div.HasAttributes AndAlso
    '            '                                                                       div.Attributes("class") IsNot Nothing AndAlso
    '            '                                                                       div.Attributes("class").Value.Contains("news-content-inner-w"))

    '            Dim episodeTds = htmlDoc.DocumentNode.Descendants.Where(Function(div) div.HasAttributes AndAlso
    '                                                                                  div.Attributes("class") IsNot Nothing AndAlso
    '                                                                                  div.Attributes("class").Value.Contains("articleCell"))
    '            Dim episodeCounter As Integer = 1
    '            For Each episodeTd In episodeTds
    '                Dim episodeStatusformat As String = "All Seasons - Parsing Episode {0} of {1}"
    '                RaiseEvent StatusChanged(StatusType.Busy, String.Format(episodeStatusformat, New String() {CStr(episodeCounter),
    '                                                                                                                        CStr(episodeTds.Count)}))
    '                Dim seasonEpisodeText As String = episodeTd.SelectNodes("text()").Last().InnerText
    '                If Not String.IsNullOrWhiteSpace(seasonEpisodeText) Then
    '                    Dim episodeName As String = String.Empty
    '                    Dim airedDate As String = String.Empty
    '                    Dim overView As String = String.Empty
    '                    Dim episodeLink = episodeTd.SelectSingleNode("a")
    '                    If episodeLink IsNot Nothing Then
    '                        Dim DateNameText As String = episodeLink.InnerText
    '                        Dim match As Match = Regex.Match(DateNameText, "(\d+\/\d+\/\d{4}): (.+)")
    '                        If match.Success Then
    '                            airedDate = match.Groups(1).Value
    '                            episodeName = match.Groups(2).Value
    '                        End If
    '                        overView = GetEpisodeOverView("http://www.tvguide.com" & episodeLink.Attributes("href").Value)
    '                    End If
    '                    AddData(seasonEpisodeText, episodeName, airedDate, overView, episodes)
    '                End If
    '                episodeCounter += 1
    '            Next
    '        End If
    '    End If
    'End Sub

#End Region

    Private Sub AddData(SeasonEpisodeText As String, episodeName As String, airedDate As String, overView As String, Episodes As TrulyObservableCollection(Of Episode))
        ' Parse Season & Episode
        Dim match As Match = Regex.Match(SeasonEpisodeText, "Season\s+(\d+),\s+Episode\s+(\d+)")
        Dim seasonNumber As Short
        Dim episodeNumber As Short
        If match.Success Then
            seasonNumber = CShort(match.Groups(1).Value)
            episodeNumber = CShort(match.Groups(2).Value)
        Else
            Exit Sub
        End If
        If episodeName.Trim() = m_seriesName Then
            episodeName = "Episode " & episodeNumber.ToString()
        End If
        ' Parse Date
        If Not String.IsNullOrEmpty(airedDate) Then
            Dim dte As Date
            If DateTime.TryParse(airedDate, dte) Then
                airedDate = dte.ToString("yyyy-MM-dd")
            ElseIf DateTime.TryParse(airedDate.Substring(0, airedDate.Length - 3), dte) Then
                airedDate = dte.ToString("yyyy-MM-dd")
            End If
        End If
        Dim row = (From r In Episodes
                   Where r.SeasonNumber = seasonNumber AndAlso
                         r.EpisodeNumber = episodeNumber
                   Select r).FirstOrDefault()
        If row IsNot Nothing Then
            row.TvGuide_AiredDate = airedDate
            row.TvGuide_Overview = overView.Trim()
            row.TvGuide_EpisodeName = episodeName.Trim
        Else
            Dim ep As New Episode() With {
                .EpisodeNumber = episodeNumber,
                .SeasonNumber = seasonNumber,
                .TvGuide_AiredDate = airedDate,
                .TvGuide_Overview = overView.Trim(),
                .TvGuide_EpisodeName = episodeName
            }
            ExtraEpisodes.Add(ep)
            'Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Episodes, ep})
            'Episodes.Add(ep)
        End If
    End Sub

    'Private Function GetEpisodeOverView(url As String) As String
    '    Dim returnVal As String = String.Empty

    '    Dim responseString As String = String.Empty
    '    Using wcx As New WebClientEx
    '        responseString = wcx.DownloadString(url)
    '    End Using
    '    If Not String.IsNullOrEmpty(responseString) Then
    '        Dim htmlDoc As New HtmlAgilityPack.HtmlDocument()
    '        htmlDoc.LoadHtml(responseString)
    '        Dim episodeNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='obj-lb-w obj-episodedetail']")
    '        If episodeNode IsNot Nothing Then
    '            Dim overViewNode = episodeNode.SelectSingleNode("div[@class='obj-lb-c']")
    '            If overViewNode IsNot Nothing Then
    '                Return overViewNode.InnerText.Trim()
    '            End If
    '        End If

    '        'returnVal = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='obj-lb-w obj-episodedetail']").SelectSingleNode("div[@class='obj-lb-c']").InnerText.Trim()
    '    End If
    '    Return returnVal
    'End Function

    Private Function TvGuideUrl(TvGuideSeriesId As String, season As Integer) As String
        'http://www.tvguide.com/tvshows/game-of-thrones/episodes-season-4/305628
        'Dim format As String = "http://www.tvguide.com/tvshows/{0}/episodes-season-{1}/{2}"
        'Return String.Format(format, SeriesNameUrl(), season, TvGuideSeriesId)

        'http://www.tvguide.com/shows/game-of-thrones-305628/episodes/season-1/
        'Dim format = "http://www.tvguide.com/shows/{0}-{1}/episodes/season-{2}/"
        'Return String.Format(format, SeriesNameUrl(), TvGuideSeriesId, season.ToString())

        'http://www.tvguide.com/tvshows/paw-patrol/episodes-season-2/577698/
        Dim format = "http://www.tvguide.com/tvshows/{0}/episodes-season-{1}/{2}/"
        Return String.Format(format, SeriesNameSlug(), season.ToString(), TvGuideSeriesId)

        'http://www.tvguide.com/optimizely/view/original/tvobject/406308/season/2
        'Dim format = "http://www.tvguide.com/optimizely/view/original/tvobject/{0}/season/{1}"
        'Return String.Format(format, TvGuideSeriesId, season.ToString())

    End Function

    Private Function TvGuideRefererUrl(TvGuideSeriesId As String, season As Integer) As String
        'http://www.tvguide.com/tvshows/paw-patrol/episodes-season-2/577698/
        'Dim format = "http://www.tvguide.com/tvshows/{0}/episodes-season-{1}/{2}/"
        'Return String.Format(format, SeriesNameSlug(), season.ToString(), TvGuideSeriesId)

        Dim format = "http://www.tvguide.com/tvshows/{0}/{1}/"
        Return String.Format(format, SeriesNameSlug(), TvGuideSeriesId)

    End Function

    'Private Function TvGuideUrl(TvGuideSeriesId As String) As String
    '    'http://www.tvguide.com/tvshows/great-performances-at-the-met/episodes/285612
    '    Dim format As String = "http://www.tvguide.com/tvshows/{0}/episodes/{1}"
    '    Return String.Format(format, SeriesNameSlug(), TvGuideSeriesId)
    'End Function

    Private Function SeriesNameSlug() As String
        Dim slug = m_seriesName.ToLower()
        slug = Regex.Replace(slug, "[^a-z0-9\s-]", " ")
        slug = Regex.Replace(slug, "\s+", " ").Trim()
        slug = slug.Replace(" ", "-")
        Return slug
    End Function

End Class
