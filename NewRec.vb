Public Class NewRec
    Private currentMark As Integer = 1
    Private Sub Label65_Click(sender As Object, e As EventArgs) Handles Label65.Click

    End Sub

    Private Sub Label63_Click(sender As Object, e As EventArgs) Handles Label63.Click

    End Sub

    Private Sub GroupBox8_Enter(sender As Object, e As EventArgs) Handles GroupBox8.Enter

    End Sub

    Private Async Function Button37_Click(sender As Object, e As EventArgs) As Task Handles TEACH.Click
        Dim xValue As String = X.Text
        Dim yValue As String = Y.Text
        Dim idValue As String = ID.Text
        'ye checkbox ko checked rakega 
        Dim markNode As TreeNode = New TreeNode($"{currentMark}st MARK") With {
            .Checked = True 'uss checkbox ko check rakhega 
        }
        'x and y mark ke value uss tree node mei caputure kar lega 
        markNode.Nodes.Add(New TreeNode($"X - {xValue}"))
        markNode.Nodes.Add(New TreeNode($"Y - {yValue}"))
        markNode.Nodes.Add(New TreeNode($"ID - {idValue}"))
        'parent node banaega yaha pe (papa)
        TreeView1.Nodes(0).Nodes.Add(markNode)
        ' jo add ho raha usko expand kar dega 
        TreeView1.Nodes(0).Expand()
        currentMark += 1
    End Function


    Private Async Function SUBNewRec_Load(sender As Object, e As EventArgs) As Task Handles MyBase.Load
        TreeView1.CheckBoxes = True
    End Function


End Class