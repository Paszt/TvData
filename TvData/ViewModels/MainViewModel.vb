Option Strict On

Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization
Imports System.Net
Imports System.Threading
Imports System.IO.Compression
Imports System.Collections.ObjectModel
Imports System.Text.RegularExpressions
Imports System.Windows.Threading
Imports TvData.Infrastructure
Imports TvData

Public Class MainViewModel
    Implements INotifyPropertyChanged, IFileDragDropTarget
    Private tmdb As TmdbApi
    Private WithEvents _infoRetriever As IInfoRetriever

#Region " Properties "

#Region " Data Properties "

    Private WithEvents DataField As TVSeries
    Public Property Data As TVSeries
        Get
            Return DataField
        End Get
        Set(value As TVSeries)
            DataField = value
            'OnEpisodesChanged()
            RaisePropertyChanged("Data")
            RaisePropertyChanged("XmlText")
        End Set
    End Property

    Private Sub DataChanged(sender As Object, e As PropertyChangedEventArgs) Handles DataField.PropertyChanged
        If Data.SeriesInfo IsNot Nothing AndAlso IsNumeric(Data.SeriesInfo.id) Then
            TvdbLabelCursor = Cursors.Hand
        Else
            TvdbLabelCursor = Nothing
        End If
        RaisePropertyChanged("Data")
        RaisePropertyChanged("XmlText")
        If e.PropertyName = "FileName" Then
            RaisePropertyChanged("WindowTitle")
        End If

        'If e.PropertyName = "Episodes" Then
        '    Dim IsAtLeastOneEpisodeSelected As Boolean = ((From ep In Data.Episodes Where ep.IsSelected = True).Count > 0)
        'End If
    End Sub

    'Private EpisodesCollectionViewField As ICollectionView
    'Public Property EpisodesCollectionView As ICollectionView
    '    Get
    '        Return EpisodesCollectionViewField
    '    End Get
    '    Set(value As ICollectionView)
    '        EpisodesCollectionViewField = value
    '        EpisodesCollectionViewField.GroupDescriptions.Add(New PropertyGroupDescription("SeasonNumber"))
    '        EpisodesCollectionViewField.SortDescriptions.Add(New SortDescription("SeasonNumber", ListSortDirection.Ascending))
    '        EpisodesCollectionViewField.SortDescriptions.Add(New SortDescription("EpisodeNumber", ListSortDirection.Ascending))
    '        'OnEpisodesChanged()
    '        RaisePropertyChanged("EpisodesCollectionView")
    '        RaisePropertyChanged("XmlText")
    '    End Set
    'End Property

    'Private Sub OnEpisodesChanged()
    '    EpisodesCollectionView = CollectionViewSource.GetDefaultView(Data.Episodes)
    '    EpisodesCollectionView.GroupDescriptions.Add(New PropertyGroupDescription("SeasonNumber"))
    '    EpisodesCollectionView.SortDescriptions.Add(New SortDescription("SeasonNumber", ListSortDirection.Ascending))
    '    EpisodesCollectionView.SortDescriptions.Add(New SortDescription("EpisodeNumber", ListSortDirection.Ascending))
    'End Sub

    Public ReadOnly Property XmlText As String
        Get
            Return Data.ToString()
            'RaisePropertyChanged("XmlText")
        End Get
    End Property

    Private languageField As Iso639_1 = Iso639_1.en
    Public Property Language As Iso639_1
        Get
            Return languageField
        End Get
        Set(value As Iso639_1)
            languageField = value
            RaisePropertyChanged("Language")
        End Set
    End Property

    Public ReadOnly Property Iso639_1Values() As IEnumerable(Of Iso639_1)
        Get
            Return [Enum].GetValues(GetType(Iso639_1)).Cast(Of Iso639_1)()
        End Get
    End Property

    Private TmdbSearchResultsField As List(Of TmdbApi.SearchPage.Result)
    Public Property TmdbSearchResults As List(Of TmdbApi.SearchPage.Result)
        Get
            Return TmdbSearchResultsField
        End Get
        Set(value As List(Of TmdbApi.SearchPage.Result))
            TmdbSearchResultsField = value
            RaisePropertyChanged("TmdbSearchResults")
        End Set
    End Property

    Private TmdbSearchSelectedField As TmdbApi.SearchPage.Result
    Public Property TmdbSearchSelected As TmdbApi.SearchPage.Result
        Get
            Return TmdbSearchSelectedField
        End Get
        Set(value As TmdbApi.SearchPage.Result)
            TmdbSearchSelectedField = value
            RaisePropertyChanged("TmdbSearchSelected")
        End Set
    End Property

    Private TmdbTvChangesField As TmdbApi.TvPageSet
    Public Property TmdbTvChanges As TmdbApi.TvPageSet
        Get
            Return TmdbTvChangesField
        End Get
        Set(value As TmdbApi.TvPageSet)
            TmdbTvChangesField = value
            RaisePropertyChanged("TmdbTvChanges")
        End Set
    End Property

    Private TmdbNewTvShowsField As ObservableCollection(Of TmdbApi.TvShow)
    Public Property TmdbNewTvShows As ObservableCollection(Of TmdbApi.TvShow)
        Get
            Return TmdbNewTvShowsField
        End Get
        Set(value As ObservableCollection(Of TmdbApi.TvShow))
            TmdbNewTvShowsField = value
            RaisePropertyChanged("TmdbNewTvShows")
        End Set
    End Property

    Private TmdbTvOnAirField As TmdbApi.TvPageSet
    Public Property TmdbTvOnAir As TmdbApi.TvPageSet
        Get
            Return TmdbTvOnAirField
        End Get
        Set(value As TmdbApi.TvPageSet)
            TmdbTvOnAirField = value
            RaisePropertyChanged("TmdbTvOnAir")
        End Set
    End Property

    Private ExtraEpisodesField As ObservableCollection(Of Episode)
    Public Property ExtraEpisodes As ObservableCollection(Of Episode)
        Get
            Return ExtraEpisodesField
        End Get
        Set(value As ObservableCollection(Of Episode))
            ExtraEpisodesField = value
            RaisePropertyChanged("ExtraEpisodes")
        End Set
    End Property

#End Region

#Region " GUI Properties "

    Private CursorField As Cursor
    Public Property Cursor As Cursor
        Get
            Return CursorField
        End Get
        Set(value As Cursor)
            CursorField = value
            RaisePropertyChanged("Cursor")
        End Set
    End Property

    Private TvdbLabelCursorField As Cursor
    Public Property TvdbLabelCursor As Cursor
        Get
            Return TvdbLabelCursorField
        End Get
        Set(value As Cursor)
            TvdbLabelCursorField = value
            RaisePropertyChanged("TvdbLabelCursor")
        End Set
    End Property

    Private XmlTextVisibilityField As Visibility = Visibility.Collapsed
    Public Property XmlTextVisibility As Visibility
        Get
            Return XmlTextVisibilityField
        End Get
        Set(value As Visibility)
            XmlTextVisibilityField = value
            RaisePropertyChanged("XmlTextVisibility")
        End Set
    End Property

    Private TmdbSearchVisibilityField As Visibility = Visibility.Collapsed
    Public Property TmdbSearchVisibility As Visibility
        Get
            Return TmdbSearchVisibilityField
        End Get
        Set(value As Visibility)
            TmdbSearchVisibilityField = value
            RaisePropertyChanged("TmdbSearchVisibility")
        End Set
    End Property

    Private OverlayVisibilityField As Visibility = Visibility.Collapsed
    Public Property OverlayVisibility As Visibility
        Get
            Return OverlayVisibilityField
        End Get
        Set(value As Visibility)
            OverlayVisibilityField = value
            RaisePropertyChanged("OverlayVisibility")
        End Set
    End Property

    Private OptionsVisibilityField As Visibility = Visibility.Collapsed
    Public Property OptionsVisibility As Visibility
        Get
            If DesignerProperties.GetIsInDesignMode(New DependencyObject) Then
                Return Visibility.Hidden
            End If
            Return OptionsVisibilityField
        End Get
        Set(value As Visibility)
            OptionsVisibilityField = value
            If OptionsVisibilityField = Visibility.Visible Then
                CloseListener()
            Else
                ConfigureHttpListener()
            End If
            RaisePropertyChanged("OptionsVisibility")
        End Set
    End Property

    Private MainGridIsEnabledField As Boolean = True
    Public Property MainGridIsEnabled As Boolean
        Get
            Return MainGridIsEnabledField
        End Get
        Set(value As Boolean)
            MainGridIsEnabledField = value
            RaisePropertyChanged("MainGridIsEnabled")
        End Set
    End Property

    Private StatusTextField As String = "Ready"
    Public Property StatusText As String
        Get
            Return StatusTextField
        End Get
        Set(value As String)
            StatusTextField = value
            RaisePropertyChanged("StatusText")
        End Set
    End Property

    Private StatusBarColorField As SolidColorBrush
    Public Property StatusBarColor As SolidColorBrush
        Get
            Return StatusBarColorField
        End Get
        Set(value As SolidColorBrush)
            StatusBarColorField = value
            RaisePropertyChanged("StatusBarColor")
        End Set
    End Property

    Private ChangesButtonBackgroundField As SolidColorBrush
    Public Property ChangesButtonBackground As SolidColorBrush
        Get
            Return ChangesButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            ChangesButtonBackgroundField = value
            RaisePropertyChanged("ChangesButtonBackground")
        End Set
    End Property

    Private OnAirButtonBackgroundField As SolidColorBrush
    Public Property OnAirButtonBackground As SolidColorBrush
        Get
            Return OnAirButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            OnAirButtonBackgroundField = value
            RaisePropertyChanged("OnAirButtonBackground")
        End Set
    End Property

    Private ActivatedField As Boolean
    Public Property Activated As Boolean
        Get
            Return ActivatedField
        End Get
        Set(value As Boolean)
            ActivatedField = value
            RaisePropertyChanged("Activated")
        End Set
    End Property

    Public ReadOnly Property WindowTitle As String
        Get
            If Data IsNot Nothing Then
                If Not String.IsNullOrEmpty(Data.FileName) Then
                    Return "TV Data - " & Data.FileName
                End If
            End If
            Return "TV Data"
        End Get
    End Property

    Private FindReplaceFieldIndexField As Integer = 0
    Public Property FindReplaceFieldIndex As Integer
        Get
            Return FindReplaceFieldIndexField
        End Get
        Set(value As Integer)
            FindReplaceFieldIndexField = value
            RaisePropertyChanged("FindReplaceFieldIndex")
        End Set
    End Property

    Private _episodeProperties As Dictionary(Of String, String)
    Public ReadOnly Property EpisodeProperties As Dictionary(Of String, String)
        Get
            If _episodeProperties Is Nothing Then
                _episodeProperties = New Dictionary(Of String, String)
                Dim props As Reflection.PropertyInfo() = GetType(Episode).GetProperties()
                For Each prop As Reflection.PropertyInfo In props
                    Dim attr As Attribute = Attribute.GetCustomAttribute(prop, GetType(DisplayNameAttribute))
                    If attr IsNot Nothing AndAlso TypeOf attr Is DisplayNameAttribute Then
                        Dim disNameAttr = CType(attr, DisplayNameAttribute)
                        _episodeProperties.Add(prop.Name, disNameAttr.DisplayName)
                    End If
                Next
                RaisePropertyChanged("EpisodeProperties")
                FindReplaceSelection = "Overview"
            End If
            Return _episodeProperties
        End Get
    End Property

    Private FindReplaceSelectionField As String
    Public Property FindReplaceSelection As String
        Get
            Return FindReplaceSelectionField
        End Get
        Set(value As String)
            FindReplaceSelectionField = value
            RaisePropertyChanged("FindReplaceSelection")
        End Set
    End Property

    Private FindTextField As String
    Public Property FindText As String
        Get
            Return FindTextField
        End Get
        Set(value As String)
            FindTextField = value
            RaisePropertyChanged("FindText")
        End Set
    End Property

    Private ReplaceTextField As String = String.Empty
    Public Property ReplaceText As String
        Get
            Return ReplaceTextField
        End Get
        Set(value As String)
            ReplaceTextField = value
            RaisePropertyChanged("ReplaceText")
        End Set
    End Property

    Private FindReplaceVisibilityField As Visibility = Visibility.Collapsed
    Public Property FindReplaceVisibility As Visibility
        Get
            Return FindReplaceVisibilityField
        End Get
        Set(value As Visibility)
            FindReplaceVisibilityField = value
            RaisePropertyChanged("FindReplaceVisibility")
        End Set
    End Property

    Private TmdbChangesVisibilityField As Visibility = Visibility.Collapsed
    Public Property TmdbChangesVisibility As Visibility
        Get
            Return TmdbChangesVisibilityField
        End Get
        Set(value As Visibility)
            TmdbChangesVisibilityField = value
            RaisePropertyChanged("TmdbChangesVisibility")
        End Set
    End Property

    Private TmdbOnAirVisibilityField As Visibility = Visibility.Collapsed
    Public Property TmdbOnAirVisibility As Visibility
        Get
            Return TmdbOnAirVisibilityField
        End Get
        Set(value As Visibility)
            TmdbOnAirVisibilityField = value
            RaisePropertyChanged("TmdbOnAirVisibility")
        End Set
    End Property

    Private TmdbNewTvVisibilityField As Visibility = Visibility.Collapsed
    Public Property TmdbNewTvVisibility As Visibility
        Get
            Return TmdbNewTvVisibilityField
        End Get
        Set(value As Visibility)
            TmdbNewTvVisibilityField = value
            RaisePropertyChanged("TmdbNewTvVisibility")
        End Set
    End Property

    Private ExtraEpisodesVisibilityField As Visibility = Visibility.Collapsed
    Public Property ExtraEpisodesVisibility As Visibility
        Get
            Return ExtraEpisodesVisibilityField
        End Get
        Set(value As Visibility)
            If ExtraEpisodesVisibilityField <> value Then
                ExtraEpisodesVisibilityField = value
                RaisePropertyChanged("ExtraEpisodesVisibility")
            End If
        End Set
    End Property

    Private ExtraEpisodesExternalSourceTypeField As ExternalSourceType
    Public Property ExtraEpisodesExternalSourceType As ExternalSourceType
        Get
            Return ExtraEpisodesExternalSourceTypeField
        End Get
        Set(value As ExternalSourceType)
            If ExtraEpisodesExternalSourceTypeField <> value Then
                ExtraEpisodesExternalSourceTypeField = value
                RaisePropertyChanged("ExtraEpisodesExternalSourceType")
            End If
        End Set
    End Property

    Public Enum ExternalSourceType
        IMDB
        TMDb
        TvGuide
        TvRage
        Freebase
        Alternative
    End Enum

