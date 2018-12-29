Partial Public Class TmdbApi

    Public Class TvSeason

        Public Sub New()
            episodes = New List(Of TmdbApi.TvEpisode)
        End Sub

        Public Property air_date As String
        Public Property episodes As List(Of TmdbApi.TvEpisode)
        Public Property name As String
        Public Property overview As String
        Public Property id As Integer
        Public Property poster_path As String
        Public Property season_number As Integer

    End Class

End Class
