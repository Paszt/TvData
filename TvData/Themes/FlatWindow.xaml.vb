Option Strict On
Imports System.Windows.Interop
Imports System.Runtime.InteropServices

<TemplatePart(Name:="PART_TitleBar", Type:=GetType(UIElement))>
<TemplatePart(Name:="PART_WindowTitleBackground", Type:=GetType(UIElement))>
<TemplatePart(Name:="PART_Max", Type:=GetType(Button))>
<TemplatePart(Name:="PART_Close", Type:=GetType(Button))>
<TemplatePart(Name:="PART_Min", Type:=GetType(Button))>
<TemplatePart(Name:="PART_Icon", Type:=GetType(Path))>
<DebuggerStepThrough>
Public Class FlatWindow
    Inherits Window

    Private Const PART_TitleBar As String = "PART_TitleBar"
    Private Const PART_WindowTitleBackground As String = "PART_WindowTitleBackground"
    Private Const PART_Icon As String = "PART_Icon"
    Private Const PART_Max As String = "PART_Max"
    Private Const PART_Close As String = "PART_Close"
    Private Const PART_Min As String = "PART_Min"

    Private titleBar As UIElement
    Private titleBarBackground As UIElement
    Private titleBaricon As Path
    Private minButton As Button
    Private maxButton As Button
    Private closeButton As Button
    Private resizeGrid As Grid

    Shared Sub New()
        'This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
        'This style is defined in themes\generic.xaml
        DefaultStyleKeyProperty.OverrideMetadata(GetType(FlatWindow), New FrameworkPropertyMetadata(GetType(FlatWindow)))
    End Sub