#End Region

#Region " Columns Properties "

#Region " IMDB "

    Private ImdbColumnsVisibilityField As Visibility = Visibility.Collapsed
    Public Property ImdbColumnsVisibility As Visibility
        Get
            Return ImdbColumnsVisibilityField
        End Get
        Set(value As Visibility)
            ImdbColumnsVisibilityField = value
            RaisePropertyChanged("ImdbColumnsVisibility")
            OnImdbColumnsVisibilityChanged()
        End Set
    End Property

    Protected Sub OnImdbColumnsVisibilityChanged()
        If ImdbColumnsVisibility = Visibility.Collapsed Then
            ImdbColumnsButtonText = "Show IMDB Cols"
            ImdbColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Else
            ImdbColumnsButtonText = "Hide IMDB Cols"
            ImdbColumnsButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
    End Sub

    Private ImdbColumnsButtonTextField As String = "Show IMDB Cols"
    Public Property ImdbColumnsButtonText As String
        Get
            Return ImdbColumnsButtonTextField
        End Get
        Set(value As String)
            ImdbColumnsButtonTextField = value
            RaisePropertyChanged("ImdbColumnsButtonText")
        End Set
    End Property

    Private ImdbColumnsButtonBackgroundField As SolidColorBrush
    Public Property ImdbColumnsButtonBackground As SolidColorBrush
        Get
            Return ImdbColumnsButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            ImdbColumnsButtonBackgroundField = value
            RaisePropertyChanged("ImdbColumnsButtonBackground")
        End Set
    End Property

#End Region

#Region " TMDb "

    Private TmdbColumnsVisibilityField As Visibility = Visibility.Collapsed
    Public Property TmdbColumnsVisibility As Visibility
        Get
            Return TmdbColumnsVisibilityField
        End Get
        Set(value As Visibility)
            TmdbColumnsVisibilityField = value
            RaisePropertyChanged("TmdbColumnsVisibility")
            OnTmdbColumnsVisibilityChanged()
        End Set
    End Property

    Protected Sub OnTmdbColumnsVisibilityChanged()
        If TmdbColumnsVisibility = Visibility.Collapsed Then
            TmdbColumnsButtonText = "Show TMDb Cols"
            TmdbColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Else
            TmdbColumnsButtonText = "Hide TMDb Cols"
            TmdbColumnsButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
    End Sub

    Private TmdbColumnsButtonTextField As String = "Show TMDb Cols"
    Public Property TmdbColumnsButtonText As String
        Get
            Return TmdbColumnsButtonTextField
        End Get
        Set(value As String)
            TmdbColumnsButtonTextField = value
            RaisePropertyChanged("TmdbColumnsButtonText")
        End Set
    End Property

    Private TmdbColumnsButtonBackgroundField As SolidColorBrush
    Public Property TmdbColumnsButtonBackground As SolidColorBrush
        Get
            Return TmdbColumnsButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            TmdbColumnsButtonBackgroundField = value
            RaisePropertyChanged("TmdbColumnsButtonBackground")
        End Set
    End Property

#End Region

#Region " TvGuide "

    Private TvGuideColumnsVisibilityField As Visibility = Visibility.Collapsed
    Public Property TvGuideColumnsVisibility As Visibility
        Get
            Return TvGuideColumnsVisibilityField
        End Get
        Set(value As Visibility)
            TvGuideColumnsVisibilityField = value
            RaisePropertyChanged("TvGuideColumnsVisibility")
            OnTvGuideColumnsVisibilityChanged()
        End Set
    End Property

    Protected Sub OnTvGuideColumnsVisibilityChanged()
        If TvGuideColumnsVisibility = Visibility.Collapsed Then
            TvGuideColumnsButtonText = "Show TvGuide Cols"
            TvGuideColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Else
            TvGuideColumnsButtonText = "Hide TvGuide Cols"
            TvGuideColumnsButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
    End Sub

    Private TvGuideColumnsButtonTextField As String = "Show TvGuide Cols"
    Public Property TvGuideColumnsButtonText As String
        Get
            Return TvGuideColumnsButtonTextField
        End Get
        Set(value As String)
            TvGuideColumnsButtonTextField = value
            RaisePropertyChanged("TvGuideColumnsButtonText")
        End Set
    End Property

    Private TvGuideColumnsButtonBackgroundField As SolidColorBrush
    Public Property TvGuideColumnsButtonBackground As SolidColorBrush
        Get
            Return TvGuideColumnsButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            TvGuideColumnsButtonBackgroundField = value
            RaisePropertyChanged("TvGuideColumnsButtonBackground")
        End Set
    End Property

#End Region

#Region " TvRage "

    Private TvRageColumnsVisibilityField As Visibility = Visibility.Collapsed
    Public Property TvRageColumnsVisibility As Visibility
        Get
            Return TvRageColumnsVisibilityField
        End Get
        Set(value As Visibility)
            TvRageColumnsVisibilityField = value
            RaisePropertyChanged("TvRageColumnsVisibility")
            OnTvRageColumnsVisibilityChanged()
        End Set
    End Property

    Protected Sub OnTvRageColumnsVisibilityChanged()
        If TvRageColumnsVisibility = Visibility.Collapsed Then
            TvRageColumnsButtonText = "Show TvRage Cols"
            TvRageColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Else
            TvRageColumnsButtonText = "Hide TvRage Cols"
            TvRageColumnsButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
    End Sub

    Private TvRageColumnsButtonTextField As String = "Show TvRage Cols"
    Public Property TvRageColumnsButtonText As String
        Get
            Return TvRageColumnsButtonTextField
        End Get
        Set(value As String)
            TvRageColumnsButtonTextField = value
            RaisePropertyChanged("TvRageColumnsButtonText")
        End Set
    End Property

    Private TvRageColumnsButtonBackgroundField As SolidColorBrush
    Public Property TvRageColumnsButtonBackground As SolidColorBrush
        Get
            Return TvRageColumnsButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            TvRageColumnsButtonBackgroundField = value
            RaisePropertyChanged("TvRageColumnsButtonBackground")
        End Set
    End Property

#End Region

#Region " Freebase "

    Private FreebaseColumnsVisibilityField As Visibility = Visibility.Collapsed
    Public Property FreebaseColumnsVisibility As Visibility
        Get
            Return FreebaseColumnsVisibilityField
        End Get
        Set(value As Visibility)
            FreebaseColumnsVisibilityField = value
            RaisePropertyChanged("FreebaseColumnsVisibility")
            OnFreebaseColumnsVisibilityChanged()
        End Set
    End Property

    Protected Sub OnFreebaseColumnsVisibilityChanged()
        If FreebaseColumnsVisibility = Visibility.Collapsed Then
            FreebaseColumnsButtonText = "Show Freebase Cols"
            FreebaseColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Else
            FreebaseColumnsButtonText = "Hide Freebase Cols"
            FreebaseColumnsButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
    End Sub

    Private FreebaseColumnsButtonTextField As String = "Show Freebase Cols"
    Public Property FreebaseColumnsButtonText As String
        Get
            Return FreebaseColumnsButtonTextField
        End Get
        Set(value As String)
            FreebaseColumnsButtonTextField = value
            RaisePropertyChanged("FreebaseColumnsButtonText")
        End Set
    End Property

    Private FreebaseColumnsButtonBackgroundField As SolidColorBrush
    Public Property FreebaseColumnsButtonBackground As SolidColorBrush
        Get
            Return FreebaseColumnsButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            FreebaseColumnsButtonBackgroundField = value
            RaisePropertyChanged("FreebaseColumnsButtonBackground")
        End Set
    End Property

#End Region

#Region " Alternative "

    Private AlternativeColumnsVisibilityField As Visibility = Visibility.Collapsed
    Public Property AlternativeColumnsVisibility As Visibility
        Get
            Return AlternativeColumnsVisibilityField
        End Get
        Set(value As Visibility)
            AlternativeColumnsVisibilityField = value
            RaisePropertyChanged("AlternativeColumnsVisibility")
            OnAlternativeColumnsVisibilityChanged()
        End Set
    End Property

    Protected Sub OnAlternativeColumnsVisibilityChanged()
        If AlternativeColumnsVisibility = Visibility.Collapsed Then
            AlternativeColumnsButtonText = "Show Alternative Cols"
            AlternativeColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Else
            AlternativeColumnsButtonText = "Hide Alternative Cols"
            AlternativeColumnsButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
    End Sub

    Private AlternativeColumnsButtonTextField As String = "Show Alternative Cols"
    Public Property AlternativeColumnsButtonText As String
        Get
            Return AlternativeColumnsButtonTextField
        End Get
        Set(value As String)
            AlternativeColumnsButtonTextField = value
            RaisePropertyChanged("AlternativeColumnsButtonText")
        End Set
    End Property

    Private AlternativeColumnsButtonBackgroundField As SolidColorBrush
    Public Property AlternativeColumnsButtonBackground As SolidColorBrush
        Get
            Return AlternativeColumnsButtonBackgroundField
        End Get
        Set(value As SolidColorBrush)
            AlternativeColumnsButtonBackgroundField = value
            RaisePropertyChanged("AlternativeColumnsButtonBackground")
        End Set
    End Property

#End Region

#End Region

    Private _httpListenerActive As Boolean = False
    Public Property HttpListenerActive As Boolean
        Get
            Return _httpListenerActive
        End Get
        Set(value As Boolean)
            _httpListenerActive = value
            RaisePropertyChanged("HttpListenerActive")
            RaisePropertyChanged("HttpListenerNotActive")
        End Set
    End Property

    Public ReadOnly Property HttpListenerNotActive As Boolean
        Get
            Return Not HttpListenerActive
        End Get
    End Property

