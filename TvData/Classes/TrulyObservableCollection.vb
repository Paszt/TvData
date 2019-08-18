Option Strict On

Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized

Public Class TrulyObservableCollection(Of T As INotifyPropertyChanged)
    Inherits ObservableCollection(Of T)

    Public Event ItemPropertyChanged As System.ComponentModel.PropertyChangedEventHandler

    Public Sub New()
        MyBase.New()
        'CollectionChanged += New NotifyCollectionChangedEventHandler(AddressOf TrulyObservableCollection_CollectionChanged)
    End Sub

    Public Sub New(truObservColl As TrulyObservableCollection(Of T))
        MyBase.New(truObservColl)
    End Sub

    Public Sub New(Collection As IEnumerable(Of T))
        MyBase.New(Collection)
    End Sub

    Private Sub TrulyObservableCollection_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles Me.CollectionChanged
        If e.NewItems IsNot Nothing Then
            For Each item As Object In e.NewItems
                AddHandler TryCast(item, INotifyPropertyChanged).PropertyChanged, AddressOf Item_PropertyChanged
            Next
        End If
        If e.OldItems IsNot Nothing Then
            For Each item As Object In e.OldItems
                RemoveHandler TryCast(item, INotifyPropertyChanged).PropertyChanged, AddressOf Item_PropertyChanged
            Next
        End If
    End Sub

    Private Sub Item_PropertyChanged(sender As Object, e As PropertyChangedEventArgs)
        RaiseEvent ItemPropertyChanged(sender, e)
        'Dim a As New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
        'OnCollectionChanged(a)
    End Sub

End Class