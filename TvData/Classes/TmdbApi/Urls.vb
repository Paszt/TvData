Partial Public Class TmdbApi

    Public Class Urls

        Public Shared Function TvChanges(pageNumber As Integer) As String
            Return String.Format("https://api.themoviedb.org/3/tv/changes?api_key={0}&page={1}", TmdbApi.ApiKey, pageNumber)
        End Function

        Public Shared Function TvShowLatest() As String
            Return String.Format("http://api.themoviedb.org/3/tv/latest?api_key={0}", TmdbApi.ApiKey)
        End Function

        Public Shared Function TvSeason(TmdbId As String, SeasonNumber As Integer, Language As MainViewModel.Iso639_1) As String
            ' https://api.themoviedb.org/3/tv/60804/season/1?api_key=57983e31fb435df4df77afb854740ea9
            Dim url = "https://api.themoviedb.org/3/tv/{0}/season/{1}?language={2}&api_key=" & TmdbApi.ApiKey
            Return String.Format(url, TmdbId, SeasonNumber, Language.ToString())
        End Function

        Public Shared Function TvSearch(query As String) As String
            Dim url As String = "https://api.themoviedb.org/3/search/tv?query={0}&api_key=" & TmdbApi.ApiKey
            Return String.Format(url, System.Uri.EscapeDataString(query))
        End Function

        Public Shared Function TvShow(id As Integer) As String
            Return String.Format("https://api.themoviedb.org/3/tv/{0}?api_key={1}", id, TmdbApi.ApiKey)
        End Function

        Public Shared Function TvOnAir(pageNumber As Integer) As String
            Return String.Format("https://api.themoviedb.org/3/tv/on_the_air?api_key={0}&page={1}", TmdbApi.ApiKey, pageNumber)
        End Function

        Public Shared Function TmdbIdFromTvdbId(tvdbId As String) As String
            ' https://api.themoviedb.org/3/find/121361?external_source=tvdb_id&api_key=57983e31fb435df4df77afb854740ea9
            Dim url As String = "https://api.themoviedb.org/3/find/{0}?external_source=tvdb_id&api_key=" & TmdbApi.ApiKey
            Return String.Format(url, tvdbId)
        End Function

        Public Shared Function TvShowExternalIds(TmdbId As String) As String
            Dim url As String = "http://api.themoviedb.org/3/tv/{0}/external_ids?api_key=" & TmdbApi.ApiKey
            Return String.Format(url, TmdbId)
        End Function

        Friend Shared Function TvEpisode(tMDBSeriesId As String, seasonNo As Integer, episodeNo As Integer) As String
            Dim url = "https://api.themoviedb.org/3/tv/{0}/season/{1}/episode/{2}?append_to_response=external_ids&api_key=" & ApiKey
            Return String.Format(url, tMDBSeriesId, seasonNo, episodeNo)
        End Function
    End Class

End Class
