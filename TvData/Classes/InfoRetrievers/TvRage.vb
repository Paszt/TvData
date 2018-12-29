Option Strict On

Imports CsQuery

Public Class TvRage

    Public Shared Sub GetEpisodeIds(TvRageId As Integer, episodes As IList(Of Episode))
        Dim url = String.Format("http://www.tvrage.com/shows/id-{0}/episode_list/all", TvRageId)
        Dim dom = CQ.CreateFromUrl(url)

        For Each link In dom("tr[id=""brow""] td:nth-of-type(2) a")
            Dim linkUrl = link.Cq.Attr("href")
            '  /shows/id-43926/episodes/1065616834
            '  /Game_of_Thrones/episodes/1065008299
            Dim tvRageEpisodeId = linkUrl.Substring(linkUrl.LastIndexOf("/") + 1)

            Dim match = System.Text.RegularExpressions.Regex.Match(link.Cq.Text(), "(\d+)x(\d+)")
            If match.Success Then
                Dim seasonNumber As Short = CShort(match.Groups(1).Value)
                Dim episodeNumber As Short = CShort(match.Groups(2).Value)
                Dim ep = (From e In episodes Where e.SeasonNumber = seasonNumber AndAlso e.EpisodeNumber = episodeNumber).FirstOrDefault
                If ep IsNot Nothing Then
                    ep.TvRageId = tvRageEpisodeId
                End If
            End If
        Next
    End Sub

End Class
