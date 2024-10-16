Public Class stopoper
    Private Sub stopoper_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btcystop.Hide()
        btforce.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        btcystop.Show()
        btforce.Hide()
        btcystop.Dock = DockStyle.Fill
    End Sub

    Private Sub btforce_Click(sender As Object, e As EventArgs) Handles btforce.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        btforce.Show()
        btcystop.Hide()
        btforce.Dock = DockStyle.Fill
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
End Class