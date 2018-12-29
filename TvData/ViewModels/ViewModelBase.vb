Imports System.ComponentModel

Public Class ViewModelBase
    Implements INotifyPropertyChanged

#Region " INotifyPropertyChanged Members "

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Function SetProperty(Of T)(ByRef storage As T, value As T,
                                     <Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing) As Boolean
        If Equals(storage, value) Then
            Return False
        End If
        storage = value
        OnPropertyChanged(propertyName)
        Return True
    End Function

    Protected Sub OnPropertyChanged(<Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        Dim propertyChanged As PropertyChangedEventHandler = PropertyChangedEvent
        If propertyChanged IsNot Nothing Then
            propertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End If
    End Sub

#End Region

End Class
