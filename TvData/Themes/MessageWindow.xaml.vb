Public Class MessageWindow

    Public Delegate Function ShowDialogDelegate(messageWindowText As String, caption As String, ShowCancel As Boolean) As Boolean?

    Public Overloads Shared Function ShowDialog(messageWindowText As String, caption As String, Optional ShowCancel As Boolean = False) As Boolean?
        If Application.Current IsNot Nothing Then
            Return CType(Application.Current.Dispatcher.Invoke(New ShowDialogDelegate(AddressOf ShowDialogInternal), New Object() {messageWindowText, caption, ShowCancel}), Boolean?)
        End If
    End Function

    Private Shared Function ShowDialogInternal(messageWindowText As String, caption As String, Optional ShowCancel As Boolean = False) As Boolean?
        Dim mw As New MessageWindow() With {
            .WindowStartupLocation = WindowStartupLocation.CenterOwner,
            .MinHeight = 200,
            .MinWidth = 450,
            .MaxWidth = 650,
            .SizeToContent = SizeToContent.WidthAndHeight,
            .ShowMinMax = False,
            .ShowInTaskbar = False,
            .Title = caption
        }
        mw.Owner = Application.Current.Windows.OfType(Of Window)().FirstOrDefault(Function(w) w.IsActive)
        'If Application.Current.MainWindow IsNot Nothing Then
        '    mw.Owner = Application.Current.MainWindow
        'End If
        mw.MessageTextBlock.Text = messageWindowText
        If ShowCancel Then
            mw.CancelButton.Visibility = Visibility.Visible
        Else
            mw.CancelButton.Visibility = Visibility.Collapsed
        End If
        If ShowCancel = True Then
            mw.CancelButton.Focus()
        Else
            mw.OkButton.Focus()
        End If
        Return mw.ShowDialog()
    End Function

    Private Overloads Function ShowDialog() As Boolean?
        Return MyBase.ShowDialog()
    End Function

    Private Overloads Sub Show()
    End Sub

    Private Sub OkButton_Click(sender As Object, e As RoutedEventArgs) Handles OkButton.Click
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        Me.DialogResult = False
        Me.Close()
    End Sub

End Class
