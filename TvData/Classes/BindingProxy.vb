Public Class BindingProxy
    Inherits Freezable

#Region " Freezable Overrides "

    Protected Overrides Function CreateInstanceCore() As Freezable
        Return New BindingProxy()
    End Function

#End Region

    Public Property Data() As Object
        Get
            Return GetValue(DataProperty)
        End Get
        Set(value As Object)
            SetValue(DataProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DataProperty As DependencyProperty = DependencyProperty.Register("Data", GetType(Object), GetType(BindingProxy))

End Class
