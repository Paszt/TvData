Partial Public Class TmdbApi

    Public Class TvEpisode

        'Public Sub New()
        '    external_ids = New ExternalIds
        'End Sub

        Public Property id As Integer
        Public Property air_date As String
        Public Property season_number As Integer
        Public Property episode_number As Integer
        Public Property name As String
        Public Property overview As String
        Public Property still_path As String
        Public Property vote_average As Double
        Public Property vote_count As Integer
        Public Property production_code As String
        Public Property external_ids As ExternalIds

        Public Class ExternalIds
            Public Property imdb_id As String
            Public Property freebase_mid As String
            Public Property freebase_id As String
            Public Property tvdb_id As Integer?
            Public Property tvrage_id As Integer?
        End Class

    End Class

End Class
