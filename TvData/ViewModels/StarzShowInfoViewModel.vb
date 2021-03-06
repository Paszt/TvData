﻿Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.Serialization
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports TvData
Imports TvData.Extensions

Public Class StarzShowInfoViewModel
    Inherits ViewModelBase

    Public Sub New()
        GetShowList()
        ShowListView = New ListCollectionView(_showList) With {
            .Filter = New Predicate(Of Object)(
            Function(stid As Object)
                Dim shwTitlId As ShowTitleId = CType(stid, ShowTitleId)
                Return shwTitlId.Id.HasValue = True
            End Function)
        }
        ShowListView.SortDescriptions.Add(New SortDescription("Title", ListSortDirection.Ascending))

        ShowListManageView = New ListCollectionView(_showList)
        ShowListManageView.SortDescriptions.Add(New SortDescription("Title", ListSortDirection.Ascending))

        SeasonInfos = New ObservableCollection(Of StarzSeasonInformation)

        _episodes = New ObservableCollection(Of StarzEpisodeInfo)
        EpisodesCollectionView = New ListCollectionView(_episodes)
        EpisodesCollectionView.SortDescriptions.Add(New SortDescription("Number", ListSortDirection.Ascending))
    End Sub

    Private _showList As ObservableCollection(Of ShowTitleId)
    Private _episodes As ObservableCollection(Of StarzEpisodeInfo)

#Region " Properties "

    Private _showTitleToAdd As String
    Public Property ShowTitleToAdd As String
        Get
            Return _showTitleToAdd
        End Get
        Set(value As String)
            SetProperty(_showTitleToAdd, value)
        End Set
    End Property

    Public Property ShowListView As ListCollectionView

    Public Property ShowListManageView As ListCollectionView

    Public Property EpisodesCollectionView As ListCollectionView

    Private _selectedShow As ShowTitleId
    Public Property SelectedShow As ShowTitleId
        Get
            Return _selectedShow
        End Get
        Set(value As ShowTitleId)
            If SetProperty(_selectedShow, value) Then
                If SeasonInfos IsNot Nothing Then
                    SeasonInfos.Clear()
                End If
                OnPropertyChanged("SeasonInfos")
            End If
        End Set
    End Property

    Private _seasonInfos As ObservableCollection(Of StarzSeasonInformation)
    Public Property SeasonInfos As ObservableCollection(Of StarzSeasonInformation)
        Get
            Return _seasonInfos
        End Get
        Set(value As ObservableCollection(Of StarzSeasonInformation))
            SetProperty(_seasonInfos, value)
        End Set
    End Property

    Private _selectedSeason As StarzSeasonInformation
    Public Property SelectedSeason As StarzSeasonInformation
        Get
            Return _selectedSeason
        End Get
        Set(value As StarzSeasonInformation)
            If SetProperty(_selectedSeason, value) Then
                OnSelectedSeasonChanged()
            End If
        End Set
    End Property

    Private _ManageShowsVisibility As Visibility = Visibility.Hidden
    Public Property ManageShowsVisibility As Visibility
        Get
            Return _ManageShowsVisibility
        End Get
        Set(value As Visibility)
            SetProperty(_ManageShowsVisibility, value)
        End Set
    End Property

#End Region

