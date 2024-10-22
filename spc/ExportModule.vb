Imports System.IO
Imports System.Windows.Forms
Module ExportModule
    Public Sub ExportToCSV(dataGridView As DataGridView)
        Using saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv"
            saveFileDialog.Title = "Save as CSV File"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Try
                    Using writer As New StreamWriter(saveFileDialog.FileName)
                        ' Write the header
                        For i As Integer = 0 To dataGridView.Columns.Count - 1
                            writer.Write(dataGridView.Columns(i).HeaderText)
                            If i < dataGridView.Columns.Count - 1 Then writer.Write(",")
                        Next
                        writer.WriteLine()

                        ' Write the data rows
                        For Each row As DataGridViewRow In dataGridView.Rows
                            If Not row.IsNewRow Then
                                For i As Integer = 0 To dataGridView.Columns.Count - 1
                                    writer.Write(row.Cells(i).Value.ToString())
                                    If i < dataGridView.Columns.Count - 1 Then writer.Write(",")
                                Next
                                writer.WriteLine()
                            End If
                        Next
                    End Using
                    MessageBox.Show("Data exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub
End Module
