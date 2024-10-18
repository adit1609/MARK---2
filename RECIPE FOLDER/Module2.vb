Module Module2
    ' Lists to store individual elements
    Public XValues As New List(Of Integer)()
    Public YValues As New List(Of Integer)()
    Public IDValues As New List(Of String)()
    Public SideValues As New List(Of Integer)() ' 0 = Top, 1 = Bottom
    Public time As Integer


    ' Clear all lists at once
    Public Sub ClearAllLists()
        XValues.Clear()
        YValues.Clear()
        IDValues.Clear()
        SideValues.Clear()

    End Sub
End Module
