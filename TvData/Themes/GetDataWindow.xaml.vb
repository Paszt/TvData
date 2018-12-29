Public Class GetDataWindow

    Private Sub Initialize()
        Me.ShowInTaskbar = False
        Me.Owner = Application.Current.MainWindow
        Me.WindowStartupLocation = WindowStartupLocation.CenterOwner
        Me.ShowMinMax = False
    End Sub

    Public Shadows Function ShowDialog() As Boolean?
        Return Show()
    End Function

    Private Shadows Function Show() As Boolean?
        Initialize()
        Return MyBase.ShowDialog()
    End Function

    Public Shadows Function Show(TheMessage As String, TheDataName As String) As Boolean?
        Message = TheMessage
        DataName = TheDataName
        Return ShowDialog()
    End Function

    Public Shadows Function ShowDialog(TheMessage As String, TheDataName As String) As Boolean?
        Message = TheMessage
        DataName = TheDataName
        Return ShowDialog()
    End Function

#Region " Properties "

    Public Property Data As String
        Get
            Return DataTextBox.Text
        End Get
        Set(value As String)
            DataTextBox.Text = value
        End Set
    End Property

    Public Property Message As String
        Get
            Return MessageTextBlock.Text
        End Get
        Set(value As String)
            MessageTextBlock.Text = value
        End Set
    End Property

    Public Property DataName As String
        Get
            Return DataNameTextBlock.Text
        End Get
        Set(value As String)
            DataNameTextBlock.Text = value
        End Set
    End Property

#End Region

#Region " Control Event Handlers "

    Private Sub OkButton_Click(sender As Object, e As RoutedEventArgs) Handles OkButton.Click
        Me.DialogResult = True
        Me.Hide()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
        Me.Hide()
    End Sub

#End Region

End Class
