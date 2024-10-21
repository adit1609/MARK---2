Imports Guna.UI2.WinForms
Imports System.IO
Imports System.Xml
Imports ActUtlTypeLib
Imports System.Web.UI.Design
Imports System.Diagnostics.Eventing.Reader

Public Class NewRec
    Private currentMark As Integer = 1
    Private _loadedFiles As New HashSet(Of String)()
    Public val As Integer
    Dim plc As New ActUtlType
    Public Property RecipeName As String

    Public Async Function plccon() As Task
        plc.ActLogicalStationNumber = 1
        plc.Open()
        Timer1.Start()
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


                                                 Dim totalMarkingNode As XmlNode = xmlDoc.SelectSingleNode("/RecipeDetails/TotalMarking")
                                                 Dim totalMarking As String = If(totalMarkingNode IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(totalMarkingNode.InnerText), totalMarkingNode.InnerText.Trim(), "-")

                                                 Dim totalFiducialNode As XmlNode = xmlDoc.SelectSingleNode("/RecipeDetails/TotalFiducial")
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
            Dim totalMarkingNode As XmlElement = root.SelectSingleNode("TotalMarking")
            If totalMarkingNode Is Nothing Then
                totalMarkingNode = xmlDoc.CreateElement("TotalMarking")
                root.AppendChild(totalMarkingNode)
            End If

            Dim markingPositionsNode As XmlElement = root.SelectSingleNode("Marking_Positions")
            If markingPositionsNode Is Nothing Then
                markingPositionsNode = xmlDoc.CreateElement("Marking_Positions")
                root.AppendChild(markingPositionsNode)
            Else
                markingPositionsNode.RemoveAll()
            End If
            Dim markCount As Integer = 0

            For Each markNode As TreeNode In TreeView1.Nodes(0).Nodes
                markCount += 1
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
            totalMarkingNode.InnerText = markCount.ToString()

            ' Save the updated XML
            xmlDoc.Save(filePath)
            MessageBox.Show("Marking positions saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As XmlException
            MessageBox.Show($"XML Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


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
        DataGridView1.Rows.Clear()
        _loadedFiles.Clear()
        LoadRecipeAsync()
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
        currentIndex = 0
        boardexit = 0
        Module2.ClearAllLists()

        ' Loop through all child nodes under the first (root) node in TreeView1
        For Each markNode As TreeNode In TreeView1.Nodes(0).Nodes
            ' Skip this node if it's checked
            If markNode.Checked Then Continue For

            Dim xValue, yValue As String
            Dim idValue As String
            Dim sideValue As Integer

            ' Loop through each child node of the current mark node
            For Each childNode As TreeNode In markNode.Nodes
                Dim parts() As String = childNode.Text.Split("_"c)
                If parts.Length = 2 Then
                    Select Case parts(0).ToLower()
                        Case "x"
                            ' Store X value as string (words or numbers)
                            xValue = parts(1)

                        Case "y"
                            ' Store Y value as string (words or numbers)
                            yValue = parts(1)

                        Case "id"
                            ' Store ID as string
                            idValue = parts(1)

                        Case "side"
                            ' Store Side value as integer (0 for top, 1 for bottom)
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

        ' Update total X count
        totalXCount = Module2.XValues.Count
        Module2.time = totalXCount

        ' Confirm success
        MessageBox.Show("TreeView data saved to lists successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
        Dim floatValueCW As Single
        If Single.TryParse(WidthTextbox.Text, floatValueCW) Then
            ' Convert the float value to two 16-bit integers
            Dim words() As Integer = ConvertFloatToWord(floatValueCW)

            ' Write the integers to the PLC registers
            plc.SetDevice("D320", words(0))
            plc.SetDevice("D321", words(1))
        Else
            ' If parsing fails, you can handle the invalid input here
        End If
        StartTimer()
    End Sub
    '
    Public WithEvents timer As New Timer()
    Private Async Function StartTimer() As Task
        ' Set up the timer interval (1 second or as appropriate)

        timer.Start()

    End Function

    Private previousState As Boolean = False   ' Track the previous state of M224
    Private currentIndex As Integer = 0
    Dim boardexit As Integer = 0 ' Track the index of the next value to send

    Private Async Function Timer_Tick(sender As Object, e As EventArgs) As Task Handles timer.Tick
        Dim currentState As Integer


        ' Read the state of M224
        plc.GetDevice("M224", currentState)

            ' Detect rising edge: M224 goes from 0 -> 1
            If currentState = 1 AndAlso Not previousState Then
                ' Send the current value from the list
                If currentIndex < Module2.time Then
                    ' Send X and Y values to the PLC
                    SendFloatValues(Module2.XValues(currentIndex), "D370", "D371")
                    SendFloatValues(Module2.YValues(currentIndex), "D372", "D373")

                    ' Send pattern ID to D390
                    plc.SetDevice("D390", Module2.IDValues(currentIndex))

                    ' Send Side value to D391 (0 = top, 1 = bottom)
                    plc.SetDevice("D391", Module2.SideValues(currentIndex))

                    ' Move to the next index
                    currentIndex += 1
                    boardexit += 1
                    plc.SetDevice("M300", 1)
                End If
                If (boardexit = Module2.time) Then
                    plc.SetDevice("M301", 1)
                    boardexit = 0
                End If



                ' If the end of the list is reached, reset the index to start over
                If currentIndex >= Module2.XValues.Count Then
                    currentIndex = 0
                End If
            End If

            ' Update the previous state for the next tick
            previousState = (currentState = 1)





    End Function

    ' Helper function to send float values to two consecutive PLC addresses



    Private Sub SendFloatValues(valueStr As String, address1 As String, address2 As String)
        Dim floatValue As Single
        If Single.TryParse(valueStr, floatValue) Then
            Dim words() As Integer = ConvertFloatToWord(floatValue)
            plc.SetDevice(address1, words(0))
            plc.SetDevice(address2, words(1))
        End If
    End Sub

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


    Public check As Integer
    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click
        MessageBox.Show(Module2.XValues(1))




    End Sub


    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click

    End Sub

    Private Async Function NewRec_Load(sender As Object, e As EventArgs) As Task Handles MyBase.Load
        Design()
        LoadRecipeAsync()
        plccon()
        plc.SetDevice("M224", 0)

    End Function

    Private Sub Button9_MouseDown(sender As Object, e As MouseEventArgs) Handles PLOAD.MouseDown
        plc.SetDevice("M234", 1)
    End Sub

    Private Sub PLOAD_MouseUp(sender As Object, e As MouseEventArgs) Handles PLOAD.MouseUp
        plc.SetDevice("M234", 0)
    End Sub

    Private Sub PUNLOAD_MouseDown(sender As Object, e As MouseEventArgs) Handles PUNLOAD.MouseDown
        plc.SetDevice("M233", 1)
    End Sub

    Private Sub PUNLOAD_MouseUp(sender As Object, e As MouseEventArgs) Handles PUNLOAD.MouseUp
        plc.SetDevice("M233", 0)
    End Sub

    Private Sub Button13_MouseDown(sender As Object, e As MouseEventArgs) Handles STOPER.MouseDown
        plc.SetDevice("M237", 1)
    End Sub

    Private Sub STOPER_MouseUp(sender As Object, e As MouseEventArgs) Handles STOPER.MouseUp
        plc.SetDevice("M237", 0)
    End Sub

    Private Sub CLAMPSIDE_MouseDown(sender As Object, e As MouseEventArgs) Handles CLAMPSIDE.MouseDown
        plc.SetDevice("M239", 1)
    End Sub

    Private Sub CLAMPSIDE_MouseUp(sender As Object, e As MouseEventArgs) Handles CLAMPSIDE.MouseUp
        plc.SetDevice("M239", 0)
    End Sub

    Private Sub CLAMPTOP_MouseDown(sender As Object, e As MouseEventArgs) Handles CLAMPTOP.MouseDown
        plc.SetDevice("M248", 1)

    End Sub

    Private Sub CLAMPTOP_MouseUp(sender As Object, e As MouseEventArgs) Handles CLAMPTOP.MouseUp
        plc.SetDevice("M248", 0)
    End Sub

    Private Sub Button11_MouseDown(sender As Object, e As MouseEventArgs) Handles Button11.MouseDown
        plc.SetDevice("M235", 1)
    End Sub

    Private Sub Button11_MouseUp(sender As Object, e As MouseEventArgs) Handles Button11.MouseUp
        plc.SetDevice("M235", 0)
    End Sub

    Private Sub Button12_MouseDown(sender As Object, e As MouseEventArgs) Handles Button12.MouseDown
        plc.SetDevice("M236", 1)
    End Sub

    Private Sub Button12_MouseUp(sender As Object, e As MouseEventArgs) Handles Button12.MouseUp
        plc.SetDevice("M236", 0)
    End Sub

    Private Sub Button6_MouseDown(sender As Object, e As MouseEventArgs) Handles Button6.MouseDown
        plc.SetDevice("M252", 1)
    End Sub

    Private Sub Button6_MouseUp(sender As Object, e As MouseEventArgs) Handles Button6.MouseUp
        plc.SetDevice("M252", 0)
    End Sub

    Private Sub Button47_Click(sender As Object, e As EventArgs) Handles YUP.Click

    End Sub

    Private Sub Button46_Click(sender As Object, e As EventArgs) Handles XMIN.Click

    End Sub

    Private Sub YDOWN_MouseDown(sender As Object, e As MouseEventArgs) Handles YDOWN.MouseDown
        plc.SetDevice("M200", 1)
    End Sub

    Private Sub YDOWN_MouseUp(sender As Object, e As MouseEventArgs) Handles YDOWN.MouseUp
        plc.SetDevice("M200", 0)
    End Sub

    Private Sub YUP_MouseDown(sender As Object, e As MouseEventArgs) Handles YUP.MouseDown
        plc.SetDevice("M201", 1)
    End Sub

    Private Sub YUP_MouseUp(sender As Object, e As MouseEventArgs) Handles YUP.MouseUp
        plc.SetDevice("M201", 0)
    End Sub

    Private Sub XMAX_MouseDown(sender As Object, e As MouseEventArgs) Handles XMAX.MouseDown
        plc.SetDevice("M205", 1)
    End Sub

    Private Sub XMAX_MouseUp(sender As Object, e As MouseEventArgs) Handles XMAX.MouseUp
        plc.SetDevice("M205", 0)
    End Sub

    Private Sub XMIN_MouseDown(sender As Object, e As MouseEventArgs) Handles XMIN.MouseDown
        plc.SetDevice("M206", 1)
    End Sub

    Private Sub XMIN_MouseUp(sender As Object, e As MouseEventArgs) Handles XMIN.MouseUp
        plc.SetDevice("M206", 0)
    End Sub

    Private Sub CWWID_MouseDown(sender As Object, e As MouseEventArgs) Handles CWWID.MouseDown
        plc.SetDevice("M249", 1)
    End Sub

    Private Sub CWWID_MouseUp(sender As Object, e As MouseEventArgs) Handles CWWID.MouseUp
        plc.SetDevice("M249", 0)
    End Sub

    Private Sub CWCLOSE_MouseDown(sender As Object, e As MouseEventArgs) Handles CWCLOSE.MouseDown
        plc.SetDevice("M250", 1)
    End Sub

    Private Sub CWCLOSE_MouseUp(sender As Object, e As MouseEventArgs) Handles CWCLOSE.MouseUp
        plc.SetDevice("M250", 0)
    End Sub

    Private Async Function Button9_Click(sender As Object, e As EventArgs) As Task Handles Button9.Click
        Dim floatValueCW As Single
        If Single.TryParse(WidthTextbox.Text, floatValueCW) Then
            ' Convert the float value to two 16-bit integers
            Dim words() As Integer = ConvertFloatToWord(floatValueCW)

            ' Write the integers to the PLC registers
            plc.SetDevice("D320", words(0))
            plc.SetDevice("D321", words(1))
        Else
            ' If parsing fails, you can handle the invalid input here
        End If
    End Function

    Private Sub WidthTextbox_MouseDown(sender As Object, e As MouseEventArgs) Handles WidthTextbox.MouseDown
        plc.SetDevice("M240", 1)

    End Sub

    Private Sub WidthTextbox_MouseUp(sender As Object, e As MouseEventArgs) Handles WidthTextbox.MouseUp
        plc.SetDevice("M240", 0)
    End Sub

    Private messageBoxShown As Boolean = False
    Private Async Function Timer1_Tick(sender As Object, e As EventArgs) As Task Handles Timer1.Tick
        plc.GetDevice("M220", AUTORUN)

        If AUTORUN = False AndAlso Not messageBoxShown Then
            messageBoxShown = True
            MessageBox.Show("MACHINE NOT READY")
        End If

        ' Reset the flag if AUTORUN is on
        If AUTORUN Then
            messageBoxShown = False
        End If


        Dim Xval(1) As Integer
        plc.GetDevice("D342", Xval(0))
        plc.GetDevice("D343", Xval(1))
        Dim xnum As Single = ConvertWordToFloat(Xval)
        X.Text = xnum.ToString("F4")
        Dim Yval(1) As Integer
        plc.GetDevice("D344", Xval(0))
        plc.GetDevice("D345", Xval(1))
        Dim ynum As Single = ConvertWordToFloat(Yval)
        Y.Text = xnum.ToString("F4")
        Dim CWval(1) As Integer
        plc.GetDevice("D312", Xval(0))
        plc.GetDevice("D313", Xval(1))
        Dim CWnum As Single = ConvertWordToFloat(CWval)
        CW.Text = xnum.ToString("F4")
    End Function

    Private Sub Button3_MouseEnter(sender As Object, e As EventArgs) Handles Button3.MouseEnter

    End Sub

    Private Sub Button3_MouseDown(sender As Object, e As MouseEventArgs) Handles Button3.MouseDown
        plc.SetDevice("M202", 1)
    End Sub

    Private Sub Button3_MouseUp(sender As Object, e As MouseEventArgs) Handles Button3.MouseUp
        plc.SetDevice("M202", 0)
    End Sub



    ' Converts a Single (float) to two words (integers) for the PLC

End Class