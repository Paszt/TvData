Option Strict On

Public Class ExternalSourceColumnVisibilityConverter
    Implements IValueConverter

    Public Function Convert(value As Object,
                            targetType As Type,
                            parameter As Object,
                            culture As Globalization.CultureInfo) _
                        As Object Implements IValueConverter.Convert
        Dim externalSourceType As MainViewModel.ExternalSourceType
        If [Enum].TryParse(CStr(value), externalSourceType) Then
            Select Case externalSourceType
                Case MainViewModel.ExternalSourceType.IMDB
                    If parameter.ToString() = "IMDB" Then
                        Return Visibility.Visible
                    End If
                Case MainViewModel.ExternalSourceType.TMDb
                    If parameter.ToString() = "TMDb" Then
                        Return Visibility.Visible
                    End If
                Case MainViewModel.ExternalSourceType.TvGuide
                    If parameter.ToString() = "TvGuide" Then
                        Return Visibility.Visible
                    End If
                Case MainViewModel.ExternalSourceType.TvRage
                    If parameter.ToString = "TvRage" Then
                        Return Visibility.Visible
                    End If
                Case MainViewModel.ExternalSourceType.Freebase
                    If parameter.ToString = "Freebase" Then
                        Return Visibility.Visible
                    End If
                Case MainViewModel.ExternalSourceType.Alternative
                    If parameter.ToString = "Alternative" Then
                        Return Visibility.Visible
                    End If
            End Select
        End If
        Return Visibility.Collapsed
    End Function

    Public Function ConvertBack(value As Object,
                                targetType As Type,
                                parameter As Object,
                                culture As Globalization.CultureInfo) _
                            As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function

End Class