#End Region

    Public Enum Iso639_1 As Integer
        aa = 1
        ab = 2
        ae = 3
        af = 4
        ak = 5
        am = 6
        an = 7
        ar = 8
        [as] = 9
        av = 10
        ay = 11
        az = 12
        ba = 13
        be = 14
        bg = 15
        bh = 16
        bi = 17
        bm = 18
        bn = 19
        bo = 20
        br = 21
        bs = 22
        ca = 23
        ce = 24
        ch = 25
        co = 26
        cr = 27
        cs = 28
        cu = 29
        cv = 30
        cy = 31
        da = 32
        de = 33
        dv = 34
        dz = 35
        ee = 36
        el = 37
        en = 38
        eo = 39
        es = 40
        et = 41
        eu = 42
        fa = 43
        ff = 44
        fi = 45
        fj = 46
        fo = 47
        fr = 48
        fy = 49
        ga = 50
        gd = 51
        gl = 52
        gn = 53
        gu = 54
        gv = 55
        ha = 56
        he = 57
        hi = 58
        ho = 59
        hr = 60
        ht = 61
        hu = 62
        hy = 63
        hz = 64
        ia = 65
        id = 66
        ie = 67
        ig = 68
        ii = 69
        ik = 70
        io = 71
        [is] = 72
        it = 73
        iu = 74
        ja = 75
        jv = 76
        ka = 77
        kg = 78
        ki = 79
        kj = 80
        kk = 81
        kl = 82
        km = 83
        kn = 84
        ko = 85
        kr = 86
        ks = 87
        ku = 88
        kv = 89
        kw = 90
        ky = 91
        la = 92
        lb = 93
        lg = 94
        li = 95
        ln = 96
        lo = 97
        lt = 98
        lu = 99
        lv = 100
        mg = 101
        mh = 102
        mi = 103
        mk = 104
        ml = 105
        mn = 106
        mr = 107
        ms = 108
        mt = 109
        [my] = 110
        na = 111
        nb = 112
        nd = 113
        ne = 114
        ng = 115
        nl = 116
        nn = 117
        no = 118
        nr = 119
        nv = 120
        ny = 121
        oc = 122
        oj = 123
        om = 124
        [or] = 125
        os = 126
        pa = 127
        pi = 128
        pl = 129
        ps = 130
        pt = 131
        qu = 132
        rm = 133
        rn = 134
        ro = 135
        ru = 136
        rw = 137
        sa = 138
        sc = 139
        sd = 140
        se = 141
        sg = 142
        si = 143
        sk = 144
        sl = 145
        sm = 146
        sn = 147
        so = 148
        sq = 149
        sr = 150
        ss = 151
        st = 152
        su = 153
        sv = 154
        sw = 155
        ta = 156
        te = 157
        tg = 158
        th = 159
        ti = 160
        tk = 161
        tl = 162
        tn = 163
        [to] = 164
        tr = 165
        ts = 166
        tt = 167
        tw = 168
        ty = 169
        ug = 170
        uk = 171
        ur = 172
        uz = 173
        ve = 174
        vi = 175
        vo = 176
        wa = 177
        wo = 178
        xh = 179
        yi = 180
        yo = 181
        za = 182
        zh = 183
        zu = 184
        'en = 0
        'es = 1
        'de = 2
        'fr = 3
        'nl = 4
        'ru = 5
    End Enum

    Public Sub New()
        tmdb = New TmdbApi()
        'DEBUG
        'Dim eps As New List(Of Episode)
        'eps.Add(New Episode() With {.SeasonNumber = 1, .EpisodeNumber = 1})
        'eps.Add(New Episode() With {.SeasonNumber = 1, .EpisodeNumber = 2})
        'eps.Add(New Episode() With {.SeasonNumber = 1, .EpisodeNumber = 3})
        'TvRage.GetEpisodeIds(43926, eps)
        'DEBUG END
        Language = Iso639_1.en
        StatusBarColor = CType(Application.Current.Resources("BackgroundSelected"), SolidColorBrush)
        ImdbColumnsButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        TmdbColumnsButtonBackground = ImdbColumnsButtonBackground
        TvGuideColumnsButtonBackground = ImdbColumnsButtonBackground
        ChangesButtonBackground = CType(Application.Current.Resources("Background"), SolidColorBrush)
        Data = New TVSeries()
        ConfigureEpisodes()

        ExtraEpisodes = New TrulyObservableCollection(Of Episode)
        Dim ExtraEpisodesCollectionView = CollectionViewSource.GetDefaultView(ExtraEpisodes)
        ExtraEpisodesCollectionView.GroupDescriptions.Add(New PropertyGroupDescription("SeasonNumber"))
        ExtraEpisodesCollectionView.SortDescriptions.Add(New SortDescription("SeasonNumber", ListSortDirection.Ascending))
        ExtraEpisodesCollectionView.SortDescriptions.Add(New SortDescription("EpisodeNumber", ListSortDirection.Ascending))

        Data.SeriesInfo.id = "266189"
        Data.SeriesInfo.TvGuideId = "555487"
        If Not IO.Directory.Exists(My.Settings.SavePath) Then
            My.Settings.SavePath = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.MyDocuments, "TvData")
            OptionsVisibility = Visibility.Visible
        End If

        If ComponentModel.DesignerProperties.GetIsInDesignMode(New DependencyObject) Then
            TmdbNewTvShows = New ObservableCollection(Of TmdbApi.TvShow)
            Dim show1 = New TmdbApi.TvShow() With {.name = "Show1", .id = 134560, .poster_path = "/45Mup5w7a5pEfJ9ztWtdTLJhOGq.jpg"}
            TmdbNewTvShows.Add(show1)
            Dim show2 = New TmdbApi.TvShow() With {.name = "Show2", .id = 234561, .poster_path = "/45Mup5w7a5pEfJ9ztWtdTLJhOGq.jpg"}
            TmdbNewTvShows.Add(show1)
        End If

        'Dim counter As Integer = 1
        'For Each weekendDay In GetWeekendDays(2018)
        '    Dim ep As New Episode() With {
        '        .EpisodeName = weekendDay.ToString("MMMM d, yyyy"),
        '        .EpisodeNumber = CShort(counter),
        '        .FirstAired = weekendDay.ToString("yyyy-MM-dd"),
        '        .SeasonNumber = 6
        '    }
        '    Data.Episodes.Add(ep)
        '    counter += 1
        'Next

        'Dim counter As Integer = 1
        'For Each weekendDay In GetWeekDayDays(2018)
        '    Dim ep As New Episode() With {
        '        .EpisodeName = weekendDay.ToString("MMMM d, yyyy"),
        '        .EpisodeNumber = CShort(counter),
        '        .FirstAired = weekendDay.ToString("yyyy-MM-dd"),
        '        .SeasonNumber = 43
        '    }
        '    Data.Episodes.Add(ep)
        '    counter += 1
        'Next

    End Sub

    Private Iterator Function GetWeekendDays(year As Integer) As IEnumerable(Of DateTime)
        Dim i As DateTime = New Date(year, 1, 1)
        Dim lastDay As DateTime = New Date(year, 12, 31)
        While i <= lastDay
            If i.DayOfWeek = DayOfWeek.Saturday OrElse i.DayOfWeek = DayOfWeek.Sunday Then
                Yield i
            End If
            i = i.AddDays(1)
        End While
    End Function

    Private Iterator Function GetWeekDayDays(year As Integer) As IEnumerable(Of DateTime)
        Dim i As DateTime = New Date(year, 1, 1)
        Dim lastDay As DateTime = New Date(year, 12, 31)
        While i <= lastDay
            If i.DayOfWeek <> DayOfWeek.Saturday AndAlso i.DayOfWeek <> DayOfWeek.Sunday Then
                Yield i
            End If
            i = i.AddDays(1)
        End While
    End Function

    Private Sub ConfigureEpisodes()
        Dim EpisodesCollectionView = CollectionViewSource.GetDefaultView(Data.Episodes)
        EpisodesCollectionView.GroupDescriptions.Clear()
        EpisodesCollectionView.GroupDescriptions.Add(New PropertyGroupDescription("SeasonNumber"))
        EpisodesCollectionView.SortDescriptions.Clear()
        EpisodesCollectionView.SortDescriptions.Add(New SortDescription("SeasonNumber", ListSortDirection.Ascending))
        EpisodesCollectionView.SortDescriptions.Add(New SortDescription("EpisodeNumber", ListSortDirection.Ascending))
    End Sub

