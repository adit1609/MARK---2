Module Module2
    ' Lists to store individual elements
    Public XValues As New List(Of String)()
    Public YValues As New List(Of String)()
    Public IDValues As New List(Of String)()
    Public SideValues As New List(Of Integer)() ' 0 = Top, 1 = Bottom
    Public time As Integer
    Public length As Integer
    Public width As Single

    ' Array of lists to group them together
    Public AllValues As List(Of List(Of Object)) = New List(Of List(Of Object)) From {
        XValues.Cast(Of Object).ToList(),
        YValues.Cast(Of Object).ToList(),
        IDValues.Cast(Of Object).ToList(),
        SideValues.Cast(Of Object).ToList()
    }

    ' Clear all lists at once
    Public Sub ClearAllLists()
        XValues.Clear()
        YValues.Clear()
        IDValues.Clear()
        SideValues.Clear()
    End Sub
End Module
