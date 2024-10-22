Imports System.IO
Public Class BoardDetailForm


    Friend Sub LoadBoardDetails(pcbaImage() As Byte, boardDetails As DataTable)
        ' Ensure the DataTable contains data
        If boardDetails IsNot Nothing AndAlso boardDetails.Rows.Count > 0 Then
            ' Display full table data in DataGridView
            DataGridViewBoardDetails.DataSource = boardDetails

            ' Load and display the PCBA image, if available
            If pcbaImage IsNot Nothing AndAlso pcbaImage.Length > 0 Then
                Try
                    ' Ensure the correct MemoryStream is being used from System.IO
                    Using ms As New MemoryStream(pcbaImage)
                        PictureBoxPCBA.Image = Image.FromStream(ms)
                    End Using
                Catch ex As Exception
                    MessageBox.Show($"Error loading image: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    PictureBoxPCBA.Image = Nothing ' Optionally, set to default or clear image
                End Try
            Else
                ' Optionally display a placeholder image if no image is available
                PictureBoxPCBA.Image = Nothing

            End If
        Else
            MessageBox.Show("No details available for the selected board.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub


    Friend Sub LoadBoardDetailform(pcbaImage() As Byte, boardDetails As DataTable)
        Throw New NotImplementedException()
    End Sub
End Class