#Region " Save/Load To/From File"

    Private saveFile As Microsoft.Win32.SaveFileDialog

    Public Sub SaveToFile()
        If String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
            SetStatus(StatusType.Error, "A Series Name is required to save")
        Else
            'Dim safeSeriesName = IO.Path.GetInvalidFileNameChars().Aggregate(Data.SeriesInfo.SeriesName, Function(current, c) current.Replace(c, "-"c))
            Dim savePath As String = GetSafeSaveFilePath(Data.SeriesInfo.SeriesName)
            If Not Directory.Exists(My.Settings.SavePath) Then
                Directory.CreateDirectory(My.Settings.SavePath)
            End If
            If File.Exists(savePath) Then
                If MessageWindow.ShowDialog("File " & savePath & " exists." & Environment.NewLine & Environment.NewLine & "Overwrite existing File?",
                                            "Overwrite existing?", True) = False Then
                    Exit Sub

                End If
            End If
            Using objStreamWriter As New StreamWriter(savePath)
                Dim x As New XmlSerializer(Data.GetType)
                x.Serialize(objStreamWriter, Data)
                objStreamWriter.Close()
            End Using
            SetStatus(StatusType.OK, "Saved to " & savePath)
        End If
    End Sub

    Public Sub SaveAsToFile()
        If saveFile Is Nothing Then
            saveFile = New Microsoft.Win32.SaveFileDialog() With {.DefaultExt = "tvxml",
                                                                  .Filter = "TVData XML File|*.tvxml"}
        End If
        If saveFile.ShowDialog = True Then
            Using objStreamWriter As New StreamWriter(saveFile.FileName)
                Dim x As New XmlSerializer(Data.GetType)
                x.Serialize(objStreamWriter, Data)
                objStreamWriter.Close()
            End Using
            Data.FileName = IO.Path.GetFileName(saveFile.FileName)
            SetStatus(StatusType.OK, "Saved to " & saveFile.FileName)
        End If
    End Sub

    Private Sub SaveSeasonsToFile()
        If String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
            SetStatus(StatusType.Error, "A Series Name is required to save")
        Else
            If Not IO.Directory.Exists(My.Settings.SavePath) Then
                IO.Directory.CreateDirectory(My.Settings.SavePath)
            End If
            Dim overWriteConfirmed As Boolean = False
            For Each seasonNo In (From eps In Data.Episodes Select eps.SeasonNumber Distinct).ToList()
                Dim saveFileName As String = GetSafeSaveFilePath(Data.SeriesInfo.SeriesName & "_S" & CInt(seasonNo).ToString("00"))
                Dim savePath As String = IO.Path.Combine(My.Settings.SavePath, saveFileName)
                If IO.File.Exists(savePath) AndAlso Not overWriteConfirmed Then
                    If MessageWindow.ShowDialog("File " & savePath & " exists." & Environment.NewLine & Environment.NewLine & "Overwrite existing File and all other Seasons?",
                                                "Overwrite existing?", True) = False Then
                        Exit Sub
                    Else
                        overWriteConfirmed = True
                    End If
                    'If MessageBox.Show("File " & savePath & " exists." & Environment.NewLine & Environment.NewLine & "Overwrite existing File and all other Seasons?",
                    '                   "Overwrite existing?",
                    '                   MessageBoxButton.OKCancel) = MessageBoxResult.Cancel Then
                    '    Exit Sub
                    'Else
                    '    overWriteConfirmed = True
                    'End If
                End If
                Dim tempData As New TVSeries() With {
                    .SeriesInfo = Data.SeriesInfo.Copy()
                }

                For Each ep In (From eps In Data.Episodes Where eps.SeasonNumber = seasonNo Or CInt(eps.SeasonNumber) = 0 Select eps)
                    tempData.Episodes.Add(ep)
                Next
                'tempData.SeriesInfo.SeriesName += "_S" & seasonNo.ToString("00")
                tempData.FileName = IO.Path.GetFileName(saveFileName)
                Using objStreamWriter As New StreamWriter(savePath)
                    Dim x As New XmlSerializer(tempData.GetType)
                    x.Serialize(objStreamWriter, tempData)
                    objStreamWriter.Close()
                End Using
                SetStatus(StatusType.OK, "Saved to " & savePath)
            Next
        End If
    End Sub

    Private Function GetSafeSaveFilePath(fileName As String) As String
        Dim safeFilename As String = IO.Path.GetInvalidFileNameChars().Aggregate(fileName, Function(current, c) current.Replace(c, "-"c))
        Return IO.Path.Combine(My.Settings.SavePath, safeFilename & ".tvxml")
    End Function

    Friend Sub LoadFromFile(FileName As Object)
        'Cursor = Cursors.Wait
        SetBusyState("Loading File")
        Using objStreamReader As New StreamReader(CStr(FileName))
            Dim x As New XmlSerializer(Data.GetType)
            Dim tempData = CType(x.Deserialize(objStreamReader), TVSeries)
            objStreamReader.Close()
            'SetData(tempData)
            ' Set "Data" on the UI thread so exception doesn't occur when this method is called from a non-UI thread
            Application.Current.Dispatcher.BeginInvoke(New ParameterizedThreadStart(AddressOf SetData), tempData)
        End Using
        SetReadyState(StatusType.OK, "Ready")
        'Cursor = Nothing
    End Sub

    Private Sub ImportMergeFile(FileName As Object)
        SetBusyState("Loading Merge File")
        Using objStreamReader As New StreamReader(CStr(FileName))
            Dim x As New XmlSerializer(Data.GetType)
            Dim tempData = CType(x.Deserialize(objStreamReader), TVSeries)
            objStreamReader.Close()

            For Each ep In tempData.Episodes
                Dim existingEp = (From e In Data.Episodes Where e.SeasonNumber = ep.SeasonNumber AndAlso e.EpisodeNumber = ep.EpisodeNumber).FirstOrDefault
                If existingEp IsNot Nothing Then
                    If Not String.IsNullOrWhiteSpace(ep.EpisodeName) Then
                        existingEp.EpisodeName = ep.EpisodeName
                    End If
                    If Not String.IsNullOrWhiteSpace(ep.FirstAired) Then
                        existingEp.FirstAired = ep.FirstAired
                    End If
                    If Not String.IsNullOrWhiteSpace(ep.Overview) Then
                        existingEp.Overview = ep.Overview
                    End If
                Else
                    Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Data.Episodes, ep})
                End If
            Next
        End Using
        SetReadyState(StatusType.OK, "Merge Complete")
    End Sub

    Private Sub ExportToExcel()
        Dim sb As New Text.StringBuilder("""S"",""Ep"",""Episode Name"",""First Aired"",""IMDB_ID"",""Overview"",""Production Code"",""TvRage Id"",""Freebase MId"",""Freebase Id""" & Environment.NewLine)
        For Each ep In Data.Episodes
            sb.AppendLine("""" & ep.SeasonNumber & """," &
                          """" & ep.EpisodeNumber & """," &
                          """" & ep.EpisodeName & """," &
                          """" & ep.FirstAired & """," &
                          """" & ep.IMDB_ID & """," &
                          """" & ep.Overview.Replace("""", "''") & """," &
                          """" & ep.ProductionCode & """," &
                          """" & ep.TvRageId & """," &
                          """" & ep.FreebaseMid & """," &
                          """" & ep.FreebaseId & """")
        Next
        Dim filename = IO.Path.Combine(IO.Path.GetTempPath, "TvDataExport.csv")
        My.Computer.FileSystem.WriteAllText(filename, sb.ToString, False)
        Process.Start(filename)
    End Sub

#End Region

    Private Sub SetCursor(TheCursor As Object)
        Cursor = CType(TheCursor, Input.Cursor)
    End Sub

    Private Sub SetCursorThreadSafe(TheCursor As Cursor)
        Application.Current.Dispatcher.BeginInvoke(New ParameterizedThreadStart(AddressOf SetCursor), TheCursor)
    End Sub

    Private Sub Clear(Optional SetReadyStatus As Boolean = True)
        Dim seriesId = Data.SeriesInfo.id
        Data.SeriesInfo = New Series() With {
            .id = seriesId
        }
        Data.FileName = Nothing
        Application.Current.Dispatcher.BeginInvoke(New Action(AddressOf Data.Episodes.Clear))
        ImdbColumnsVisibility = Visibility.Collapsed
        TmdbColumnsVisibility = Visibility.Collapsed
        TvGuideColumnsVisibility = Visibility.Collapsed
        If SetReadyStatus Then
            SetStatus(StatusType.OK, "Ready")
        End If
    End Sub

    Private loadedTvdbId As String = String.Empty

    Public Sub LoadFromTvdb(Optional SeasonNumber As Object = Nothing)
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.id) Then
            SetBusyState("Downloading from TVDB")
            'Dim seriesId = Data.SeriesInfo.id
            'Data.SeriesInfo = New Series
            'Application.Current.Dispatcher.BeginInvoke(New Action(AddressOf Data.Episodes.Clear))
            'Data.SeriesInfo.id = seriesId
            If loadedTvdbId IsNot Nothing AndAlso Not loadedTvdbId.Equals(Data.SeriesInfo.id) Then
                Clear(False)
            End If
            Activated = True
            'Dim TVDbUrl As String = String.Format("http://thetvdb.com/api/1D62F2F90030C444/series/{0}/all/{1}.xml", Data.SeriesInfo.id, Language.ToString())
            Dim TVDbUrl As String = String.Format("http://thetvdb.com/api/1D62F2F90030C444/series/{0}/all/{1}.zip", Data.SeriesInfo.id, Language.ToString())
            'Dim XmlString As String = String.Empty
            Using wcx As New WebClientEx()
                Try
                    Using tvdbStream As IO.Stream = wcx.OpenRead(TVDbUrl)
                        SetStatus(StatusType.Busy, "Decompressing TVDB XML Zip file")
                        Using archive As New ZipArchive(tvdbStream)
                            Dim infoEntry = archive.Entries.Where(Function(e) e.Name = Language.ToString() & ".xml").FirstOrDefault
                            If infoEntry IsNot Nothing Then
                                SetStatus(StatusType.Busy, "Deserializing TVDB XML")
                                Using infoStream = infoEntry.Open()
                                    Dim x As New XmlSerializer(Data.GetType)
                                    Dim tempData As TVSeries = CType(x.Deserialize(infoStream), TVSeries)
                                    infoStream.Close()
                                    Dim seasonNo As Short
                                    If Short.TryParse(CStr(SeasonNumber), seasonNo) Then
                                        tempData.Episodes.Where(Function(e) e.SeasonNumber <> seasonNo).ToList().ForEach(Function(ep) tempData.Episodes.Remove(ep))
                                        'tempData.Episodes.ToList().RemoveAll(Function(e) e.SeasonNumber <> seasonNo)
                                    End If
                                    'Data = CType(x.Deserialize(infoStream), TVSeries)
                                    ' Set "Data" on the UI thread so no thread exception occurs
                                    Application.Current.Dispatcher.BeginInvoke(New ParameterizedThreadStart(AddressOf SetData), tempData)
                                End Using
                            Else
                                SetStatus(StatusType.Error, Language & ".xml not found in downloaded zip file")
                                Exit Sub
                            End If
                        End Using
                        tvdbStream.Close()
                    End Using
                    'XmlString = wc.DownloadString(TVDbUrl)
                Catch wex As WebException
                    If TypeOf (wex.Response) Is HttpWebResponse Then
                        Dim response = CType(wex.Response, HttpWebResponse)
                        SetReadyState(StatusType.Error, response.StatusCode & ": " & response.StatusDescription)
                    Else
                        SetReadyState(StatusType.Error, "Unknown Error occured: " & wex.Message)
                    End If
                    Data.FileName = "- Series Not Found - -"
                    Exit Sub
                Catch ex As Exception
                    SetReadyState(StatusType.Error, ex.Message)
                    Exit Sub
                End Try
            End Using
            'SetStatus(StatusType.Busy, "Serializing TVDB XML")
            'Using sr As New StringReader(XmlString)
            '    Dim x As New XmlSerializer(Data.GetType)
            '    Dim tempData As TVSeries = CType(x.Deserialize(sr), TVSeries)
            '    sr.Close()
            '    'Data = CType(x.Deserialize(sr), TVSeries)
            '    ' Set "Data" on the UI thread so the Grouping will work
            '    Application.Current.Dispatcher.BeginInvoke(New ParameterizedThreadStart(AddressOf SetData), tempData)
            'End Using
            ImdbColumnsVisibility = Visibility.Collapsed
            TmdbColumnsVisibility = Visibility.Collapsed
            TvGuideColumnsVisibility = Visibility.Collapsed
            SetReadyState(StatusType.OK, "Download from TVDB complete")
        Else
            SetStatus(StatusType.Error, "Enter TVDB Id first")
        End If
    End Sub

    Private Sub SetData(TheData As Object)
        Dim tempData As TVSeries = CType(TheData, TVSeries)
        'If reloading information the ids will be the same
        ' keep the existing external ids that aren't saved on TVDB
        If tempData.SeriesInfo.id = Data.SeriesInfo.id Then
            'tempData may come from a saved file, check to see if external ids are empty before copying
            If String.IsNullOrEmpty(tempData.SeriesInfo.TvGuideId) Then
                tempData.SeriesInfo.TvGuideId = Data.SeriesInfo.TvGuideId
            End If
            If Not tempData.SeriesInfo.TmdbId.HasValue Then
                tempData.SeriesInfo.TmdbId = Data.SeriesInfo.TmdbId
            End If
            If String.IsNullOrEmpty(tempData.SeriesInfo.IMDB_ID) Then
                tempData.SeriesInfo.IMDB_ID = Data.SeriesInfo.IMDB_ID
            End If
        End If
        Data.SeriesInfo = tempData.SeriesInfo
        'Raises the property changed event so Window title will update:
        Data.FileName = tempData.FileName
        Data.Episodes.Clear()
        For Each ep As Episode In tempData.Episodes
            Data.Episodes.Add(ep)
            ep.Validate()
        Next
        'Data.Episodes = TheData.Episodes
        If String.IsNullOrEmpty(Data.FileName) Then
            Data.FileName = Data.SeriesInfo.SeriesName
        End If
        loadedTvdbId = Data.SeriesInfo.id
    End Sub

    Private Delegate Sub EpisodeParameterDelegate(ep As Episode)
    Private Delegate Sub IntegerParameterDelegate(i As Integer)

    Public Sub LoadImdbInfo()
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.IMDB_ID) Then
            SetBusyState("Downloading from IMDB")
            ExtraEpisodesExternalSourceType = ExternalSourceType.IMDB
            ImdbColumnsVisibility = Visibility.Visible
            _infoRetriever = New ImdbInfoRetriever()
            Try
                _infoRetriever.RetrieveInfo(Data.SeriesInfo.IMDB_ID, Data.Episodes)
                Thread.Sleep(500)
                AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
            Catch ex As Exception
                SetReadyState(StatusType.Error, "Error Downloading from IMDB. " & ex.Message)
                ImdbColumnsVisibility = Visibility.Collapsed
                Exit Sub
            End Try
            SetReadyState(StatusType.OK, "Download from IMDB complete")
        Else
            SetStatus(StatusType.Error, "Enter IMDB Id first")
        End If
    End Sub

    Public Sub LoadImdbByDateInfo()
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.IMDB_ID) Then
            SetBusyState("Downloading from IMDB by date")
            ExtraEpisodesExternalSourceType = ExternalSourceType.IMDB
            ImdbColumnsVisibility = Visibility.Visible
            _infoRetriever = New ImdbByDateInfoRetriever()
            Try
                _infoRetriever.RetrieveInfo(Data.SeriesInfo.IMDB_ID, Data.Episodes)
                Thread.Sleep(500)
                AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
            Catch ex As Exception
                SetReadyState(StatusType.Error, "Error Downloading from IMDB by date. " & ex.Message)
                ImdbColumnsVisibility = Visibility.Collapsed
                Exit Sub
            End Try
            SetReadyState(StatusType.OK, "Download from IMDB by date complete")
        Else
            SetStatus(StatusType.Error, "Enter IMDB Id first")
        End If
    End Sub

    Private Sub AddExtraEpisodesFromInfoRetriever(ExtraEpisodesCollection As IList(Of Episode))
        If ExtraEpisodesCollection.Count > 0 Then
            Application.Current.Dispatcher.BeginInvoke(New Action(AddressOf ExtraEpisodes.Clear))
            For Each ep In ExtraEpisodesCollection
                Application.Current.Dispatcher.BeginInvoke(New EpisodeParameterDelegate(AddressOf ExtraEpisodes.Add), ep)
            Next
            ExtraEpisodesVisibility = Visibility.Visible
        End If
    End Sub

    Private Sub LoadTmdbInfo()
        If Data.SeriesInfo.TmdbId.HasValue Then
            SetBusyState("Downloading from TMDb")
            ExtraEpisodesExternalSourceType = ExternalSourceType.TMDb
            TmdbColumnsVisibility = Visibility.Visible
            _infoRetriever = New TmdbInfoRetriever(tmdb)
            Try
                _infoRetriever.RetrieveInfo(CStr(Data.SeriesInfo.TmdbId), Data.Episodes, Language)
                AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
            Catch ex As Exception
                SetReadyState(StatusType.Error, "Error Downloading from TMDb. " & ex.Message)
                TmdbColumnsVisibility = Visibility.Collapsed
                Exit Sub
            End Try
            SetReadyState(StatusType.OK, "Download from TMDb complete")
        End If
    End Sub

    Private Sub SearchTmdb()
        SetBusyState("Searching TMDb")
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.id) Then
            Dim tmdbId As Integer? = tmdb.GetTmdbIdFromTvdbId(Data.SeriesInfo.id)
            If tmdbId.HasValue Then
                Data.SeriesInfo.TmdbId = tmdbId
                LoadTmdbInfo()
                Exit Sub
            End If
        End If

        If Not Data.SeriesInfo.TmdbId.HasValue AndAlso Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
            Dim searchResults = tmdb.Search(Data.SeriesInfo.SeriesName, Data.SeriesInfo.FirstAired)
            'TODO: implement search TMDb without date
            TmdbSearchResults = searchResults.results
            TmdbSearchVisibility = Visibility.Visible
            SetReadyState(StatusType.OK, "TMDb Search complete")
        Else
            SetReadyState(StatusType.Error, "Search was unable to find TMDb entry with supplied information.")
        End If
    End Sub

    Private Sub HideTmdbSearchResults()
        TmdbSearchVisibility = Visibility.Collapsed
        SetStatus(StatusType.OK, "Ready")
    End Sub

    Private Sub LoadTvGuideInfo()
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.TvGuideId) Then
            SetBusyState("Downloading from TV Guide")
            ExtraEpisodesExternalSourceType = ExternalSourceType.TvGuide
            TvGuideColumnsVisibility = Visibility.Visible
            _infoRetriever = New TvGuideInfoRetriever(Data.SeriesInfo.SeriesName)
            Try
                _infoRetriever.RetrieveInfo(Data.SeriesInfo.TvGuideId, Data.Episodes)
                AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
            Catch ex As Exception
                SetReadyState(StatusType.Error, "Error Downloading from TV Guide. " & ex.Message)
                TvGuideColumnsVisibility = Visibility.Collapsed
                Exit Sub
            End Try
            SetReadyState(StatusType.OK, "Download from TV Guide Complete")
        Else
            SetStatus(StatusType.Error, "Enter TV Guide Id first")
        End If
    End Sub

    Private Sub LoadTvRageInfo()
        If Data.SeriesInfo.TvRageId.HasValue Then
            SetBusyState("Downloading from TvRage")
            ExtraEpisodesExternalSourceType = ExternalSourceType.TvRage
            TvRageColumnsVisibility = Visibility.Visible
            _infoRetriever = New TvRageInfoRetriever()
            Try
                _infoRetriever.RetrieveInfo(Data.SeriesInfo.TvRageId.ToString(), Data.Episodes)
                AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
            Catch ex As Exception
                SetReadyState(StatusType.Error, "Error Downloading from TvRage. " & ex.Message)
                TvGuideColumnsVisibility = Visibility.Collapsed
                Exit Sub
            End Try
            SetReadyState(StatusType.OK, "Download from TvRage Complete.")
        Else
            SetStatus(StatusType.Error, "Enter TvRage Id first")
        End If
    End Sub

    'Private WithEvents FreebaseRetriever As FreebaseInfoRetriever

    Private Sub LoadFreebaseInfo()
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.FreebaseMid) Then
            SetBusyState("Downloading from Freebase")
            ExtraEpisodesExternalSourceType = ExternalSourceType.Freebase
            FreebaseColumnsVisibility = Visibility.Visible
            _infoRetriever = New FreebaseInfoRetriever()
            'Try
            _infoRetriever.RetrieveInfo(Data.SeriesInfo.FreebaseMid, Data.Episodes)
            AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
            'Catch ex As Exception
            '    SetReadyState(StatusType.Error, "Error Downloading from Freebase. " & ex.Message)
            '    FreebaseColumnsVisibility = Visibility.Collapsed
            '    Exit Sub
            'End Try
            SetReadyState(StatusType.OK, "Download from Freebase Complete.")
        Else
            SetStatus(StatusType.Error, "Enter Freebase MId first")
        End If
    End Sub

    Private Sub LoadAlternativeInfo(filepath As Object)
        SetBusyState("Opening Alternative File")
        ExtraEpisodesExternalSourceType = ExternalSourceType.Alternative
        AlternativeColumnsVisibility = Visibility.Visible
        _infoRetriever = New AlternativeInfoRetriever()
        _infoRetriever.RetrieveInfo(filepath.ToString, Data.Episodes)
        AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
        SetReadyState(StatusType.OK, "Loaded Alternative File")
    End Sub

    Private Sub PasteToAlt()
        SetBusyState("Pasting from Clipboard")
        ExtraEpisodesExternalSourceType = ExternalSourceType.Alternative
        AlternativeColumnsVisibility = Visibility.Visible
        _infoRetriever = New AlternativeInfoRetriever()
        _infoRetriever.RetrieveInfo(Nothing, Data.Episodes)
        AddExtraEpisodesFromInfoRetriever(_infoRetriever.ExtraEpisodes)
        SetReadyState(StatusType.OK, "Loaded Alternative File")
    End Sub

    Private Sub GetAllInfo()
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.id) Then
            SearchTmdb()
            If Data.SeriesInfo.TmdbId.HasValue Then
                GetExternalIds()
                LoadImdbInfo()
                LoadTvRageInfo()
                LoadFreebaseInfo()
            End If
        Else
            SetStatus(StatusType.Error, "Enter TVDB Id first")
        End If
    End Sub

    Private Sub ClearEpisodeNames()
        If MessageWindow.ShowDialog("Are you sure you want to clear all episode names?  This cannot be undone.", "Clear all episode names", True) = True Then
            SetBusyState("Clearing episode names")
            For Each ep In Data.Episodes
                ep.EpisodeName = Nothing
            Next
            SetReadyState(StatusType.OK, "Episode names cleared")
        End If
    End Sub

    Private Sub ClearEpisodeOverviews()
        If MessageWindow.ShowDialog("Are you sure you want to clear all episode overviews?  This cannot be undone.", "Clear all episode overviews", True) = True Then
            SetBusyState("Clearing episode overviews")
            For Each ep In Data.Episodes
                ep.Overview = Nothing
            Next
            SetReadyState(StatusType.OK, "Episode overviews cleared")
        End If
    End Sub

    Private Sub ClearEpisodeFirstAired()
        If MessageWindow.ShowDialog("Are you sure you want to clear all episode First Aired Dates?  This cannot be undone.", "Clear all episode First Aired Dates", True) = True Then
            SetBusyState("Clearing episode First Aired Dates")
            For Each ep In Data.Episodes
                ep.FirstAired = Nothing
            Next
            SetReadyState(StatusType.OK, "Episode First Aired Dates cleared")
        End If
    End Sub

    Private Sub ClearEpisodeProductionCode()
        If MessageWindow.ShowDialog("Are you sure you want to clear all episode Production Codes?  This cannot be undone.", "Clear all episode Production Codes", True) = True Then
            SetBusyState("Clearing episode Production Codes")
            For Each ep In Data.Episodes
                ep.ProductionCode = Nothing
            Next
            SetReadyState(StatusType.OK, "Episode Production Codes cleared")
        End If
    End Sub


    Private Sub FixEpisodesList()
        SetBusyState("Repairing episodes")
        Application.Current.Dispatcher.BeginInvoke(New ThreadStart(AddressOf FixEpisodesListSafe))
        'TODO Finish FixEpisodesList
        SetReadyState(StatusType.OK, "Episodes repaired")
    End Sub

    Private Sub FixEpisodesListSafe()
        SetBusyState("Attempting to fix dataset")
        Try
            Dim tempEpisodes As TrulyObservableCollection(Of Episode) = New TrulyObservableCollection(Of Episode)(Data.Episodes)
            Data.Episodes = New TrulyObservableCollection(Of Episode)(tempEpisodes)
            ConfigureEpisodes()
        Catch ex As Exception
        Finally
            SetReadyState(StatusType.OK, "Dataset fix attempt complete.")
        End Try
    End Sub

    Private Sub GetExternalIds()
        If Data.SeriesInfo.TmdbId.HasValue Then
            SetBusyState("Getting external Ids")
            Dim externalIds = tmdb.GetTvShowExternalIds(CStr(Data.SeriesInfo.TmdbId))
            If externalIds IsNot Nothing Then
                Data.SeriesInfo.TvRageId = externalIds.tvrage_id
                Data.SeriesInfo.FreebaseMid = externalIds.freebase_mid
                If String.IsNullOrWhiteSpace(Data.SeriesInfo.IMDB_ID) Then
                    Data.SeriesInfo.IMDB_ID = externalIds.imdb_id
                End If
            End If
            SetReadyState(StatusType.OK, "Finished getting external Ids")
        Else
            SetStatus(StatusType.Error, "Enter TMDb Id first")
        End If
    End Sub

    'Private Sub LoadTvRageEpisodeIds()
    '    If Data.SeriesInfo.TvRageId.HasValue Then
    '        SetBusyState("Downloading TvRage Episode Ids")
    '        Try
    '            TvRage.GetEpisodeIds(Data.SeriesInfo.TvRageId.Value, Data.Episodes)
    '            SetReadyState(StatusType.OK, "Episode TvRage Id download complete")
    '        Catch ex As Exception
    '            SetReadyState(StatusType.Error, "Error downloading TvRage Episode Ids: " & ex.Message)
    '        End Try
    '    Else
    '        SetStatus(StatusType.Error, "Enter Series TvRage Id first")
    '    End If
    'End Sub

    Private Sub LoadFreebaseEpisodeMIds()
        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.FreebaseMid) Then
            SetBusyState("Downloading Freebase Episode MIds")
            'Using fb As New Freebase
            '    fb.GetEpisodeMIds(Data.SeriesInfo.FreebaseMid, Data.Episodes)
            'End Using
            Try
                FreebaseMQL.GetEpisodeMIds(Data.SeriesInfo.FreebaseMid, Data.Episodes)
            Catch ex As Exception
                SetStatus(StatusType.Error, "Error getting Freebase Info: " & ex.Message)
            End Try

            SetReadyState(StatusType.OK, "Episode Freebase MId download complete")
        Else
            SetStatus(StatusType.Error, "Enter Series Freebase MId first")
        End If
    End Sub

    Private Sub TitleToAiredDate()
        SetBusyState("Converting titles to aired date.......")
        Dim re As New System.Text.RegularExpressions.Regex("part \d", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        For Each ep In Data.Episodes
            If String.IsNullOrWhiteSpace(ep.FirstAired) Then
                Dim airedDate As Date
                Dim title As String = String.Empty
                Try
                    title = re.Replace(ep.EpisodeName, String.Empty)
                    title = title.Trim()
                    title = title.Replace("Thurs", "Thu")
                Catch ex As Exception
                    MessageWindow.ShowDialog(ex.Message, "Error")
                End Try

                Dim parseSucceded As Boolean = Date.TryParse(title, airedDate)
                If parseSucceded Then
                    ep.FirstAired = airedDate.ToString("yyyy-MM-dd")
                End If
            End If
        Next
        SetReadyState(StatusType.OK, "Finished converting titles to aired date.")
    End Sub

    Private Sub ConvertDateTitle()
        SetBusyState("Converting date title format.......")
        Dim format = TextWindow.GetText("Convert Date Title Format",
                                        "Enter the date format to convert to:  (Democracy Now!: dddd, MMMM d, yyyy)",
                                        "https://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx")
        If Not String.IsNullOrWhiteSpace(format) Then
            For Each ep In Data.Episodes
                Dim dte As Date
                If Date.TryParse(ep.EpisodeName, dte) Then
                    ep.EpisodeName = dte.ToString(format)
                End If
            Next
        Else
            SetReadyState(StatusType.OK, "Convert date title format cancelled.")
            Exit Sub
        End If
        SetReadyState(StatusType.OK, "Finished converting date title format.")
    End Sub

    Private Sub AddExtraEpisodes()
        For i = ExtraEpisodes.Count - 1 To 0 Step -1
            If ExtraEpisodes(i).IsSelected Then
                Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Data.Episodes, ExtraEpisodes(i)})
                Application.Current.Dispatcher.BeginInvoke(New IntegerParameterDelegate(AddressOf ExtraEpisodes.RemoveAt), i)
                ExtraEpisodes(i).Validate()
            End If
        Next
        ExtraEpisodesVisibility = Visibility.Collapsed
    End Sub

    Private Sub FindReplaceExecute()
        FindReplaceVisibility = Visibility.Collapsed
        If String.IsNullOrWhiteSpace(FindText) Then
            Exit Sub
        End If
        SetBusyState("Executing Find Replace")
        'Select Case FindReplaceFieldIndex
        '    Case 0 'Overview
        '        'Data.Episodes.Where(Function(e) e.Overview.Contains(FindText, StringComparer.OrdinalIgnoreCase))
        '        For Each ep As Episode In (From e In Data.Episodes Where e.Overview IsNot Nothing AndAlso e.Overview.Contains(FindText) Select e)
        '            If Not String.IsNullOrEmpty(ep.Overview) Then
        '                ep.Overview = ep.Overview.Replace(FindText, ReplaceText)
        '            End If
        '        Next
        '    Case 1 'Episode Name
        '        For Each ep In Data.Episodes
        '            If Not String.IsNullOrEmpty(ep.EpisodeName) Then
        '                ep.EpisodeName = ep.EpisodeName.Replace(FindText, ReplaceText)
        '            End If
        '        Next
        '    Case 2 'First Aired
        '        For Each ep In Data.Episodes
        '            If Not String.IsNullOrEmpty(ep.FirstAired) Then
        '                ep.FirstAired = ep.FirstAired.Replace(FindText, ReplaceText)
        '            End If
        '        Next
        '    Case 3 'Production Code
        '        For Each ep In Data.Episodes
        '            If Not String.IsNullOrEmpty(ep.ProductionCode) Then
        '                ep.ProductionCode = ep.ProductionCode.Replace(FindText, ReplaceText)
        '            End If
        '        Next
        'End Select


        'Dim propertyName As String = FindReplaceSelection
        'Select Case FindReplaceFieldIndex
        '    Case 0 'Overview
        '        'Data.Episodes.Where(Function(e) e.Overview.Contains(FindText, StringComparer.OrdinalIgnoreCase))
        '        propertyName = "Overview"
        '    Case 1 'Episode Name
        '        propertyName = "EpisodeName"
        '    Case 2 'First Aired
        '        propertyName = "FirstAired"
        '    Case 3 'Production Code
        '        propertyName = "ProductionCode"
        'End Select

        For Each ep As Episode In (From e In Data.Episodes Where e(FindReplaceSelection) IsNot Nothing Select e)
            If Not String.IsNullOrEmpty(ep(FindReplaceSelection).ToString()) Then
                If My.Settings.FindReplaceUseRegex Then
                    ep(FindReplaceSelection) = Regex.Replace(ep(FindReplaceSelection).ToString(), FindText, ReplaceText,
                                                             RegexOptions.Multiline Or RegexOptions.IgnoreCase)
                Else
                    ep(FindReplaceSelection) = ep(FindReplaceSelection).ToString().Replace(FindText, ReplaceText)
                End If
            End If
        Next

        SetReadyState(StatusType.OK, "Ready")
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

    Private Sub _infoRetriever_StatusChanged(TypeOfStatus As StatusType, Message As String) Handles _infoRetriever.StatusChanged
        SetStatus(TypeOfStatus, Message)
    End Sub

    Private Sub SetBusyState(Message As String)
        Cursor = Cursors.Wait
        'SetCursorThreadSafe(Cursors.Wait)
        OverlayVisibility = Visibility.Visible
        MainGridIsEnabled = False
        SetStatus(StatusType.Busy, Message)
    End Sub

    Private Sub SetReadyState(TypeOfStatus As StatusType, Message As String)
        'SetCursorThreadSafe(Nothing)
        Cursor = Nothing
        OverlayVisibility = Visibility.Hidden
        MainGridIsEnabled = True
        SetStatus(TypeOfStatus, Message)
    End Sub

    Public Sub TmdbTvChangesLinkClicked(result As Object)
        Dim changeResult As TmdbApi.TvPageSet.Result = CType(result, TmdbApi.TvPageSet.Result)
        Process.Start(String.Format("http://www.themoviedb.org/tv/{0}/changes", changeResult.id))
        changeResult.visited = True
    End Sub

    Public Sub TmdbTvOnAirLinkClicked(result As Object)
        Dim onAirResult As TmdbApi.TvPageSet.Result = CType(result, TmdbApi.TvPageSet.Result)
        Process.Start(String.Format("http://www.themoviedb.org/tv/{0}", onAirResult.id))
        onAirResult.visited = True
    End Sub

    Public Sub TmdbNewTvLinkClicked(clickedShow As Object)
        Dim show = CType(clickedShow, TmdbApi.TvShow)
        Process.Start(String.Format("http://www.themoviedb.org/tv/{0}", show.id))
        show.Visited = True
    End Sub

    Public Sub ShowTmdbTvChanges()
        SetBusyState("Downloading TMDb TV Changes")
        If TmdbTvChanges Is Nothing Then
            TmdbTvChanges = tmdb.GetTvChanges()
            ChangesButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
        End If
        TmdbChangesVisibility = Visibility.Visible
        If TmdbTvChanges.results Is Nothing OrElse TmdbTvChanges.results.Count = 0 Then
            SetReadyState(StatusType.Error, "No TMDb TV changes found!")
        Else
            SetReadyState(StatusType.OK, "TMDb TV changes download complete")
        End If

        For Each change In TmdbTvChanges.results.LimitRate(39, TimeSpan.FromSeconds(10))
            Dim show = tmdb.GetTvShow(change.id)
            change.name = show.name
            change.poster_path = show.poster_path_full
        Next

    End Sub

    Public Sub ShowTmdbTvOnAir()
        SetBusyState("Downloading TMDb On The Air TV Shows")
        If TmdbTvOnAir Is Nothing Then
            TmdbTvOnAir = tmdb.GetTvOnAir()
            OnAirButtonBackground = CType(Application.Current.Resources("BackgroundHighlighted"), SolidColorBrush)
            If TmdbTvOnAir Is Nothing OrElse TmdbTvOnAir.results Is Nothing OrElse TmdbTvOnAir.results.Count = 0 Then
                SetReadyState(StatusType.Error, "No on air shows founds.  Please check connection.")
            Else
                SetReadyState(StatusType.OK, "TMDb On The Air TV Shows download complete")
            End If
        End If
        TmdbOnAirVisibility = Visibility.Visible

        'For Each onAirShow In TmdbTvOnAir.results.LimitRate(30, TimeSpan.FromSeconds(10))
        '    Dim show = TmdbApi.GetTvShow(onAirShow.id)
        '    onAirShow.name = show.name
        '    onAirShow.poster_path = show.poster_path_full
        'Next

    End Sub

    Public Delegate Sub AddNewTvShowDelegate(show As TmdbApi.TvShow)

    Public Sub ShowNewTv()
        SetBusyState("Downloading latest TV Show additions")
        TmdbNewTvShows = New ObservableCollection(Of TmdbApi.TvShow)
        Dim latestShow = tmdb.GetLatestTvShow()
        Application.Current.Dispatcher.BeginInvoke(New AddNewTvShowDelegate(AddressOf TmdbNewTvShows.Add), latestShow)
        'TmdbNewTvShows.Add(TmdbApi.GetLatestTvShow())
        If latestShow Is Nothing Then
            SetReadyState(StatusType.Error, "Unable to get latest TV show.  Please check connection.")
            Exit Sub
        End If
        Dim id = latestShow.id
        Dim gate = New RateGate(40, TimeSpan.FromSeconds(10))
        Dim showCount = 1
        Do
            showCount += 1
            StatusText = String.Format("Getting show {0} of 20", showCount)
            gate.WaitToProceed()
            id -= 1
            Dim show = tmdb.GetTvShow(id)
            If show IsNot Nothing Then
                Application.Current.Dispatcher.BeginInvoke(New AddNewTvShowDelegate(AddressOf TmdbNewTvShows.Add), show)
                Thread.Sleep(10)
                'TmdbNewTvShows.Add(show)
            End If
        Loop Until TmdbNewTvShows.Count = 20
        TmdbNewTvVisibility = Visibility.Visible
        SetReadyState(StatusType.OK, "TMDb latest TV show additions download complete")
    End Sub

