Option Strict On
Imports System.Windows.Threading
Imports System.ComponentModel

Class Application

    Private mainVM As MainViewModel

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Dim mainWin As New MainWindow()
        mainVM = CType(mainWin.DataContext, MainViewModel)
        AddHandler mainWin.Closing,
            Sub()
                My.Settings.MainWindowPlacement = mainWin.GetPlacement()
                My.Settings.Save()

                mainVM.CloseListener()
            End Sub
        AddHandler mainWin.SourceInitialized,
            Sub()
                mainWin.SetPlacement(My.Settings.MainWindowPlacement)
            End Sub
        mainVM.ConfigureHttpListener()
        mainWin.Show()
    End Sub

    Private Sub Application_DispatcherUnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        MessageWindow.ShowDialog(e.Exception.Message, "Unhandled Exception")
        e.Handled = True
    End Sub

    Private WithEvents Settings As Configuration.ApplicationSettingsBase = My.Settings
    Private Sub Settings_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Settings.PropertyChanged
        If Not DesignerProperties.GetIsInDesignMode(New DependencyObject()) Then
            If e.PropertyName = "EnableHttpListener" OrElse e.PropertyName = "HttpListenerPort" Then
                mainVM.ConfigureHttpListener()
            End If
        End If
    End Sub

End Class
