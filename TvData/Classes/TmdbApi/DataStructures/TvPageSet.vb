Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Partial Public Class TmdbApi

    Public Class TvPageSet
        Implements INotifyPropertyChanged

        Public Class Result
            Implements INotifyPropertyChanged

            Private idField As Integer
            Public Property id As Integer
                Get
                    Return idField
                End Get
                Set(value As Integer)
                    SetProperty(idField, value)
                End Set
            End Property

            Private adultField As Boolean?
            Public Property adult As Boolean?
                Get
                    Return adultField
                End Get
                Set(value As Boolean?)
                    SetProperty(adultField, value)
                End Set
            End Property

            Private visitedField As Boolean
            Public Property visited As Boolean
                Get
                    Return visitedField
                End Get
                Set(value As Boolean)
                    SetProperty(visitedField, value)
                End Set
            End Property

            Private nameField As String
            Public Property name As String
                Get
                    Return nameField
                End Get
                Set(value As String)
                    SetProperty(nameField, value)
                End Set
            End Property

            Private poster_pathField As String
            Property poster_path As String
                Get
                    Return poster_pathField
                End Get
                Set(value As String)
                    SetProperty(poster_pathField, value)
                End Set
            End Property

#Region " Additional Properties "

            Public ReadOnly Property poster_path_full As String
                Get
                    If String.IsNullOrEmpty(poster_path) Then
                        Return "http://d3a8mw37cqal2z.cloudfront.net/assets/f996aa2014d2ffddfda8463c479898a3/images/no-poster-w185.jpg"
                    End If
                    Return "http://image.tmdb.org/t/p/w90" & poster_path
                End Get
            End Property

#End Region

#Region " INotifyPropertyChanged Members "

            Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

            Protected Function SetProperty(Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As String = Nothing) As Boolean
                If Equals(storage, value) Then
                    Return False
                End If
                storage = value
                Me.OnPropertyChanged(propertyName)
                Return True
            End Function

            Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
                Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
                If (Not (propertyChanged) Is Nothing) Then
                    propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
                End If
            End Sub

#End Region

        End Class

#Region " Properties "

        Private resultsField As List(Of TvPageSet.Result)
        Public Property results As List(Of TvPageSet.Result)
            Get
                Return resultsField
            End Get
            Set(value As List(Of TvPageSet.Result))
                SetProperty(resultsField, value)
            End Set
        End Property

        Private pageField As Integer
        Public Property page As Integer
            Get
                Return pageField
            End Get
            Set(value As Integer)
                SetProperty(pageField, value)
            End Set
        End Property

        Private total_pagesField As Integer
        Public Property total_pages As Integer
            Get
                Return total_pagesField
            End Get
            Set(value As Integer)
                SetProperty(total_pagesField, value)
            End Set
        End Property

        Private total_resultsField As Integer
        Public Property total_results As Integer
            Get
                Return total_resultsField
            End Get
            Set(value As Integer)
                SetProperty(total_resultsField, value)
            End Set
        End Property

#End Region

#Region " INotifyPropertyChanged Members "

        Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Protected Function SetProperty(Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As String = Nothing) As Boolean
            If Equals(storage, value) Then
                Return False
            End If
            storage = value
            Me.OnPropertyChanged(propertyName)
            Return True
        End Function

        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            Dim propertyChanged As System.ComponentModel.PropertyChangedEventHandler = Me.PropertyChangedEvent
            If (Not (propertyChanged) Is Nothing) Then
                propertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
            End If
        End Sub

#End Region

    End Class

End Class
