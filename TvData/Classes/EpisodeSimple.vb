Imports System.ComponentModel
Imports System.Xml.Serialization

<Serializable(),
DesignerCategory("code"),
XmlType(AnonymousType:=True)>
Public Class EpisodeSimple
    Inherits ViewModelBase

#Region " Properties "

    Private showNameField As String
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified),
        DisplayName("Show Name")>
    Public Property ShowName As String
        Get
            Return showNameField
        End Get
        Set(value As String)
            If value IsNot Nothing Then
                value = value.Trim()
            End If
            SetProperty(showNameField, value)
        End Set
    End Property

    Private episodeNameField As String
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified),
        DisplayName("Episode Name")>
    Public Property EpisodeName As String
        Get
            Return episodeNameField
        End Get
        Set(value As String)
            If value IsNot Nothing Then
                value = value.Trim()
            End If
            SetProperty(episodeNameField, value)
        End Set
    End Property

    Private episodeNumberField As Integer
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified)>
    Public Property EpisodeNumber As Integer
        Get
            Return episodeNumberField
        End Get
        Set(value As Integer)
            SetProperty(episodeNumberField, value)
        End Set
    End Property

    Private firstAiredField As String
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified),
        DisplayName("First Aired")>
    Public Property FirstAired As String
        Get
            Return firstAiredField
        End Get
        Set(value As String)
            SetProperty(firstAiredField, value)
        End Set
    End Property

    Private overviewField As String
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified),
        DisplayName("Overview")>
    Public Property Overview As String
        Get
            Return overviewField
        End Get
        Set(value As String)
            If value IsNot Nothing Then
                value = value.Trim()
            End If
            SetProperty(overviewField, value)
        End Set
    End Property

    Private productionCodeField As String
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified),
        DisplayName("Production Code")>
    Public Property ProductionCode As String
        Get
            Return productionCodeField
        End Get
        Set(value As String)
            SetProperty(productionCodeField, value)
        End Set
    End Property

    Private seasonNumberField As Integer
    <XmlElement(Form:=Xml.Schema.XmlSchemaForm.Unqualified)>
    Public Property SeasonNumber As Integer
        Get
            Return seasonNumberField
        End Get
        Set(value As Integer)
            SetProperty(seasonNumberField, value)
        End Set
    End Property

    Private isSelectedField As Boolean
    <XmlIgnore()>
    Public Property IsSelected As Boolean
        Get
            Return isSelectedField
        End Get
        Set(value As Boolean)
            SetProperty(isSelectedField, value)
        End Set
    End Property

#End Region

    Public Function ToEpisode() As Episode
        Return New Episode With {
            .EpisodeName = EpisodeName,
            .EpisodeNumber = CShort(EpisodeNumber),
            .FirstAired = FirstAired,
            .Overview = Overview,
            .ProductionCode = ProductionCode,
            .SeasonNumber = CShort(SeasonNumber)}
    End Function

End Class
