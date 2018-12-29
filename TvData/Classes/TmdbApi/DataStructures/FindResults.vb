Partial Public Class TmdbApi

    Public Class FindResults

        Public Class MovieResult
            Public Property adult() As Boolean?
            Public Property backdrop_path() As String
            Public Property id() As Integer
            Public Property original_title() As String
            Public Property release_date() As String
            Public Property poster_path() As String
            Public Property popularity() As Double
            Public Property title() As String
            Public Property vote_average() As Integer
            Public Property vote_count() As Integer
        End Class

        Public Class PersonResult
            Public Property adult() As Boolean
            Public Property id() As Integer
            Public Property name() As String
            Public Property popularity() As Double
            Public Property profile_path() As String
        End Class

        Public Class TvResult
            Public Property backdrop_path() As String
            Public Property id() As Integer
            Public Property original_name() As String
            Public Property first_air_date() As String
            Public Property poster_path() As String
            Public Property popularity() As Double
            Public Property name() As String
            Public Property vote_average() As Double
            Public Property vote_count() As Integer
        End Class

        Public Property movie_results() As List(Of MovieResult)
        Public Property person_results() As List(Of PersonResult)
        Public Property tv_results() As List(Of TvResult)

    End Class

End Class
