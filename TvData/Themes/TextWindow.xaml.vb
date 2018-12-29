Public Class TextWindow

    Public Delegate Function ShowDialogDelegate(caption As String, messageText As String, HyperlinkUrl As String) As Boolean?

    Public Overloads Shared Function ShowDialog(caption As String, messageText As String, Optional HyperlinkUrl As String = "") As Boolean?
        If Application.Current IsNot Nothing Then
            Return CType(Application.Current.Dispatcher.Invoke(New ShowDialogDelegate(AddressOf ShowDialogInternal), New Object() {caption, messageText, HyperlinkUrl}), Boolean?)
        End If
    End Function

    Public Delegate Function GetTextDelegate(caption As String, messageText As String, HyperlinkUrl As String) As String

    Public Overloads Shared Function GetText(caption As String, messageText As String, Optional HyperlinkUrl As String = "") As String
        If Application.Current IsNot Nothing Then
            Return CType(Application.Current.Dispatcher.Invoke(New GetTextDelegate(AddressOf GetTextInternal), New Object() {caption, messageText, HyperlinkUrl}), String)
        End If
        Throw New ApplicationException("Can't find Application")
    End Function

    Private Shared Function GetTextInternal(caption As String, messageText As String, HyperlinkUrl As String) As String
        'tw.SizeToContent = Windows.SizeToContent.WidthAndHeight
        Dim tw As New TextWindow() With {
            .Owner = Application.Current.MainWindow,
            .WindowStartupLocation = WindowStartupLocation.CenterOwner,
            .MinHeight = 350,
            .MinWidth = 450,
            .ShowMinMax = False,
            .ShowInTaskbar = False,
            .Title = caption
        }
        tw.messageTextBlock.Text = messageText
        If String.IsNullOrEmpty(HyperlinkUrl) Then
            tw.hyperlinkContainer.Visibility = Visibility.Collapsed
        Else
            tw.theHyperLink.NavigateUri = New Uri(HyperlinkUrl)
            tw.hyperLinkTextBlock.Text = HyperlinkUrl
        End If
        tw.CancelButton.Focus()

        If tw.ShowDialog() = True Then
            Return tw.InputText
        End If
        Return String.Empty
    End Function

    Private Shared Function ShowDialogInternal(caption As String, messageText As String, HyperlinkUrl As String) As Boolean?
        Dim tw As New TextWindow() With {
            .Owner = Application.Current.MainWindow,
            .WindowStartupLocation = WindowStartupLocation.CenterOwner,
            .MinHeight = 350,
            .MinWidth = 450,
            .ShowMinMax = False,
            .ShowInTaskbar = False,
            .Title = caption
        }
        tw.messageTextBlock.Text = messageText
        'tw.SizeToContent = Windows.SizeToContent.WidthAndHeight
        If String.IsNullOrEmpty(HyperlinkUrl) Then
            tw.hyperlinkContainer.Visibility = Visibility.Collapsed
        Else
            tw.theHyperLink.NavigateUri = New Uri(HyperlinkUrl)
            tw.hyperLinkTextBlock.Text = HyperlinkUrl
        End If

        tw.CancelButton.Focus()
        Return tw.ShowDialog()
    End Function

    Private Overloads Function ShowDialog() As Boolean?
        Return MyBase.ShowDialog()
    End Function

    Private Overloads Sub Show()
    End Sub

    Private Sub OkButton_Click(sender As Object, e As RoutedEventArgs) Handles OkButton.Click
        DialogResult = True
        Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        DialogResult = False
        Close()
    End Sub

    Public Property InputText As String
        Get
            Return InputTextBox.Text
        End Get
        Set(value As String)
            InputTextBox.Text = value
        End Set
    End Property

    Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
        Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))
        e.Handled = True
    End Sub

End Class
