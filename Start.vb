Public Class Start
    Private Sub Start_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BOX1.Hide()
        btrs.Hide()
        btsrt.Hide()
    End Sub

    Private Sub BOX1_TextChanged(sender As Object, e As EventArgs) Handles BOX1.TextChanged

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        BOX1.Show()
        btrs.Show()
        btsrt.Show()
    End Sub
End Class