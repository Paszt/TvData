Interface IFileDragDropTarget

    Sub OnFileDrop(ByVal filepaths As String())

End Interface

Public Class FileDragDropHandler

    Public Shared ReadOnly IsEnabledProperty As DependencyProperty = DependencyProperty.RegisterAttached("IsEnabled",
                                                                                                         GetType(Boolean),
                                                                                                         GetType(FileDragDropHandler),
                                                                                                         New PropertyMetadata(AddressOf OnEnabled))

    Public Shared Function GetIsEnabled(ByVal obj As DependencyObject) As Boolean
        Return CBool(obj.GetValue(IsEnabledProperty))
    End Function

    Public Shared Sub SetIsEnabled(ByVal obj As DependencyObject, ByVal value As Boolean)
        obj.SetValue(IsEnabledProperty, value)
    End Sub

    'Public Property IsEnabled As Boolean
    '    Get
    '        Return DirectCast(GetValue(IsEnabledProperty), Boolean)
    '    End Get
    '    Set(value As Boolean)
    '        SetValue(IsEnabledProperty, value)
    '    End Set
    'End Property

    Public Shared ReadOnly TargetProperty As DependencyProperty = DependencyProperty.RegisterAttached("Target",
                                                                                                      GetType(Object),
                                                                                                      GetType(FileDragDropHandler),
                                                                                                      Nothing)

    Public Shared Function GetTarget(ByVal obj As DependencyObject) As Object
        Return obj.GetValue(TargetProperty)
    End Function

    Public Shared Sub SetTarget(ByVal obj As DependencyObject, ByVal value As Object)
        obj.SetValue(TargetProperty, value)
    End Sub

    'Public Property Target As Object
    '    Get
    '        Return GetValue(TargetProperty)
    '    End Get
    '    Set(value As Object)
    '        SetValue(TargetProperty, value)
    '    End Set
    'End Property

    Private Shared Sub OnEnabled(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
        If e.NewValue Is e.OldValue Then Return
        Dim control As Control = TryCast(d, Control)
        If control IsNot Nothing Then AddHandler control.Drop, AddressOf OnDrop
    End Sub

    Private Shared Sub OnDrop(ByVal _sender As Object, ByVal _dragEventArgs As DragEventArgs)
        Dim d As DependencyObject = TryCast(_sender, DependencyObject)
        If d Is Nothing Then Return
        Dim target As Object = d.GetValue(TargetProperty)
        Dim fileTarget As IFileDragDropTarget = TryCast(target, IFileDragDropTarget)

        If fileTarget IsNot Nothing Then
            If _dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop) Then
                fileTarget.OnFileDrop(CType(_dragEventArgs.Data.GetData(DataFormats.FileDrop), String()))
            End If
        Else
            Throw New Exception("FileDragDropTarget object must be of type IFileDragDropTarget")
        End If
    End Sub

End Class
