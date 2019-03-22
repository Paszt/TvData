Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports TvData.Extensions

Public Class SundanceTvViewModel
    Inherits ViewModelBase

#Region " Class Variables "

    Private EpisodesInner As ObservableCollection(Of EpisodeSimple)
    Private EpisodesCollectionViewSource As CollectionViewSource

#End Region

#Region " Constructor "

    Public Sub New()
        StatusBarColor = CType(Application.Current.Resources("BackgroundSelected"), SolidColorBrush)
    End Sub

#End Region

#Region " Properties "

    Private _startDate As Date = Date.Now.AddDays(-14)
    Public Property StartDate As Date
        Get
            Return _startDate
        End Get
        Set(value As Date)
            SetProperty(_startDate, value)
        End Set
    End Property

    Private _endDate As Date = Date.Now.AddDays(21)
    Public Property EndDate As Date
        Get
            Return _endDate
        End Get
        Set(value As Date)
            SetProperty(_endDate, value)
        End Set
    End Property

    Public ReadOnly Property EpisodesCollectionView As ICollectionView
        Get
            If EpisodesCollectionViewSource IsNot Nothing Then
                Return EpisodesCollectionViewSource.View
            End If
            Return Nothing
        End Get
    End Property

    Private _isLoading As Boolean = False
    Public Property IsLoading As Boolean
        Get
            Return _isLoading
        End Get
        Set(value As Boolean)
            SetProperty(_isLoading, value)
        End Set
    End Property

    Public ReadOnly Property ShowNames As List(Of String)
        Get
            If EpisodesInner IsNot Nothing Then
                Return (From ei In EpisodesInner
                        Select ei.ShowName
                        Distinct).ToList()
            End If
            Return Nothing
        End Get
    End Property

    Private _statusBarColor As SolidColorBrush
    Public Property StatusBarColor As SolidColorBrush
        Get
            Return _statusBarColor
        End Get
        Set(value As SolidColorBrush)
            SetProperty(_statusBarColor, value)
        End Set
    End Property

    Private _StatusText As String = "Ready"
    Public Property StatusText As String
        Get
            Return _StatusText
        End Get
        Set(value As String)
            SetProperty(_StatusText, value)
        End Set
    End Property

    Private _overlayVisibility As Visibility = Visibility.Collapsed
    Public Property OverlayVisibility As Visibility
        Get
            Return _overlayVisibility
        End Get
        Set(value As Visibility)
            SetProperty(_overlayVisibility, value)
        End Set
    End Property

#End Region

#Region " Methods "

    Private Sub ScrapeData()
        Dim dte As Date = StartDate
        EpisodesInner = New ObservableCollection(Of EpisodeSimple)
        Do
            Dim json = String.Empty
            Using wcx As New WebClientEx
                Try
                    json = wcx.DownloadString(GetJsonUrl(dte))
                Catch ex As Exception
                End Try
            End Using
            Dim sundanceSchedule = json.FromJSON(Of SundanceTvSchedule)
            For Each item In sundanceSchedule.ScheduleItem.Where(Function(i) i.Video.New = "yes" AndAlso i.Video.EpisodeSeason <> 0)
                Dim ep As New EpisodeSimple With {
                    .ShowName = item.Video.Title,
                    .EpisodeName = item.Video.EpisodeTitle,
                    .EpisodeNumber = item.Video.EpisodeNumber,
                    .SeasonNumber = item.Video.EpisodeSeason,
                    .FirstAired = item.Video.FirstAirDate,
                    .Overview = item.Video.FullDescription}
                EpisodesInner.Add(ep)
            Next

            'Dim daysEntries = json.FromJSONArray(Of VicelandScheduleEntry)()
            'For Each daysEntry In daysEntries
            '    EpisodesInner.Add(daysEntry)
            'Next
            dte = dte.AddDays(1)
        Loop Until dte > EndDate
        InitializeEpisodesCollecionViewSource()
    End Sub

    Private Sub InitializeEpisodesCollecionViewSource()
        System.Windows.Application.Current.Dispatcher.Invoke(
                Sub()
                    EpisodesCollectionViewSource = New CollectionViewSource() With {.Source = EpisodesInner}
                    '''AddHandler EpisodesCollectionViewSource.Filter, AddressOf ScheduleEntriesCollection_Filter
                    EpisodesCollectionView.GroupDescriptions.Add(New PropertyGroupDescription("ShowName"))
                    EpisodesCollectionView.SortDescriptions.Add(New SortDescription("SeasonNumber", ListSortDirection.Ascending))
                    EpisodesCollectionView.SortDescriptions.Add(New SortDescription("EpisodeNumber", ListSortDirection.Ascending))
                    OnPropertyChanged("EpisodesCollectionView")
                    OnPropertyChanged("ShowNames")
                End Sub)
    End Sub

    Private Sub SetBusyState(Message As String)
        OverlayVisibility = Visibility.Visible
        'MainGridIsEnabled = False
        SetStatus(StatusType.Busy, Message)
    End Sub

    Private Sub SetReadyState(TypeOfStatus As StatusType, Message As String)
        OverlayVisibility = Visibility.Hidden
        'MainGridIsEnabled = True
        SetStatus(TypeOfStatus, Message)
    End Sub

    Private Sub SetStatus(TypeofStatus As StatusType, Message As String)
        Try
            Select Case TypeofStatus
                Case StatusType.Busy
                    StatusBarColor = CType(Application.Current.Resources("BusyBrush"), SolidColorBrush)
                Case StatusType.Error
                    StatusBarColor = CType(Application.Current.Resources("ErrorBrush"), SolidColorBrush)
                Case StatusType.OK
                    StatusBarColor = CType(Application.Current.Resources("BackgroundSelected"), SolidColorBrush)
            End Select
            StatusText = Message
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region " Commands "

    Public ReadOnly Property ScrapeDataCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim th As New Threading.Thread(AddressOf ScrapeData)
                                        th.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property SelectedRowsToXmlCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = EpisodesInner.Where(Function(e) e.IsSelected = True).ToList()
                    If selectedEpisodes.Count > 0 Then
                        Dim series As New TVSeries()
                        For Each ep In selectedEpisodes
                            series.Episodes.Add(ep.ToEpisode)
                        Next
                        My.Computer.Clipboard.SetText(series.ToString())
                    End If
                End Sub)
        End Get
    End Property


#End Region

#Region " Helper Functions "

    Private Function GetJsonUrl(dte As Date) As String
        'https://tribune.svc.ds.amcn.com/Sundance/OnAir/JSON?view=day&from=2018-3-1&tz=ET&bc=east&related=false&verbose=true&f=1.js
        Dim format = "https://tribune.svc.ds.amcn.com/Sundance/OnAir/JSON?view=day&from={0}&tz=ET&bc=east&related=false&verbose=true&f=1.js"
        Return String.Format(format, dte.Year.ToString("0000") & "-" & dte.Month.ToString("0") & "-" & dte.Day.ToString("0"))
    End Function

#End Region

End Class
