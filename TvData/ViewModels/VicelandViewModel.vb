Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports TvData.Extensions

Public Class VicelandViewModel
    Inherits ViewModelBase

#Region " Class Variables "

    Private ScheduleEntriesInner As ObservableCollection(Of VicelandScheduleEntry)
    Private ScheduleEntriesCollectionViewSource As CollectionViewSource

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

    Private _selectedShowName As String = Nothing
    Public Property SelectedShowName As String
        Get
            Return _selectedShowName
        End Get
        Set(value As String)
            If SetProperty(_selectedShowName, value) Then
                OnPropertyChanged("SelectedSeriesNameIsNotEmpty")
                OnFilterChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property ScheduleEntriesCollectionView As ICollectionView
        Get
            If ScheduleEntriesCollectionViewSource IsNot Nothing Then
                Return ScheduleEntriesCollectionViewSource.View
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

    Private _showRepeats As Boolean
    Public Property ShowRepeats As Boolean
        Get
            Return _showRepeats
        End Get
        Set(value As Boolean)
            If SetProperty(_showRepeats, value) Then
                OnFilterChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property SelectedSeriesNameIsNotEmpty As Boolean
        Get
            If DesignerProperties.GetIsInDesignMode(New DependencyObject()) Then
                Return True
            Else
                Return Not String.IsNullOrWhiteSpace(SelectedShowName)
            End If
        End Get
    End Property

    Public ReadOnly Property ShowNames As List(Of String)
        Get
            If ScheduleEntriesInner IsNot Nothing Then
                Return (From sei In ScheduleEntriesInner
                        Select sei.DisplayedProgramTitle
                        Distinct).ToList()
            End If
            Return Nothing
        End Get
    End Property

#End Region

#Region " Methods "

    Private Sub ScrapeData()
        Dim dte As Date = StartDate
        ScheduleEntriesInner = New ObservableCollection(Of VicelandScheduleEntry)
        Do
            Dim json = String.Empty
            Using wcx As New WebClientEx
                Try
                    json = wcx.DownloadString(GetJsonUrl(dte))
                Catch ex As Exception
                End Try
            End Using
            Dim daysEntries = json.FromJSONArray(Of VicelandScheduleEntry)()
            For Each daysEntry In daysEntries
                daysEntry.DisplayedProgramTitle = Net.WebUtility.HtmlDecode(daysEntry.DisplayedProgramTitle)
                ScheduleEntriesInner.Add(daysEntry)
            Next
            dte = dte.AddDays(1)
        Loop Until dte > EndDate
        InitializeScheduleEntriesCollecionViewSource()
    End Sub

    Private Sub InitializeScheduleEntriesCollecionViewSource()
        System.Windows.Application.Current.Dispatcher.Invoke(
                Sub()
                    ScheduleEntriesCollectionViewSource = New CollectionViewSource() With {.Source = ScheduleEntriesInner}
                    AddHandler ScheduleEntriesCollectionViewSource.Filter, AddressOf ScheduleEntriesCollection_Filter
                    ScheduleEntriesCollectionView.GroupDescriptions.Add(New PropertyGroupDescription("DisplayedProgramTitle"))
                    'ScheduleEntriesCollectionView.SortDescriptions.Add(New SortDescription("SeasonNumber", ListSortDirection.Ascending))
                    OnPropertyChanged("ScheduleEntriesCollectionView")
                    OnPropertyChanged("ShowNames")
                End Sub)
    End Sub

    Private Sub ScheduleEntriesCollection_Filter(sender As Object, e As FilterEventArgs)
        Dim entry As VicelandScheduleEntry = TryCast(e.Item, VicelandScheduleEntry)
        Dim nameMatches As Boolean
        Dim repeatStatusMatches As Boolean

        If String.IsNullOrWhiteSpace(SelectedShowName) Then
            nameMatches = True
        Else
            nameMatches = (entry.DisplayedProgramTitle.ToLower = SelectedShowName.ToLower)
        End If

        If ShowRepeats = True Then
            repeatStatusMatches = True
        Else
            repeatStatusMatches = (entry.AiringType = "NEW")
        End If

        e.Accepted = (nameMatches And repeatStatusMatches)
    End Sub

    Private Sub OnFilterChanged()
        If ScheduleEntriesCollectionViewSource IsNot Nothing AndAlso
               ScheduleEntriesCollectionViewSource.View IsNot Nothing Then
            ScheduleEntriesCollectionViewSource.View.Refresh()
        End If
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

    Public ReadOnly Property ClearSelectedSeriesName As ICommand
        Get
            Return New RelayCommand(Sub()
                                        SelectedShowName = String.Empty
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property SelectedRowsToXmlCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = ScheduleEntriesInner.Where(Function(e) e.IsSelected = True).ToList()
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
        'https://vice-videos-assets-cdn.vice.com/json/viceland/20170521.json
        Dim format = "https://vice-videos-assets-cdn.vice.com/json/viceland/{0}.json"
        Return String.Format(format, dte.Year.ToString("0000") & dte.Month.ToString("00") & dte.Day.ToString("00"))
    End Function

#End Region

End Class
