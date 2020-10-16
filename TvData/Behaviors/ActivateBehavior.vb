Imports Microsoft.Xaml.Behaviors

Namespace Behaviors

    Public Class ActivateBehavior
        Inherits Behavior(Of Window)

        Private isActivated As Boolean

        Public Shared ReadOnly ActivatedProperty As DependencyProperty = DependencyProperty.Register("Activated",
                                                                                                     GetType(Boolean),
                                                                                                     GetType(ActivateBehavior),
                                                                                                     New PropertyMetadata(AddressOf OnActivatedChanged))

        Public Property Activated() As Boolean
            Get
                Return DirectCast(GetValue(ActivatedProperty), Boolean)
            End Get
            Set(value As Boolean)
                SetValue(ActivatedProperty, value)
            End Set
        End Property

        Private Shared Sub OnActivatedChanged(dependencyObject As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim behavior As ActivateBehavior = DirectCast(dependencyObject, ActivateBehavior)
            If Not behavior.Activated OrElse behavior.isActivated Then
                Return
            End If
            ' The Activated property is set to true but the Activated event (tracked by the
            ' isActivated field) hasn't been fired. Go ahead and activate the window.
            If behavior.AssociatedObject.WindowState = WindowState.Minimized Then
                behavior.AssociatedObject.WindowState = WindowState.Normal
            End If
            behavior.AssociatedObject.Activate()
        End Sub

        Protected Overrides Sub OnAttached()
            AddHandler AssociatedObject.Activated, AddressOf OnActivated
            AddHandler AssociatedObject.Deactivated, AddressOf OnDeactivated
        End Sub

        Protected Overrides Sub OnDetaching()
            RemoveHandler AssociatedObject.Activated, AddressOf OnActivated
            RemoveHandler AssociatedObject.Deactivated, AddressOf OnDeactivated
        End Sub

        Private Sub OnActivated(sender As Object, eventArgs As EventArgs)
            Me.isActivated = True
            Activated = True
        End Sub

        Private Sub OnDeactivated(sender As Object, eventArgs As EventArgs)
            Me.isActivated = False
            Activated = False
        End Sub

    End Class

End Namespace
