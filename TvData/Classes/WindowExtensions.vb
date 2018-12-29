Option Strict On

Public Module WindowExtensions

#Region "Properties"
	Private Property Encoding As System.Text.Encoding = New System.Text.UTF8Encoding()
	Private Property Serializer As New System.Xml.Serialization.XmlSerializer(GetType(NativeMethods.WINDOWPLACEMENT))
#End Region

#Region "Extension Methods"
	<System.Runtime.CompilerServices.Extension> _
	Public Sub SetPlacement(window As Window, placementXml As String)
		If Not String.IsNullOrEmpty(placementXml) Then
			SetPlacement(New System.Windows.Interop.WindowInteropHelper(window).Handle, placementXml)
		End If
	End Sub

	'<System.Runtime.CompilerServices.Extension> _
	'Public Sub SetPlacement(form As System.Windows.Forms.Form, placementXml As String)
	'	If Not String.IsNullOrEmpty(placementXml) Then
	'		SetPlacement(form.Handle, placementXml)
	'	End If
	'End Sub

	<System.Runtime.CompilerServices.Extension> _
	Public Function GetPlacement(window As Window) As String
		Return GetPlacement(New System.Windows.Interop.WindowInteropHelper(window).Handle)
	End Function

	'<System.Runtime.CompilerServices.Extension> _
	'Public Function GetPlacement(form As System.Windows.Forms.Form) As String
	'	Return GetPlacement(form.Handle)
	'End Function

#End Region

#Region "Supporting Methods"

	Private Sub SetPlacement(windowHandle As IntPtr, placementXml As String)
		If String.IsNullOrEmpty(placementXml) Then
			Return
		End If
		Dim placement As NativeMethods.WINDOWPLACEMENT
		Dim xmlBytes As Byte() = Encoding.GetBytes(placementXml)
		Try
			Using memoryStream As New System.IO.MemoryStream(xmlBytes)
				placement = DirectCast(Serializer.Deserialize(memoryStream), NativeMethods.WINDOWPLACEMENT)
			End Using
			placement.length = System.Runtime.InteropServices.Marshal.SizeOf(GetType(NativeMethods.WINDOWPLACEMENT))
			placement.flags = 0
			placement.showCmd = (If(placement.showCmd = NativeMethods.ShowWindowCommands.ShowMinimized, NativeMethods.ShowWindowCommands.Normal, placement.showCmd))
			NativeMethods.SetWindowPlacement(windowHandle, placement)
			' Parsing placement XML failed. Fail silently.
		Catch generatedExceptionName As InvalidOperationException
		End Try
	End Sub

	Private Function GetPlacement(windowHandle As IntPtr) As String
		Dim placement As New NativeMethods.WINDOWPLACEMENT()
		NativeMethods.GetWindowPlacement(windowHandle, placement)
		Using memoryStream As New System.IO.MemoryStream
			Using xmlTextWriter As New System.Xml.XmlTextWriter(memoryStream, System.Text.Encoding.UTF8)
				Serializer.Serialize(xmlTextWriter, placement)
				Dim xmlBytes As Byte() = memoryStream.ToArray()
				Return Encoding.GetString(xmlBytes)
			End Using
		End Using
	End Function

#End Region

End Module

Public Class NativeMethods

	<System.Runtime.InteropServices.DllImport("user32.dll")> _
	Public Shared Function SetWindowPlacement(ByVal hWnd As IntPtr, ByRef lpwndpl As WINDOWPLACEMENT) As Boolean
	End Function

	<System.Runtime.InteropServices.DllImport("user32.dll")> _
	Public Shared Function GetWindowPlacement(ByVal hWnd As IntPtr, ByRef lpwndpl As WINDOWPLACEMENT) As Boolean
	End Function

	' RECT structure required by WINDOWPLACEMENT structure
	<Serializable>
	<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>
	Public Structure RECT

		Public Left As Integer
		Public Top As Integer
		Public Right As Integer
		Public Bottom As Integer

		Public Sub New(left As Integer, top As Integer, right As Integer, bottom As Integer)
			Me.Left = left
			Me.Top = top
			Me.Right = right
			Me.Bottom = bottom
		End Sub

	End Structure

	' POINT structure required by WINDOWPLACEMENT structure
	<Serializable>
	<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>
	Public Structure POINT

		Public X As Integer
		Public Y As Integer

		Public Sub New(x As Integer, y As Integer)
			Me.X = x
			Me.Y = y
		End Sub

	End Structure

	' WINDOWPLACEMENT stores the position, size, and state of a window
	<Serializable>
	<System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)>
	Public Structure WINDOWPLACEMENT
		Public length As Integer
		Public flags As Integer
		Public showCmd As ShowWindowCommands
		Public minPosition As POINT
		Public maxPosition As POINT
		Public normalPosition As RECT
	End Structure

	Public Enum ShowWindowCommands As Integer
		Hide = 0
		Normal = 1
		ShowMinimized = 2
		Maximize = 3
		ShowMaximized = 3
		ShowNoActivate = 4
		Show = 5
		Minimize = 6
		ShowMinNoActive = 7
		ShowNA = 8
		Restore = 9
		ShowDefault = 10
		ForceMinimize = 11
	End Enum

End Class