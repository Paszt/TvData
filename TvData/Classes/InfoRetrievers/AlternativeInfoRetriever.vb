Option Strict On
Imports System.IO
Imports System.Xml.Serialization
Imports TvData

Public Class AlternativeInfoRetriever
    Implements IInfoRetriever

    Public Property ExtraEpisodes As IList(Of Episode) Implements IInfoRetriever.ExtraEpisodes
    Public Event StatusChanged(TypeOfStatus As StatusType, Message As String) Implements IInfoRetriever.StatusChanged

    Public Sub RetrieveInfo(FilePath As String,
                            episodes As TrulyObservableCollection(Of Episode),
                            Optional Language As MainViewModel.Iso639_1 = MainViewModel.Iso639_1.en) Implements IInfoRetriever.RetrieveInfo
        Dim altData As TVSeries = Nothing
        ExtraEpisodes = New List(Of Episode)

        If FilePath IsNot Nothing Then
            'read from file and deserialize
            Using objStreamReader As New StreamReader(FilePath)
                Dim x As New XmlSerializer(GetType(TVSeries))
                altData = CType(x.Deserialize(objStreamReader), TVSeries)
                objStreamReader.Close()
            End Using
        Else
            'read from clipboard and deserialize
            Dim x As New XmlSerializer(GetType(TVSeries))
            Using reader As TextReader = New StringReader(Clipboard.GetText(TextDataFormat.Text))
                Try
                    altData = CType(x.Deserialize(reader), TVSeries)
                Catch ex As Exception
                    Dim msg As String = ex.Message
                    If Not String.IsNullOrEmpty(ex.InnerException.Message) Then
                        msg &= Environment.NewLine & ex.InnerException.Message
                    End If
                    MessageWindow.ShowDialog(msg, "Error while pasting from Clipboard")
                    Exit Sub
                End Try
            End Using
        End If

        Dim existingSeasons = (From eps In episodes Where eps.SeasonNumber <> 0 Select eps.SeasonNumber Distinct).ToList()
        For Each ep In altData.Episodes
            If existingSeasons.Contains(ep.SeasonNumber) Then
                Dim existingEpisode = (From e In episodes
                                       Where e.SeasonNumber = ep.SeasonNumber AndAlso
                                         e.EpisodeNumber = ep.EpisodeNumber).FirstOrDefault()
                If existingEpisode IsNot Nothing Then
                    existingEpisode.Alternative_AiredDate = ep.FirstAired
                    existingEpisode.Alternative_EpisodeName = ep.EpisodeName
                    existingEpisode.Alternative_Overview = ep.Overview
                Else
                    Dim extraEpisode As New Episode() With {
                        .Alternative_EpisodeName = ep.EpisodeName,
                        .SeasonNumber = ep.SeasonNumber,
                        .EpisodeNumber = ep.EpisodeNumber,
                        .Alternative_Overview = ep.Overview,
                        .Alternative_AiredDate = ep.FirstAired
                    }
                    ExtraEpisodes.Add(extraEpisode)
                End If
            End If
        Next
    End Sub

End Class
