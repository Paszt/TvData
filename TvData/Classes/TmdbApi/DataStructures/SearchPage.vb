Partial Public Class TmdbApi

    Public Class SearchPage
        Public Class Result

            Public Property backdrop_path As String
            Public Property id As Integer
            Public Property original_name As String
            Public Property first_air_date As String
            Public Property poster_path As String
            Public Property popularity As Double
            Public Property name As String
            Public Property vote_average As Double
            Public Property vote_count As Integer

        End Class

        Public Property page As Integer
        Public Property results As List(Of SearchPage.Result)
        Public Property total_pages As Integer
        Public Property total_results As Integer

    End Class

End Class
