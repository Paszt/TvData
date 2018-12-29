Imports System.Runtime.Serialization

<DataContract>
Public Class VicelandScheduleEntry

    <DataMember(Name:="DURATION")>
    Public Property Duration As String

    <DataMember(Name:="SOURCE_ID")>
    Public Property SourceId As String

    <DataMember(Name:="SHOWTYPE")>
    Public Property ShowType As String

    <DataMember(Name:="PROGRAM_ID")>
    Public Property ProgramId As String

    <DataMember(Name:="CAPTION")>
    Public Property Caption As String

    <DataMember(Name:="SERIES_EPISODE_TITLE")>
    Public Property SeriesEpisodeTitle As String

    <DataMember(Name:="AUDIO_FORMAT")>
    Public Property AudioFormat As String

    <DataMember(Name:="PRIMARY_LANGUAGE")>
    Public Property PrimaryLanguage As String

    <DataMember(Name:="DISPLAYED_PROGRAM_TITLE")>
    Public Property DisplayedProgramTitle As String

    <DataMember(Name:="ORIG_BROADCAST_DATE")>
    Public Property OrigBroadcastDate As String

    <DataMember(Name:="CALENDAR_DATE_TIME")>
    Public Property CalendarDateTime As String

    <DataMember(Name:="SERIES_ID")>
    Public Property SeriesId As String

    <DataMember(Name:="COPY_4000")>
    Public Property Copy4000 As String

    <DataMember(Name:="START_TIME")>
    Public Property StartTime As String

    <DataMember(Name:="AIRING_TYPE")>
    Public Property AiringType As String

    <DataMember(Name:="PART_NUMBER")>
    Public Property PartNumber As String

    <DataMember(Name:="GENRE")>
    Public Property Genre As String

    <DataMember(Name:="SECOND_AUDIO_PROGRAMMING")>
    Public Property SecondAudioProgramming As String

    <DataMember(Name:="OFFSET")>
    Public Property Offset As String

    <DataMember(Name:="DELETE_FLAG")>
    Public Property DeleteFlag As String

    <DataMember(Name:="PART_TOTAL")>
    Public Property PartTotal As String

    <DataMember(Name:="DESCRIPTIVE_VIDEO_SERVICE")>
    Public Property DescriptiveVideoService As String

    <DataMember(Name:="LETTER_B0X")>
    Public Property LetterBox As String

    <DataMember(Name:="PROGRAM_TYPE")>
    Public Property ProgramType As String

    <DataMember(Name:="TIME_ZONE")>
    Public Property TimeZone As String

    <DataMember(Name:="PROGRAM_COLOR_TYPE")>
    Public Property ProgramColorType As String

    <DataMember(Name:="HDTV_LEVEL")>
    Public Property HdtvLevel As String

    <DataMember(Name:="YEAR_OF_RELEASE")>
    Public Property YearOfRelease As String

    <DataMember(Name:="RATING")>
    Public Property Rating As String

    <DataMember(Name:="RATING_REASON")>
    Public Property RatingReason As String

    <IgnoreDataMember>
    Public Property EpisodeNumber As Integer?

    <IgnoreDataMember>
    Public Property SeasonNumber As Integer?

    <IgnoreDataMember>
    Public Property IsSelected As Boolean

    Public Function ToEpisode() As Episode
        Dim ep As New Episode With {
            .EpisodeName = SeriesEpisodeTitle,
            .FirstAired = CalendarDateTime.ToIso8601DateString,
            .Overview = Copy4000}
        If SeasonNumber.HasValue Then
            ep.SeasonNumber = CShort(SeasonNumber.Value)
        End If
        If EpisodeNumber.HasValue Then
            ep.EpisodeNumber = CShort(EpisodeNumber.Value)
        End If
        Return ep
    End Function

End Class
