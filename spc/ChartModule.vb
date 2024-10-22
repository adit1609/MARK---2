Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting

Module ChartModule

    Public Sub PopulateChart(Chart1 As Chart, DataGridView1 As DataGridView)

        Chart1.Series.Clear()


        If Chart1.ChartAreas.Count = 0 Then
            Chart1.ChartAreas.Add(New ChartArea("MainArea"))
        End If

        Dim chartArea = Chart1.ChartAreas(0)
        chartArea.BackColor = Color.LightGray
        chartArea.BorderColor = Color.Black
        chartArea.BorderWidth = 2


        Dim seriesInfo As New List(Of Tuple(Of String, String, Color)) From {
            Tuple.Create("Recipe Name", "ReceipeNameDataGridViewTextBoxColumn", Color.Green),
            Tuple.Create("Status", "StatusDataGridViewTextBoxColumn", Color.Red),
            Tuple.Create("User Name", "UserNameDataGridViewTextBoxColumn", Color.Blue),
            Tuple.Create("Side", "SideDataGridViewTextBoxColumn", Color.Purple),
            Tuple.Create("Shift", "ShiftDataGridViewTextBoxColumn", Color.Orange)
        }

        For Each info In seriesInfo
            Dim seriesName As String = info.Item1
            Dim columnName As String = info.Item2
            Dim seriesColor As Color = info.Item3

            Dim series As New Series(seriesName) With {
                .ChartType = SeriesChartType.Column,
                .Color = seriesColor,
                .IsValueShownAsLabel = True,
                .XValueType = ChartValueType.String
            }


            Chart1.Series.Add(series)


            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow Then

                    Dim dateOfCompletion As DateTime
                    If DateTime.TryParse(row.Cells("DateOfMarkingCompletionDataGridViewTextBoxColumn").Value.ToString(), dateOfCompletion) Then

                        Dim columnValue As String = row.Cells(columnName).Value.ToString()


                        Dim pointIndex As Integer = series.Points.AddXY(dateOfCompletion.ToShortDateString(), 1)

                        Dim dataPoint As DataPoint = series.Points(pointIndex)

                        dataPoint.Label = columnValue
                        dataPoint.LabelForeColor = Color.White
                        dataPoint.LabelBackColor = seriesColor
                    End If
                End If
            Next
        Next


        If Chart1.Titles.Count = 0 Then
            Dim title As New Title("Data Comparison by Date", Docking.Top, New Font("Arial", 16, FontStyle.Bold), Color.Black)
            Chart1.Titles.Add(title)
        End If

        chartArea.AxisX.Title = "Completion Date"
        chartArea.AxisY.Title = "Count"
        chartArea.AxisX.TitleFont = New Font("Arial", 12, FontStyle.Bold)
        chartArea.AxisY.TitleFont = New Font("Arial", 12, FontStyle.Bold)

        chartArea.AxisX.LabelStyle.Font = New Font("Arial", 10, FontStyle.Regular)
        chartArea.AxisY.LabelStyle.Font = New Font("Arial", 10, FontStyle.Regular)


        chartArea.AxisX.LabelStyle.Angle = -45
    End Sub


    'Public Sub Chart_MouseClick(Chart1 As Chart, DataGridView1 As DataGridView, e As MouseEventArgs)

    '    Dim result As HitTestResult = Chart1.HitTest(e.X, e.Y)

    '    If result.ChartElementType = ChartElementType.DataPoint Then

    '        Dim series As Series = result.Series
    '        Dim pointIndex As Integer = result.PointIndex
    '        Dim point As DataPoint = series.Points(pointIndex)


    '        Dim dateClicked As String = point.AxisLabel

    '        Dim message As String = $"Date: {dateClicked}" & vbCrLf &
    '                            $"Details: {point.Label}"
    '        MessageBox.Show(message, "Details", MessageBoxButtons.OK, MessageBoxIcon.Information)

    '        Dim dialogResult As DialogResult = MessageBox.Show($"Do you want to download the data for {dateClicked}?", "Download Data", MessageBoxButtons.YesNo)


    '        If dialogResult = DialogResult.Yes Then
    '            DownloadDataForDate(dateClicked, DataGridView1)
    '        End If
    '    End If
    'End Sub

    'Public Sub DownloadDataForDate(dateClicked As String, DataGridView1 As DataGridView)

    '    Dim dataForDate As New List(Of String)

    '    For Each row As DataGridViewRow In DataGridView1.Rows
    '        If Not row.IsNewRow Then

    '            Dim dateOfCompletion As DateTime
    '            If DateTime.TryParse(row.Cells("DateOfMarkingCompletionDataGridViewTextBoxColumn").Value.ToString(), dateOfCompletion) Then
    '                If dateOfCompletion.ToShortDateString() = dateClicked Then

    '                    Dim recipeName As String = row.Cells("ReceipeNameDataGridViewTextBoxColumn").Value.ToString()
    '                    Dim status As String = row.Cells("StatusDataGridViewTextBoxColumn").Value.ToString()
    '                    Dim userName As String = row.Cells("UserNameDataGridViewTextBoxColumn").Value.ToString()
    '                    Dim side As String = row.Cells("SideDataGridViewTextBoxColumn").Value.ToString()
    '                    Dim shift As String = row.Cells("ShiftDataGridViewTextBoxColumn").Value.ToString()

    '                    dataForDate.Add($"Date: {dateClicked}, Recipe: {recipeName}, Status: {status}, User: {userName}, Side: {side}, Shift: {shift}")
    '                End If
    '            End If
    '        End If
    '    Next



    '    If dataForDate.Count > 0 Then
    '        SaveDataToTextFile(dateClicked, dataForDate)
    '    Else
    '        MessageBox.Show($"No data available for {dateClicked}.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    End If
    'End Sub


    Private Sub SaveDataToTextFile(dateClicked As String, data As List(Of String))

        Dim invalidChars As String = New String(Path.GetInvalidFileNameChars())
        Dim validFileName As String = String.Join("_", dateClicked.Split(invalidChars.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))

        Using saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "Text Files|*.txt"
            saveFileDialog.Title = "Save Data as Text File"
            saveFileDialog.FileName = $"Data_{validFileName}.txt"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Try

                    File.WriteAllLines(saveFileDialog.FileName, data)
                    MessageBox.Show("Data has been successfully downloaded!", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As IOException

                    MessageBox.Show($"Error writing file: {ex.Message}", "File Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If

        End Using
    End Sub


End Module
