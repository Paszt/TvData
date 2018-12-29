Imports System.Security
Imports System.Runtime.InteropServices

<SuppressUnmanagedCodeSecurity> _
Friend NotInheritable Class UnsafeNativeMethods

    <DllImport("user32.dll")> _
    Public Shared Function ReleaseCapture() As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    '<DllImport("user32.dll", EntryPoint:="SetClassLongPtrA", SetLastError:=True, CharSet:=CharSet.Ansi)> _
    'Public Shared Function SetClassLongPtr(hWnd As IntPtr, <MarshalAs(UnmanagedType.I4)> nIndex As ClassLongFlags, newLong As Integer) As Integer
    'End Function

    <DllImport("user32.dll", EntryPoint:="SetClassLong")> _
    Friend Shared Function SetClassLongPtr32(hWnd As IntPtr, nIndex As Integer, dwNewLong As UInteger) As UInteger
    End Function

    <DllImport("user32.dll", EntryPoint:="SetClassLongPtr")> _
    Friend Shared Function SetClassLongPtr64(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", ExactSpelling:=True)> _
    Public Shared Function MonitorFromWindow(ByVal handle As IntPtr, ByVal flags As Integer) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Public Shared Function GetMonitorInfo(hMonitor As IntPtr, ByRef lpmi As MONITORINFO) As Boolean
    End Function

End Class
