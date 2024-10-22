Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing

Module ImageUploadModule

    ' ImageUploader Class
    Public Class ImageUploader

        Private Const ConnectionString As String = "Data Source=(localdb)\Deepak;Initial Catalog=SPC-LLM;Integrated Security=True;Encrypt=False"

        ' Method to upload image
        Public Sub UploadImage(sNo As Integer)
            Try
                ' Open File Dialog to select an image
                Dim openFileDialog As New OpenFileDialog()
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"

                If openFileDialog.ShowDialog() = DialogResult.OK Then
                    ' Get the selected image file path
                    Dim imagePath As String = openFileDialog.FileName

                    ' Convert the image file into a byte array
                    Dim imageBytes As Byte() = ConvertImageToByteArray(imagePath)

                    ' Save the image byte array into the database
                    SaveImageToDatabase(sNo, imageBytes)
                End If

            Catch ex As Exception
                MessageBox.Show($"Error uploading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ' Method to convert an image file to byte array
        Private Function ConvertImageToByteArray(imagePath As String) As Byte()
            Dim imgBytes As Byte()
            Using stream As New FileStream(imagePath, FileMode.Open, FileAccess.Read)
                Using br As New BinaryReader(stream)
                    imgBytes = br.ReadBytes(CInt(stream.Length))
                End Using
            End Using
            Return imgBytes
        End Function

        ' Method to save the byte array to the SQL Server database
        Private Sub SaveImageToDatabase(sNo As Integer, imageBytes As Byte())
            Try
                Using conn As New SqlConnection(ConnectionString)
                    Dim query As String = "UPDATE SPC SET PCBAImage = @PCBAImage WHERE SNo = @SNo"

                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@SNo", sNo)
                        cmd.Parameters.Add("@PCBAImage", SqlDbType.VarBinary).Value = If(imageBytes IsNot Nothing, imageBytes, DBNull.Value)

                        conn.Open()
                        Dim rowsAffected = cmd.ExecuteNonQuery()
                        MessageBox.Show($"{rowsAffected} row(s) updated with image.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show($"Error saving image to database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

    End Class

End Module
