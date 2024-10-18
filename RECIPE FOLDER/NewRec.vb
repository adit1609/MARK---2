Imports Guna.UI2.WinForms
Imports System.IO
Imports System.Xml
Imports ActUtlTypeLib

Public Class NewRec
    Private currentMark As Integer = 1
    Private _loadedFiles As New HashSet(Of String)()
    Public val As Integer
    Dim plc As New ActUtlType
    Public Property RecipeName As String

    Public Async Function plccon() As Task
        plc.ActLogicalStationNumber = 1
        plc.Open()
    End Function

    Private Sub Label65_Click(sender As Object, e As EventArgs) Handles Label65.Click

    End Sub

    Private Sub Label63_Click(sender As Object, e As EventArgs) Handles Label63.Click

    End Sub

    Private Sub GroupBox8_Enter(sender As Object, e As EventArgs) Handles GroupBox8.Enter

    End Sub
    Private Sub LoadMarkingPositionsToTreeView(recipeName As String)

        Dim filePath As String = $"C:\Logs\Default\{recipeName}.xml"

        Try

            Debug.WriteLine($"Loading XML from: {filePath}")

            Dim xmlDoc As New XmlDocument()
            xmlDoc.Load(filePath) ' Load the XML file
            TreeView1.Nodes.Clear()


            Dim markingPositionsNode As XmlNode = xmlDoc.SelectSingleNode("/RecipeDetails/Marking_Positions")

            ' Log the found node for debugging
            'ye node ko save kar dega 
            If markingPositionsNode IsNot Nothing Then
                Debug.WriteLine("Marking_Positions node found.")

            Else
                Debug.WriteLine("Marking_Positions node NOT found.")

            End If

            If markingPositionsNode IsNot Nothing Then

                Dim parentNode As TreeNode = New TreeNode("Marking Positions")


                For Each markNode As XmlNode In markingPositionsNode.ChildNodes
                    Dim markTreeNode As TreeNode = New TreeNode(markNode.Name)


                    For Each childNode As XmlNode In markNode.ChildNodes
                        Dim childTreeNode As TreeNode = New TreeNode($"{childNode.Name}_{childNode.InnerText.Trim()}")
                        markTreeNode.Nodes.Add(childTreeNode)
                    Next

                    parentNode.Nodes.Add(markTreeNode)
                Next

                TreeView1.Nodes.Add(parentNode)
                TreeView1.ExpandAll()
            Else
                'MessageBox.Show("No marking positions found for the selected recipe.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error loading marking positions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Public Async Function LoadRecipeAsync() As Task
        Await Task.Run(Sub()
                           Dim basePath As String = "C:\Logs\Default\"
                           Dim xmlFiles As String() = Directory.GetFiles(basePath, "*.xml", SearchOption.AllDirectories)


                           Me.Invoke(Sub()
                                         Dim countRecipe As Integer = DataGridView1.Rows.Count + 1

                                         For Each file As String In xmlFiles
                                             If Not _loadedFiles.Contains(file) Then
                                                 Dim xmlDoc As New XmlDocument()
                                                 xmlDoc.Load(file)


                                                 Dim programName As String = Path.GetFileNameWithoutExtension(file)


                                                 Dim totalMarkingNode As XmlNode = xmlDoc.SelectSingleNode("Project_Specification/TotalMarking")
                                                 Dim totalMarking As String = If(totalMarkingNode IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(totalMarkingNode.InnerText), totalMarkingNode.InnerText.Trim(), "-")

                                                 Dim totalFiducialNode As XmlNode = xmlDoc.SelectSingleNode("Project_Specification/TotalFiducial")
                                                 Dim totalFiducial As String = If(totalFiducialNode IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(totalFiducialNode.InnerText), totalFiducialNode.InnerText.Trim(), "-")


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
            xmlDoc.Load(filePath)


            Dim root As XmlElement = xmlDoc.DocumentElement


            Dim markingPositionsNode As XmlElement = root.SelectSingleNode("Marking_Positions")
            If markingPositionsNode Is Nothing Then
                markingPositionsNode = xmlDoc.CreateElement("Marking_Positions")
                root.AppendChild(markingPositionsNode)
            Else
                markingPositionsNode.RemoveAll()
            End If


            For Each markNode As TreeNode In TreeView1.Nodes(0).Nodes

                Dim validMarkName As String = markNode.Text.Replace(" ", "_")

                If Char.IsDigit(validMarkName(0)) Then
                    validMarkName = "_" & validMarkName
                End If
                Dim markElement As XmlElement = xmlDoc.CreateElement(validMarkName)


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
    Private Async Function NewRec_Load(sender As Object, e As EventArgs) As Task Handles MyBase.Load
        Design()
        LoadRecipeAsync()
        plccon()
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

    Private Async Function SAVEE_Click(sender As Object, e As EventArgs) As Task Handles SAVE.Click
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


            Dim recipeName As String = selectedRow.Cells(1).Value.ToString()

            ' Load and display the marking positions in the TreeView for the selected recipe
            LoadMarkingPositionsToTreeView(recipeName)
        End If
    End Sub
    Public totalXCount As Integer
    Private Sub SaveTreeViewDataToLists()
        ' Clear all lists before saving new data
        Module2.ClearAllLists()

        ' Loop through all child nodes under the first (root) node in TreeView1
        For Each markNode As TreeNode In TreeView1.Nodes(0).Nodes
            ' Skip this node if it's checked
            If markNode.Checked Then Continue For

            Dim xValue, yValue As Integer
            Dim idValue As String
            Dim sideValue As Integer

            ' Loop through each child node of the current mark node
            For Each childNode As TreeNode In markNode.Nodes
                Dim parts() As String = childNode.Text.Split("_"c)
                If parts.Length = 2 Then
                    Select Case parts(0).ToLower()
                        Case "x"
                            xValue = Integer.Parse(parts(1))
                        Case "y"
                            yValue = Integer.Parse(parts(1))
                        Case "id"
                            idValue = parts(1)
                        Case "side"
                            sideValue = If(parts(1).ToLower() = "top", 0, 1)
                    End Select
                End If
            Next


            ' Add the values to the respective lists
            Module2.XValues.Add(xValue)
            Module2.YValues.Add(yValue)
            Module2.IDValues.Add(idValue)
            Module2.SideValues.Add(sideValue)
        Next
        totalXCount = Module2.XValues.Count


        MessageBox.Show("TreeView data saved to lists successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub



    Public Sub PrintLists()
        MessageBox.Show("X Values: " & String.Join(", ", Module2.XValues))
        MessageBox.Show("Y Values: " & String.Join(", ", Module2.YValues))
        MessageBox.Show("ID Values: " & String.Join(", ", Module2.IDValues))
        MessageBox.Show("Side Values: " & String.Join(", ", Module2.SideValues))
        MessageBox.Show(totalXCount)
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SaveTreeViewDataToLists()
        PrintLists()
    End Sub

    Private Async Function btnclear_MouseDown(sender As Object, e As MouseEventArgs) As Task Handles btnclear.MouseDown
        plc.SetDevice("M232", 1)
    End Function

    Private Async Function btnclear_MouseUp(sender As Object, e As MouseEventArgs) As Task Handles btnclear.MouseUp
        plc.SetDevice("M232", 0)
    End Function


    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        StartTimer()
    End Sub
    Private previousM224State As Boolean = False
    Private WithEvents timer As New Timer()
    Private Async Function StartTimer() As Task
        ' Set up the timer interval (1 second or as appropriate)
        timer.Interval = 100
        timer.Start()
    End Function
    Private Async Function Timer_Tick(sender As Object, e As EventArgs) As Task Handles timer.Tick
        ' Check if M224 is turned on
        Dim isM224On As Boolean = False
        plc.GetDevice("M224", isM224On)

        ' Send values only when M224 just turned on
        If isM224On AndAlso Not previousM224State Then
            SendValuesToPLC()
        End If

        ' Update previous state
        previousM224State = isM224On
    End Function
    Public Function ConvertFloatToWord(ByVal value As Single) As Integer()
        Dim floatBytes As Byte() = BitConverter.GetBytes(value)
        Dim lowWord As Integer = BitConverter.ToInt16(floatBytes, 0)
        Dim highWord As Integer = BitConverter.ToInt16(floatBytes, 2)
        Return {lowWord, highWord}
    End Function


    ' Convert word to float
    Public Function ConvertWordToFloat(ByVal register As Integer()) As Single
        Dim bytes(3) As Byte
        Dim lowWordBytes() As Byte = BitConverter.GetBytes(register(0))
        Dim highWordBytes() As Byte = BitConverter.GetBytes(register(1))

        Array.Copy(lowWordBytes, 0, bytes, 0, 2)
        Array.Copy(highWordBytes, 0, bytes, 2, 2)

        Return BitConverter.ToSingle(bytes, 0)
    End Function
    Private Sub SendValuesToPLC()
        ' Send X, Y, and Pattern ID data to PLC
        For index As Integer = 0 To Module2.XValues.Count - 1
            ' Convert X value and send to PLC (D370, D371)
            Dim floatValueX As Single
            If Single.TryParse(Module2.XValues(index), floatValueX) Then
                Dim wordsX() As Integer = ConvertFloatToWord(floatValueX)
                plc.SetDevice("D370", wordsX(0))
                plc.SetDevice("D371", wordsX(1))
            End If

            ' Convert Y value and send to PLC (D372, D373)
            Dim floatValueY As Single
            If Single.TryParse(Module2.YValues(index).ToString(), floatValueY) Then
                Dim wordsY() As Integer = ConvertFloatToWord(floatValueY)
                plc.SetDevice("D372", wordsY(0))
                plc.SetDevice("D373", wordsY(1))
            End If

            ' Send Pattern ID to D390
            Dim patternID As Integer = Module2.IDValues(index)
            plc.SetDevice("D390", patternID)

            ' Send Side value to D391 (assuming 0 = top, 1 = bottom)
            Dim sideValue As Integer = Module2.SideValues(index) ' 0 or 1
            plc.SetDevice("D391", sideValue)

            ' Break the loop after Module1.time seconds
            If index >= Module2.time Then
                Exit For
            End If
        Next
    End Sub

    Private Sub SAVE_Click(sender As Object, e As EventArgs) Handles SAVE.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Async Function Button37_Click(sender As Object, e As EventArgs) As Task Handles TEACH.Click
        Dim xValue As String = X.Text.Trim()
        Dim yValue As String = Y.Text.Trim()
        Dim idValue As String = ID.Text.Trim()
        Dim pside As String = SIDE.SelectedItem.ToString().Trim()

        If TreeView1.Nodes.Count = 0 Then
            TreeView1.Nodes.Add(New TreeNode("Marking Position"))
        End If

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

    ' Converts a Single (float) to two words (integers) for the PLC

End Class