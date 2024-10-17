Imports Guna.UI2.WinForms
Imports System.IO
Imports System.Xml

Public Class NewRec
    Private currentMark As Integer = 1
    Private _loadedFiles As New HashSet(Of String)()
    Public Property RecipeName As String
    Private Sub Label65_Click(sender As Object, e As EventArgs) Handles Label65.Click

    End Sub

    Private Sub Label63_Click(sender As Object, e As EventArgs) Handles Label63.Click

    End Sub

    Private Sub GroupBox8_Enter(sender As Object, e As EventArgs) Handles GroupBox8.Enter

    End Sub
    Public Async Function LoadRecipeAsync() As Task
        Await Task.Run(Sub()
                           Dim basePath As String = "C:\Logs\Default\"
                           Dim xmlFiles As String() = Directory.GetFiles(basePath, "*.xml", SearchOption.AllDirectories)

                           ' Use Invoke to update UI elements from the non-UI thread
                           Me.Invoke(Sub()
                                         Dim countRecipe As Integer = DataGridView1.Rows.Count + 1

                                         For Each file As String In xmlFiles
                                             If Not _loadedFiles.Contains(file) Then
                                                 Dim xmlDoc As New XmlDocument()
                                                 xmlDoc.Load(file)

                                                 ' Use the file name as Program_Name (without extension)
                                                 Dim programName As String = Path.GetFileNameWithoutExtension(file)

                                                 ' Retrieve the values, ensuring we handle cases where the nodes might not exist
                                                 Dim totalMarkingNode As XmlNode = xmlDoc.SelectSingleNode("Project_Specification/TotalMarking")
                                                 Dim totalMarking As String = If(totalMarkingNode IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(totalMarkingNode.InnerText), totalMarkingNode.InnerText.Trim(), "-")

                                                 Dim totalFiducialNode As XmlNode = xmlDoc.SelectSingleNode("Project_Specification/TotalFiducial")
                                                 Dim totalFiducial As String = If(totalFiducialNode IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(totalFiducialNode.InnerText), totalFiducialNode.InnerText.Trim(), "-")

                                                 ' Add data to DataGridView
                                                 DataGridView1.Rows.Add(countRecipe, programName, totalMarking, totalFiducial)
                                                 countRecipe += 1

                                                 _loadedFiles.Add(file)
                                             End If
                                         Next
                                     End Sub)
                       End Sub)
    End Function

    Private Async Function Button37_Click(sender As Object, e As EventArgs) As Task Handles TEACH.Click
        Dim xValue As String = X.Text
        Dim yValue As String = Y.Text
        Dim idValue As String = ID.Text
        Dim pside As String = SIDE.SelectedItem.ToString()
        'ye checkbox ko checked rakega 
        Dim markNode As TreeNode = New TreeNode($"{currentMark}st MARK") With {
            .Checked = True 'uss checkbox ko check rakhega 
        }
        'x and y mark ke value uss tree node mei caputure kar lega 
        markNode.Nodes.Add(New TreeNode($"X - {xValue}"))
        markNode.Nodes.Add(New TreeNode($"Y - {yValue}"))
        markNode.Nodes.Add(New TreeNode($"ID - {idValue}"))
        markNode.Nodes.Add(New TreeNode($"Side - {pside}"))
        'parent node banaega yaha pe (papa)
        TreeView1.Nodes(0).Nodes.Add(markNode)
        ' jo add ho raha usko expand kar dega 
        TreeView1.Nodes(0).Expand()
        currentMark += 1
    End Function


    Private Async Function NewRec_Load(sender As Object, e As EventArgs) As Task Handles MyBase.Load
        Design()
        LoadRecipeAsync()
    End Function

    Public Sub Design()
        TreeView1.CheckBoxes = True
        For Each row As DataGridViewRow In DataGridView1.Rows
            row.Height = 100
        Next

    End Sub

    Private Sub X_TextChanged(sender As Object, e As EventArgs) Handles X.TextChanged

    End Sub

    Private Async Function Button4_Click(sender As Object, e As EventArgs) As Task Handles Button4.Click
        Dim recipeName As String = InputBox("Enter the recipe name:", "Add Recipe")
        If String.IsNullOrWhiteSpace(recipeName) Then
            MessageBox.Show("Please enter a valid recipe name.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim length As String = LengthTextbox.Text
        Dim width As String = WidthTextbox.Text
        If String.IsNullOrWhiteSpace(length) OrElse String.IsNullOrWhiteSpace(width) Then
            MessageBox.Show("Please enter valid Length and Width values.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim recipeXml As New RECIPEXML()
        recipeXml.CreateRecipeXML(recipeName, length, width)
        DataGridView1.Rows.Clear()
        _loadedFiles.Clear()
        LoadRecipeAsync()
    End Function


End Class