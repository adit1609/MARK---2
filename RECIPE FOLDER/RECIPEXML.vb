Imports System.IO
Imports System.Xml

Public Class RECIPEXML
    Private Const defaultPath As String = "C:\Logs\Default\"

    ' Method to create an XML file with three elements
    Public Sub CreateRecipeXML(recipeName As String, length As String, width As String)
        ' Ensure the directory exists
        If Not Directory.Exists(defaultPath) Then
            Directory.CreateDirectory(defaultPath)
        End If

        ' Sanitize the recipe name to be a valid file name
        Dim safeFileName As String = String.Join("_", recipeName.Split(Path.GetInvalidFileNameChars()))
        Dim filePath As String = Path.Combine(defaultPath, $"{safeFileName}.xml")

        ' Check if the file already exists
        If File.Exists(filePath) Then
            MessageBox.Show($"The recipe '{recipeName}' already exists!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Create a new XML document
        Dim xmlDocument As New XmlDocument()

        ' Create the XML declaration and root element
        Dim xmlDeclaration As XmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
        xmlDocument.AppendChild(xmlDeclaration)

        Dim rootElement As XmlElement = xmlDocument.CreateElement("RecipeDetails")
        xmlDocument.AppendChild(rootElement)

        ' Create and append the Programe_Name element
        Dim nameElement As XmlElement = xmlDocument.CreateElement("Programe_Name")
        nameElement.InnerText = recipeName
        rootElement.AppendChild(nameElement)

        ' Create and append the Length element
        Dim lengthElement As XmlElement = xmlDocument.CreateElement("Length")
        lengthElement.InnerText = length
        rootElement.AppendChild(lengthElement)

        ' Create and append the Width element
        Dim widthElement As XmlElement = xmlDocument.CreateElement("Width")
        widthElement.InnerText = width
        rootElement.AppendChild(widthElement)

        Dim TotalMarking As XmlElement = xmlDocument.CreateElement("TotalMarking")
        TotalMarking.InnerText = "-"
        rootElement.AppendChild(TotalMarking)

        Dim TotalFid As XmlElement = xmlDocument.CreateElement("TotalFiducial")
        TotalFid.InnerText = "-"
        rootElement.AppendChild(TotalFid)


        ' Save the XML file
        xmlDocument.Save(filePath)

        ' Notify the user
        MessageBox.Show($"Recipe '{recipeName}' has been created.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class
