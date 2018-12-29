Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
Public Class MONITORINFO

    Public cbSize As Integer = Marshal.SizeOf(GetType(MONITORINFO))
    Public rcMonitor As New RECTAPI()
    Public rcWork As New RECTAPI()
    Public dwFlags As Integer = 0

End Class