#Region " HTTP Listener "

    Private listener As HttpListener
    Private listenThread As Thread

    Friend Sub ConfigureHttpListener()
        If My.Settings.EnableHttpListener AndAlso (Not OptionsVisibility = Visibility.Visible) Then
            Try
                listener = New HttpListener With {.AuthenticationSchemes = AuthenticationSchemes.Anonymous}
                listener.Prefixes.Add("http://localhost:" & My.Settings.HttpListenerPort & "/")
                listener.Start()
                listenThread = New Thread(AddressOf StartListener)
                listenThread.Start()
                HttpListenerActive = True
            Catch ex As Exception
                MessageBox.Show("Error starting HttpListener: " & ex.Message, "Http Listener Error")
                'MessageWindow.ShowDialog("Error starting HttpListener: " & ex.Message, "Http Listener Error")
            End Try
        Else
            CloseListener()
        End If
    End Sub

    Private Sub StartListener(s As Object)
        While True
            ProcessRequest()
        End While
    End Sub

    Friend Sub CloseListener()
        If listenThread IsNot Nothing Then
            listenThread.Abort()
        End If
        If listener IsNot Nothing AndAlso listener.IsListening Then
            listener.Stop()
            listener.Close()
        End If
        HttpListenerActive = False
    End Sub

    Private WithEvents App As System.Windows.Application = Application.Current

    Private Sub App_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles App.DispatcherUnhandledException
        CloseListener()
    End Sub

    Private Sub ProcessRequest()
        If listener.IsListening Then
            Dim result = listener.BeginGetContext(New AsyncCallback(AddressOf ListenerCallback), listener)
            result.AsyncWaitHandle.WaitOne()
        End If
    End Sub

    Private Sub ListenerCallback(result As IAsyncResult)
        If listener.IsListening Then
            Dim context = listener.EndGetContext(result)

            context.Response.StatusCode = 200
            context.Response.StatusDescription = "OK"
            context.Response.Headers.Add("Access-Control-Allow-Origin: *")
            Dim responseText = String.Empty

            Dim rawUrl = context.Request.RawUrl
            Dim requestUrl As Uri = context.Request.Url
            Dim command = requestUrl.LocalPath.ToLower()
            Dim queryCollection As Specialized.NameValueCollection = Web.HttpUtility.ParseQueryString(requestUrl.Query)
            Select Case command
                Case "/loadtvdb"
                    Dim id As Integer
                    If Integer.TryParse(queryCollection("id"), id) Then
                        If Data.SeriesInfo.id Is Nothing OrElse Data.SeriesInfo.id.Trim() <> CStr(id) Then
                            Data.SeriesInfo.TmdbId = Nothing
                            Data.SeriesInfo.TvGuideId = Nothing
                            Data.SeriesInfo.IMDB_ID = Nothing
                        End If
                        Data.SeriesInfo.id = CStr(id)
                        responseText = "Load TVDb command recieved. TvData application will now load the information for TVDb id " & id
                        Dim thr As Thread
                        Dim seasonNumber As Integer
                        If Integer.TryParse(queryCollection("seasonNumber"), seasonNumber) Then
                            thr = New Thread(New ParameterizedThreadStart(AddressOf LoadFromTvdb))
                            thr.Start(seasonNumber)
                            responseText &= " Season Number " & seasonNumber & "."
                        Else
                            thr = New Thread(New ThreadStart(AddressOf LoadFromTvdb))
                            thr.Start()
                            responseText &= "."
                        End If
                    Else
                        responseText = "Load TVDb command received but the id parameter was either not supplied or not an integer." &
                            Environment.NewLine &
                            "Example URL: http://localhost:65534/loadtvdb?id=258310&seasonNumber=2"
                    End If
                Case "/loadimdbid"
                    Dim ImdbId As String = queryCollection("id")
                    responseText = "Load IMDb Id command recieved."
                    If Not String.IsNullOrEmpty(ImdbId) Then
                        Data.SeriesInfo.IMDB_ID = ImdbId
                    End If
                Case "/loadtvguideid"
                    Dim tvGuideId As String = queryCollection("id")
                    responseText = "Load TV Guide Id command recieved."
                    If Not String.IsNullOrEmpty(tvGuideId) Then
                        Data.SeriesInfo.TvGuideId = tvGuideId
                    End If
            End Select

            Dim buffer As Byte() = Text.Encoding.UTF8.GetBytes(responseText)
            context.Response.ContentLength64 = buffer.Length
            context.Response.OutputStream.Write(buffer, 0, buffer.Length)
            context.Response.Close()
        End If
    End Sub

