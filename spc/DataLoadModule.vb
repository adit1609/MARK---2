Imports System.Data.SqlClient
Imports System.Windows.Forms

Module DataLoadModule
    Private Const ConnectionString As String = "Data Source=(localdb)\Deepak;Initial Catalog=SPC-LLM;Integrated Security=True;Encrypt=False"

    Public Sub LoadData(dataGridView As DataGridView)
        Try
            Using conn As New SqlConnection(ConnectionString)
                Dim query As String = "SELECT * FROM SPC"
                Using cmd As New SqlCommand(query, conn)
                    conn.Open()
                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim table As New DataTable()
                    adapter.Fill(table)

                    dataGridView.DataSource = table
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error loading all data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub LoadData(dataGridView As DataGridView, startDate As DateTime, endDate As DateTime)
        Try
            Using conn As New SqlConnection(ConnectionString)
                Dim query As String = "SELECT * FROM SPC WHERE [DateOfMarkingCompletion] BETWEEN @StartDate AND @EndDate"
                Using cmd As New SqlCommand(query, conn)

                    cmd.Parameters.AddWithValue("@StartDate", startDate)
                    cmd.Parameters.AddWithValue("@EndDate", endDate)

                    conn.Open()

                    Dim adapter As New SqlDataAdapter(cmd)
                    Dim table As New DataTable()
                    adapter.Fill(table)


                    dataGridView.DataSource = table
                End Using
            End Using

        Catch ex As Exception
            MessageBox.Show($"Error loading filtered data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Module

