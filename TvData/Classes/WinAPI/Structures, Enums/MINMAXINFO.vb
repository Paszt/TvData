Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential)> _
Public Structure MINMAXINFO

    Public ptReserved As POINTAPI
    Public ptMaxSize As POINTAPI
    Public ptMaxPosition As POINTAPI
    Public ptMinTrackSize As POINTAPI
    Public ptMaxTrackSize As POINTAPI

End Structure