#End Region

#Region " Commands "

    Public ReadOnly Property LoadFromTvdbCommand() As ICommand
        Get
            'Return New DelegateCommand(AddressOf LoadFromTvdb)

            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadFromTvdb))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property SaveToFileCommand() As ICommand
        Get
            Return New RelayCommand(AddressOf SaveToFile)
        End Get
    End Property

    Public ReadOnly Property SaveAsToFileCommand() As ICommand
        Get
            Return New RelayCommand(AddressOf SaveAsToFile)
        End Get
    End Property

    Public ReadOnly Property SaveSeasonsToFileCommand() As ICommand
        Get
            Return New RelayCommand(AddressOf SaveSeasonsToFile)
        End Get
    End Property

    Public ReadOnly Property PreviousIdCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Data.SeriesInfo.id = CStr(CDbl(Data.SeriesInfo.id) - 1)
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadFromTvdb)) With {
                                            .Priority = ThreadPriority.AboveNormal
                                        }
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property NextIdCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Data.SeriesInfo.id = CStr(CDbl(Data.SeriesInfo.id) + 1)
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadFromTvdb)) With {
                                            .Priority = ThreadPriority.AboveNormal
                                        }
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ClearCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf Clear))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Private openFile As Microsoft.Win32.OpenFileDialog
    Public ReadOnly Property LoadFromFileCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If openFile Is Nothing Then
                                            openFile = New Microsoft.Win32.OpenFileDialog With {.DefaultExt = "tvxml",
                                                                                                .Filter = "TVDb XML File|*.tvxml",
                                                                                                .Multiselect = False}
                                        End If
                                        If openFile.ShowDialog = True Then
                                            Dim thr As New Thread(New ParameterizedThreadStart(AddressOf LoadFromFile))
                                            thr.Start(openFile.FileName)
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ImportMergeFileCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If openFile Is Nothing Then
                                            openFile = New Microsoft.Win32.OpenFileDialog With {.DefaultExt = "tvxml",
                                                                                                .Filter = "TVDb XML File|*.tvxml",
                                                                                                .Multiselect = False}
                                        End If
                                        If openFile.ShowDialog = True Then
                                            Dim thr As New Thread(New ParameterizedThreadStart(AddressOf ImportMergeFile))
                                            thr.Start(openFile.FileName)
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ExportToExcelCommand As ICommand
        Get
            Return New RelayCommand(AddressOf ExportToExcel,
                                    Function() As Boolean
                                        Return Data.Episodes.Count > 0
                                    End Function)
        End Get
    End Property

    Public ReadOnly Property ShowXmlCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        XmlTextVisibility = Visibility.Visible
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideXmlCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        XmlTextVisibility = Visibility.Collapsed
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyXmlCommand() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Data.Episodes = New TrulyObservableCollection(Of Episode)(
                                            Data.Episodes.OrderBy(Function(e) e.SeasonNumber).
                                                          ThenBy(Function(e) e.EpisodeNumber))
                    My.Computer.Clipboard.SetText(Data.ToString())
                End Sub)
        End Get
    End Property

    Public ReadOnly Property SelectedRowsToXmlCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = Data.Episodes.Where(Function(e) e.IsSelected = True).
                                                         OrderBy(Function(e) e.SeasonNumber).
                                                         ThenBy(Function(e) e.EpisodeNumber).
                                                         ToList()
                    If selectedEpisodes.Count > 0 Then
                        Dim series As New TVSeries() With {.SeriesInfo = Data.SeriesInfo}
                        For Each ep In selectedEpisodes
                            series.Episodes.Add(ep)
                        Next
                        My.Computer.Clipboard.SetText(series.ToString())
                    End If
                End Sub)
        End Get
    End Property

    Public ReadOnly Property EditSelectedRowsSeasonNumberCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = Data.Episodes.Where(Function(e) e.IsSelected = True).ToList()
                    If selectedEpisodes.Count > 0 Then
                        Dim currentSeasonNos = From se In selectedEpisodes Select se.SeasonNumber Distinct
                        Dim msg As String = "Current Season Number"
                        If currentSeasonNos.Count > 1 Then
                            msg &= "s"
                        End If
                        msg &= ": " & String.Join(",", currentSeasonNos)

                        Dim gdw As New GetDataWindow() With {.Message = msg, .DataName = "New Season No:"}
                        If gdw.ShowDialog() = True Then
                            Dim newSeasonNo As Short
                            If Short.TryParse(gdw.Data, newSeasonNo) Then
                                For Each ep In selectedEpisodes
                                    ep.SeasonNumber = newSeasonNo
                                Next
                                ConfigureEpisodes()
                            Else
                                MessageWindow.ShowDialog("New Season Number must be a number, you entered: " & gdw.Data,
                                                         "Error while getting new season number")
                            End If
                        End If
                    End If
                End Sub)
        End Get
    End Property

    Public ReadOnly Property EditSelectedRowsEpisodeNumberCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = Data.Episodes.Where(Function(e) e.IsSelected = True).ToList()
                    If selectedEpisodes.Count > 0 Then
                        Dim gdw As New GetDataWindow() With {
                            .Message = "Enter integer to add to Episode Number. Use negative numbers to subtract.",
                            .DataName = "Episode Number Increment:"}
                        If gdw.ShowDialog = True Then
                            Dim incrementValue As Integer
                            If Integer.TryParse(gdw.Data, incrementValue) Then
                                For Each ep In selectedEpisodes
                                    ep.EpisodeNumber = CShort(ep.EpisodeNumber + incrementValue)
                                Next
                            Else
                                MessageWindow.ShowDialog("Episode increment value must be a number, you entered: " & gdw.Data,
                                                         "Error while getting increment value")
                            End If
                        End If
                    End If
                End Sub)
        End Get
    End Property

    Public ReadOnly Property RemoveSelectedRowsAiredDateCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = Data.Episodes.Where(Function(e) e.IsSelected = True).ToList()
                    For Each ep In selectedEpisodes
                        ep.FirstAired = String.Empty
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property RemoveSelectedRowsProductionCodeCommand As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim selectedEpisodes = Data.Episodes.Where(Function(e) e.IsSelected = True).ToList()
                    For Each ep In selectedEpisodes
                        ep.ProductionCode = String.Empty
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property ToggleImdbColumnVisibility() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If ImdbColumnsVisibility = Visibility.Collapsed Then
                                            ImdbColumnsVisibility = Visibility.Visible
                                        Else
                                            ImdbColumnsVisibility = Visibility.Collapsed
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ToggleTmdbColumnVisibility() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If TmdbColumnsVisibility = Visibility.Collapsed Then
                                            TmdbColumnsVisibility = Visibility.Visible
                                        Else
                                            TmdbColumnsVisibility = Visibility.Collapsed
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ToggleTvGuideColumnVisibility() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If TvGuideColumnsVisibility = Visibility.Collapsed Then
                                            TvGuideColumnsVisibility = Visibility.Visible
                                        Else
                                            TvGuideColumnsVisibility = Visibility.Collapsed
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ToggleTvRageColumnVisibility() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If TvRageColumnsVisibility = Visibility.Collapsed Then
                                            TvRageColumnsVisibility = Visibility.Visible
                                        Else
                                            TvRageColumnsVisibility = Visibility.Collapsed
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ToggleFreebaseColumnVisibility() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If FreebaseColumnsVisibility = Visibility.Collapsed Then
                                            FreebaseColumnsVisibility = Visibility.Visible
                                        Else
                                            FreebaseColumnsVisibility = Visibility.Collapsed
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ToggleAlternativeColumnVisibility() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If AlternativeColumnsVisibility = Visibility.Collapsed Then
                                            AlternativeColumnsVisibility = Visibility.Visible
                                        Else
                                            AlternativeColumnsVisibility = Visibility.Collapsed
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideTmdbSearchResultsCommand() As ICommand
        Get
            Return New RelayCommand(AddressOf HideTmdbSearchResults)
        End Get
    End Property

    Public ReadOnly Property HideExtraEpisodesCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        ExtraEpisodesVisibility = Visibility.Collapsed
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ShowOptionsCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        OptionsVisibility = Visibility.Visible
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideOptionsCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        OptionsVisibility = Visibility.Collapsed
                                    End Sub)
        End Get
    End Property

    'Public ReadOnly Property HideAllDialogsCommand() As ICommand
    '    Get
    '        Return New RelayCommand(Sub()
    '                                    XmlTextVisibility = Visibility.Collapsed
    '                                    TmdbSearchVisibility = Visibility.Collapsed
    '                                    OptionsVisibility = Visibility.Collapsed
    '                                    FindReplaceVisibility = Visibility.Collapsed
    '                                End Sub)
    '    End Get
    'End Property

    Public ReadOnly Property GetSavePathCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim ofd As New Forms.FolderBrowserDialog()
                                        If ofd.ShowDialog() = Forms.DialogResult.OK Then
                                            My.Settings.SavePath = ofd.SelectedPath
                                            My.Settings.Save()
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property GetImdbInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Cursor = Cursors.Wait
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadImdbInfo))
                                        'thr.Priority = ThreadPriority.AboveNormal
                                        thr.Start()
                                    End Sub,
                                       Function() As Boolean
                                           Return Not String.IsNullOrWhiteSpace(Data.SeriesInfo.IMDB_ID)
                                       End Function)
        End Get
    End Property

    Public ReadOnly Property GetImdbByDateInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Cursor = Cursors.Wait
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadImdbByDateInfo))
                                        'thr.Priority = ThreadPriority.AboveNormal
                                        thr.Start()
                                    End Sub,
                                       Function() As Boolean
                                           Return Not String.IsNullOrWhiteSpace(Data.SeriesInfo.IMDB_ID)
                                       End Function)
        End Get
    End Property

    Public ReadOnly Property GetTmdbInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Data.SeriesInfo.TmdbId.HasValue Then
                                            Cursor = Cursors.Wait
                                            Dim thr As New Thread(New ThreadStart(AddressOf LoadTmdbInfo))
                                            'thr.Priority = ThreadPriority.AboveNormal
                                            thr.Start()
                                        Else
                                            Dim thr As New Thread(New ThreadStart(AddressOf SearchTmdb))
                                            'thr.Priority = ThreadPriority.AboveNormal
                                            thr.Start()
                                        End If
                                    End Sub,
                                    Function() As Boolean
                                        Return (Data.SeriesInfo.TmdbId.HasValue OrElse
                                                Not String.IsNullOrWhiteSpace(Data.SeriesInfo.id) OrElse
                                                Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName)) AndAlso
                                                Data.Episodes.Count > 0
                                    End Function)
        End Get
    End Property

    Public ReadOnly Property TmdbSearchFromResultsCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If TmdbSearchSelected IsNot Nothing Then
                                            Data.SeriesInfo.TmdbId = TmdbSearchSelected.id
                                            TmdbSearchVisibility = Visibility.Collapsed
                                            Dim thr As New Thread(New ThreadStart(AddressOf LoadTmdbInfo))
                                            'thr.Priority = ThreadPriority.AboveNormal
                                            thr.Start()
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property GetTvGuideInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Cursor = Cursors.Wait
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadTvGuideInfo))
                                        'thr.Priority = ThreadPriority.AboveNormal
                                        thr.Start()
                                    End Sub,
                                       Function() As Boolean
                                           Return Not String.IsNullOrWhiteSpace(Data.SeriesInfo.TvGuideId)
                                       End Function)
        End Get
    End Property

    Public ReadOnly Property GetTvRageInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Cursor = Cursors.Wait
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadTvRageInfo))
                                        thr.Start()
                                    End Sub,
                                       Function() As Boolean
                                           Return Data.SeriesInfo.TvRageId.HasValue
                                       End Function)
        End Get
    End Property

    Public ReadOnly Property GetFreebaseInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Cursor = Cursors.Wait
                                        Dim thr As New Thread(New ThreadStart(AddressOf LoadFreebaseInfo))
                                        thr.Start()
                                    End Sub,
                                       Function() As Boolean
                                           Return Not String.IsNullOrWhiteSpace(Data.SeriesInfo.FreebaseMid)
                                       End Function)
        End Get
    End Property

    Public ReadOnly Property GetAlternativeInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If openFile Is Nothing Then
                                            openFile = New Microsoft.Win32.OpenFileDialog With {.DefaultExt = "tvxml",
                                                                                                .Filter = "TVDb XML File|*.tvxml",
                                                                                                .Multiselect = False}
                                        End If
                                        If openFile.ShowDialog = True Then
                                            Dim thr As New Thread(New ParameterizedThreadStart(AddressOf LoadAlternativeInfo))
                                            thr.Start(openFile.FileName)
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property PasteToAltCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(AddressOf PasteToAlt)
                                        thr.SetApartmentState(ApartmentState.STA)
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property GetAllInfoCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf GetAllInfo))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ClearEpisodeNamesCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf ClearEpisodeNames))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ClearEpisodeOverviewsCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf ClearEpisodeOverviews))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ClearEpisodeFirstAiredCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf ClearEpisodeFirstAired))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ClearEpisodeProductionCodeCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf ClearEpisodeProductionCode))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property FixEpisodesListCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf FixEpisodesList))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property OpenVicelandWindowCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim vlWindow As New VicelandWindow
                                        AddHandler vlWindow.Closing,
                                            Sub()
                                                My.Settings.VicelandWindowPlacement = vlWindow.GetPlacement
                                                My.Settings.Save()
                                            End Sub
                                        AddHandler vlWindow.SourceInitialized,
                                            Sub()
                                                vlWindow.SetPlacement(My.Settings.VicelandWindowPlacement)
                                            End Sub
                                        vlWindow.Show()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property OpenSundanceWindowCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim sdWindow As New SundanceTvWindow
                                        AddHandler sdWindow.Closing,
                                            Sub()
                                                My.Settings.SundanceTvWindowPlacement = sdWindow.GetPlacement
                                                My.Settings.Save()
                                            End Sub
                                        AddHandler sdWindow.SourceInitialized,
                                            Sub()
                                                sdWindow.SetPlacement(My.Settings.SundanceTvWindowPlacement)
                                            End Sub
                                        sdWindow.Show()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property OpenStarzWindowCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        My.Application.OpenStarzWindow()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property OpenHBOWindowCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        My.Application.OpenHBOWindow()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ConvertDateTitleCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf ConvertDateTitle))
                                        thr.Start()
                                    End Sub,
                                    Function() As Boolean
                                        Return Data.Episodes.Count > 0
                                    End Function)
        End Get
    End Property

    Public ReadOnly Property TitleToAiredDateCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf TitleToAiredDate))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property AddExtraEpisodesCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf AddExtraEpisodes))
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property GetExternalIdsCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(New ThreadStart(AddressOf GetExternalIds))
                                        thr.Start()
                                    End Sub,
                                    Function() As Boolean
                                        Return Data.SeriesInfo.TmdbId.HasValue()
                                    End Function)
        End Get
    End Property

    'Public ReadOnly Property LoadTvRageEpisodeIdsCommand() As ICommand
    '    Get
    '        Return New DelegateCommand(Sub()
    '                                       Dim thr As New Thread(New ThreadStart(AddressOf LoadTvRageEpisodeIds))
    '                                       thr.Start()
    '                                   End Sub)
    '    End Get
    'End Property

    'Public ReadOnly Property LoadFreebaseEpisodeMIdsCommand() As ICommand
    '    Get
    '        Return New DelegateCommand(Sub()
    '                                       Dim thr As New Thread(New ThreadStart(AddressOf LoadFreebaseEpisodeMIds))
    '                                       thr.Start()
    '                                   End Sub)
    '    End Get
    'End Property

    Public ReadOnly Property ShowFindReplaceCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        FindReplaceVisibility = Visibility.Visible
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideFindReplaceCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        FindReplaceVisibility = Visibility.Collapsed
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property FindReplaceExecuteCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(AddressOf FindReplaceExecute)
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property TmdbTvChangesLinkCommand() As ICommand
        Get
            Return New RelayCommand(Of Object)(AddressOf TmdbTvChangesLinkClicked)
        End Get
    End Property

    Public ReadOnly Property TmdbTvOnAirLinkCommand() As ICommand
        Get
            Return New RelayCommand(Of Object)(AddressOf TmdbTvOnAirLinkClicked)
        End Get
    End Property

    Public ReadOnly Property TmdbNewTvLinkCommand() As ICommand
        Get
            Return New RelayCommand(Of Object)(AddressOf TmdbNewTvLinkClicked)
        End Get
    End Property

    Public ReadOnly Property ShowTmdbTvChangesCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(AddressOf ShowTmdbTvChanges)
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ShowTmdbTvOnAirCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(AddressOf ShowTmdbTvOnAir)
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ShowNewTvCommand As ICommand
        Get
            Return New RelayCommand(Sub()
                                        Dim thr As New Thread(AddressOf ShowNewTv)
                                        thr.Start()
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideTmdbTvChangesCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        TmdbChangesVisibility = Visibility.Collapsed
                                        SetReadyState(StatusType.OK, "")
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideTmdbNewTvCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        TmdbNewTvVisibility = Visibility.Collapsed
                                        SetReadyState(StatusType.OK, "")
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property HideTmdbTvOnAirCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        TmdbOnAirVisibility = Visibility.Collapsed
                                        SetReadyState(StatusType.OK, "")
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property AddNewEpisodesCommand() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    Dim newEpWindow As New NewEpisodesWindow
                    If newEpWindow.ShowDialog() = True Then
                        Select Case newEpWindow.SelectedEpisodeGenerationMethod
                            Case NewEpisodesWindow.EpisodeGenerationMethod.Sequential
                                Dim previousDate As Date
                                Dim hasPreviousDate As Boolean = False
                                If newEpWindow.ConsecutiveDates Then
                                    Dim previousEpisode = Data.Episodes.Where(Function(e) e.EpisodeNumber = newEpWindow.StartEpisode - 1).FirstOrDefault()
                                    hasPreviousDate = Date.TryParse(previousEpisode.FirstAired, previousDate)
                                End If
                                For i = newEpWindow.StartEpisode To newEpWindow.EndEpisode
                                    Dim ep As New Episode() With {.EpisodeNumber = i, .SeasonNumber = newEpWindow.SeasonNumber}
                                    If newEpWindow.UseEpisodeXNames Then
                                        ep.EpisodeName = "Episode " & i
                                    End If
                                    If newEpWindow.ConsecutiveDates AndAlso hasPreviousDate Then
                                        ep.FirstAired = GetNextWeekdayDate(previousDate).ToString("yyyy-MM-dd")
                                    End If
                                    Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Data.Episodes, ep})
                                Next
                            Case NewEpisodesWindow.EpisodeGenerationMethod.SpecificType
                                Dim seasonNo As Short = CShort(If(newEpWindow.SeasonNumberSpecific <> Nothing, newEpWindow.SeasonNumberSpecific, newEpWindow.YearSpecific))
                                Dim days As IEnumerable(Of Date)
                                Select Case newEpWindow.SelectedEpisodeGenerationSubMethod
                                    Case NewEpisodesWindow.EpisodeGenerationSubMethod.Everyday
                                        MessageWindow.ShowDialog("Everyday generation not yet implemented", "Not Yet Implemented")
                                    Case NewEpisodesWindow.EpisodeGenerationSubMethod.Fridays
                                        MessageWindow.ShowDialog("Fridays generation not yet implemented", "Not Yet Implemented")
                                    Case NewEpisodesWindow.EpisodeGenerationSubMethod.Weekdays
                                        days = GetWeekDayDays(newEpWindow.YearSpecific)
                                        AddEpisodes(days, seasonNo)
                                    Case NewEpisodesWindow.EpisodeGenerationSubMethod.Weekends
                                        days = GetWeekendDays(newEpWindow.YearSpecific)
                                        AddEpisodes(days, seasonNo)
                                End Select
                        End Select
                    End If
                    newEpWindow.Close()
                End Sub)
        End Get
    End Property

    Private Sub AddEpisodes(days As IEnumerable(Of Date), seasonNumber As Short)
        Dim episodeCounter As Integer = 1
        For Each dy In days
            Dim ep As New Episode() With {
                .EpisodeNumber = CShort(episodeCounter),
                .SeasonNumber = seasonNumber,
                .FirstAired = dy.ToIso8601DateString,
                .EpisodeName = dy.ToString("MMMM d, yyyy")
            }
            Application.Current.Dispatcher.BeginInvoke(New TVSeries.AddEpisodeDelegate(AddressOf TVSeries.AddEpisode), New Object() {Data.Episodes, ep})
            episodeCounter += 1
        Next
    End Sub

    Private Function GetNextWeekdayDate(ByRef dte As Date) As Date
        Do
            dte = dte.AddDays(1)
        Loop While (dte.DayOfWeek = DayOfWeek.Saturday) Or (dte.DayOfWeek = DayOfWeek.Sunday)
        Return dte
    End Function

