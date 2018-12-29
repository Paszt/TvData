Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)> _
Friend Structure WINDOWPOS

    Public hwnd As IntPtr
    Public hwndInsertAfter As IntPtr
    Public x As Integer
    Public y As Integer
    Public cx As Integer
    Public cy As Integer
    Public flags As Integer

End Structure