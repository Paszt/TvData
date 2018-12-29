Option Strict On

Public Class NewEpisodesWindow

    Private Sub Initialize()
        ShowInTaskbar = False
        Owner = Application.Current.MainWindow
        WindowStartupLocation = WindowStartupLocation.CenterOwner
        ShowMinMax = False
    End Sub

    Public Shadows Function ShowDialog() As Boolean?
        Initialize()
        Return Show()
    End Function

    Private Shadows Function Show() As Boolean?
        Initialize()
        Return MyBase.ShowDialog()
    End Function

#Region " Properties "

#Region " Sequential "

    Public Property SeasonNumber As Short
        Get
            Dim retvalue As Short
            If Short.TryParse(SeasonTextBox.Text, retvalue) Then
                Return retvalue
            Else
                Return Nothing
            End If
        End Get
        Set(value As Short)
            SeasonTextBox.Text = CStr(value)
        End Set
    End Property

    Public Property StartEpisode As Short
        Get
            Dim retValue As Short
            If Short.TryParse(StartEpisodeTextBox.Text, retValue) Then
                Return retValue
            Else
                Return Nothing
            End If
        End Get
        Set(value As Short)
            StartEpisodeTextBox.Text = CStr(value)
        End Set
    End Property

    Public Property EndEpisode As Short
        Get
            Dim retValue As Short
            If Short.TryParse(EndEpisodeTextBox.Text, retValue) Then
                Return retValue
            Else
                Return Nothing
            End If
        End Get
        Set(value As Short)
            EndEpisodeTextBox.Text = CStr(value)
        End Set
    End Property

    Public Property UseEpisodeXNames As Boolean
        Get
            Return UseEpisodeXNameCheckbox.IsChecked.Value
        End Get
        Set(value As Boolean)
            UseEpisodeXNameCheckbox.IsChecked = value
        End Set
    End Property

    Public Property ConsecutiveDates As Boolean
        Get
            Return ConsecutiveDatesCheckbox.IsChecked.Value
        End Get
        Set(value As Boolean)
            ConsecutiveDatesCheckbox.IsChecked = value
        End Set
    End Property

    Public Property SkipWeekends As Boolean
        Get
            Return SkipWeekendsCheckbox.IsChecked.Value
        End Get
        Set(value As Boolean)
            SkipWeekendsCheckbox.IsChecked = value
        End Set
    End Property

#End Region

#Region " Specific Days "

    Public ReadOnly Property SeasonNumberSpecific As Integer
        Get
            Return CInt(SeasonSpecificTextBox.Text)
        End Get
    End Property

    Public ReadOnly Property SelectedEpisodeGenerationSubMethod As EpisodeGenerationSubMethod
        Get
            Return CType(GenerationSubTypeCombobox.SelectedValue, EpisodeGenerationSubMethod)
        End Get
    End Property

#End Region

    Public ReadOnly Property SelectedEpisodeGenerationMethod As EpisodeGenerationMethod
        Get
            Return CType(MainTabControl.SelectedIndex, EpisodeGenerationMethod)
        End Get
    End Property

#End Region

#Region " Control Event Handlers "

    Private Sub NewEpisodesWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        GenerationSubTypeCombobox.ItemsSource = [Enum].GetValues(GetType(EpisodeGenerationSubMethod)).Cast(Of EpisodeGenerationSubMethod)()
    End Sub

    Private Sub OkButton_Click(sender As Object, e As RoutedEventArgs) Handles OkButton.Click
        DialogResult = True
        Hide()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        DialogResult = False
        Hide()
    End Sub

    Private Sub ConsecutiveDatesCheckbox_Click(sender As Object, e As RoutedEventArgs) Handles ConsecutiveDatesCheckbox.Click
        SkipWeekendsCheckbox.IsEnabled = ConsecutiveDatesCheckbox.IsChecked.Value
    End Sub

#End Region

    Public Enum EpisodeGenerationMethod As Integer
        Sequential = 0
        SpecificType = 1
    End Enum

    Public Enum EpisodeGenerationSubMethod
        Everyday
        Weekdays
        Weekends
        Fridays
    End Enum

End Class