#Region " Copy Columns "

    Public ReadOnly Property CopyImdbEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_EpisodeName) Then
                            ep.EpisodeName = ep.IMDB_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedImdbEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_EpisodeName) Then
                            ep.EpisodeName = ep.IMDB_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTmdbEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.TMDB_EpisodeName) Then
                            ep.EpisodeName = ep.TMDB_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTmdbEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.TMDB_EpisodeName) Then
                            ep.EpisodeName = ep.TMDB_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvGuideEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrEmpty(ep.TvGuide_EpisodeName) Then
                            ep.EpisodeName = ep.TvGuide_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTvGuideEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.TvGuide_EpisodeName) Then
                            ep.EpisodeName = ep.TvGuide_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvRageEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.TvRage_EpisodeName) Then
                            ep.EpisodeName = ep.TvRage_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTvRageEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrEmpty(ep.TvRage_EpisodeName) Then
                            ep.EpisodeName = ep.TvRage_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyAlternativeEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.Alternative_EpisodeName) Then
                            ep.EpisodeName = ep.Alternative_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedAlternativeEpisodeNames() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.Alternative_EpisodeName) Then
                            ep.EpisodeName = ep.Alternative_EpisodeName
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyImdbAiredDate() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_AiredDate) Then
                            ep.FirstAired = ep.IMDB_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedImdbAiredDate() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_AiredDate) Then
                            ep.FirstAired = ep.IMDB_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTmdbAiredDate() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.TMDB_AiredDate) Then
                            ep.FirstAired = ep.TMDB_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTmdbAiredDates() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.TMDB_AiredDate) Then
                            ep.FirstAired = ep.TMDB_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvGuideAiredDate() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.TvGuide_AiredDate) Then
                            ep.FirstAired = ep.TvGuide_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTvGuideAiredDates() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.TvGuide_AiredDate) Then
                            ep.FirstAired = ep.TvGuide_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvRageAiredDate() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.TvRage_AiredDate) Then
                            ep.FirstAired = ep.TvRage_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTvRageAiredDates() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.TvRage_AiredDate) Then
                            ep.FirstAired = ep.TvRage_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyAlternativeAiredDates() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.Alternative_AiredDate) Then
                            ep.FirstAired = ep.Alternative_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedAlternativeAiredDates() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.Alternative_AiredDate) Then
                            ep.FirstAired = ep.Alternative_AiredDate
                        End If
                    Next
                End Sub)
        End Get
    End Property


    Public ReadOnly Property CopyImdbImdbId() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_ImdbId) Then
                            ep.IMDB_ID = ep.IMDB_ImdbId
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedImdbImdbId() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_ImdbId) Then
                            ep.IMDB_ID = ep.IMDB_ImdbId
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedImdbImdbIdDownOne() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                        If Not String.IsNullOrWhiteSpace(ep.IMDB_ImdbId) Then
                            Dim nextEp = Data.Episodes.Where(Function(e) e.SeasonNumber = ep.SeasonNumber AndAlso
                                                                         e.EpisodeNumber = ep.EpisodeNumber + 1).First()
                            If nextEp IsNot Nothing Then
                                nextEp.IMDB_ID = ep.IMDB_ImdbId
                            End If
                        End If
                    Next
                End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyImdbPlot() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            If Not String.IsNullOrWhiteSpace(ep.IMDB_Plot) Then
                                                ep.Overview = ep.IMDB_Plot
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedImdbPlots() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                                            If Not String.IsNullOrWhiteSpace(ep.IMDB_Plot) Then
                                                ep.Overview = ep.IMDB_Plot
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTmdbOverview() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            If Not String.IsNullOrWhiteSpace(ep.TMDB_Overview) Then
                                                ep.Overview = ep.TMDB_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTmdbOverviews() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                                            If Not String.IsNullOrWhiteSpace(ep.TMDB_Overview) Then
                                                ep.Overview = ep.TMDB_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvGuideOverview() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            If Not String.IsNullOrWhiteSpace(ep.TvGuide_Overview) Then
                                                ep.Overview = ep.TvGuide_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvGuideOverviewToBlank() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            If Not String.IsNullOrWhiteSpace(ep.TvGuide_Overview) AndAlso
                                               String.IsNullOrWhiteSpace(ep.Overview) Then
                                                ep.Overview = ep.TvGuide_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTvGuideOverviews() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                                            If Not String.IsNullOrWhiteSpace(ep.TvGuide_Overview) Then
                                                ep.Overview = ep.TvGuide_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyTvRagePlot() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            If Not String.IsNullOrWhiteSpace(ep.TvRage_Plot) Then
                                                ep.Overview = ep.TvRage_Plot
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedTvRagePlots() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                                            If Not String.IsNullOrWhiteSpace(ep.TvRage_Plot) Then
                                                ep.Overview = ep.TvRage_Plot
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopyAlternativeOverview() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            If Not String.IsNullOrWhiteSpace(ep.Alternative_Overview) Then
                                                ep.Overview = ep.Alternative_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property CopySelectedAlternativeOverviews() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True)
                                            If Not String.IsNullOrWhiteSpace(ep.Alternative_Overview) Then
                                                ep.Overview = ep.Alternative_Overview
                                            End If
                                        Next
                                    End Sub)
        End Get
    End Property

