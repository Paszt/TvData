Public NotInheritable Class Constants

    Public Const WM_SYSCOMMAND As Integer = &H112

    Public Const MONITOR_DEFAULTTONEAREST As Integer = &H2
    Public Const WM_NCLBUTTONDOWN As Integer = &HA1
    Public Const WM_NCCALCSIZE As Integer = &H83
    Public Const WM_NCPAINT As Integer = &H85
    Public Const WM_NCACTIVATE As Integer = &H86
    Public Const WM_GETMINMAXINFO As Integer = &H24
    Public Const WM_WINDOWPOSCHANGING As Integer = &H46
    Public Const WM_CREATE As Integer = &H1
    Public Const WS_MAXIMIZE As Long = &H1000000
    Public Const GCLP_HBRBACKGROUND As Integer = -&HA
    Public Const WM_NCHITTEST As Integer = &H84
    Public Const HT_CAPTION As Integer = &H2
    Public Const HTLEFT As Integer = &HA
    Public Const HTRIGHT As Integer = &HB
    Public Const HTTOP As Integer = &HC
    Public Const HTTOPLEFT As Integer = &HD
    Public Const HTTOPRIGHT As Integer = &HE
    Public Const HTBOTTOM As Integer = &HF
    Public Const HTBOTTOMLEFT As Integer = &H10
    Public Const HTBOTTOMRIGHT As Integer = &H11
    Public Const TPM_RETURNCMD As UInteger = &H100
    Public Const TPM_LEFTBUTTON As UInteger = &H0
    Public Const SW_SHOWNORMAL As Integer = 1
    Public Const SW_SHOWMINIMIZED As Integer = 2
    Public Const SYSCOMMAND As UInteger = &H112
    Public Const WM_INITMENU As Integer = &H116

    Public Const SC_MAXIMIZE As Integer = &HF030
    Public Const SC_SIZE As Integer = &HF000
    Public Const SC_MINIMIZE As Integer = &HF020
    Public Const SC_RESTORE As Integer = &HF120
    Public Const SC_MOVE As Integer = &HF010
    Public Const MF_GRAYED As Integer = &H1
    Public Const MF_BYCOMMAND As Integer = &H0
    Public Const MF_ENABLED As Integer = &H0

    Public Const SWP_NOSIZE As UInteger = &H1
    Public Const SWP_NOMOVE As UInteger = &H2
    Public Const SWP_NOZORDER As UInteger = &H4
    Public Const SWP_NOREDRAW As UInteger = &H8
    Public Const SWP_NOACTIVATE As UInteger = &H10

    ' The frame changed: send WM_NCCALCSIZE 
    Public Const SWP_FRAMECHANGED As UInteger = &H20
    Public Const SWP_SHOWWINDOW As UInteger = &H40
    Public Const SWP_HIDEWINDOW As UInteger = &H80
    Public Const SWP_NOCOPYBITS As UInteger = &H100
    ' Don’t do owner Z ordering 
    Public Const SWP_NOOWNERZORDER As UInteger = &H200
    ' Don’t send WM_WINDOWPOSCHANGING
    Public Const SWP_NOSENDCHANGING As UInteger = &H400

    Public Const WM_MOVE As Integer = &H3

    Public Const TOPMOST_FLAGS As UInteger = SWP_NOACTIVATE Or SWP_NOOWNERZORDER Or SWP_NOSIZE Or SWP_NOMOVE Or SWP_NOREDRAW Or SWP_NOSENDCHANGING

End Class