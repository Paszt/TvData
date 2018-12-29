Partial Public Class TmdbApi

    Public Class TvShow

        Public Class CreatedBy
            Public Property id() As Integer
                Get
                    Return m_id
                End Get
                Set(value As Integer)
                    m_id = value
                End Set
            End Property
            Private m_id As Integer

            Public Property name() As String
                Get
                    Return m_name
                End Get
                Set(value As String)
                    m_name = value
                End Set
            End Property
            Private m_name As String

            Public Property profile_path() As Object
                Get
                    Return m_profile_path
                End Get
                Set(value As Object)
                    m_profile_path = value
                End Set
            End Property
            Private m_profile_path As Object
        End Class

        Public Class Genre
            Public Property id() As Integer
                Get
                    Return m_id
                End Get
                Set(value As Integer)
                    m_id = value
                End Set
            End Property
            Private m_id As Integer
            Public Property name() As String
                Get
                    Return m_name
                End Get
                Set(value As String)
                    m_name = value
                End Set
            End Property
            Private m_name As String
        End Class

        Public Class Network
            Public Property id() As Integer
                Get
                    Return m_id
                End Get
                Set(value As Integer)
                    m_id = value
                End Set
            End Property
            Private m_id As Integer

            Public Property name() As String
                Get
                    Return m_name
                End Get
                Set(value As String)
                    m_name = value
                End Set
            End Property
            Private m_name As String

        End Class

        Public Class Season
            Public Property air_date() As String
                Get
                    Return m_air_date
                End Get
                Set(value As String)
                    m_air_date = value
                End Set
            End Property
            Private m_air_date As String

            Public Property id() As Integer
                Get
                    Return m_id
                End Get
                Set(value As Integer)
                    m_id = value
                End Set
            End Property
            Private m_id As Integer

            Public Property poster_path() As String
                Get
                    Return m_poster_path
                End Get
                Set(value As String)
                    m_poster_path = value
                End Set
            End Property
            Private m_poster_path As String

            Public Property season_number() As Integer?
                Get
                    Return m_season_number
                End Get
                Set(value As Integer?)
                    m_season_number = value
                End Set
            End Property
            Private m_season_number As Integer?

        End Class

#Region " Additional Properties "

        Public ReadOnly Property poster_path_full As String
            Get
                If String.IsNullOrEmpty(poster_path) Then
                    Return "http://d3a8mw37cqal2z.cloudfront.net/assets/f996aa2014d2ffddfda8463c479898a3/images/no-poster-w185.jpg"
                End If
                Return "http://image.tmdb.org/t/p/w90" & poster_path
            End Get
        End Property

        Public ReadOnly Property backdrop_path_full As String
            Get
                If String.IsNullOrWhiteSpace(backdrop_path) Then
                    Return "https://d3a8mw37cqal2z.cloudfront.net/assets/780d1a9b1e878090531fc7000c7a926e/images/no-backdrop-w500_and_h281_bestv2-v2.png"
                End If
                Return "https://image.tmdb.org/t/p/w300" & backdrop_path
            End Get
        End Property

        Property Visited As Boolean = False

#End Region

        Public Property backdrop_path() As String
            Get
                Return m_backdrop_path
            End Get
            Set(value As String)
                m_backdrop_path = value
            End Set
        End Property
        Private m_backdrop_path As String

        Public Property created_by() As List(Of CreatedBy)
            Get
                Return m_created_by
            End Get
            Set(value As List(Of CreatedBy))
                m_created_by = value
            End Set
        End Property
        Private m_created_by As List(Of CreatedBy)

        Public Property episode_run_time() As List(Of Integer)
            Get
                Return m_episode_run_time
            End Get
            Set(value As List(Of Integer))
                m_episode_run_time = value
            End Set
        End Property
        Private m_episode_run_time As List(Of Integer)

        Public Property first_air_date() As String
            Get
                Return m_first_air_date
            End Get
            Set(value As String)
                m_first_air_date = value
            End Set
        End Property
        Private m_first_air_date As String

        Public Property genres() As List(Of Genre)
            Get
                Return m_genres
            End Get
            Set(value As List(Of Genre))
                m_genres = value
            End Set
        End Property
        Private m_genres As List(Of Genre)

        Public Property homepage() As String
            Get
                Return m_homepage
            End Get
            Set(value As String)
                m_homepage = value
            End Set
        End Property
        Private m_homepage As String

        Public Property id() As Integer
            Get
                Return m_id
            End Get
            Set(value As Integer)
                m_id = value
            End Set
        End Property
        Private m_id As Integer

        Public Property in_production() As Boolean
            Get
                Return m_in_production
            End Get
            Set(value As Boolean)
                m_in_production = value
            End Set
        End Property
        Private m_in_production As Boolean

        Public Property languages() As List(Of String)
            Get
                Return m_languages
            End Get
            Set(value As List(Of String))
                m_languages = value
            End Set
        End Property
        Private m_languages As List(Of String)

        Public Property last_air_date() As String
            Get
                Return m_last_air_date
            End Get
            Set(value As String)
                m_last_air_date = value
            End Set
        End Property
        Private m_last_air_date As String

        Public Property name() As String
            Get
                Return m_name
            End Get
            Set(value As String)
                m_name = value
            End Set
        End Property
        Private m_name As String

        Public Property networks() As List(Of Network)
            Get
                Return m_networks
            End Get
            Set(value As List(Of Network))
                m_networks = value
            End Set
        End Property
        Private m_networks As List(Of Network)

        Public Property number_of_episodes() As Integer?
            Get
                Return m_number_of_episodes
            End Get
            Set(value As Integer?)
                m_number_of_episodes = value
            End Set
        End Property
        Private m_number_of_episodes As Integer?

        Public Property number_of_seasons() As Integer?
            Get
                Return m_number_of_seasons
            End Get
            Set(value As Integer?)
                m_number_of_seasons = value
            End Set
        End Property
        Private m_number_of_seasons As Integer?

        Public Property original_name() As String
            Get
                Return m_original_name
            End Get
            Set(value As String)
                m_original_name = value
            End Set
        End Property
        Private m_original_name As String

        Public Property origin_country() As List(Of String)
            Get
                Return m_origin_country
            End Get
            Set(value As List(Of String))
                m_origin_country = value
            End Set
        End Property
        Private m_origin_country As List(Of String)

        Public Property overview() As String
            Get
                Return m_overview
            End Get
            Set(value As String)
                m_overview = value
            End Set
        End Property
        Private m_overview As String

        Public Property popularity() As Double
            Get
                Return m_popularity
            End Get
            Set(value As Double)
                m_popularity = value
            End Set
        End Property
        Private m_popularity As Double

        Public Property poster_path() As String
            Get
                Return m_poster_path
            End Get
            Set(value As String)
                m_poster_path = value
            End Set
        End Property
        Private m_poster_path As String

        Public Property seasons() As List(Of Season)
            Get
                Return m_seasons
            End Get
            Set(value As List(Of Season))
                m_seasons = value
            End Set
        End Property
        Private m_seasons As List(Of Season)

        Public Property status() As String
            Get
                Return m_status
            End Get
            Set(value As String)
                m_status = value
            End Set
        End Property
        Private m_status As String

        Public Property vote_average() As Double
            Get
                Return m_vote_average
            End Get
            Set(value As Double)
                m_vote_average = value
            End Set
        End Property
        Private m_vote_average As Double

        Public Property vote_count() As Integer
            Get
                Return m_vote_count
            End Get
            Set(value As Integer)
                m_vote_count = value
            End Set
        End Property
        Private m_vote_count As Integer

    End Class

End Class
