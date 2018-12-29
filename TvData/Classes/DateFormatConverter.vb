Public Class DateFormatConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return value
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim dte As Date
        If DateTime.TryParse(CType(value, String), dte) Then
            Return dte.ToString("yyyy-MM-dd")
        Else
            Return value
        End If
    End Function

End Class
