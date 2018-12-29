Imports System.Runtime.Serialization

<DataContract>
Public Class SundanceTvSchedule

    <DataMember(Name:="EndDate")>
    Public Property EndDate As String

    <DataMember(Name:="StartDate")>
    Public Property StartDate As String

    <DataMember(Name:="ScheduleItem")>
    Public Property ScheduleItem As List(Of SundanceTvScheduleItem)

End Class

<DataContract>
Public Class SundanceTvScheduleItem

    <DataMember(Name:="EndDate")>
    Public Property EndDate As String

    <DataMember(Name:="StartDate")>
    Public Property StartDate As String

    <DataMember(Name:="EndTime")>
    Public Property EndTime As String

    <DataMember(Name:="StartTime")>
    Public Property StartTime As String

    <DataMember(Name:="Video")>
    Public Property Video As SundanceTvVideo

End Class

<DataContract>
Public Class SundanceTvVideo

    ''' <summary>
    ''' The short description. See FullDescription for more information.
    ''' </summary>
    <DataMember(Name:="Description")>
    Public Property Description As String

    <DataMember(Name:="FirstAirDate")>
    Public Property FirstAirDate As String

    <DataMember(Name:="EpisodeTitle")>
    Public Property EpisodeTitle As String

    <DataMember(Name:="SeriesPremiere")>
    Public Property SeriesPremiere As Boolean

    <DataMember(Name:="New")>
    Public Property [New] As String

    <DataMember(Name:="SynNum")>
    Public Property SynNum As String

    <DataMember(Name:="Premiere")>
    Public Property Premiere As Boolean

    ''' <summary>
    ''' For TV Shows, the Title of the show. See EpisodeTitle is the item is an episode.
    ''' </summary>
    <DataMember(Name:="Title")>
    Public Property Title As String

    <DataMember(Name:="Year")>
    Public Property Year As String

    <DataMember(Name:="TvSubRating")>
    Public Property TvSubRating As String

    <DataMember(Name:="SeasonPremiere")>
    Public Property SeasonPremiere As Boolean

    <DataMember(Name:="EpisodeNumbers")>
    Public Property EpisodeNumbers As String

    <DataMember(Name:="FullDescription")>
    Public Property FullDescription As String

    <DataMember(Name:="TvRating")>
    Public Property TvRating As String

    <DataMember(Name:="EpisodeSeason")>
    Public Property EpisodeSeason As Integer

    <DataMember(Name:="EpisodeNumber")>
    Public Property EpisodeNumber As Integer

    <DataMember(Name:="Thumbnail")>
    Public Property Thumbnail As String

    <DataMember(Name:="Link")>
    Public Property Link As String

End Class