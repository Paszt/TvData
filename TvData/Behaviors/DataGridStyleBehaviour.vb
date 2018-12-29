Option Strict On

Namespace Behaviors

    Public NotInheritable Class DataGridStyleBehaviour
        Private Sub New()
        End Sub

#Region " Attached Property "

        Public Shared Function GetSelectAllButtonTemplate(obj As DataGrid) As ControlTemplate
            Return DirectCast(obj.GetValue(SelectAllButtonTemplateProperty), ControlTemplate)
        End Function

        Public Shared Sub SetSelectAllButtonTemplate(obj As DataGrid, value As ControlTemplate)
            obj.SetValue(SelectAllButtonTemplateProperty, value)
        End Sub

        Public Shared ReadOnly SelectAllButtonTemplateProperty As DependencyProperty =
            DependencyProperty.RegisterAttached("SelectAllButtonTemplate",
                                                GetType(ControlTemplate),
                                                GetType(DataGridStyleBehaviour),
                                                New UIPropertyMetadata(Nothing, AddressOf OnSelectAllButtonTemplateChanged))

#End Region

#Region " Property Behaviour "

        ' property change event handler for SelectAllButtonTemplate
        Private Shared Sub OnSelectAllButtonTemplateChanged(depObj As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim grid As DataGrid = TryCast(depObj, DataGrid)
            If grid Is Nothing Then
                Return
            End If

            ' handle the grid's Loaded event
            AddHandler grid.Loaded, AddressOf Grid_Loaded
        End Sub

        ' Handles the DataGrid's Loaded event
        Private Shared Sub Grid_Loaded(sender As Object, e As RoutedEventArgs)
            Dim grid As DataGrid = TryCast(sender, DataGrid)
            Dim dep As DependencyObject = grid
            Try
                ' Navigate down the visual tree to the button
                While Not (TypeOf dep Is Button)
                    dep = VisualTreeHelper.GetChild(dep, 0)

                End While
                Dim button As Button = TryCast(dep, Button)

                ' apply our new template
                Dim template As ControlTemplate = GetSelectAllButtonTemplate(grid)
                button.Template = template
            Catch ex As Exception

            End Try
        End Sub

#End Region

    End Class

End Namespace