#Region " Commands "

    Public ReadOnly Property AddShowCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim result = GetShowId(ShowTitleToAdd)
                    If Not result.Id.HasValue Then
                        If MessageWindow.ShowDialog("Unable to get ID. " & result.Message & Environment.NewLine &
                                                    "Click OK to add it to the list to check later.",
                                                    "Show ID Not Found", True) = False Then
                            Exit Sub
                        End If
                    End If
                    Dim newShow = New ShowTitleId() With {.Id = result.Id, .Title = ShowTitleToAdd}
                    _showList.Add(newShow)
                    SaveShowList()
                    ShowTitleToAdd = String.Empty
                    If result.Id.HasValue Then
                        SelectedShow = newShow
                    End If
                End Sub,
                Function() As Boolean
                    Return Not String.IsNullOrWhiteSpace(ShowTitleToAdd)
                End Function)
        End Get
    End Property

    Public ReadOnly Property DownloadSeasonInfoCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    If SeasonInfos IsNot Nothing Then
                        SeasonInfos.Clear()
                    End If
                    'get number of seasons
                    Dim json = WebResources.DownloadString("https://www.starz.com/api/model.json?paths=[[""contentById""," & SelectedShow.Id &
                                                           ",""childContent"",""length""]]&method=get")
                    Dim result As JObject = JObject.Parse(json)
                    Dim numberOfSeasons = CInt(DirectCast(result.SelectToken("jsonGraph.contentById").First.First.First.First.First.First("value"), JValue).Value)

                    'get season ids
                    json = WebResources.DownloadString("https://www.starz.com/api/model.json?paths=[[""contentById""," & SelectedShow.Id &
                                                           ",""childContent"",{""from"":0,""to"":" & numberOfSeasons - 1 & "},""contentId""]]&method=get")
                    result = JObject.Parse(json)
                    Dim contentById As JToken = result.SelectToken("jsonGraph.contentById")
                    Dim seasonNoCounter As Integer = 1
                    For Each propertyValue In CType(contentById, JObject).PropertyValues
                        Dim seasonId = propertyValue.First.First.Value(Of Integer)("value")
                        If seasonId > 0 Then
                            SeasonInfos.Add(New StarzSeasonInformation() With {.Id = seasonId, .Number = seasonNoCounter})
                            seasonNoCounter += 1
                        End If
                    Next

                    'get season Information (number of episodes and title)
                    Dim seasonNosJoined As String = String.Join(",", (From s In SeasonInfos Select s.Id))
                    json = WebResources.DownloadString("https://www.starz.com/api/model.json?paths=[[""contentById"",[" & seasonNosJoined &
                                                       "],[""childContent"",""title""],""length""]]&method=get")
                    result = JObject.Parse(json)
                    contentById = result.SelectToken("jsonGraph")
                    Dim sInfos = JsonConvert.DeserializeObject(Of StarzSeasonInfos)(contentById.ToString())
                    For Each dictEntry In sInfos.ContentById
                        Dim sNumber = CInt(dictEntry.Key)
                        Dim si = SeasonInfos.Where(Function(s) s.Id.HasValue AndAlso s.Id.Value = sNumber).First()
                        si.Title = dictEntry.Value.Title.Value
                        si.NumberOfEpisodes = dictEntry.Value.ChildContent.Length.Value
                    Next

                    If SeasonInfos.Count > 0 Then
                        SelectedSeason = SeasonInfos.Last()
                    End If
                End Sub)
        End Get
    End Property

    Public ReadOnly Property DisplayManageShowsCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        ManageShowsVisibility = Visibility.Visible
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideManageShowsCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        ManageShowsVisibility = Visibility.Hidden
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property DownloadSeasonEpisodesCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    If _episodes Is Nothing Then
                        _episodes = New ObservableCollection(Of StarzEpisodeInfo)
                    Else
                        _episodes.Clear()
                    End If
                    Dim json = WebResources.DownloadString("https://www.starz.com/api/model.json?paths=[[""contentById""," & SelectedSeason.Id &
                                                       ",""childContent"",{""from"":0,""to"":" & SelectedSeason.NumberOfEpisodes - 1 &
                                                       "},[""contentId"",""contentType"",""images"",""logLine"",""order"",""properCaseTitle"",""releaseYear"",""startDate"",""title""]]]&method=get")
                    Dim result As JObject = JObject.Parse(json)
                    Dim contentById As JToken = result.SelectToken("jsonGraph.contentById")
                    Dim eps = CType(contentById, JObject).PropertyValues.Select(Function(o) o.ToObject(Of StarzEpisodeInfo))
                    For Each ep In eps
                        If ep.ProperCaseTitle IsNot Nothing Then
                            _episodes.Add(ep)
                        End If
                    Next
                    ''EpisodesCollectionView.Refresh()
                End Sub,
                Function() As Boolean
                    Return SelectedSeason IsNot Nothing
                End Function)
        End Get
    End Property

    Public ReadOnly Property ViewSeasonJsonCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim url = "https://www.starz.com/api/model.json?paths=[[""contentById""," & SelectedSeason.Id &
                    ",""childContent"",{""from"":0,""to"":" & SelectedSeason.NumberOfEpisodes - 1 &
                    "},[""contentType"",""images"",""logLine"",""properCaseTitle"",""startDate""]]]&method=get"
                    url = url.Replace("""", "%22")
                    Process.Start(url)
                End Sub,
                Function() As Boolean
                    Return SelectedSeason IsNot Nothing
                End Function)
        End Get
    End Property

    Public ReadOnly Property UpdateShowTitleIdCommand As ICommand
        Get
            Return New RelayCommand(Of ShowTitleId)(
                Sub(show As ShowTitleId)
                    Dim result = GetShowId(show.Title)
                    If Not result.Id.HasValue Then
                        MessageWindow.ShowDialog("Unable to get ID. " & result.Message, "Show ID Not Found")
                        Exit Sub
                    End If
                    show.Id = result.Id
                    SaveShowList()
                    ShowListManageView.Refresh()
                    ShowListView.Refresh()
                    If result.Id.HasValue Then
                        SelectedShow = show
                    End If
                End Sub)
        End Get
    End Property

    Public ReadOnly Property RemoveShowTitleIdCommand As ICommand
        Get
            Return New RelayCommand(Of ShowTitleId)(
                Sub(show As ShowTitleId)
                    _showList.Remove(show)
                    SaveShowList()
                End Sub)
        End Get
    End Property

    Public ReadOnly Property ViewEpisodeWebpageCommand As ICommand
        Get
            Return New RelayCommand(Of StarzEpisodeInfo)(
                Sub(starzEp As StarzEpisodeInfo)
                    Dim epUrl = "https://www.starz.com/series/" & SelectedShow.Id & "/episodes/" & starzEp.ContentId.Value & "/details"
                    Process.Start(epUrl)
                End Sub)
        End Get
    End Property

    Public ReadOnly Property ViewShowWebpageCommand As ICommand
        Get
            Return New RelayCommand(Of ShowTitleId)(
                Sub(starzShow As ShowTitleId)
                    Dim showUrl = "https://www.starz.com/series/" & starzShow.Id
                    Process.Start(showUrl)
                End Sub)
        End Get
    End Property

#End Region

#Region " Private Methods "

    Private Sub GetShowList()
        _showList = New ObservableCollection(Of ShowTitleId)(My.Settings.StarzShows.FromJSONArray(Of ShowTitleId).OrderBy(Function(st) st.Title))
        SelectedShow = _showList.FirstOrDefault()
    End Sub

    Private Sub SaveShowList()
        My.Settings.StarzShows = _showList.ToJSONArray()
        My.Settings.Save()
    End Sub

    Private Function GetShowId(showTitle As String) As GetShowIdResult
        Dim returnValue As New GetShowIdResult()
        Dim EpisodeListUrl = "https://www.starz.com/series/" & showTitle.ToVanitySlug() & "/episodes"
        Dim html = String.Empty
        Try
            html = WebResources.DownloadString(EpisodeListUrl)
        Catch ex As Exception
            returnValue.Message = ex.Message
            'MessageWindow.ShowDialog(ex.Message, "Error Getting Show Id")
            'Return Nothing
        End Try

        If Not String.IsNullOrEmpty(html) Then
            Dim doc = CsQuery.CQ.CreateDocument(html)
            Dim canonicalLink = doc.Find("link[rel=canonical]").Attr("href")
            Dim showIdMatch = Regex.Match(canonicalLink, "\/series\/(\d+)\/", RegexOptions.IgnoreCase)
            If showIdMatch.Success Then
                returnValue.Id = CInt(showIdMatch.Groups(1).Value)
                'Return CInt(showIdMatch.Groups(1).Value)
            Else
                returnValue.Message = "Could not find ID in the HTML on the Starz website."
                'MessageWindow.ShowDialog("Unable to find show id while scraping Starz website.", "Unable to find show Id")
                'Return Nothing
            End If
        End If
        Return returnValue
    End Function

    Private Sub OnSelectedSeasonChanged()
        If _episodes IsNot Nothing Then
            _episodes.Clear()
        End If
    End Sub

#End Region

#Region " Data Structures"

#Region " Seasons "

    <DataContract>
    Public Class StarzSeasonInfos
        <JsonProperty("contentById")>
        Public Property ContentById As Dictionary(Of String, SeasonInfo)
    End Class

    <DataContract>
    Public Class SeasonInfo
        <JsonProperty("childContent")>
        Public Property ChildContent As ChildContentLength

        <JsonProperty("title")>
        Public Property Title As AtomString
    End Class

    <DataContract>
    Public Class ChildContentLength
        <JsonProperty("length")>
        Public Property Length As AtomInteger
    End Class

#End Region

#Region " Episodes "

    <DataContract>
    Public Class StarzEpisodeInfo

        <JsonProperty("contentId")>
        Public Property ContentId As AtomInteger

        <JsonProperty("contentType")>
        Public Property ContentType As AtomString

        <JsonProperty("logLine")>
        Public Property LogLine As AtomString

        <JsonProperty("order")>
        Public Property Order As AtomInteger

        <JsonProperty("properCaseTitle")>
        Public Property ProperCaseTitle As AtomString ' "Ep 201 - Inside Out"

        <JsonProperty("releaseYear")>
        Public Property ReleaseYear As AtomString

        <JsonProperty("startDate")>
        Public Property StartDate As AtomString

        <JsonProperty("title")>
        Public Property Title As AtomString ' "Counterpart: Ep 201 - Inside Out"

        <JsonProperty("images")>
        Public Property Images As AtomImages

        Private ReadOnly titleRegex As Regex = New Regex("Ep (?<season>\d+)(?<episode>\d{2}) - (?<title>.+)")

        <JsonIgnore>
        Public ReadOnly Property Number As Integer
            Get
                If ProperCaseTitle Is Nothing Then Return Nothing
                Dim titleMatch = titleRegex.Match(ProperCaseTitle.Value)
                If titleMatch.Success Then
                    Return CInt(titleMatch.Groups("episode").Value)
                End If
                Return -1
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property SeasonNumber As Integer
            Get
                If ProperCaseTitle Is Nothing Then Return Nothing
                Dim titleMatch = titleRegex.Match(ProperCaseTitle.Value)
                If titleMatch.Success Then
                    Return CInt(titleMatch.Groups("season").Value)
                End If
                Return -1
            End Get
        End Property

        <JsonIgnore>
        Public ReadOnly Property EpisodeTitle As String
            Get
                If ProperCaseTitle Is Nothing Then Return Nothing
                Dim titleMatch = titleRegex.Match(ProperCaseTitle.Value)
                If titleMatch.Success Then
                    Return titleMatch.Groups("title").Value
                End If
                Return String.Empty
            End Get
        End Property

        Private _firstAired As String = Nothing
        <JsonIgnore>
        Public ReadOnly Property FirstAired As String
            Get
                If String.IsNullOrEmpty(_firstAired) Then
                    If StartDate Is Nothing Then Return Nothing
                    If String.IsNullOrEmpty(StartDate.Value) Then
                        Dim json = WebResources.DownloadString("https://www.starz.com/api/schedule/search/" & ContentId.Value)
                        Dim scheduleItem = JsonConvert.DeserializeObject(Of StarzScheduleItem)(json)
                        Dim dte As Date
                        If Date.TryParse(scheduleItem.Start, dte) Then
                            _firstAired = dte.ToIso8601DateString
                        Else
                            _firstAired = scheduleItem.Start
                        End If
                    Else
                        Dim dte As Date
                        If Date.TryParse(StartDate.Value, dte) Then
                            _firstAired = dte.ToIso8601DateString()
                        Else
                            _firstAired = StartDate.Value
                        End If
                    End If
                End If
                Return _firstAired
            End Get
        End Property

    End Class

    <DataContract>
    Public Class AtomImages

        <JsonProperty("value")>
        Public Property Value As StarzImages

    End Class

    <DataContract>
    Public Class StarzImages

        Private _landscapeBg As String

        <JsonProperty("landscapeBg")>
        Public Property LandscapeBg As String
            Get
                Return _landscapeBg
            End Get
            Set(value As String)
                _landscapeBg = value.Replace("https", "http")
            End Set
        End Property

    End Class

#End Region

    <DataContract>
    Public Class AtomInteger
        <JsonProperty("value")>
        Public Property Value As Integer
    End Class

    <DataContract>
    Public Class AtomString
        <JsonProperty("value")>
        Public Property Value As String
    End Class

    <DataContract>
    Public Class StarzScheduleItem

        <JsonProperty("start")>
        Public Property Start As String

        <JsonProperty("end")>
        Public Property [End] As String

    End Class

#End Region

    Public Class StarzSeasonInformation

        Public Property Number As Integer?
        Public Property Id As Integer?
        Public Property Title As String
        Public Property NumberOfEpisodes As Integer?

        Public ReadOnly Property DisplayTitle As String
            Get
                Return Title & " {" & Id & "}" & " (" & NumberOfEpisodes & " Episodes)"
            End Get
        End Property

    End Class

    Public Class GetShowIdResult
        Public Property Id As Integer?
        Public Property Message As String
    End Class

End Class

Public Class ShowTitleId

    Public Property Title As String
    Public Property Id As Integer?

End Class

Public Class ShowTitleIdSampleData

    Public Property ShowListManageView As IList(Of ShowTitleId)

    Public Sub New()
        ShowListManageView = New List(Of ShowTitleId) From {
            New ShowTitleId With {.Id = 12345, .Title = "Test Show 1"},
            New ShowTitleId With {.Title = "Test Show 2"},
            New ShowTitleId With {.Id = 12346, .Title = "Test Show 3"}
        }
    End Sub

End Class

Public Class StarzEpisodesSampleData

    Public Property EpisodesCollectionView As IList(Of StarzShowInfoViewModel.StarzEpisodeInfo)

    Public Sub New()
        EpisodesCollectionView = New List(Of StarzShowInfoViewModel.StarzEpisodeInfo) From {
            New StarzShowInfoViewModel.StarzEpisodeInfo With {
                .Images = New StarzShowInfoViewModel.AtomImages() With {.Value = New StarzShowInfoViewModel.StarzImages() With {.LandscapeBg = "http://stz1.imgix.net/web/contentId/43919/type/STUDIO/dimension/2560x1440"}},
                .ProperCaseTitle = New StarzShowInfoViewModel.AtomString() With {.Value = "Ep 201 - House On The Rock"},
                .StartDate = New StarzShowInfoViewModel.AtomString() With {.Value = "03/10/2019 00:00:00"},
                .LogLine = New StarzShowInfoViewModel.AtomString() With {.Value = "Following the epic showdown at Easter's party, Mr. Wednesday continues his quest to pitch the case for war to the Old Gods. Meanwhile, Mr. World plans revenge and Technical Boy goes on the hunt for Media."}
            },
            New StarzShowInfoViewModel.StarzEpisodeInfo With {
                .Images = New StarzShowInfoViewModel.AtomImages() With {.Value = New StarzShowInfoViewModel.StarzImages() With {.LandscapeBg = "http://stz1.imgix.net/web/contentId/43945/type/STUDIO/dimension/2560x1440"}},
                .ProperCaseTitle = New StarzShowInfoViewModel.AtomString() With {.Value = "Ep 202 - The Beguiling Man"},
                .StartDate = New StarzShowInfoViewModel.AtomString() With {.Value = "03/17/2019 00:00:00"},
                .LogLine = New StarzShowInfoViewModel.AtomString() With {.Value = "Promising vengeance for the death of a beloved old god, Mr. Wednesday begins preparation for a great battle. Meanwhile Laura and Mad Sweeney chase Shadow's diminishing light after he disappears."}
            },
            New StarzShowInfoViewModel.StarzEpisodeInfo With {
                .Images = New StarzShowInfoViewModel.AtomImages() With {.Value = New StarzShowInfoViewModel.StarzImages() With {.LandscapeBg = "http://stz1.imgix.net/web/contentId/43966/type/STUDIO/dimension/2560x1440"}},
                .ProperCaseTitle = New StarzShowInfoViewModel.AtomString() With {.Value = "Ep 203 - Muninn"},
                .StartDate = New StarzShowInfoViewModel.AtomString() With {.Value = "03/24/2019 00:00:00"},
                .LogLine = New StarzShowInfoViewModel.AtomString() With {.Value = "As he is tracked by Mr. World, Shadow makes his way to Cairo, thanks to a ride from Sam Black Crow. Mr. Wednesday slyly gains Laura's help in forging an alliance with a powerful god."}
            },
            New StarzShowInfoViewModel.StarzEpisodeInfo With {
                .Images = New StarzShowInfoViewModel.AtomImages() With {.Value = New StarzShowInfoViewModel.StarzImages() With {.LandscapeBg = "http://stz1.imgix.net/web/contentId/43987/type/STUDIO/dimension/2560x1440"}},
                .ProperCaseTitle = New StarzShowInfoViewModel.AtomString() With {.Value = "Ep 204 - The Greatest Story Ever Told"},
                .StartDate = New StarzShowInfoViewModel.AtomString() With {.Value = "03/31/2019 00:00:00"},
                .LogLine = New StarzShowInfoViewModel.AtomString() With {.Value = "While Shadow and Mr. Wednesday take a secret meeting in St. Louis, Bilquis arrives at the funeral home in Cairo, where she engages in a debate with Mr. Nancy and Mr. Ibis. Laura rejoins Mad Sweeney."}
            },
            New StarzShowInfoViewModel.StarzEpisodeInfo With {
                .Images = New StarzShowInfoViewModel.AtomImages() With {.Value = New StarzShowInfoViewModel.StarzImages() With {.LandscapeBg = "http://stz1.imgix.net/web/contentId/44077/type/STUDIO/dimension/2560x1440"}},
                .ProperCaseTitle = New StarzShowInfoViewModel.AtomString() With {.Value = "Ep 205 - The Ways Of The Dead"},
                .StartDate = New StarzShowInfoViewModel.AtomString() With {.Value = "04/07/2019 00:00:00"},
                .LogLine = New StarzShowInfoViewModel.AtomString() With {.Value = "Steeped in Cairo's history, Shadow learns the ways of the dead with the help of Mr. Ibis and Mr. Nancy. In New Orleans, Mad Sweeney introduces Laura to old friends who share their world of voodoo healing."}
            },
            New StarzShowInfoViewModel.StarzEpisodeInfo With {
                .Images = New StarzShowInfoViewModel.AtomImages() With {.Value = New StarzShowInfoViewModel.StarzImages() With {.LandscapeBg = "http://stz1.imgix.net/web/contentId/44117/type/STUDIO/dimension/2560x1440"}},
                .ProperCaseTitle = New StarzShowInfoViewModel.AtomString() With {.Value = "Ep 206 - Donar The Great"},
                .StartDate = New StarzShowInfoViewModel.AtomString() With {.Value = "04/14/2019 00:00:00"},
                .LogLine = New StarzShowInfoViewModel.AtomString() With {.Value = "Shadow and Mr. Wednesday seek out Dvalin to repair the Gungnir spear. But before the dwarf is able to etch the runes of war, he requires a powerful artifact in exchange."}
            }
        }
    End Sub


End Class
