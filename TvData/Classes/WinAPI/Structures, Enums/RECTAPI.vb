Imports System.Runtime.InteropServices

<StructLayout(LayoutKind.Sequential, Pack:=0)> _
Public Structure RECTAPI
    Public Left As Integer
    Public Top As Integer
    Public Right As Integer
    Public Bottom As Integer
    Public Shared ReadOnly Empty As RECTAPI

    Public ReadOnly Property Width() As Integer
        Get
            Return Math.Abs(Me.Right - Me.Left)
        End Get
    End Property
    ' Abs needed for BIDI OS

    Public ReadOnly Property Height() As Integer
        Get
            Return Me.Bottom - Me.Top
        End Get
    End Property

    Public Sub New(left As Integer, top As Integer, right As Integer, bottom As Integer)
        Me.Left = left
        Me.Top = top
        Me.Right = right
        Me.Bottom = bottom
    End Sub

    Public Sub New(rcSrc As RECTAPI)
        Me.Left = rcSrc.Left
        Me.Top = rcSrc.Top
        Me.Right = rcSrc.Right
        Me.Bottom = rcSrc.Bottom
    End Sub

    Public ReadOnly Property IsEmpty() As Boolean
        Get
            ' BUGBUG : On Bidi OS (hebrew arabic) left > right
            Return Me.Left >= Me.Right OrElse Me.Top >= Me.Bottom
        End Get
    End Property

    Public Overrides Function ToString() As String
        If Me = Empty Then
            Return "RECT {Empty}"
        End If
        Return "RECT { left : " & Left & " / top : " & Top & " / right : " & Right & " / bottom : " & Bottom & " }"
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not (TypeOf obj Is RECTAPI) Then
            Return False
        End If
        Return (Me = CType(obj, RECTAPI))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Return Me.Left.GetHashCode() + Me.Top.GetHashCode() + Me.Right.GetHashCode() + Me.Bottom.GetHashCode()
    End Function

    Public Shared Operator =(rect1 As RECTAPI, rect2 As RECTAPI) As Boolean
        Return (rect1.Left = rect2.Left AndAlso rect1.Top = rect2.Top AndAlso rect1.Right = rect2.Right AndAlso rect1.Bottom = rect2.Bottom)
    End Operator

    Public Shared Operator <>(rect1 As RECTAPI, rect2 As RECTAPI) As Boolean
        Return Not (rect1 = rect2)
    End Operator

End Structure