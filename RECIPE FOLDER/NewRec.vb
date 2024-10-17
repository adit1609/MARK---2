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
    Private Sub LoadMarkingPositionsToTreeView(recipeName As String)
        ' Construct the file path based on the recipe name
        Dim filePath As String = $"C:\Logs\Default\{recipeName}.xml"

        Try
            ' Log the file path for debugging
            Debug.WriteLine($"Loading XML from: {filePath}")

            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(filePath) ' Load the XML file

            ' Clear existing nodes in the TreeView before loading new data
            TreeView1.Nodes.Clear()

            ' Get the Marking_Positions node
            Dim markingPositionsNode As XmlNode = xmlDoc.SelectSingleNode("/RecipeDetails/Marking_Positions")

            ' Log the found node for debugging
            If markingPositionsNode IsNot Nothing Then
                Debug.WriteLine("Marking_Positions node found.")
            Else
                Debug.WriteLine("Marking_Positions node NOT found.")
            End If

            If markingPositionsNode IsNot Nothing Then
                ' Create a parent node for Marking Positions
                Dim parentNode As TreeNode = New TreeNode("Marking Positions")

                ' Loop through each mark element
                For Each markNode As XmlNode In markingPositionsNode.ChildNodes
                    Dim markTreeNode As TreeNode = New TreeNode(markNode.Name)

                    ' Loop through each child node (X, Y, ID, Side)
                    For Each childNode As XmlNode In markNode.ChildNodes
                        Dim childTreeNode As TreeNode = New TreeNode($"{childNode.Name}_{childNode.InnerText.Trim()}")
                        markTreeNode.Nodes.Add(childTreeNode)
                    Next

                    parentNode.Nodes.Add(markTreeNode) ' Add the mark node to the parent
                Next

                TreeView1.Nodes.Add(parentNode) ' Add the parent node to the TreeView
                TreeView1.ExpandAll() ' Expand all nodes to show the data
            Else
                MessageBox.Show("No marking positions found for the selected recipe.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error loading marking positions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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
    Private Sub SaveMarkingPositionsToXML(filePath As String)
        Try
            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(filePath) ' Load the existing XML

            ' Get the root element
            Dim root As XmlElement = xmlDoc.DocumentElement

            ' Create or get the 'Marking_Positions' node
            Dim markingPositionsNode As XmlElement = root.SelectSingleNode("Marking_Positions")
            If markingPositionsNode Is Nothing Then
                markingPositionsNode = xmlDoc.CreateElement("Marking_Positions")
                root.AppendChild(markingPositionsNode)
            Else
                markingPositionsNode.RemoveAll() ' Clear old data to avoid duplication
            End If

            ' Loop through all child nodes of the first node in the TreeView
            For Each markNode As TreeNode In TreeView1.Nodes(0).Nodes
                ' Use the node name as the element name for XML
                Dim validMarkName As String = markNode.Text.Replace(" ", "_")
                ' Ensure the validMarkName does not start with a number
                If Char.IsDigit(validMarkName(0)) Then
                    validMarkName = "_" & validMarkName ' Prefix with underscore if it starts with a number
                End If
                Dim markElement As XmlElement = xmlDoc.CreateElement(validMarkName)

                ' Add child nodes (X, Y, ID, Side) based on the TreeNode names
                For Each childNode As TreeNode In markNode.Nodes
                    Dim nodeName As String = childNode.Text.Trim() ' Get the text of the child node
                    Dim nodeValue As String = "" ' Default value for the node

                    ' Extract the value from the node name
                    If nodeName.Contains("_") Then
                        Dim parts() As String = nodeName.Split("_"c) ' Split using underscore
                        If parts.Length = 2 Then
                            nodeValue = parts(1).Trim() ' Get the value part after the underscore
                            Dim subElement As XmlElement = xmlDoc.CreateElement(parts(0).Trim()) ' Use the key part as the element name
                            subElement.InnerText = nodeValue
                            markElement.AppendChild(subElement) ' Add to mark element
                        End If
                    End If
                Next

                ' Append the mark element to 'Marking_Positions'
                markingPositionsNode.AppendChild(markElement)
            Next

            ' Save the updated XML
            xmlDoc.Save(filePath)
            MessageBox.Show("Marking positions saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As XmlException
            MessageBox.Show($"XML Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub















    Private Async Function Button37_Click(sender As Object, e As EventArgs) As Task Handles TEACH.Click
        Dim xValue As String = X.Text.Trim()
        Dim yValue As String = Y.Text.Trim()
        Dim idValue As String = ID.Text.Trim()
        Dim pside As String = SIDE.SelectedItem.ToString().Trim()

        ' Create the mark node without spaces
        Dim markNode As TreeNode = New TreeNode($"{currentMark}st_MARK") With {
        .Checked = True ' Check the checkbox
    }

        ' Capture x and y mark values as child nodes without spaces
        markNode.Nodes.Add(New TreeNode($"X_{xValue}"))   ' Replaced space with underscore
        markNode.Nodes.Add(New TreeNode($"Y_{yValue}"))   ' Replaced space with underscore
        markNode.Nodes.Add(New TreeNode($"ID_{idValue}"))  ' Replaced space with underscore
        markNode.Nodes.Add(New TreeNode($"Side_{pside}"))  ' Replaced space with underscore

        ' Add the mark node as a child of the first node
        TreeView1.Nodes(0).Nodes.Add(markNode)

        ' Expand the parent node to show the new child
        TreeView1.Nodes(0).Expand()

        currentMark += 1 ' Increment the current mark counter
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
        'yaha pe ye load karne ke kaam mei aayega 
        recipeXml.CreateRecipeXML(recipeName, length, width)
        DataGridView1.Rows.Clear()
        _loadedFiles.Clear()
        LoadRecipeAsync()
    End Function

    Private Async Function SAVE_Click(sender As Object, e As EventArgs) As Task Handles SAVE.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a recipe to save the marking position.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Get the recipe name from the 2nd column (e.g., "mm")
        Dim recipeName As String = DataGridView1.SelectedRows(0).Cells(1).Value.ToString()

        ' Combine the base path with the recipe name, ensuring it has the .xml extension
        Dim basePath As String = "C:\Logs\Default\"
        Dim fileUri As String = $"file:///{basePath}{recipeName}.xml"

        ' Convert the URI to a valid local path
        Dim filePath As String = New Uri(fileUri).LocalPath

        ' Save the marking positions to the XML file
        SaveMarkingPositionsToXML(filePath)
    End Function



    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick

        If e.RowIndex >= 0 Then
            Dim selectedRow As DataGridViewRow = DataGridView1.Rows(e.RowIndex)
            Selected.Text = selectedRow.Cells(1).Value.ToString()

            ' Get the recipe name (or relevant identifier) from the second column
            Dim recipeName As String = selectedRow.Cells(1).Value.ToString()

            ' Load and display the marking positions in the TreeView for the selected recipe
            LoadMarkingPositionsToTreeView(recipeName)
        End If
    End Sub
End Class