#Region " Dependency Properties "

    Public Shared ReadOnly TitlebarHeightProperty As DependencyProperty = DependencyProperty.Register("TitlebarHeight", GetType(Integer),
                                                                                                      GetType(FlatWindow), New PropertyMetadata(34))

    Public Property TitlebarHeight() As Integer
        Get
            Return CInt(GetValue(TitlebarHeightProperty))
        End Get
        Set(value As Integer)
            SetValue(TitlebarHeightProperty, value)
        End Set
    End Property

    Public Shared ReadOnly BorderColorProperty As DependencyProperty = DependencyProperty.Register("BorderColor", GetType(SolidColorBrush),
                                                                                                   GetType(FlatWindow), New PropertyMetadata(CType(Application.Current.Resources("BackgroundSelected"), SolidColorBrush)))

    Public Property BorderColor() As SolidColorBrush
        Get
            Return CType(GetValue(BorderColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(BorderColorProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ShowMinMaxProperty As DependencyProperty = DependencyProperty.Register("ShowMinMax", GetType(Boolean),
                                                                                                  GetType(FlatWindow), New PropertyMetadata(True))

    Public Property ShowMinMax As Boolean
        Get
            Return CBool(GetValue(ShowMinMaxProperty))
        End Get
        Set(value As Boolean)
            SetValue(ShowMinMaxProperty, value)
        End Set
    End Property

#End Region

    Protected Sub TitleBarMouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.ChangedButton = MouseButton.Left Then
            ' if UseNoneWindowStyle = true no movement, no maximize please
            Dim windowHandle As IntPtr = New WindowInteropHelper(Me).Handle
            UnsafeNativeMethods.ReleaseCapture()

            Dim mPoint = Mouse.GetPosition(Me)

            Dim wpfPoint = Me.PointToScreen(mPoint)
            Dim x = Convert.ToInt16(wpfPoint.X)
            Dim y = Convert.ToInt16(wpfPoint.Y)
            Dim lParam = x Or (y << 16)
            UnsafeNativeMethods.SendMessage(windowHandle, Constants.WM_NCLBUTTONDOWN, CType(Constants.HT_CAPTION, IntPtr), CType(lParam, IntPtr))

            If e.ClickCount = 2 AndAlso (Me.ResizeMode = ResizeMode.CanResizeWithGrip OrElse Me.ResizeMode = ResizeMode.CanResize) AndAlso mPoint.Y <= Me.TitlebarHeight AndAlso Me.TitlebarHeight > 0 Then
                If Me.WindowState = WindowState.Maximized Then
                    'Microsoft.Windows.Shell.SystemCommands.RestoreWindow(Me)
                    Me.WindowState = WindowState.Normal
                Else
                    'Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(Me)
                    Me.WindowState = WindowState.Maximized
                End If
            End If
            e.Handled = True
        End If
    End Sub

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()
        'AddHandler Me.MouseDown, AddressOf TitleBarMouseDown
        titleBar = CType(GetTemplateChild(PART_TitleBar), UIElement)
        AddHandler titleBar.MouseDown, AddressOf TitleBarMouseDown
        titleBarBackground = CType(GetTemplateChild(PART_WindowTitleBackground), UIElement)
        AddHandler titleBarBackground.MouseDown, AddressOf TitleBarMouseDown
        titleBaricon = CType(GetTemplateChild(PART_Icon), Path)
        AddHandler titleBaricon.MouseDown, AddressOf TitleBarMouseDown

        closeButton = TryCast(Template.FindName("PART_Close", Me), Button)
        If closeButton IsNot Nothing Then
            AddHandler closeButton.Click, AddressOf CloseClick
        End If

        maxButton = TryCast(Template.FindName("PART_Max", Me), Button)
        If maxButton IsNot Nothing Then
            AddHandler maxButton.Click, AddressOf MaximiseClick
        End If

        minButton = TryCast(Template.FindName("PART_Min", Me), Button)
        If minButton IsNot Nothing Then
            AddHandler minButton.Click, AddressOf MinimiseClick
        End If

        resizeGrid = TryCast(GetTemplateChild("resizeGrid"), Grid)
        If resizeGrid IsNot Nothing Then
            For Each element As UIElement In resizeGrid.Children
                Dim resizeRectangle As Rectangle = TryCast(element, Rectangle)
                If resizeRectangle IsNot Nothing Then
                    If Me.ResizeMode = ResizeMode.NoResize Then
                        resizeRectangle.Cursor = Nothing
                    Else
                        AddHandler resizeRectangle.PreviewMouseLeftButtonDown, AddressOf Resize
                        'AddHandler resizeRectangle.MouseMove, AddressOf resizeRectangle_MouseMove
                    End If

                End If
            Next
        End If
        RefreshMaximiseIconState()
    End Sub


#Region " Maximize Covers Taskbar Fix "


    Private Function WindowProc(hwnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr, ByRef handled As Boolean) As IntPtr
        Select Case msg
            'Case Constants.WM_GETMINMAXINFO
            '    Dim mmi = CType(Marshal.PtrToStructure(lParam, GetType(MINMAXINFO)), MINMAXINFO)
            '    mmi.ptMaxPosition.x = Screen.WorkingArea.Left - 1
            '    mmi.ptMaxPosition.y = Screen.WorkingArea.Top - 1
            '    mmi.ptMaxSize.x = Screen.WorkingArea.Width + 2
            '    mmi.ptMaxSize.y = Screen.WorkingArea.Height + 2
            '    Marshal.StructureToPtr(mmi, lParam, True)
            '    handled = True
            Case Constants.WM_GETMINMAXINFO
                Dim mmi = CType(Marshal.PtrToStructure(lParam, GetType(MINMAXINFO)), MINMAXINFO)
                Dim nearestScreen = Screen

                mmi.ptMaxPosition.x = 0
                mmi.ptMaxPosition.y = 0
                mmi.ptMaxSize.x = nearestScreen.WorkingArea.Right - nearestScreen.WorkingArea.Left
                mmi.ptMaxSize.y = nearestScreen.WorkingArea.Bottom - nearestScreen.WorkingArea.Top
                mmi.ptMinTrackSize.x = CInt(MinWidth)
                mmi.ptMinTrackSize.y = CInt(MinHeight)

                Marshal.StructureToPtr(mmi, lParam, True)
                handled = True
                'Case Constants.WM_WINDOWPOSCHANGING
                '    Dim pos As WINDOWPOS = CType(Marshal.PtrToStructure(lParam, GetType(WINDOWPOS)), WINDOWPOS)
                '    If (pos.flags And CInt(SetWindowPosFlags.NOMOVE)) <> 0 Then
                '        Return IntPtr.Zero
                '    End If
                '    Dim wnd As Window = DirectCast(HwndSource.FromHwnd(hwnd).RootVisual, Window)
                '    If wnd Is Nothing Then
                '        Return IntPtr.Zero
                '    End If
                '    Dim changedPos As Boolean = False
                '    If pos.cx < MinWidth Then
                '        pos.cx = CInt(MinWidth)
                '        changedPos = True
                '    End If
                '    If pos.cy < MinHeight Then
                '        pos.cy = CInt(MinHeight)
                '        changedPos = True
                '    End If
                '    If Not changedPos Then
                '        Return IntPtr.Zero
                '    End If

                '    Marshal.StructureToPtr(pos, lParam, True)
                '    handled = True

        End Select
        Return IntPtr.Zero
    End Function

    Private Sub FlatWindow_SourceInitialized(sender As Object, e As EventArgs) Handles Me.SourceInitialized
        Dim window = TryCast(sender, Window)

        If window IsNot Nothing Then
            Dim handle As IntPtr = New WindowInteropHelper(window).Handle
            HwndSource.FromHwnd(handle).AddHook(AddressOf WindowProc)
        End If
    End Sub

    Private ReadOnly Property Screen As Forms.Screen
        Get
            Return System.Windows.Forms.Screen.FromHandle(New WindowInteropHelper(Me).Handle)
        End Get
    End Property

#End Region

#Region " Window Button Event Handlers "

    Public Event ClosingWindow As ClosingWindowEventHandler
    Public Delegate Sub ClosingWindowEventHandler(sender As Object, args As ClosingWindowEventHandlerArgs)

    Private Sub CloseClick(sender As Object, e As RoutedEventArgs)
        Dim closingWindowEventHandlerArgs = New ClosingWindowEventHandlerArgs()
        OnClosingWindow(closingWindowEventHandlerArgs)

        If closingWindowEventHandlerArgs.Cancelled Then
            Return
        End If

        Me.Close()
    End Sub

    Protected Sub OnClosingWindow(args As ClosingWindowEventHandlerArgs)
        RaiseEvent ClosingWindow(Me, args)
    End Sub

    Private Sub MaximiseClick(sender As Object, e As RoutedEventArgs)
        If Me.WindowState = WindowState.Maximized Then
            'Microsoft.Windows.Shell.SystemCommands.RestoreWindow(parentWindow)
            Me.WindowState = WindowState.Normal
        Else
            'Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(parentWindow)
            Me.WindowState = WindowState.Maximized
        End If

        RefreshMaximiseIconState()
    End Sub

    Private Sub MinimiseClick(sender As Object, e As RoutedEventArgs)
        Me.WindowState = WindowState.Minimized
    End Sub

    Private Sub RefreshMaximiseIconState()
        Dim maxpath = DirectCast(maxButton.FindName("MaximisePath"), Path)
        Dim restorepath = DirectCast(maxButton.FindName("RestorePath"), Path)
        If Me.WindowState = WindowState.Normal Then
            maxpath.Visibility = Visibility.Visible
            restorepath.Visibility = Visibility.Collapsed
            maxButton.ToolTip = "Maximize"
            resizeGrid.Visibility = Visibility.Visible
        Else
            restorepath.Visibility = Visibility.Visible
            maxpath.Visibility = Visibility.Collapsed
            resizeGrid.Visibility = Visibility.Hidden
            maxButton.ToolTip = "Restore"
        End If
    End Sub

    Protected Overrides Sub OnStateChanged(e As EventArgs)
        RefreshMaximiseIconState()
        MyBase.OnStateChanged(e)
    End Sub

#End Region

#Region " Window Resize "

    Private Sub Resize(sender As Object, e As MouseButtonEventArgs)
        Dim resizeRect As Rectangle = TryCast(sender, Rectangle)
        Cursor = resizeRect.Cursor
        If WindowState <> WindowState.Maximized AndAlso Me.ResizeMode <> ResizeMode.NoResize Then
            UnsafeNativeMethods.SendMessage(New WindowInteropHelper(Me).Handle, Constants.WM_SYSCOMMAND, CType(61440 + CInt(CType(sender, Rectangle).Tag), IntPtr), IntPtr.Zero)
        End If
    End Sub

    'Private Sub resizeRectangle_MouseMove(sender As Object, e As MouseEventArgs)
    '    Dim ResizeRect As Rectangle = TryCast(sender, Rectangle)
    '    Cursor = ResizeRect.Cursor
    'End Sub

    Private Sub FlatWindow_PreviewMouseMove(sender As Object, e As MouseEventArgs) Handles Me.PreviewMouseMove
        If Mouse.LeftButton = MouseButtonState.Released Then
            Cursor = Nothing
        End If
    End Sub

    Private Sub FlatWindow_StateChanged(sender As Object, e As EventArgs) Handles Me.StateChanged
        Dim border As Border = CType(GetTemplateChild("PART_BorderOutline"), Border)
        If Me.WindowState = WindowState.Maximized Then
            border.BorderThickness = New Thickness(0.00)
            'border.BorderBrush = CType(Windows.Application.Current.FindResource("Background"), Brush)
        Else
            border.BorderThickness = New Thickness(1.0)
            'border.BorderBrush = BorderColor
        End If
    End Sub

#End Region

End Class
