Option Strict On
Imports System.Windows.Threading
Imports System.ComponentModel

Class Application

    Private mainVM As MainViewModel

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        '' Startup Options
        ''  -starz        opens the starz window only
        ''  {file name}   open the mainwindow and load the file

        If e.Args.Length > 0 AndAlso e.Args(0).ToLower() = "-starz" Then
            OpenStarzWindow()
        ElseIf e.Args.Length > 0 AndAlso e.Args(0).ToLower() = "-hbo" Then
            OpenHBOWindow()
        Else
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
            'Set Toolbar MenuItem Style
            Dim darkMenuItemStyle = CType(Current.FindResource("DarkMenuItem"), Style)
            For Each mi As MenuItem In mainWin.ToolBarMenu.Items
                mi.Style = darkMenuItemStyle
            Next
            mainWin.Show()

            If e.Args.Length > 0 AndAlso IO.File.Exists(e.Args(0)) Then
                Dim mvm = CType(mainWin.DataContext, MainViewModel)
                mvm.LoadFromFile(e.Args(0))
            End If
        End If

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

    Friend Sub OpenStarzWindow()
        Dim szWindow As New StarzWindow()
        AddHandler szWindow.Closing,
            Sub()
                My.Settings.StarzWindowPlacement = szWindow.GetPlacement()
                My.Settings.Save()
            End Sub
        AddHandler szWindow.SourceInitialized,
            Sub()
                szWindow.SetPlacement(My.Settings.StarzWindowPlacement)
            End Sub
        szWindow.Show()
    End Sub

    Friend Sub OpenHBOWindow()
        Dim hWindow As New HBOWindow()
        AddHandler hWindow.Closing,
            Sub()
                My.Settings.HBOWindowPlacement = hWindow.GetPlacement()
                My.Settings.Save()
            End Sub
        AddHandler hWindow.SourceInitialized,
            Sub()
                hWindow.SetPlacement(My.Settings.HBOWindowPlacement)
            End Sub
        hWindow.Show()
    End Sub

End Class
