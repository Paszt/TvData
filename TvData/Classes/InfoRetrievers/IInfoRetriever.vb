'Imports System.Collections.ObjectModel
Imports System.ComponentModel

Public Interface IInfoRetriever

    'Sub RetrieveInfo(SeriesId As String, episodes As TrulyObservableCollection(Of Episode))
    Sub RetrieveInfo(SeriesId As String, episodes As TrulyObservableCollection(Of Episode), Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en)
    Event StatusChanged(TypeOfStatus As StatusType, Message As String)
    Property ExtraEpisodes As IList(Of Episode)

End Interface

'Public Class InfoRetrieverException
'    Inherits Exception

'    Public Sub New(ByVal message As String)
'        MyBase.New(message)
'    End Sub

'End Class