#End Region

#Region " Move Episodes "

    Public ReadOnly Property MoveSelectedIMDbDownOne() As ICommand
        Get
            Return New RelayCommand(
                Sub()
                    For Each ep In Data.Episodes.Where(Function(epi) epi.IsSelected = True).Reverse
                        Dim nextEp = Data.Episodes.Where(Function(e) e.SeasonNumber = ep.SeasonNumber AndAlso
                                                                     e.EpisodeNumber = ep.EpisodeNumber + 1).First()
                        If nextEp IsNot Nothing Then
                            nextEp.IMDB_AiredDate = ep.IMDB_AiredDate
                            ep.IMDB_AiredDate = Nothing
                            nextEp.IMDB_EpisodeName = ep.IMDB_EpisodeName
                            ep.IMDB_EpisodeName = Nothing
                            nextEp.IMDB_ImdbId = ep.IMDB_ImdbId
                            ep.IMDB_ImdbId = Nothing
                            nextEp.IMDB_Plot = ep.IMDB_Plot
                            ep.IMDB_Plot = Nothing
                        End If
                    Next
                End Sub)
        End Get
    End Property


#End Region

#Region " Select Episodes Commands "

    Public ReadOnly Property SelectAllEpisodes() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            ep.IsSelected = True
                                        Next
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property InvertEpisodeSelection() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        For Each ep In Data.Episodes
                                            ep.IsSelected = Not ep.IsSelected
                                        Next
                                    End Sub)
        End Get
    End Property

#End Region

#Region " Label Click Commands "

    Public ReadOnly Property TvdbIdLabelClickCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Not String.IsNullOrEmpty(Data.SeriesInfo.id) Then
                                            Process.Start("http://thetvdb.com/?tab=series&id=" & Data.SeriesInfo.id)
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property TmdbIdLabelClickCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Data.SeriesInfo.TmdbId.HasValue Then
                                            Process.Start("http://themoviedb.org/tv/" & Data.SeriesInfo.TmdbId)
                                        ElseIf Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
                                            Process.Start("http://www.themoviedb.org/search?query=" & Web.HttpUtility.HtmlEncode(Data.SeriesInfo.SeriesName))
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property ImdbIdLabelClickCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Not String.IsNullOrEmpty(Data.SeriesInfo.IMDB_ID) Then
                                            Process.Start("http://imdb.com/title/" & Data.SeriesInfo.IMDB_ID)
                                        ElseIf Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
                                            Process.Start("http://www.imdb.com/find?s=all&q=" & Web.HttpUtility.HtmlEncode(Data.SeriesInfo.SeriesName))
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property TvGuideIdLabelClickCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Not String.IsNullOrEmpty(Data.SeriesInfo.TvGuideId) Then
                                            If Data.SeriesInfo.SeriesName Is Nothing Then
                                                Data.SeriesInfo.SeriesName = String.Empty
                                            End If
                                            Dim seriesNameUrl = Data.SeriesInfo.SeriesName.ToLower().Replace(" ", "-")
                                            Process.Start("http://www.tvguide.com/tvshows/" & seriesNameUrl & "/" & Data.SeriesInfo.TvGuideId)
                                        ElseIf Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
                                            Process.Start("http://www.tvguide.com/search/media/?keyword=" & Web.HttpUtility.HtmlEncode(Data.SeriesInfo.SeriesName))
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property TvRageIdLabelClickCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Data.SeriesInfo.TvRageId.HasValue Then
                                            Process.Start("http://www.tvrage.com/shows/id-" & Data.SeriesInfo.TvRageId.Value)
                                        ElseIf Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
                                            Process.Start(String.Format("http://www.tvrage.com/search.php?search={0}&searchin=2&button=Go", Web.HttpUtility.HtmlEncode(Data.SeriesInfo.SeriesName)))
                                        End If
                                    End Sub)
        End Get
    End Property

    Public ReadOnly Property FreebaseMIdLabelClickCommand() As ICommand
        Get
            Return New RelayCommand(Sub()
                                        If Not String.IsNullOrWhiteSpace(Data.SeriesInfo.FreebaseMid) Then
                                            Process.Start("http://freebase.com" & Data.SeriesInfo.FreebaseMid)
                                        ElseIf Not String.IsNullOrWhiteSpace(Data.SeriesInfo.SeriesName) Then
                                            Process.Start(String.Format("https://www.freebase.com/search?query={0}&any=%2Ftv%2Ftv_program", Web.HttpUtility.HtmlEncode(Data.SeriesInfo.SeriesName)))
                                        End If
                                    End Sub)
        End Get
    End Property


#End Region

    'Private SelectionChangedCommand =
    '    New RelayCommand(Of IList)(Function(items)
    '                                   If items Is Nothing Then
    '                                       NumberOfItemsSelected = 0
    '                                       Return
    '                                   End If

    '                                   NumberOfItemsSelected = items.Count

    '                               End Function)


#End Region

#Region " INotifyPropertyChanged Members "

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub RaisePropertyChanged(ByVal propertyName As String)
        Dim propertyChanged As PropertyChangedEventHandler = PropertyChangedEvent
        If (Not (propertyChanged) Is Nothing) Then
            propertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End If
    End Sub

#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        tmdb.Dispose()
        CloseListener()
    End Sub

    Public Sub OnFileDrop(filepaths() As String) Implements IFileDragDropTarget.OnFileDrop
        If filepaths.Length > 1 Then
            If Not MessageWindow.ShowDialog("Multiple files dropped. Only the first will be opened.", "Multiple Files Detected", True) Then
                Exit Sub
            End If
        End If

        Dim loadAsAlternative = Not MessageWindow.ShowDialog("Click Cancel to load file in Alternative columns", "Replace current data?", True)
        If loadAsAlternative Then
            LoadAlternativeInfo(filepaths(0))
        Else
            LoadFromFile(filepaths(0))
        End If
    End Sub

End Class
