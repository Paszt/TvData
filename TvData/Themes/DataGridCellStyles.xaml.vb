Partial Public Class DataGridCellStyles
    Inherits ResourceDictionary

    Private Sub DataGridCell_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Dim cell As DataGridCell = TryCast(sender, DataGridCell)
        If Not cell.IsEditing Then
            ' enables editing on single click
            If Not cell.IsFocused Then
                cell.Focus()
            End If
            If Not cell.IsSelected Then
                cell.IsSelected = True
            End If
        End If
    End Sub

End Class
