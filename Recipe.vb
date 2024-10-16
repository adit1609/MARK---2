Imports System.Windows.Forms
Imports System.Net.Mime.MediaTypeNames
Imports ActUtlTypeLib
Imports System.Xml
Imports System.IO
Imports System.Xml.Linq
Imports Guna.UI2.WinForms
Imports System.Configuration
Imports Gui_Tset.RecepieOperation
Imports System.ComponentModel
Imports System.Timers
Imports System.Text
Imports System.Security.Policy
Imports Microsoft.SqlServer
Imports System.Windows
Imports System.ComponentModel.Design
Imports System.Drawing.Drawing2D
Imports System.Xml.Schema
Imports Gui_Tset.My
Imports System.Runtime.InteropServices
Imports MvCamCtrl.NET
Imports Microsoft.NET.Sdk
Imports Emgu.CV
Imports System.Drawing.Imaging
Imports System.Text.RegularExpressions
Imports System.Runtime.Remoting.Lifetime
Imports System.Threading
Imports System.Runtime.InteropServices.ComTypes
Imports Emgu.CV.Features2D
Imports Emgu.CV.ML.KNearest
Imports System.Web.UI.WebControls.Expressions
Imports Emgu.CV.Flann



Public Class Recipe
    Dim plc As New ActUtlType
    Public Shared GRPATH As String = String.Empty
    ' Track the active button
    Dim fidpic As PictureBox
    Dim livepic As PictureBox
    Dim livepic1 As PictureBox
    Private isF1Active As Boolean = False
    Private isF2Active As Boolean = False
    Private isDrawing As Boolean = False
    Private startPoint As Point
    Private endPoint As Point
    Private shapes As New List(Of Shape)()

    Private currentID As Integer = 1
    Private currentColor As Color = Color.Blue
    Private drawSquares As Boolean = False
    Dim openFileDialog As New OpenFileDialog()
    Dim m_nBufSizeForDriver As UInt32 = 1000 * 1000 * 3
    Dim m_pBufForDriver(m_nBufSizeForDriver) As Byte
    Dim m_nBufSizeForDriver1 As UInt32 = 1000 * 1000 * 3
    Dim m_pBufForDriver1(m_nBufSizeForDriver1) As Byte
    Dim m_stDeviceInfoList As CCamera.MV_CC_DEVICE_INFO_LIST = New CCamera.MV_CC_DEVICE_INFO_LIST
    Dim m_nDeviceIndex As UInt32
    Dim m_stFrameInfoEx As CCamera.MV_FRAME_OUT_INFO_EX = New CCamera.MV_FRAME_OUT_INFO_EX()
    Private xposs As String
    Private yposs As String
    Private pythonProcess As Process

    Private Sub ValidateTextBox(tb As TextBox, ep As ErrorProvider)
        Dim input As String = tb.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ep.SetError(tb, "Please enter a valid numeric value.")
            Else
                ep.SetError(tb, String.Empty) ' Clear the error
            End If
        Else
            ep.SetError(tb, String.Empty) ' Clear the error if empty
        End If
    End Sub

    ' Generic validation method for ComboBoxes
    Private Sub ValidateComboBox(comboBox As ComboBox, e As CancelEventArgs)
        If comboBox.SelectedIndex = -1 Then
            ErrorProvider1.SetError(comboBox, "Please select a valid option.")
            e.Cancel = True
        Else
            ErrorProvider1.SetError(comboBox, String.Empty) ' Clear the error
        End If
    End Sub



    Dim ListXMLPath As List(Of String) = New List(Of String)()
    Private Function ToggleButtonColor(button As Button) As Boolean
        Static originalBackColor As New Dictionary(Of Button, Color)

        If originalBackColor.ContainsKey(button) Then
            If button.BackColor = Color.Green Then
                button.BackColor = originalBackColor(button)
                Return False
            Else
                button.BackColor = Color.Green
                Return True
            End If
        Else
            originalBackColor.Add(button, button.BackColor)
            button.BackColor = Color.Green
            Return True
        End If
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
    Private Sub Panel5_Paint(sender As Object, e As PaintEventArgs)

    End Sub
    Private WithEvents m251Timer As System.Timers.Timer
    Dim isMonitoringM251 As Boolean = False
    Dim previousM251State As Integer = 0

    Private Sub Recipe_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Guna2CircleButton1.BackColor = normal
        plc.ActLogicalStationNumber = 1
        plc.Open()
        Timer1.Start()
        PictureBox1.BackColor = Color.Red
        PictureBox2.BackColor = Color.Red


        rtxtcurrentpg.Text = My.Settings.ProgramName

        livepic = PictureBox8
        livepic1 = PictureBox3

        LoadReceipe()
        loadData()
        DATAGRID.MultiSelect = False
        DATAGRID.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Timer2.Start()
        Timer3.Start()
        Dim nRet As Int32
        AddHandler rtxtcurrentpg.TextChanged, AddressOf rtxtcurrentpg_TextChanged
        Panel27.Hide()
        Panel26.Hide()
        RadioButtonTriggerOff.Enabled = True
        RadioButtonTriggerOn.Checked = True
        RadioButtonTriggerOn.Enabled = False
        RadioButtonTriggerOff.Checked = False
        ButtonSoftwareOnce.Enabled = True
        nRet = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
        nRet = Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)
        LiveTriggerOff.Enabled = True
        LiveTriggerOn.Checked = True
        LiveTriggerOn.Enabled = False
        LiveTriggerOff.Checked = False
        LiveTriggerOnce.Enabled = True
        nRet = Home_Page.LiveCamera1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
        nRet = Home_Page.LiveCamera1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)


        'BackgroundWorker1.RunWorkerAsync()
        'BackgroundWorker2.RunWorkerAsync()
        'BackgroundWorker3.RunWorkerAsync()



    End Sub

    Private Sub bt_Loadpos_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub btn_Widthadj_Click(sender As Object, e As EventArgs)
        ToggleButtonColor(btn_Widthadj)
        Dim floatValue As Single
        If Single.TryParse(txt_p_wed.Text, floatValue) Then
            ' Round the float value to two decimal places
            Dim roundedValue As Single = Math.Round(floatValue, 3)

            ' Convert the rounded float value to two 16-bit integers
            Dim words() As Integer = ConvertFloatToWord(roundedValue)

            ' Example: Write to different data registers
            plc.SetDevice("D320", words(0))
            plc.SetDevice("D321", words(1))
        Else

        End If
    End Sub



    Private Sub btLoadpos_MouseUp(sender As Object, e As MouseEventArgs) Handles btLoadpos.MouseUp
        plc.SetDevice("M252", 0)
    End Sub

    Private Sub btLoadpos_MouseDown(sender As Object, e As MouseEventArgs) Handles btLoadpos.MouseDown
        plc.SetDevice("M252", 1)
    End Sub

    Private Sub btUnloadpos_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M232", 0)

    End Sub

    Private Sub btUnloadpos_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M232", 1)

    End Sub





    Private Sub btPcbunload_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M234", 0)

    End Sub

    Private Sub btPcbunload_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M234", 1)
    End Sub




    Private Sub btPcbstopper_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M237", 0)
    End Sub

    Private Sub btPcbstopper_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M237", 1)
    End Sub

    Private Sub btServoon_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M238", 0)
    End Sub

    Private Sub btServoon_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M238", 1)
    End Sub

    Private Sub btPcbclamp_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Pcbclamp.MouseUp
        plc.SetDevice("M239", 0)
    End Sub

    Private Sub btPcbclamp_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Pcbclamp.MouseDown
        plc.SetDevice("M239", 1)
    End Sub

    Private Sub bt_Move_Click(sender As Object, e As EventArgs)

    End Sub
    Private Sub bt_Gateopenl_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Gateopenl.MouseDown
        plc.SetDevice("M235", 1)
    End Sub



    Private Sub bt_Gateopenl_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Gateopenl.MouseUp
        plc.SetDevice("M235", 0)
    End Sub



    Private Sub bt_Gateopenr_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Gateopenr.MouseDown
        plc.SetDevice("M236", 1)


    End Sub

    Private Sub bt_Gateopenr_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Gateopenr.MouseUp
        plc.SetDevice("M236", 0)

    End Sub

    Private Sub bt_Pcbload_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Pcbload.MouseDown
        plc.SetDevice("M233", 1)

    End Sub

    Private Sub bt_Pcbload_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Pcbload.MouseUp
        plc.SetDevice("M233", 0)
    End Sub

    Private Sub bt_Pcbunload_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Pcbunload.MouseDown
        plc.SetDevice("M234", 1)

    End Sub

    Private Sub bt_Pcbunload_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Pcbunload.MouseUp
        plc.SetDevice("M234", 0)
    End Sub

    Private Sub bt_Unclamp_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Pcbunclamp.MouseDown
        plc.SetDevice("M248", 1)

    End Sub

    Private Sub bt_Pcbunclamp_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Pcbunclamp.MouseUp
        plc.SetDevice("M248", 0)
    End Sub



    Private Sub bt_Teachpos_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M244", 1)
    End Sub

    Private Sub bt_Teachpos_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M244", 0)
    End Sub

    Private Sub bt_Move_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M243", 1)
    End Sub

    Private Sub bt_Move_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M243", 0)
    End Sub

    Private Sub bt_Learnshape_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M245", 0)
    End Sub

    Private Sub bt_Learnshape_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M245", 1)
    End Sub

    Private Sub bt_Save_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M246", 1)
    End Sub

    Private Sub bt_Save_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M246", 1)
    End Sub

    Private Sub bt_Test_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M247", 1)
    End Sub

    Private Sub bt_Test_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M247", 0)
    End Sub

    Private Sub RadioButton3_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M238", 1)
        PictureBox1.BackColor = Color.Green

    End Sub

    Private Sub btservo_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M238", 0)
        PictureBox1.BackColor = Color.Red
    End Sub

    Private Sub btn_Widthadj_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M240", 0)
    End Sub

    Private Sub btn_Widthadj_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M240", 1)
    End Sub

    Private Sub btn_Trackmov_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M241", 1)
    End Sub

    Private Sub btn_Trackmov_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M240", 0)
    End Sub

    Private Sub btn_Array_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M242", 1)
    End Sub

    Private Sub btn_Array_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M242", 1)
    End Sub

    Private Sub HOMEPOS_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M231", 1)
        PictureBox2.BackColor = Color.Green
    End Sub

    Private Sub HOMEPOS_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M231", 1)
        PictureBox2.BackColor = Color.Red
    End Sub



    Private Sub txt_p_wed_TextChanged(sender As Object, e As EventArgs)
        Validatetxt_p_wed()
    End Sub





    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click


        Dim progname As String
        progname = InputBox("PLEASE ENTER PROGRAM NAME", MessageBoxButtons.OKCancel)



        If progname = "" Then
            Return

        ElseIf (progname IsNot "") Then
            Dim folderPath As String = "C:\Manage Files\Fid_Image\" & progname


            Try
                If Not Directory.Exists(folderPath) Then
                    Directory.CreateDirectory(folderPath)
                    MessageBox.Show("Folder created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    MessageBox.Show("Folder already exists.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Catch ex As Exception
                MessageBox.Show("An error occurred while creating the folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            Dim isValid1 As Boolean = True



            Dim isValid As Boolean = True



            Dim side As String

            side = cmbpanelside.SelectedText
            If side = "" Then
                cmbpanelside.SelectedIndex = 0
                side = cmbpanelside.Text
            End If
            txt_p_len.Text = 0.00
            txt_p_wed.Text = 0.00
            thk.Text = 0.00
            txt_p_weight.Text = 0.00
            txt_p_wed.Text = 0

            UD_R_count.Text = 0
            UD_R_pitch.Text = 0
            UD_C_count.Text = 0
            UD_C_pitch.Text = 0
            txt_Mark_Id.Text = 0



            Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
            Dim Backupfname As String = "" & ConfigurationManager.AppSettings("BackupReceipeFilepath").ToString().Trim()

            '  Dim XMLOutPutPath As String = "D:\LM- Test/"

            Dim path As String = fname
            Dim Logdir As String = "" & fname
            Dim BackupLogdir As String = "" & Backupfname
            Dim ReceipeFileName As String = String.Empty

            If Not Directory.Exists(BackupLogdir) Then
                Directory.CreateDirectory(BackupLogdir)
            End If
            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If


            ReceipeFileName = progname

            Dim generatedFile As String

            Dim generatedFile1 As String = Logdir & ReceipeFileName & ".xml"
            Dim BaackupgeneratedFile As String = BackupLogdir & ReceipeFileName & ".xml"
            Dim nam As String = ReceipeFileName & ".xml"
            If Not File.Exists(generatedFile1) Then
                generatedFile = Logdir & ReceipeFileName & ".xml"

            Else
                Dim _LaserHeadLocation As String() = Directory.GetFiles(Logdir, nam, System.IO.SearchOption.AllDirectories)
                Dim count As Integer = _LaserHeadLocation.Count()
                ReceipeFileName = progname & "_" & count + 1
                generatedFile = Logdir & ReceipeFileName & ".xml"
            End If





            Try



                Dim xdoc As XDocument = XDocument.Parse("<JOBList></JOBList>")
                Dim contacts As XElement = New XElement("JOB",
                                                            New XElement("TAGTYPE",
                                                                         New XAttribute("NAME", "RECEIPE"),
                                                                         New XElement("RECEIPE", New XAttribute("NO", "0"),
                                                                                      New XElement("BOARD", New XElement("BOARDNAME", "xxxxxxxxxx"),
                                                                                                   New XElement("P_LENGTH", txt_p_len.Text.ToString()),
                                                                                                   New XElement("P_WIDTH", txt_p_wed.Text),
                                                                                                   New XElement("P_THK", thk.Text),
                                                                                                   New XElement("P_WEIGHT", txt_p_weight.Text),
                                                                                                   New XElement("WIDTH", txt_p_wed.Text),
                                                                                                   New XElement("MARGIN", ""),
                                                                                                   New XElement("ROWCOUNT", UD_R_count.Text),
                                                                                                   New XElement("ROWPITCH", UD_R_pitch.Text),
                                                                                                   New XElement("COULMNCOUNT", UD_C_count.Text),
                                                                                                   New XElement("COULMNPITCH", UD_C_pitch.Text),
                                                                                                   New XElement("MARKID", txt_Mark_Id.Text))),
                                                                          New XElement("SIDE",
                                                                                                   New XElement("SIDES", cmbpanelside.Text)),
                                                                         New XElement("LASTSAVINGTIME", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")),
                                                                         New XElement("RECEIPENAME", New XAttribute("NAME", "RECIEPE"),
                                                                                                  New XElement("RNAME", ReceipeFileName), New XElement("RPATH", generatedFile)),
                                                             New XElement("MARKPOSITION"),
                                                             New XElement("FIDUCIAL")),
                    New XElement("PrevProg", generatedFile))
                xdoc.Root.Add(contacts)




                xdoc.Save(generatedFile)
                xdoc.Save(BaackupgeneratedFile)



                Dim Lfname As String = "" & ConfigurationManager.AppSettings("Success_Logs").ToString().Trim()


                Dim Lpath As String = Lfname
                Dim LLogdir As String = "" & Lfname

                Dim LReceipeFileName As String = String.Empty
                If Not Directory.Exists(LLogdir) Then
                    Directory.CreateDirectory(LLogdir)
                End If




                Dim LgeneratedFile1 As String = LLogdir & ReceipeFileName & ".xml"








                Dim xdoc1 = XDocument.Parse("<SuccessLogs></SuccessLogs>")
                Dim contacts1 As XElement = New XElement("Error",
                                                            New XElement("FileName", generatedFile),
                                                            New XElement("DateTime", DateTime.Now().ToString()),
                                                            New XElement("UserName", ""),
                                                             New XElement("PageName", "Recipe"),
                                                            New XElement("Path", generatedFile),
                                                            New XElement("Message", "Recipe added successfully"))












                xdoc1.Root.Add(contacts1)
                xdoc1.Save(LgeneratedFile1)





                LoadReceipe()


            Catch ex As Exception



                Dim Lfname As String = "" & ConfigurationManager.AppSettings("Success_Logs").ToString().Trim()



                Dim Lpath As String = Lfname
                Dim LLogdir As String = "" & Lfname

                Dim LReceipeFileName As String = String.Empty
                If Not Directory.Exists(LLogdir) Then
                    Directory.CreateDirectory(LLogdir)
                End If




                Dim LgeneratedFile1 As String = LLogdir & ReceipeFileName & ".xml"








                Dim xdoc1 = XDocument.Parse("<Error_Logs></Error_Logs>")
                Dim contacts1 As XElement = New XElement("Error",
                                                            New XElement("FileName", generatedFile),
                                                            New XElement("DateTime", DateTime.Now().ToString()),
                                                            New XElement("UserName", ""),
                                                             New XElement("PageName", "Recipe"),
                                                            New XElement("Path", generatedFile),
                                                            New XElement("Message", "Recipe added successfully"))
                xdoc1.Root.Add(contacts1)
                xdoc1.Save(LgeneratedFile1)


                MessageBox.Show("An error occurred while saving the file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub LoadReceipe()

        'load xml and read xml and display on grid view
        Dim dtRecipetable As DataTable = New DataTable()
        dtRecipetable.Clear()
        dtRecipetable.Clone()
        dtRecipetable.TableName = "dtRecipetable"
        dtRecipetable.Columns.Add("S_NO", GetType(Int32))
        dtRecipetable.Columns.Add("pname", GetType(String))
        dtRecipetable.Columns.Add("cwidht", GetType(String))
        dtRecipetable.Columns.Add("date_time", GetType(String))
        dtRecipetable.Columns.Add("x", GetType(String))
        dtRecipetable.Columns.Add("y", GetType(String))
        dtRecipetable.Columns.Add("RPATH", GetType(String))

        Dim countRecipe As Integer = 0

        Dim ReceipeFilepath As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
        Dim _PrgLogPath As String() = Directory.GetFiles(ReceipeFilepath, "*.xml", System.IO.SearchOption.AllDirectories)
        Dim recipename As String = String.Empty
        Dim RPATH As String = String.Empty
        Dim LASTSAVINGTIME As String = String.Empty
        Dim ITEMCOUNT As Int32 = _PrgLogPath.Count()
        If ITEMCOUNT > 0 Then
            For Each item In _PrgLogPath
                Dim fs As FileStream = New FileStream(item, FileMode.Open)
                Dim sr As StreamReader = New StreamReader(fs)
                Dim s As String = sr.ReadToEnd()
                sr.Close()
                fs.Close()
                Dim xmDocument As XmlDocument = New XmlDocument()
                xmDocument.Load(item)
                Dim selectedoperationnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/SIDE")
                Dim RECEIPENAMEnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/RECEIPENAME")
                Dim BOARDnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/RECEIPE/BOARD")






                For Each _RECEIPENAMEnodes As XmlNode In RECEIPENAMEnodes
                    recipename = _RECEIPENAMEnodes.ChildNodes(0).InnerText
                    RPATH = _RECEIPENAMEnodes.ChildNodes(1).InnerText
                Next

                countRecipe = countRecipe + 1



                Dim P_Lenghth As String = String.Empty
                Dim P_with As String = String.Empty
                Dim P_thik As String = String.Empty
                For Each BOARD As XmlNode In BOARDnodes
                    P_Lenghth = BOARD.ChildNodes(0).InnerText
                    P_with = BOARD.ChildNodes(1).InnerText
                    P_thik = BOARD.ChildNodes(2).InnerText


                Next


                Dim Xvalue As String = String.Empty
                Dim Yvalue As String = String.Empty

                dtRecipetable.Rows.Add(countRecipe, recipename, P_with, LASTSAVINGTIME, Xvalue, Yvalue, RPATH)
                DATAGRID.DataSource = dtRecipetable

            Next





        Else
            dtRecipetable.Clear()

            DATAGRID.DataSource = dtRecipetable

        End If

    End Sub






    Private Sub txt_p_len_TextChanged(sender As Object, e As EventArgs)
        ValidateTxt_p_len()
    End Sub
    Public Function GetSideName(ByVal val As String) As String
        Dim name As String = String.Empty

        Select Case val
            Case "1"
                name = "TOP"
                Return name
            Case "2"
                name = "BOT"
                Return name
        End Select

        Return name
    End Function
    Public Function GetShapeName(ByVal val As String) As String
        Dim name As String = String.Empty

        Select Case val
            Case "0"
                name = "Circle"
                Return name
            Case "1"
                name = "Rectangle"
                Return name
            Case "2"
                name = "Genric"
                Return name
        End Select

        Return name
    End Function
    Public Function GetColorName(ByVal val As String) As String
        Dim name As String = String.Empty

        Select Case val
            Case "0"
                name = "RED"
                Return name
            Case "1"
                name = "BLUE"
                Return name
            Case "2"
                name = "GREEN"
                Return name
        End Select

        Return name
    End Function

    Private Sub Button5_Click_1(sender As Object, e As EventArgs)

        Try

            If ListXMLPath.Count > 0 Then

                If Not String.IsNullOrEmpty(GRPATH) Then
                    Dim path As String = GRPATH
                    Dim Panel As New Panel
                    Panel.Setb = path
                    Panel.Show()

                    ' create success or process logs



                Else
                    MessageBox.Show("Please select correct recipe!")
                End If
            Else
                MessageBox.Show("Please select correct recipe!")
            End If

        Catch ex As Exception


            '




        End Try



    End Sub
    Private isCellClicked As Boolean = False
    Private Sub DATAGRID_CellClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DATAGRID.CellClick
        shapes.Clear()
        If e.RowIndex >= 0 Then
            isCellClicked = True

            ' Deselect any currently selected rows
            For Each row As DataGridViewRow In DATAGRID.SelectedRows
                row.DefaultCellStyle.Font = DATAGRID.DefaultCellStyle.Font
            Next

            ' Select the new row
            Dim selectedRow As DataGridViewRow = DATAGRID.Rows(e.RowIndex)
            selectedRow.Selected = True
            Dim style As New DataGridViewCellStyle
            style.BackColor = Color.LightGray
            style.Font = New System.Drawing.Font("Tahoma", 8)
            selectedRow.DefaultCellStyle = style
            DATAGRID.MultiSelect = False
            DATAGRID.SelectionMode = DataGridViewSelectionMode.FullRowSelect

            If DATAGRID.Rows.Count > 0 AndAlso DATAGRID.Rows(0) IsNot Nothing Then





                Dim _RecepieOperation As RecepieOperation = New RecepieOperation()
                If e.RowIndex >= 0 Then
                    Dim row As DataGridViewRow = Me.DATAGRID.Rows(e.RowIndex)
                    Dim filename As String = row.Cells("RPATH").Value.ToString()
                    GRPATH = row.Cells("RPATH").Value.ToString()
                    'Dim row As DataGridViewRow = Me.DataGridView1.Rows(e.RowIndex)
                    row.DefaultCellStyle.BackColor = Color.LightGray
                    row.DefaultCellStyle.Font = New System.Drawing.Font("Tahoma", 8)

                    ' Adding array for delete path
                    ListXMLPath.Add(GRPATH)

                    ' Add file name into panel name textbox
                    Dim pname As String = Path.GetFileNameWithoutExtension(GRPATH)
                    txt_Sel_Prog_name.Text = pname
                    'rtxtcurrentpg.Text = pname





                    ' Dim FileName As String = Path.GetFileNameWithoutExtension(item)
                    Dim fs As FileStream = New FileStream(filename, FileMode.Open)
                    Dim sr As StreamReader = New StreamReader(fs)
                    Dim s As String = sr.ReadToEnd()
                    sr.Close()
                    fs.Close()

                    Dim xmDocument1 As XDocument = XDocument.Load(filename)
                    Dim xmDocument As XmlDocument = New XmlDocument()
                    xmDocument.Load(filename)
                    DataGridView1.Rows.Clear()

                    ' Iterate through the XML elements and populate the DataGridView
                    For Each rowElement As XmlNode In xmDocument.SelectNodes("//Row")
                        Dim newRow As DataGridViewRow = DataGridView1.Rows(DataGridView1.Rows.Add())
                        Dim cellIndex As Integer = 0
                        For Each cellElement As XmlNode In rowElement.ChildNodes
                            newRow.Cells(cellIndex).Value = cellElement.InnerText
                            cellIndex += 1
                        Next
                    Next

                    Dim Boardnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/RECEIPE/BOARD")

                    For Each Feeder As XmlNode In Boardnodes
                        Dim BOARDNAME As String = Feeder.ChildNodes(0).InnerText
                        Dim P_LENGTH As String = Feeder.ChildNodes(1).InnerText
                        Dim P_WIDTH As String = Feeder.ChildNodes(2).InnerText
                        Dim P_THK As String = Feeder.ChildNodes(3).InnerText
                        Dim P_WEIGHT As String = Feeder.ChildNodes(4).InnerText
                        Dim WIDTH As String = Feeder.ChildNodes(5).InnerText
                        Dim MARGIN As String = Feeder.ChildNodes(6).InnerText
                        Dim ROWCOUNT As String = Feeder.ChildNodes(7).InnerText
                        Dim ROWPITCH As String = Feeder.ChildNodes(8).InnerText
                        Dim COULMNCOUNT As String = Feeder.ChildNodes(9).InnerText
                        Dim COULMNPITCH As String = Feeder.ChildNodes(10).InnerText
                        Dim markid As String = Feeder.ChildNodes(11).InnerText

                        Dim BOARD As BOARD = New BOARD()
                        BOARD.BOARDNAME = BOARDNAME
                        BOARD.P_LENGTH = P_LENGTH
                        BOARD.P_WIDTH = P_WIDTH
                        BOARD.P_THK = P_THK
                        BOARD.P_WEIGHT = P_WEIGHT
                        BOARD.WIDTH = WIDTH
                        BOARD.MARGIN = MARGIN
                        BOARD.ROWCOUNT = ROWCOUNT
                        BOARD.ROWPITCH = ROWPITCH
                        BOARD.COULMNCOUNT = COULMNCOUNT
                        BOARD.COULMNPITCH = COULMNPITCH
                        _RecepieOperation._RECIPIEDETAILS._BOARD = BOARD


                        txt_p_len.Text = P_LENGTH
                        txt_p_wed.Text = P_WIDTH
                        thk.Text = P_THK
                        txt_p_weight.Text = P_WEIGHT
                        UD_R_count.Text = ROWCOUNT
                        UD_R_pitch.Text = ROWPITCH

                        UD_C_count.Text = COULMNCOUNT
                        UD_C_pitch.Text = COULMNPITCH
                        txt_Mark_Id.Text = markid

                        Dim SIDEnodes As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/SIDE")
                        Dim FRONT As String

                        FRONT = SIDEnodes.ChildNodes(0).InnerText
                        cmbpanelside.Text = FRONT
                        Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/MARKPOSITION")
                        Dim Nodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")
                        ' Dim i As Integer
                        For Each Node As XmlNode In Nodes

                            Dim xvalue As String = Node.ChildNodes(0).InnerText
                            'Cons'ole.WriteLine(Node.ChildNodes(0).InnerText)

                            'Dim 'xvalue As String = Node.PreviousSibling.InnerText
                            Dim yvalue As String = Node.ChildNodes(1).InnerText
                            'Dim rowIndex As Integer = DataGridView1.Rows.Add()
                            Dim che As Boolean = Node.ChildNodes(2).InnerText
                            Dim rowIndex As Integer = DataGridView1.Rows.Add()
                            DataGridView1.Rows(rowIndex).Cells(0).Value = xvalue ' Column 2
                            DataGridView1.Rows(rowIndex).Cells(1).Value = yvalue
                            DataGridView1.Rows(rowIndex).Cells(2).Value = che

                        Next








                        Dim RECEIPENAMEnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/RECEIPENAME")
                        For Each _PLCTAGnodes As XmlNode In RECEIPENAMEnodes
                            Dim RNAME As String = _PLCTAGnodes.ChildNodes(0).InnerText
                            Dim RPATH As String = _PLCTAGnodes.ChildNodes(1).InnerText

                        Next


                        Dim FIDUCIAL As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/FIDUCIAL")
                        Dim FNodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/FIDUCIAL/F")
                        datagrdFid.Rows.Clear()
                        shapes.Clear()
                        For Each Node1 As XmlNode In FNodes

                            Dim VALUE(20) As String
                            VALUE(0) = Node1.ChildNodes(0).InnerText
                            VALUE(1) = Node1.ChildNodes(1).InnerText
                            VALUE(2) = Node1.ChildNodes(2).InnerText
                            VALUE(3) = Node1.ChildNodes(3).InnerText
                            VALUE(4) = Node1.ChildNodes(4).InnerText
                            VALUE(5) = Node1.ChildNodes(5).InnerText
                            VALUE(6) = Node1.ChildNodes(6).InnerText
                            VALUE(7) = Node1.ChildNodes(7).InnerText
                            VALUE(8) = Node1.ChildNodes(8).InnerText
                            VALUE(9) = Node1.ChildNodes(9).InnerText
                            VALUE(10) = Node1.ChildNodes(10).InnerText
                            VALUE(11) = Node1.ChildNodes(11).InnerText
                            VALUE(12) = Node1.ChildNodes(12).InnerText
                            VALUE(13) = Node1.ChildNodes(13).InnerText
                            VALUE(14) = Node1.ChildNodes(14).InnerText
                            VALUE(15) = Node1.ChildNodes(15).InnerText

                            Dim rowIndex1 As Integer = datagrdFid.Rows.Add()



                            datagrdFid.Rows(rowIndex1).Cells(0).Value = VALUE(0)
                            datagrdFid.Rows(rowIndex1).Cells(1).Value = VALUE(1)
                            datagrdFid.Rows(rowIndex1).Cells(2).Value = VALUE(2)
                            datagrdFid.Rows(rowIndex1).Cells(3).Value = VALUE(3)
                            datagrdFid.Rows(rowIndex1).Cells(4).Value = VALUE(4)
                            datagrdFid.Rows(rowIndex1).Cells(5).Value = VALUE(5)
                            datagrdFid.Rows(rowIndex1).Cells(6).Value = VALUE(6)
                            datagrdFid.Rows(rowIndex1).Cells(7).Value = VALUE(7)
                            datagrdFid.Rows(rowIndex1).Cells(8).Value = VALUE(8)
                            datagrdFid.Rows(rowIndex1).Cells(9).Value = VALUE(9)
                            datagrdFid.Rows(rowIndex1).Cells(10).Value = VALUE(10)
                            datagrdFid.Rows(rowIndex1).Cells(11).Value = VALUE(11)
                            datagrdFid.Rows(rowIndex1).Cells(12).Value = VALUE(12)
                            datagrdFid.Rows(rowIndex1).Cells(13).Value = VALUE(13)
                            datagrdFid.Rows(rowIndex1).Cells(14).Value = VALUE(14)
                            datagrdFid.Rows(rowIndex1).Cells(15).Value = VALUE(15)

                            currentID = rowIndex1 + 1

                            If VALUE(1) = "Square" Then
                                ' Draw square as before
                                Dim size As New Size(Convert.ToInt16(VALUE(4)), Convert.ToInt16(VALUE(5)))
                                Dim topLeft As New Point(Convert.ToInt16(VALUE(2)), Convert.ToInt16(VALUE(3)))
                                shapes.Add(New Square(topLeft, size, currentID, currentColor))

                            ElseIf VALUE(1) = "Circle" Then
                                ' Draw circle with dynamic size

                                Dim parts() As String = VALUE(6).Trim("()").Split(","c)


                                Dim trimmedPart() As String = parts(1).Split(")"c)
                                Dim centrx As String = parts(0)
                                Dim centey As String = trimmedPart(0)
                                Dim radius As Integer = CInt(Convert.ToInt16(VALUE(4)))
                                Dim centerX As Integer = Convert.ToInt16(centrx)
                                Dim centerY As Integer = Convert.ToInt16(centey)
                                shapes.Add(New Circle(New Point(centerX, centerY), radius / 2, currentID, currentColor))


                            End If


                        Next




                        'Next

                    Next

                End If
            End If
        Else
            MessageBox.Show("Please create recipe !.")
        End If
    End Sub
    Private Sub loadData()

        Dim progname As String
        progname = "Defualt"
        If progname = "" Then
            Return
        ElseIf (progname IsNot "") Then

            Dim isValid1 As Boolean = True
            Dim isValid As Boolean = True
            Dim fname As String = "" & ConfigurationManager.AppSettings("DefaultPath").ToString().Trim()

            Dim path As String = fname
            Dim Logdir As String = "" & fname

            Dim ReceipeFileName As String = String.Empty

            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If
            ReceipeFileName = progname
            Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
            Dim file1 As String = ReceipeFileName & ".xml"
            Dim side As String = cmbpanelside.Text

            'Dim files As String = Directory.GetFiles(Logdir, file1, System.IO.SearchOption.AllDirectories)
            Dim check As Boolean = File.Exists(generatedFile)

            If check = False Then
                MySettings.Default.ProgramName = ""


                Return
            End If
            txt_Sel_Prog_name.Text = MySettings.Default.ProgramName
            Dim xmDocument As XmlDocument = New XmlDocument()
            xmDocument.Load(generatedFile)
            Dim Boardnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/RECEIPE/BOARD")

            For Each Feeder As XmlNode In Boardnodes
                Dim BOARDNAME As String = Feeder.ChildNodes(0).InnerText
                Dim P_LENGTH As String = Feeder.ChildNodes(1).InnerText
                Dim P_WIDTH As String = Feeder.ChildNodes(2).InnerText
                Dim P_THK As String = Feeder.ChildNodes(3).InnerText
                Dim P_WEIGHT As String = Feeder.ChildNodes(4).InnerText
                Dim WIDTH As String = Feeder.ChildNodes(5).InnerText
                Dim MARGIN As String = Feeder.ChildNodes(6).InnerText
                Dim ROWCOUNT As String = Feeder.ChildNodes(7).InnerText
                Dim ROWPITCH As String = Feeder.ChildNodes(8).InnerText
                Dim COULMNCOUNT As String = Feeder.ChildNodes(9).InnerText
                Dim COULMNPITCH As String = Feeder.ChildNodes(10).InnerText
                Dim markid As String = Feeder.ChildNodes(11).InnerText

                Dim BOARD As BOARD = New BOARD()
                BOARD.BOARDNAME = BOARDNAME
                BOARD.P_LENGTH = P_LENGTH
                BOARD.P_WIDTH = P_WIDTH
                BOARD.P_THK = P_THK
                BOARD.P_WEIGHT = P_WEIGHT
                BOARD.WIDTH = WIDTH
                BOARD.MARGIN = MARGIN
                BOARD.ROWCOUNT = ROWCOUNT
                BOARD.ROWPITCH = ROWPITCH
                BOARD.COULMNCOUNT = COULMNCOUNT
                BOARD.COULMNPITCH = COULMNPITCH



                txt_p_len.Text = P_LENGTH
                txt_p_wed.Text = P_WIDTH
                thk.Text = P_THK
                txt_p_weight.Text = P_WEIGHT
                UD_R_count.Text = ROWCOUNT
                UD_R_pitch.Text = ROWPITCH

                UD_C_count.Text = COULMNCOUNT
                UD_C_pitch.Text = COULMNPITCH
                txt_Mark_Id.Text = markid

                Dim SIDEnodes As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/SIDE")
                Dim FRONT As String

                FRONT = SIDEnodes.ChildNodes(0).InnerText



                cmbpanelside.Text = FRONT
                Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/MARKPOSITION")
                Dim Nodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")

                For Each Node As XmlNode In Nodes

                    Dim xvalue As String = Node.ChildNodes(0).InnerText
                    'Cons'ole.WriteLine(Node.ChildNodes(0).InnerText)

                    'Dim 'xvalue As String = Node.PreviousSibling.InnerText
                    Dim yvalue As String = Node.ChildNodes(1).InnerText
                    'Dim rowIndex As Integer = DataGridView1.Rows.Add()
                    Dim che As Boolean = Node.ChildNodes(2).InnerText
                    Dim rowIndex As Integer = DataGridView1.Rows.Add()
                    DataGridView1.Rows(rowIndex).Cells(0).Value = xvalue ' Column 2
                    DataGridView1.Rows(rowIndex).Cells(1).Value = yvalue
                    DataGridView1.Rows(rowIndex).Cells(2).Value = che

                Next



                Dim RECEIPENAMEnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/RECEIPENAME")
                For Each _PLCTAGnodes As XmlNode In RECEIPENAMEnodes
                    Dim RNAME As String = _PLCTAGnodes.ChildNodes(0).InnerText
                    Dim RPATH As String = _PLCTAGnodes.ChildNodes(1).InnerText

                Next







                'Next

            Next

            Dim FIDUCIAL As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/FIDUCIAL")
            Dim FNodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/FIDUCIAL/F")
            datagrdFid.Rows.Clear()
            shapes.Clear()
            For Each Node As XmlNode In FNodes
                Dim VALUE(20) As String
                VALUE(0) = Node.ChildNodes(0).InnerText
                VALUE(1) = Node.ChildNodes(1).InnerText
                VALUE(2) = Node.ChildNodes(2).InnerText
                VALUE(3) = Node.ChildNodes(3).InnerText
                VALUE(4) = Node.ChildNodes(4).InnerText
                VALUE(5) = Node.ChildNodes(5).InnerText
                VALUE(6) = Node.ChildNodes(6).InnerText
                VALUE(7) = Node.ChildNodes(7).InnerText
                VALUE(8) = Node.ChildNodes(8).InnerText
                VALUE(9) = Node.ChildNodes(9).InnerText
                VALUE(10) = Node.ChildNodes(10).InnerText
                VALUE(11) = Node.ChildNodes(11).InnerText
                VALUE(12) = Node.ChildNodes(12).InnerText
                VALUE(13) = Node.ChildNodes(13).InnerText
                VALUE(14) = Node.ChildNodes(14).InnerText
                VALUE(15) = Node.ChildNodes(15).InnerText





                Dim rowIndex As Integer = datagrdFid.Rows.Add()



                datagrdFid.Rows(rowIndex).Cells(0).Value = VALUE(0)
                datagrdFid.Rows(rowIndex).Cells(1).Value = VALUE(1)
                datagrdFid.Rows(rowIndex).Cells(2).Value = VALUE(2)
                datagrdFid.Rows(rowIndex).Cells(3).Value = VALUE(3)
                datagrdFid.Rows(rowIndex).Cells(4).Value = VALUE(4)
                datagrdFid.Rows(rowIndex).Cells(5).Value = VALUE(5)
                datagrdFid.Rows(rowIndex).Cells(6).Value = VALUE(6)
                datagrdFid.Rows(rowIndex).Cells(7).Value = VALUE(7)
                datagrdFid.Rows(rowIndex).Cells(8).Value = VALUE(8)
                datagrdFid.Rows(rowIndex).Cells(9).Value = VALUE(9)
                datagrdFid.Rows(rowIndex).Cells(10).Value = VALUE(10)
                datagrdFid.Rows(rowIndex).Cells(11).Value = VALUE(11)
                datagrdFid.Rows(rowIndex).Cells(12).Value = VALUE(12)
                datagrdFid.Rows(rowIndex).Cells(13).Value = VALUE(13)
                datagrdFid.Rows(rowIndex).Cells(14).Value = VALUE(14)
                datagrdFid.Rows(rowIndex).Cells(15).Value = VALUE(15)


                currentID = rowIndex + 1


                If VALUE(1) = "Square" Then
                    ' Draw square as before
                    Dim size As New Size(Convert.ToInt16(VALUE(4)), Convert.ToInt16(VALUE(5)))
                    Dim topLeft As New Point(Convert.ToInt16(VALUE(2)), Convert.ToInt16(VALUE(3)))
                    shapes.Add(New Square(topLeft, size, currentID, currentColor))

                ElseIf VALUE(1) = "Circle" Then
                    Dim cnter(2) As String

                    Dim AX(2) As String

                    cnter = VALUE(6).Split(","c)
                    AX(0) = cnter(0).Trim("("c)
                    AX(1) = cnter(1).Trim(")"c)
                    ' Draw circle with dynamic size
                    Dim radius As Integer = CInt(Convert.ToInt16(VALUE(4)))
                    Dim centerX As Integer = Convert.ToInt16(AX(0))
                    Dim centerY As Integer = Convert.ToInt16(AX(1))
                    shapes.Add(New Circle(New Point(centerX, centerY), radius / 2, currentID, currentColor))

                    'AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius, 0, centerX, centerY)
                End If





            Next


        End If




    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Dim progname As String
        progname = txt_Sel_Prog_name.Text
        If progname = "" Then
            Return
        ElseIf (progname IsNot "") Then
            Dim isValid1 As Boolean = True
            Dim isValid As Boolean = True
            Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
            Dim Backupfname As String = "" & ConfigurationManager.AppSettings("BackupReceipeFilepath").ToString().Trim()
            Dim path As String = fname
            Dim Logdir As String = "" & fname
            Dim BackupLogdir As String = "" & Backupfname
            Dim ReceipeFileName As String = String.Empty
            If Not Directory.Exists(BackupLogdir) Then
                Directory.CreateDirectory(BackupLogdir)
            End If
            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If
            ReceipeFileName = progname
            Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
            Dim BaackupgeneratedFile As String = BackupLogdir & ReceipeFileName & ".xml"
            Dim side As String = cmbpanelside.Text
            Dim xmDocument As XmlDocument = New XmlDocument()
            xmDocument.Load(generatedFile)
            Dim Boardnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/RECEIPE/BOARD")

            For Each Feeder As XmlNode In Boardnodes
                Dim BOARD As BOARD = New BOARD()





                'Dim BOARDNAME As String = Feeder.ChildNodes(0).InnerText
                Feeder.ChildNodes(1).InnerText = txt_p_len.Text
                Feeder.ChildNodes(2).InnerText = txt_p_wed.Text
                Feeder.ChildNodes(3).InnerText = thk.Text
                Feeder.ChildNodes(4).InnerText = txt_p_weight.Text
                Feeder.ChildNodes(5).InnerText = txt_p_wed.Text
                Feeder.ChildNodes(6).InnerText = ""
                Feeder.ChildNodes(7).InnerText = UD_R_count.Text
                Feeder.ChildNodes(8).InnerText = UD_R_pitch.Text
                Feeder.ChildNodes(9).InnerText = UD_C_count.Text
                Feeder.ChildNodes(10).InnerText = UD_C_pitch.Text
                Feeder.ChildNodes(11).InnerText = txt_Mark_Id.Text







                Dim SIDEnodes As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/SIDE")


                SIDEnodes.ChildNodes(0).InnerText = cmbpanelside.Text




                Dim RECEIPENAMEnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/RECEIPENAME")
                For Each _PLCTAGnodes As XmlNode In RECEIPENAMEnodes
                    Dim RNAME As String = _PLCTAGnodes.ChildNodes(0).InnerText
                    Dim RPATH As String = _PLCTAGnodes.ChildNodes(1).InnerText

                Next







                'Next

            Next

            xmDocument.Save(generatedFile)
            xmDocument.Save(BaackupgeneratedFile)


            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''














        End If
    End Sub
    Private Sub btnfiducial_Click(sender As Object, e As EventArgs)
        Try
            If ListXMLPath.Count > 0 Then
                If Not String.IsNullOrEmpty(GRPATH) Then
                    Dim path As String = GRPATH
                    Dim Fiducial As New Fiducial
                    Fiducial.Setb = path
                    Fiducial.Show()
                Else
                    MessageBox.Show("Please select correct recipe!")
                End If
            Else
                MessageBox.Show("Please select correct recipe!")
            End If


        Catch ex As Exception

        End Try

    End Sub



    Private Sub txt_p_name_TextChanged(sender As Object, e As EventArgs)
        ValidateTxt_p_name()
    End Sub

    Private Sub txt_p_name_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ValidateTxt_p_name()
    End Sub

    Private Sub ValidateTxt_p_name()
        Dim input As String = txt_Sel_Prog_name.Text

        ' Check if the input contains invalid characters (decimal points)
        If input.Contains(".") Then
            ErrorProvider1.SetError(txt_Sel_Prog_name, "Decimal points are not allowed.")
        ElseIf String.IsNullOrWhiteSpace(input) Then
            ErrorProvider1.SetError(txt_Sel_Prog_name, "Please enter a value.")
        Else
            ErrorProvider1.SetError(txt_Sel_Prog_name, String.Empty)
        End If
    End Sub

    Private Sub txt_p_len_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ValidateTxt_p_len()
    End Sub

    Private Sub ValidateTxt_p_len()
        Dim input As String = txt_p_len.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(txt_p_len, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(txt_p_len, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(txt_p_len, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub txt_p_wed_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Validatetxt_p_wed()
    End Sub

    Private Sub Validatetxt_p_wed()
        Dim input As String = txt_p_wed.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(txt_p_wed, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(txt_p_wed, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(txt_p_wed, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub thk_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Validatethk()
    End Sub

    Private Sub Validatethk()
        Dim input As String = thk.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(thk, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(thk, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(thk, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub txt_p_weight_TextChanged(sender As Object, e As EventArgs)
        Validatetxt_p_weight()
    End Sub

    Private Sub txt_p_weight_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        Validatetxt_p_weight()
    End Sub

    Private Sub Validatetxt_p_weight()
        Dim input As String = txt_p_weight.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(txt_p_weight, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(txt_p_weight, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(txt_p_weight, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub UD_R_count_TextChanged(sender As Object, e As EventArgs)
        ValidateUD_R_count()
    End Sub

    Private Sub UD_R_count_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ValidateUD_R_count()
    End Sub

    Private Sub ValidateUD_R_count()
        Dim input As String = UD_R_count.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(UD_R_count, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(UD_R_count, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(UD_R_count, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub UD_R_pitch_TextChanged(sender As Object, e As EventArgs)
        ValidateUD_R_pitch()
    End Sub

    Private Sub UD_R_pitch_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ValidateUD_R_pitch()
    End Sub

    Private Sub ValidateUD_R_pitch()
        Dim input As String = UD_R_pitch.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(UD_R_pitch, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(UD_R_pitch, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(UD_R_pitch, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub UD_C_count_TextChanged(sender As Object, e As EventArgs)
        ValidateUD_C_count()
    End Sub

    Private Sub UD_C_count_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ValidateUD_C_count()
    End Sub

    Private Sub ValidateUD_C_count()
        Dim input As String = UD_C_count.Text
        Dim result As Double

        ' Only validate if there is input


        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(UD_C_count, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(UD_C_count, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(UD_C_count, String.Empty) ' Clear the error if empty
        End If
    End Sub

    Private Sub UD_C_pitch_TextChanged(sender As Object, e As EventArgs)
        ValidateUD_C_pitch()
    End Sub

    Private Sub UD_C_pitch_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs)
        ValidateUD_C_pitch()
    End Sub

    Private Sub ValidateUD_C_pitch()
        Dim input As String = UD_C_pitch.Text
        Dim result As Double

        ' Only validate if there is input
        If input <> String.Empty Then
            ' Check if the input is a valid non-negative number
            If Not Double.TryParse(input, result) OrElse result < 0 Then
                ErrorProvider1.SetError(UD_C_pitch, "Please enter a valid numeric value.")
            Else
                ErrorProvider1.SetError(UD_C_pitch, String.Empty) ' Clear the error
            End If
        Else
            ErrorProvider1.SetError(UD_C_pitch, String.Empty) ' Clear the error if empty
        End If
    End Sub



    Private Sub btndelete_Click(sender As Object, e As EventArgs) Handles btndelete.Click
        Try            ' Check if a row is selected in the DataGridView
            If ListXMLPath.Count > 0 Then
                ' Get the selected row
                Dim selectedRow As DataGridViewRow = DATAGRID.SelectedRows(0)
                Dim selectedFilePath As String = selectedRow.Cells("RPATH").Value.ToString()

                ' Prompt the user for confirmation
                Dim result = MessageBox.Show("Are you sure you want to delete this file?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If result = DialogResult.Yes Then
                    ' Delete the selected file
                    File.Delete(selectedFilePath)
                    ListXMLPath.Remove(selectedFilePath)

                    ' Remove the selected row from the DataGridView
                    DATAGRID.Rows.Remove(selectedRow)

                    Dim fname As String = "" & ConfigurationManager.AppSettings("Deleted_Logs").ToString().Trim()

                    '  Dim XMLOutPutPath As String = "D:\LM- Test/"

                    Dim path As String = fname
                    Dim Logdir As String = "" & fname

                    Dim ReceipeFileName As String = String.Empty
                    If Not Directory.Exists(Logdir) Then
                        Directory.CreateDirectory(Logdir)
                    End If

                    Dim _LaserHeadLocation As String() = Directory.GetFiles(Logdir, "*.xml", System.IO.SearchOption.AllDirectories)
                    Dim count As Integer = _LaserHeadLocation.Count()

                    If count = 0 Then
                        ReceipeFileName = txt_Sel_Prog_name.Text
                    Else
                        ReceipeFileName = txt_Sel_Prog_name.Text & "_" & count + 1
                    End If

                    Dim generatedFile As String = Logdir + ReceipeFileName & ".xml"





                    Dim side As String = cmbpanelside.Text


                    Dim xdoc = XDocument.Parse("<Error_Logs></Error_Logs>")
                    Dim contacts As XElement = New XElement("Error",
                                                        New XElement("FileName", generatedFile),
                                                        New XElement("DateTime", DateTime.Now().ToString()),
                                                        New XElement("UserName", ""),
                                                         New XElement("PageName", "Recipe"),
                                                        New XElement("Path", generatedFile),
                                                        New XElement("ErrorMessage", "Successfully deleted"))
                    xdoc.Root.Add(contacts)
                    xdoc.Save(generatedFile)





                    ' Show a message that the file was deleted successfully
                    MessageBox.Show("File deleted successfully.", "Delete Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                ' If no row is selected, show a message
                MessageBox.Show("Please select a row to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            ' Handle the exception


            Dim fname As String = "" & ConfigurationManager.AppSettings("Error_Logs").ToString().Trim()

            '  Dim XMLOutPutPath As String = "D:\LM- Test/"

            Dim path As String = fname
            Dim Logdir As String = "" & fname

            Dim ReceipeFileName As String = String.Empty
            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If

            Dim _LaserHeadLocation As String() = Directory.GetFiles(Logdir, "*.xml", System.IO.SearchOption.AllDirectories)
            Dim count As Integer = _LaserHeadLocation.Count()

            If count = 0 Then
                ReceipeFileName = txt_Sel_Prog_name.Text
            Else
                ReceipeFileName = txt_Sel_Prog_name.Text & "_" & count + 1
            End If

            Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"





            Dim side As String = cmbpanelside.Text


            Dim xdoc = XDocument.Parse("<Error_Logs></Error_Logs>")
            Dim contacts As XElement = New XElement("Error",
                                                        New XElement("FileName", generatedFile),
                                                        New XElement("DateTime", DateTime.Now().ToString()),
                                                        New XElement("UserName", ""),
                                                         New XElement("PageName", "Recipe"),
                                                        New XElement("Path", generatedFile),
                                                        New XElement("ErrorMessage", ex.Message))
            xdoc.Root.Add(contacts)
            xdoc.Save(generatedFile)
            MessageBox.Show("An error occurred while deleting the file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ValidateCmbPanelSide() As Boolean
        If String.IsNullOrEmpty(cmbpanelside.Text) Then
            ErrorProvider1.SetError(cmbpanelside, "Selection is required.")
            Return False
        Else
            ErrorProvider1.SetError(cmbpanelside, String.Empty)
            Return True
        End If
    End Function

    Private Function ValidateCmbLocation() As Boolean
        If String.IsNullOrEmpty(cmblocation.Text) Then
            ErrorProvider1.SetError(cmblocation, "Selection is required.")
            Return False
        Else
            ErrorProvider1.SetError(cmblocation, String.Empty)
            Return True
        End If
    End Function

    Private Sub btnclear_Click(sender As Object, e As EventArgs) Handles btnclear.Click
        LoadReceipe()
        txt_Sel_Prog_name.Text = String.Empty
        rtxtcurrentpg.Text = ""
        GRPATH = String.Empty
        ListXMLPath.Clear()




    End Sub


    Private _parameter As String
    Public Shared CurrentPageText As String = String.Empty

    Private Sub rtxtcurrentpg_TextChanged(sender As Object, e As EventArgs) Handles rtxtcurrentpg.TextChanged
        Dim searchText As String = rtxtcurrentpg.Text
        For Each row As DataGridViewRow In DATAGRID.Rows
            If row.Cells(1).Value IsNot Nothing AndAlso row.Cells(1).Value.ToString() = searchText Then
                ' LoadDataFromRow(row)
                Exit For
            End If
        Next
        CurrentPageText = rtxtcurrentpg.Text
        Dim homePage As Home_Page = Nothing

        ' Iterate through all open forms to find Home_Page
        For Each frm As Form In System.Windows.Forms.Application.OpenForms
            If TypeOf frm Is Home_Page Then
                homePage = CType(frm, Home_Page)
                Exit For
            End If
        Next

        ' If Home_Page is found, update the label
        If homePage IsNot Nothing Then

        Else
            ' If Home_Page is not open, create a new instance, update the label, and show the form
            homePage = New Home_Page()

            homePage.Show()
        End If
    End Sub
    Public Sub ReadDataFromFiducialData1()
        ' Access the data from FiducialData1.SelectedShapeData1
        Dim serialNo As Integer = FiducialData1.SelectedShapeData1.SerialNo1
        Dim shape As String = FiducialData1.SelectedShapeData1.Shape1
        Dim x1 As Integer = FiducialData1.SelectedShapeData1.X11
        Dim y1 As Integer = FiducialData1.SelectedShapeData1.Y11
        Dim x2 As Integer = FiducialData1.SelectedShapeData1.X21
        Dim y2 As Integer = FiducialData1.SelectedShapeData1.Y21
        Dim center As String = FiducialData1.SelectedShapeData1.Center1

    End Sub
    Public Sub ReadDataFromFiducialData2()
        ' Access the data from FiducialData1.SelectedShapeData1
        Dim serialNo As Integer = FiducialData2.SelectedShapeData2.SerialNo2
        Dim shape As String = FiducialData2.SelectedShapeData2.Shape2
        Dim x1 As Integer = FiducialData2.SelectedShapeData2.X12
        Dim y1 As Integer = FiducialData2.SelectedShapeData2.Y12
        Dim x2 As Integer = FiducialData2.SelectedShapeData2.X22
        Dim y2 As Integer = FiducialData2.SelectedShapeData2.Y22
        Dim center As String = FiducialData2.SelectedShapeData2.Center2

        ' Fill the text box cbShape with the value of shape1

    End Sub

    Dim previousValuex As Single
    Dim previousValueY As Single
    Dim previousValueCW As Single
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim X(1) As Integer
        plc.GetDevice("D342", X(0))
        plc.GetDevice("D343", X(1))

        Dim xnum As Single = ConvertWordToFloat(X)
        If xnum <> previousValuex Then
            TXX.Text = xnum.ToString("F6")
            X_Current.Text = xnum.ToString("F6")
            previousValuex = xnum
        End If

        Dim Y(1) As Integer
        plc.GetDevice("D344", Y(0))
        plc.GetDevice("D345", Y(1))

        Dim ynum As Single = ConvertWordToFloat(Y)

        If ynum <> previousValueY Then
            TXY.Text = ynum.ToString("F6")
            Y_Current.Text = ynum.ToString("F6")
            previousValueY = ynum
        End If
        Dim CW(1) As Integer
        plc.GetDevice("D312", CW(0))
        plc.GetDevice("D313", CW(1))

        Dim Cnum As Single = ConvertWordToFloat(CW)

        If Cnum <> previousValueY Then
            TXCW.Text = Cnum.ToString("F6")
            C__Current.Text = Cnum.ToString("F6")
            previousValueCW = Cnum
        End If
        'Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        'Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        ' Dim nRet As Int32 = Home_Page.LiveCamera1.GetImageBuffer(stFrameOut, 1000)

        ' Else
        'If BackgroundWorker1.IsBusy Then
        'BackgroundWorker1.CancelAsync()
        'End If

        'If BackgroundWorker2.IsBusy Then
        'Bac'kgroundWorker2.CancelAsync()
        'End If
        '    If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver Then
        '        m_nBufSizeForDriver = stFrameOut.stFrameInfo.nFrameLen
        '        ReDim m_pBufForDriver(m_nBufSizeForDriver)
        '    End If
        '    m_stFrameInfoEx = stFrameOut.stFrameInfo
        '    Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver, 0, stFrameOut.stFrameInfo.nFrameLen)
        '    stDisplayInfo.hWnd = livepic.Handle
        '    stDisplayInfo.pData = stFrameOut.pBufAddr
        '    stDisplayInfo.nDataLen = stFrameOut.stFrameInfo.nFrameLen
        '    stDisplayInfo.nWidth = stFrameOut.stFrameInfo.nWidth
        '    stDisplayInfo.nHeight = stFrameOut.stFrameInfo.nHeight
        '    stDisplayInfo.enPixelType = stFrameOut.stFrameInfo.enPixelType
        '    Home_Page.LiveCamera1.DisplayOneFrame(stDisplayInfo)
        '    Home_Page.LiveCamera1.FreeImageBuffer(stFrameOut)

        'End If






        'Dim nRet2 = Home_Page.LiveCamera1.GetImageBuffer(stFrameOut, 1000)
        'If CCamera.MV_OK = nRet2 Then


        '    If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver Then
        '        m_nBufSizeForDriver = stFrameOut.stFrameInfo.nFrameLen
        '        ReDim m_pBufForDriver(m_nBufSizeForDriver)
        '    End If

        '    m_stFrameInfoEx = stFrameOut.stFrameInfo
        '    Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver, 0, stFrameOut.stFrameInfo.nFrameLen)
        '    stDisplayInfo.hWnd = livepic1.Handle
        '    stDisplayInfo.pData = stFrameOut.pBufAddr
        '    stDisplayInfo.nDataLen = stFrameOut.stFrameInfo.nFrameLen
        '    stDisplayInfo.nWidth = stFrameOut.stFrameInfo.nWidth
        '    stDisplayInfo.nHeight = stFrameOut.stFrameInfo.nHeight
        '    stDisplayInfo.enPixelType = stFrameOut.stFrameInfo.enPixelType
        '    Home_Page.LiveCamera1.DisplayOneFrame(stDisplayInfo)

        '    Home_Page.LiveCamera1.FreeImageBuffer(stFrameOut)
        'End If
        ''Dim nRet1 = Home_Page.FidCam1.GetImageBuffer(stFrameOut, 1000)
        ''If CCamera.MV_OK = nRet1 Then




        ''Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        ''Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        'Dim nRet1 As Int32 = Home_Page.FidCam1.GetImageBuffer(stFrameOut, 1000)
        'If CCamera.MV_OK = nRet1 Then

        'Else
        'If BackgroundWorker3.IsBusy Then
        'BackgroundWorker3.Dispose()
        'End If
        '    If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver1 Then
        '        m_nBufSizeForDriver1 = stFrameOut.stFrameInfo.nFrameLen
        '        ReDim m_pBufForDriver1(m_nBufSizeForDriver1)
        '    End If

        '    m_stFrameInfoEx = stFrameOut.stFrameInfo
        '    Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver1, 0, stFrameOut.stFrameInfo.nFrameLen)

        '    Dim stSaveImageParam As CCamera.MV_SAVE_IMG_TO_FILE_PARAM = New CCamera.MV_SAVE_IMG_TO_FILE_PARAM()
        '    Dim pData As IntPtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForDriver1, 0)
        '    stSaveImageParam.pData = pData
        '    stSaveImageParam.nDataLen = m_stFrameInfoEx.nFrameLen
        '    stSaveImageParam.enPixelType = m_stFrameInfoEx.enPixelType
        '    stSaveImageParam.nWidth = m_stFrameInfoEx.nWidth
        '    stSaveImageParam.nHeight = m_stFrameInfoEx.nHeight
        '    stSaveImageParam.enImageType = CCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Png
        '    stSaveImageParam.iMethodValue = 2
        '    stSaveImageParam.nQuality = 90
        '    stSaveImageParam.pImagePath = "C:\Manage Files\Load\" & "123" & ".Png"

        '    Thread.Sleep(5)

        '    nRet1 = Home_Page.FidCam1.SaveImageToFile(stSaveImageParam)


        '    Home_Page.FidCam1.DisplayOneFrame(stDisplayInfo)

        '    Home_Page.FidCam1.FreeImageBuffer(stFrameOut)
        '    Dim ss As Boolean = File.Exists("C:\Manage Files\Load\123.Png")
        '    Dim ss1 As Boolean = File.Exists("C:\Manage Files\Load\Image_w1.Png")

        '    If ss = True Then

        '        Dim img As Bitmap = New Bitmap("C:\Manage Files\Load\123.Png")

        '        Dim newImg As New Bitmap(PictureBox7.Width, PictureBox7.Height)

        '        ' Draw the resized image
        '        Using g As Graphics = Graphics.FromImage(newImg)
        '            g.InterpolationMode = InterpolationMode.HighQualityBicubic
        '            g.DrawImage(img, 0, 0, PictureBox7.Width, PictureBox7.Height)
        '        End Using
        '        newImg.Save("C:\Manage Files\Load\Image_w1.Png")

        '        PictureBox7.LoadAsync("C:\Manage Files\Load\Image_w1.Png")
        '        newImg.Dispose()
        '        img.Dispose()

        '    End If







        'End If








    End Sub





    Private Sub btn_Widthadj_Click_1(sender As Object, e As EventArgs)
        Dim floatValue As Single
        If Single.TryParse(txt_p_wed.Text, floatValue) Then
            ' Round the float value to two decimal places
            Dim roundedValue As Single = Math.Round(floatValue, 3)

            ' Convert the rounded float value to two 16-bit integers
            Dim words() As Integer = ConvertFloatToWord(roundedValue)

            ' Example: Write to different data registers
            plc.SetDevice("D320", words(0))
            plc.SetDevice("D321", words(1))
        Else
        End If

    End Sub



    Private Sub btpanWide_MouseDown(sender As Object, e As MouseEventArgs) Handles btpanWide.MouseDown
        plc.SetDevice("M200", 1)
    End Sub
    Private Sub btpanWide_MouseUp(sender As Object, e As MouseEventArgs) Handles btpanWide.MouseUp
        plc.SetDevice("M200", 0)
    End Sub
    Private Sub btpanN_MouseDown(sender As Object, e As MouseEventArgs) Handles btpanN.MouseDown
        plc.SetDevice("M201", 1)
    End Sub

    Private Sub btpanN_MouseUp(sender As Object, e As MouseEventArgs) Handles btpanN.MouseUp
        plc.SetDevice("M201", 0)
    End Sub


    Private Sub btxmin_MouseDown(sender As Object, e As MouseEventArgs) Handles btxmin.MouseDown
        plc.SetDevice("M205", 1)
    End Sub

    Private Sub btxmax_MouseUp(sender As Object, e As MouseEventArgs) Handles btxmax.MouseUp
        plc.SetDevice("M206", 0)
    End Sub

    Private Sub btxmax_MouseDown(sender As Object, e As MouseEventArgs) Handles btxmax.MouseDown
        plc.SetDevice("M206", 1)

    End Sub

    Private Sub Guna2ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox1.SelectedIndexChanged
        Dim selectedValue As String = Guna2ComboBox1.SelectedItem.ToString()
        Select Case selectedValue
            Case "HIGH"
                plc.SetDevice("D358", 3)
            Case "MEDIUM"
                plc.SetDevice("D358", 2)
            Case "LOW"
                plc.SetDevice("D358", 1)
        End Select
    End Sub
    Private Sub Button15_MouseDown(sender As Object, e As MouseEventArgs) Handles Button15.MouseDown
        plc.SetDevice("M249", 1)
    End Sub

    Private Sub Button15_MouseUp(sender As Object, e As MouseEventArgs) Handles Button15.MouseUp
        plc.SetDevice("M249", 0)
    End Sub
    Private Sub Button16_MouseDown(sender As Object, e As MouseEventArgs) Handles Button16.MouseDown
        plc.SetDevice("M250", 1)
    End Sub

    Private Sub Button16_MouseUp(sender As Object, e As MouseEventArgs) Handles Button16.MouseUp
        plc.SetDevice("M250", 0)
    End Sub

    Private Sub btn_Widthadj_MouseDown_1(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M240", 1)
    End Sub

    Private Sub btn_Widthadj_MouseUp_1(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M240", 1)
    End Sub



    Private Sub Guna2ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox2.SelectedIndexChanged
        Dim selectedValue As String = Guna2ComboBox2.SelectedItem.ToString()
        Select Case selectedValue
            Case "0.1MM"
                plc.SetDevice("D356", 3)
            Case "1MM"
                plc.SetDevice("D356", 2)
            Case "10MM"
                plc.SetDevice("D356", 1)
            Case "CONITNUES"
                plc.SetDevice("D356", 0)
        End Select
    End Sub

    Private Sub Guna2Panel1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2Panel1.Paint

    End Sub
    Public Sub SaveDataToDataGridView()
        ' Assuming txt_p_len and txt_p_wed are TextBox controls where you input data
        Dim data1 As String = TXX.Text ' Data for column 2
        Dim data2 As String = TXY.Text ' Data for column 3

        Dim rowIndex As Integer = DataGridView1.Rows.Add()
        DataGridView1.Rows(rowIndex).Cells(0).Value = data1 ' Column 2
        DataGridView1.Rows(rowIndex).Cells(1).Value = data2


    End Sub
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        SaveDataToDataGridView()
    End Sub




    ' Your other form code...

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim progname As String
        progname = txt_Sel_Prog_name.Text
        If progname = "" Then
            Return
        ElseIf (progname IsNot "") Then
            Dim isValid1 As Boolean = True
            Dim isValid As Boolean = True
            Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
            Dim Defaultfname As String = "" & ConfigurationManager.AppSettings("DefaultPath").ToString().Trim()
            Dim path As String = fname
            Dim Logdir As String = "" & fname
            Dim DefaultLogdir As String = "" & Defaultfname
            Dim ReceipeFileName As String = String.Empty
            If Not Directory.Exists(DefaultLogdir) Then
                Directory.CreateDirectory(DefaultLogdir)
            End If
            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If
            Dim files As String() = Directory.GetFiles(DefaultLogdir, "*.xml", System.IO.SearchOption.AllDirectories)
            'File.Delete(DefaultLogdir)
            For Each file1 As String In files
                File.Delete(file1)
            Next

            ReceipeFileName = progname
            Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
            Dim DefaultgeneratedFile As String = DefaultLogdir & "Defualt" & ".xml"
            Dim side As String = cmbpanelside.Text
            Dim xmDocument As XmlDocument = New XmlDocument()
            xmDocument.Load(generatedFile)

            xmDocument.Save(DefaultgeneratedFile)
            My.Settings.ProgramName = txt_Sel_Prog_name.Text
            My.Settings.Save()

            rtxtcurrentpg.Text = txt_Sel_Prog_name.Text
            MessageBox.Show("Recipe Download Complete")
            loadData()

        End If
    End Sub




    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim progname As String
        progname = txt_Sel_Prog_name.Text
        If progname = "" Then
            Return
        ElseIf (progname IsNot "") Then
            Dim isValid1 As Boolean = True
            Dim isValid As Boolean = True
            Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
            Dim Backupfname As String = "" & ConfigurationManager.AppSettings("BackupReceipeFilepath").ToString().Trim()
            Dim path As String = fname
            Dim Logdir As String = "" & fname
            Dim BackupLogdir As String = "" & Backupfname
            Dim ReceipeFileName As String = String.Empty
            If Not Directory.Exists(BackupLogdir) Then
                Directory.CreateDirectory(BackupLogdir)
            End If
            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If
            ReceipeFileName = progname
            Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
            Dim BaackupgeneratedFile As String = BackupLogdir & ReceipeFileName & ".xml"
            Dim side As String = cmbpanelside.Text
            Dim xmDocument As XmlDocument = New XmlDocument()
            xmDocument.Load(generatedFile)
            Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/MARKPOSITION")

            MARKPOSITION.RemoveAll()

            xmDocument.Save(generatedFile)
            xmDocument.Save(BaackupgeneratedFile)






            Dim i As Integer = 0
            Dim DATA(5) As String

            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow Then




                    Dim POSI As XmlElement = xmDocument.CreateElement("POSI")
                    MARKPOSITION.AppendChild(POSI)
                    Dim ID As XmlAttribute = xmDocument.CreateAttribute("id")
                    ID.Value = (i + 1).ToString



                    POSI.Attributes.Append(ID)
                    Dim Xvl As XmlElement = xmDocument.CreateElement("XvVALUE")
                    Dim Yvl As XmlElement = xmDocument.CreateElement("YvVALUE")
                    Dim Selvl As XmlElement = xmDocument.CreateElement("CHvVALUE")
                    DATA(0) = DataGridView1.Rows(i).Cells(0).Value.ToString()
                    DATA(1) = DataGridView1.Rows(i).Cells(1).Value.ToString()
                    DATA(2) = DataGridView1.Rows(i).Cells(2).Value.ToString()
                    Xvl.InnerText = DATA(0)
                    Yvl.InnerText = DATA(1)
                    Selvl.InnerText = DATA(2)
                    POSI.AppendChild(Xvl)
                    POSI.AppendChild(Yvl)
                    POSI.AppendChild(Selvl)
                    i = i + 1
                End If
            Next
            xmDocument.Save(generatedFile)
            xmDocument.Save(BaackupgeneratedFile)
        End If
    End Sub




    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' Check if there is a selected row in DataGridView1
        If DataGridView1.SelectedRows.Count > 0 Then
            ' Get the selected row and delete it
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            DataGridView1.Rows.Remove(selectedRow)

            Dim progname As String
            progname = txt_Sel_Prog_name.Text
            If progname = "" Then
                Return
            ElseIf (progname IsNot "") Then
                Dim isValid1 As Boolean = True
                Dim isValid As Boolean = True
                Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
                Dim Backupfname As String = "" & ConfigurationManager.AppSettings("BackupReceipeFilepath").ToString().Trim()
                Dim path As String = fname
                Dim Logdir As String = "" & fname
                Dim BackupLogdir As String = "" & Backupfname
                Dim ReceipeFileName As String = String.Empty
                If Not Directory.Exists(BackupLogdir) Then
                    Directory.CreateDirectory(BackupLogdir)
                End If
                If Not Directory.Exists(Logdir) Then
                    Directory.CreateDirectory(Logdir)
                End If
                ReceipeFileName = progname
                Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
                Dim BaackupgeneratedFile As String = BackupLogdir & ReceipeFileName & ".xml"
                Dim side As String = cmbpanelside.Text
                Dim xmDocument As XmlDocument = New XmlDocument()
                xmDocument.Load(generatedFile)
                Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/MARKPOSITION")
                'Dim Nodes = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")
                'Dim dd As XmlNodeList = MARKPOSITION.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")
                'MARKPOSITION.RemoveChild(dd)
                MARKPOSITION.RemoveAll()

                xmDocument.Save(generatedFile)
                xmDocument.Save(BaackupgeneratedFile)






                Dim i As Integer = 0
                Dim DATA(5) As String

                For Each row As DataGridViewRow In DataGridView1.Rows
                    If Not row.IsNewRow Then




                        Dim POSI As XmlElement = xmDocument.CreateElement("POSI")
                        MARKPOSITION.AppendChild(POSI)
                        Dim ID As XmlAttribute = xmDocument.CreateAttribute("id")
                        ID.Value = (i + 1).ToString



                        POSI.Attributes.Append(ID)
                        Dim Xvl As XmlElement = xmDocument.CreateElement("XvVALUE")
                        Dim Yvl As XmlElement = xmDocument.CreateElement("YvVALUE")
                        Dim Selvl As XmlElement = xmDocument.CreateElement("CHvVALUE")
                        DATA(0) = DataGridView1.Rows(i).Cells(0).Value.ToString()
                        DATA(1) = DataGridView1.Rows(i).Cells(1).Value.ToString()
                        DATA(2) = DataGridView1.Rows(i).Cells(2).Value.ToString()
                        Xvl.InnerText = DATA(0)
                        Yvl.InnerText = DATA(1)
                        Selvl.InnerText = DATA(2)
                        POSI.AppendChild(Xvl)
                        POSI.AppendChild(Yvl)
                        POSI.AppendChild(Selvl)
                        i = i + 1
                    End If
                Next
                xmDocument.Save(generatedFile)
                xmDocument.Save(BaackupgeneratedFile)
            End If



        Else
            MessageBox.Show("Please select a row in DataGridView1 to delete.")
        End If
    End Sub




    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ' Check if the clicked cell is valid and not the header row
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
            ' Store the clicked row index for later use
            Me.DataGridView1.Tag = e.RowIndex
        End If
    End Sub



    Private Sub btn_Widthadj_Click_2(sender As Object, e As EventArgs) Handles btn_Widthadj.Click
        Dim floatValueCW As Single
        If Single.TryParse(txt_p_wed.Text, floatValueCW) Then
            ' Convert the float value to two 16-bit integers
            Dim words() As Integer = ConvertFloatToWord(floatValueCW)

            ' Write the integers to the PLC registers
            plc.SetDevice("D320", words(0))
            plc.SetDevice("D321", words(1))
        Else
            ' If parsing fails, you can handle the invalid input here
        End If
    End Sub

    Private Sub btn_Widthadj_MouseDown_2(sender As Object, e As MouseEventArgs) Handles btn_Widthadj.MouseDown
        plc.SetDevice("M240", 1)
    End Sub

    Private Sub btn_Widthadj_MouseUp_2(sender As Object, e As MouseEventArgs) Handles btn_Widthadj.MouseUp
        plc.SetDevice("M240", 0)
    End Sub



    Private Sub PictureBox7_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox7.MouseDown
        If shapes.Count < datagrdFid.Rows.Count Then ' Subtract 1 to account for the new row placeholder
            If e.Button = MouseButtons.Left Then
                startPoint = e.Location
                endPoint = e.Location ' Set endPoint initially to start point
                isDrawing = True
            End If
        Else

            MessageBox.Show("You have reached the maximum number of shapes allowed.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub PictureBox7_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox7.MouseMove
        If isDrawing AndAlso e.Button = MouseButtons.Left Then
            endPoint = e.Location
            PictureBox7.Invalidate()
        End If
    End Sub
    Private Function GetMaxCurrentID() As Integer
        Dim maxID As Integer = 0
        For Each row As DataGridViewRow In datagrdFid.Rows
            If Not row.IsNewRow Then
                Dim rowID As Integer = Convert.ToInt32(row.Cells(0).Value)
                Dim rowIndex As Integer = DataGridView1.Rows.Add()
                If rowID > maxID Then
                    maxID = rowID
                End If
            End If
        Next
        Return maxID
    End Function
    'Private Sub Guna2PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseUp

    '    If isDrawing Then
    '        ' Check if the current number of shapes is less than the number of rows in the DataGridView
    '        If shapes.Count < .Rows.Count - 1 Then ' Subtract 1 to account for the new row placeholder
    '            If drawSquares Then
    '                ' Draw square as before
    '                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
    '                Dim topLeft As New Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y))
    '                shapes.Add(New Square(topLeft, size, currentID, currentColor))
    '                AddShapeToDataGridView(currentID, "Square", topLeft.X, topLeft.Y, size.Width, size.Height)
    '            Else
    '                ' Draw circle with dynamic size
    '                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
    '                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
    '                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
    '                shapes.Add(New Circle(New Point(centerX, centerY), radius, currentID, currentColor))

    '                AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius * 2, 0, centerX, centerY)
    '                AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius * 2, 0, centerX, centerY)
    '            End If

    '            currentID += 1
    '            isDrawing = False
    '            PictureBox7.Invalidate()
    '        Else
    '            shapes.Clear()
    '            MessageBox.Show("You cannot draw more shapes than the number of rows in the DataGridView.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '        End If
    '    End If
    'End Sub
    Private Sub AddShapeToDataGridView(id As Integer, shape As String, x1 As Integer, y1 As Integer, width As Integer, Optional height As Single = 0, Optional centerX As Integer = 0, Optional centerY As Integer = 0)
        xposs = TXX.Text
        yposs = TXY.Text
        Dim centerPoint As String

        ' Variables for main and sub IDs
        Dim mainid As Integer = 1
        Dim subid As Integer = 1
        Dim TYP As String = ""
        'Dim CELVALUE As String

        ' Determine the main or sub ID


        ' Add row based on shape type
        If shape = "Square" Then
            Dim x2 As Integer = width
            Dim y2 As Integer = height
            Dim centerXInt As Integer = CInt((x1 + width) \ 2) ' Calculate center X as integer
            Dim centerYInt As Integer = CInt((y1 + height) \ 2) ' Calculate center Y as integer
            centerPoint = $"({centerXInt}, {centerYInt})"
            'Dim row As String() = {TYP, shape, x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), centerPoint, xposs, yposs}
            'datagrdFid.SelectedRow.Add(row)
            'datagrdFid.SelectedRows(0).Cells(0).Value = TYP
            datagrdFid.SelectedRows(0).Cells(1).Value = shape
            datagrdFid.SelectedRows(0).Cells(2).Value = x1.ToString()
            datagrdFid.SelectedRows(0).Cells(3).Value = y1.ToString()
            datagrdFid.SelectedRows(0).Cells(4).Value = x2.ToString()
            datagrdFid.SelectedRows(0).Cells(5).Value = y2.ToString()
            datagrdFid.SelectedRows(0).Cells(6).Value = centerPoint


            datagrdFid.SelectedRows(0).Cells(9).Value = xposs
            datagrdFid.SelectedRows(0).Cells(10).Value = yposs


        ElseIf shape = "Circle" Then
            centerPoint = $"({centerX}, {centerY})"
            Dim row As String() = {TYP, shape, x1.ToString(), y1.ToString(), width.ToString(), height.ToString(), centerPoint, xposs, yposs}
            'datagrdFid.SelectedRows(0).Cells(0).Value = TYP
            datagrdFid.SelectedRows(0).Cells(1).Value = shape
            datagrdFid.SelectedRows(0).Cells(2).Value = x1.ToString()
            datagrdFid.SelectedRows(0).Cells(3).Value = y1.ToString()
            datagrdFid.SelectedRows(0).Cells(4).Value = width.ToString()
            datagrdFid.SelectedRows(0).Cells(5).Value = height.ToString()
            datagrdFid.SelectedRows(0).Cells(6).Value = centerPoint


            datagrdFid.SelectedRows(0).Cells(9).Value = xposs
            datagrdFid.SelectedRows(0).Cells(10).Value = yposs
        Else
            Throw New ArgumentException("Invalid shape type")
        End If
    End Sub
    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click

        Dim mainid As Integer = 1
        Dim subid As Integer = 1
        Dim TYP As String = ""
        Dim CELVALUE As String



        ' Determine the main or sub ID

        If FIDTYPE.Text = "MAIN" Then
            If datagrdFid.Rows.Count > 0 Then
                For Each row1 As DataGridViewRow In datagrdFid.Rows
                    CELVALUE = Convert.ToString(row1.Cells(0).Value)
                    If CELVALUE(0) = "M" Then
                        mainid += 1

                    End If


                Next
            End If
            TYP = "MAIN" & mainid
        ElseIf (FIDTYPE.Text = "SUB") Or (FIDTYPE.Text = "") Then
            If datagrdFid.Rows.Count > 0 Then
                For Each row1 As DataGridViewRow In datagrdFid.Rows
                    CELVALUE = Convert.ToString(row1.Cells(0).Value)
                    If CELVALUE(0) = "S" Then
                        subid += 1
                    End If

                Next
            End If
            TYP = "SUB" & subid
        End If

        Dim SHAP As String
        If isDrawing Then
            ' Check the maximum current ID from the DataGridView
            'Dim maxCurrentID As Integer = GetMaxCurrentID()
            'If maxCurrentID >= currentID Then
            '    currentID = maxCurrentID + 1
            'End If

            If drawSquares Then
                ' Draw square as before
                SHAP = "Square"
            Else
                ' Draw circle with dynamic size


                SHAP = "Circle"
            End If


        End If

        If TextBox3.Text = "" Then
            TextBox3.Text = "0"
        End If
        If TextBox1.Text = "" Then
            TextBox1.Text = "0"
        End If
        If TextBox2.Text = "" Then
            TextBox2.Text = "0"
        End If
        If TextBox4.Text = "" Then
            TextBox4.Text = "0"
        End If



        datagrdFid.Rows.Add(TYP, SHAP, "0", "0", "0", "0", "0", "0", "0", xposs, yposs,
                            TextBox3.Text, TextBox2.Text, TextBox4.Text, TextBox1.Text, False)
        datagrdFid.Rows(0).Selected = False
        Dim int1 As Integer = datagrdFid.Rows.Count
        datagrdFid.Rows(int1 - 1).Selected = True

    End Sub
    Private Class Shape
        Public Property ID As String
        Public Property Color As Color

        Public Sub New(id As String, color As Color)
            Me.ID = id
            Me.Color = color
        End Sub
    End Class

    Private Class Square
        Inherits Shape

        Public Property TopLeft As Point
        Public Property Size As Size

        Public Sub New(topLeft As Point, size As Size, id As Integer, color As Color)
            MyBase.New(id, color)
            Me.TopLeft = topLeft
            Me.Size = size
        End Sub
    End Class

    Private Class Circle
        Inherits Shape

        Public Property Center As Point
        Public Property Radius As Integer

        Public Sub New(center As Point, radius As Integer, id As Integer, color As Color)
            MyBase.New(id, color)
            Me.Center = center
            Me.Radius = radius
        End Sub
    End Class

    Private Sub PictureBox7_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox7.Paint
        Dim g As Graphics = e.Graphics
        Dim font As New Font("Arial", 12)
        Dim brush As New SolidBrush(Color.Black)

        'Draw all stored shapes
        For Each shape In shapes
            Dim pen As New Pen(shape.Color, 2)
            If TypeOf shape Is Square Then
                Dim square As Square = DirectCast(shape, Square)
                g.DrawRectangle(pen, square.TopLeft.X, square.TopLeft.Y, square.Size.Width, square.Size.Height)
                g.DrawString(square.ID.ToString(), font, brush, square.TopLeft.X, square.TopLeft.Y - 20)
            ElseIf TypeOf shape Is Circle Then
                Dim circle As Circle = DirectCast(shape, Circle)
                g.DrawEllipse(pen, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2)
                g.DrawString(circle.ID.ToString(), font, brush, circle.Center.X - circle.Radius, circle.Center.Y - circle.Radius - 20)
            End If
        Next

        ' Draw the current shape if drawing is in progress
        If isDrawing Then
            Dim pen As New Pen(currentColor, 2)
            If drawSquares Then
                Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                Dim rect As New Rectangle(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), size.Width, size.Height)
                g.DrawRectangle(pen, rect)
                g.DrawString(currentID.ToString(), font, brush, rect.X, rect.Y - 20)
            Else ' Draw circle
                Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2)
                g.DrawString(currentID.ToString(), font, brush, centerX - radius, centerY - radius - 20)
            End If
        End If

    End Sub
    Private normal As Color

    Private Sub Guna2CircleButton1_Click(sender As Object, e As EventArgs) Handles Guna2CircleButton1.Click
        isDrawing = False
        drawSquares = False
        PictureBox7.Invalidate()
        Guna2Button1.BackColor = normal
        Guna2CircleButton1.BackColor = Color.Green
        Panel26.Hide()
        Panel27.Show()
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        drawSquares = True ' Set flag to draw squares
        isDrawing = False ' Ensure we're not drawing circles
        Guna2CircleButton1.BackColor = normal
        Guna2Button1.BackColor = Color.Green
        Panel26.Show()
        Panel27.Hide()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If ColorDialog1.ShowDialog() = DialogResult.OK Then
            currentColor = ColorDialog1.Color
        End If
    End Sub
    Private Sub UpdateDataGridViewWithTextBoxValues(rowIndex As Integer)
        ' Update the DataGridView to include values from TextBox1 and TextBox2
        datagrdFid.Rows(rowIndex).Cells(7).Value = TXX.Text
        datagrdFid.Rows(rowIndex).Cells(8).Value = TXY.Text
    End Sub
    Private Sub Guna2Button4_Click(sender As Object, e As EventArgs) Handles Guna2Button4.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the top
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.TopLeft = New Point(square.TopLeft.X, square.TopLeft.Y - increaseAmount)
                square.Size = New Size(square.Size.Width, square.Size.Height + increaseAmount)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(3).Value = square.TopLeft.Y ' Update the Y1 value
                datagrdFid.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub
    Private Sub ClearAndDisposePictureBox(pictureBox As PictureBox, serialNumber As String)
        ' Check if the PictureBox contains an image with the specified serial number
        If pictureBox.Image IsNot Nothing Then
            ' Check if the image file path corresponds to the serial number
            Dim imagePath As String = Path.Combine("A:\Project", txt_Sel_Prog_name.Text.Trim(), serialNumber & ".png")
            If File.Exists(imagePath) Then
                pictureBox.Image = Nothing
                pictureBox.Invalidate()
            End If
        End If
    End Sub
    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        ' Check if a row is selected
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Show a confirmation dialog
            Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            ' If the user confirms the deletion
            If result = DialogResult.Yes Then
                ' Check if txt_Sel_Prog_name is empty
                Dim folderName As String = txt_Sel_Prog_name.Text.Trim()
                If String.IsNullOrEmpty(folderName) Then
                    MessageBox.Show("Please select a recipe first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Exit Sub
                End If

                ' Get the selected row index
                Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

                ' Get the S.No from the selected row
                '
                Dim serialNumber As String = datagrdFid.Rows(rowIndex).Cells(0).Value.ToString()

                ' Define the image file path based on S.No
                Dim imagePath As String = Path.Combine("C:\Manage Files\Fid_Image", folderName, serialNumber & ".png")

                ' Try to delete the image file if it exists
                Try
                    If File.Exists(imagePath) Then
                        File.Delete(imagePath)
                    End If
                Catch ex As IOException
                    MessageBox.Show("Error deleting the file: " & ex.Message)
                End Try

                ' Remove the corresponding shape from the shapes list
                Dim selectedShapeID As String = datagrdFid.Rows(rowIndex).Cells(0).Value.ToString()
                Dim shapeToRemove As Shape = shapes.FirstOrDefault(Function(s) s.ID = (rowIndex + 1))
                If shapeToRemove IsNot Nothing Then
                    shapes.Remove(shapeToRemove)
                End If

                ' Remove the row from the DataGridView


                ' Clear and dispose of the PictureBox images
                ClearAndDisposePictureBox(Guna2PictureBox1, serialNumber)
                ClearAndDisposePictureBox(Guna2PictureBox2, serialNumber)

                ' Update the currentID to be one more than the maximum ID in the shapes list
                If shapes.Count > 0 Then
                    currentID = shapes.Max(Function(s) Convert.ToInt32(s.ID)) + 1
                Else
                    currentID = 1
                End If

                ' Redraw PictureBox or perform any other necessary UI update
                PictureBox7.Invalidate()
                'Dim selectedShapeID As String = (datagrdFid.Rows(rowIndex).Cells(0).Value)
                'Dim shapeToRemove As Shape = shapes.FirstOrDefault(Function(s) s.ID = (rowIndex))
                If shapeToRemove IsNot Nothing Then
                    shapes.Remove(shapeToRemove)
                End If

                ' Remove the row from the DataGridView
                datagrdFid.Rows.RemoveAt(rowIndex)

                ' Redraw PictureBox


                Dim progname As String
                progname = txt_Sel_Prog_name.Text
                If progname = "" Then
                    Return
                ElseIf (progname IsNot "") Then
                    Dim isValid1 As Boolean = True
                    Dim isValid As Boolean = True
                    Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
                    Dim Backupfname As String = "" & ConfigurationManager.AppSettings("BackupReceipeFilepath").ToString().Trim()
                    Dim path As String = fname
                    Dim Logdir As String = "" & fname
                    Dim BackupLogdir As String = "" & Backupfname
                    Dim ReceipeFileName As String = String.Empty
                    If Not Directory.Exists(BackupLogdir) Then
                        Directory.CreateDirectory(BackupLogdir)
                    End If
                    If Not Directory.Exists(Logdir) Then
                        Directory.CreateDirectory(Logdir)
                    End If
                    ReceipeFileName = progname
                    Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
                    Dim BaackupgeneratedFile As String = BackupLogdir & ReceipeFileName & ".xml"
                    Dim side As String = cmbpanelside.Text
                    Dim xmDocument As XmlDocument = New XmlDocument()
                    xmDocument.Load(generatedFile)
                    Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/FIDUCIAL")
                    'Dim Nodes = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")
                    'Dim dd As XmlNodeList = MARKPOSITION.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")
                    'MARKPOSITION.RemoveChild(dd)
                    MARKPOSITION.RemoveAll()

                    xmDocument.Save(generatedFile)
                    xmDocument.Save(BaackupgeneratedFile)


                    Dim i As Integer = 0
                    Dim DATA(20) As String
                    For Each row As DataGridViewRow In datagrdFid.Rows
                        If Not row.IsNewRow Then
                            Dim POSI As XmlElement = xmDocument.CreateElement("F")
                            MARKPOSITION.AppendChild(POSI)
                            Dim ID As XmlAttribute = xmDocument.CreateAttribute("id")
                            ID.Value = (i + 1).ToString
                            POSI.Attributes.Append(ID)
                            Dim SNO As XmlElement = xmDocument.CreateElement("SNO")
                            Dim SHAPE As XmlElement = xmDocument.CreateElement("SHAPES")
                            Dim F_X1 As XmlElement = xmDocument.CreateElement("F_X1")
                            Dim F_Y1 As XmlElement = xmDocument.CreateElement("F_Y1")
                            Dim F_RX2 As XmlElement = xmDocument.CreateElement("F_RX2")
                            Dim F_RY2 As XmlElement = xmDocument.CreateElement("F_RY2")
                            Dim F_CP As XmlElement = xmDocument.CreateElement("F_CP")
                            Dim X_OFF As XmlElement = xmDocument.CreateElement("X_OFF")
                            Dim Y_OFF As XmlElement = xmDocument.CreateElement("Y_OFF")
                            Dim F_PX As XmlElement = xmDocument.CreateElement("F_PX")
                            Dim F_PY As XmlElement = xmDocument.CreateElement("F_PY")
                            Dim THRES As XmlElement = xmDocument.CreateElement("F_THRES")
                            Dim TOLE As XmlElement = xmDocument.CreateElement("F_TOLE")
                            Dim BRIGH As XmlElement = xmDocument.CreateElement("F_BRIGH")
                            Dim SCOR As XmlElement = xmDocument.CreateElement("F_SCORE")
                            Dim SEL As XmlElement = xmDocument.CreateElement("SEL")







                            DATA(0) = datagrdFid.Rows(i).Cells(0).Value.ToString()
                            DATA(1) = datagrdFid.Rows(i).Cells(1).Value.ToString()
                            DATA(2) = datagrdFid.Rows(i).Cells(2).Value.ToString()
                            DATA(3) = datagrdFid.Rows(i).Cells(3).Value.ToString()
                            DATA(4) = datagrdFid.Rows(i).Cells(4).Value.ToString()
                            DATA(5) = datagrdFid.Rows(i).Cells(5).Value.ToString()
                            DATA(6) = datagrdFid.Rows(i).Cells(6).Value.ToString()
                            DATA(7) = datagrdFid.Rows(i).Cells(7).Value.ToString()
                            DATA(8) = datagrdFid.Rows(i).Cells(8).Value.ToString()
                            DATA(9) = datagrdFid.Rows(i).Cells(9).Value.ToString()
                            DATA(10) = datagrdFid.Rows(i).Cells(10).Value.ToString()
                            DATA(11) = datagrdFid.Rows(i).Cells(11).Value.ToString()
                            DATA(12) = datagrdFid.Rows(i).Cells(12).Value.ToString()
                            DATA(13) = datagrdFid.Rows(i).Cells(13).Value.ToString()
                            DATA(14) = datagrdFid.Rows(i).Cells(14).Value.ToString()
                            DATA(15) = datagrdFid.Rows(i).Cells(15).Value.ToString()









                            'If datagrdFid.Rows(i).Cells(11).Value = False Then
                            'DATA(11) = "False"
                            'Else
                            'DATA(11) = "True"
                            'End If





                            SNO.InnerText = DATA(0)
                            SHAPE.InnerText = DATA(1)
                            F_X1.InnerText = DATA(2)
                            F_Y1.InnerText = DATA(3)
                            F_RX2.InnerText = DATA(4)
                            F_RY2.InnerText = DATA(5)
                            F_CP.InnerText = DATA(6)
                            X_OFF.InnerText = DATA(7)
                            Y_OFF.InnerText = DATA(8)
                            F_PX.InnerText = DATA(9)
                            F_PY.InnerText = DATA(10)
                            THRES.InnerText = DATA(11)
                            TOLE.InnerText = DATA(12)
                            BRIGH.InnerText = DATA(13)
                            SCOR.InnerText = DATA(14)
                            SEL.InnerText = DATA(15)


                            POSI.AppendChild(SNO)
                            POSI.AppendChild(SHAPE)
                            POSI.AppendChild(F_X1)
                            POSI.AppendChild(F_Y1)
                            POSI.AppendChild(F_RX2)
                            POSI.AppendChild(F_RY2)
                            POSI.AppendChild(F_CP)
                            POSI.AppendChild(X_OFF)
                            POSI.AppendChild(Y_OFF)
                            POSI.AppendChild(F_PX)
                            POSI.AppendChild(F_PY)
                            POSI.AppendChild(THRES)
                            POSI.AppendChild(TOLE)
                            POSI.AppendChild(BRIGH)
                            POSI.AppendChild(SCOR)
                            POSI.AppendChild(SEL)
                            i = i + 1
                        End If
                    Next
                    xmDocument.Save(generatedFile)
                    xmDocument.Save(BaackupgeneratedFile)

                End If
            End If
            PictureBox7.Invalidate()
        Else
            MessageBox.Show("Please select a row to delete.")
        End If
        'Recipe delete
        ' Check if a row is selected

    End Sub

    Private Sub Guna2Button8_Click(sender As Object, e As EventArgs) Handles Guna2Button8.Click

        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the top
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Height > contractAmount Then ' Ensure the height doesn't go negative
                    square.TopLeft = New Point(square.TopLeft.X, square.TopLeft.Y + contractAmount)
                    square.Size = New Size(square.Size.Width, square.Size.Height - contractAmount)

                    ' Update the DataGridView to reflect the changes
                    datagrdFid.Rows(rowIndex).Cells(3).Value = square.TopLeft.Y ' Update the Y1 value
                    datagrdFid.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                    datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                    UpdateDataGridViewWithTextBoxValues(rowIndex)
                    ' Redraw PictureBox
                    PictureBox7.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button9_Click(sender As Object, e As EventArgs) Handles Guna2Button9.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the bottom
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Height > contractAmount Then ' Ensure the height doesn't go negative
                    square.Size = New Size(square.Size.Width, square.Size.Height - contractAmount)

                    ' Update the DataGridView to reflect the changes
                    datagrdFid.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                    datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                    UpdateDataGridViewWithTextBoxValues(rowIndex)
                    ' Redraw PictureBox
                    PictureBox7.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button5_Click(sender As Object, e As EventArgs) Handles Guna2Button5.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the bottom
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.Size = New Size(square.Size.Width, square.Size.Height + increaseAmount)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(5).Value = square.TopLeft.Y + square.Size.Height ' Update the Y2 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button7_Click(sender As Object, e As EventArgs) Handles Guna2Button7.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the left
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.TopLeft = New Point(square.TopLeft.X - increaseAmount, square.TopLeft.Y)
                square.Size = New Size(square.Size.Width + increaseAmount, square.Size.Height)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(2).Value = square.TopLeft.X ' Update the X1 value
                datagrdFid.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button11_Click(sender As Object, e As EventArgs) Handles Guna2Button11.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the left
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Width > contractAmount Then ' Ensure the width doesn't go negative
                    square.TopLeft = New Point(square.TopLeft.X + contractAmount, square.TopLeft.Y)
                    square.Size = New Size(square.Size.Width - contractAmount, square.Size.Height)

                    ' Update the DataGridView to reflect the changes
                    datagrdFid.Rows(rowIndex).Cells(2).Value = square.TopLeft.X ' Update the X1 value
                    datagrdFid.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                    datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                    UpdateDataGridViewWithTextBoxValues(rowIndex)
                    ' Redraw PictureBox
                    PictureBox7.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button10_Click(sender As Object, e As EventArgs) Handles Guna2Button10.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Contract the boundary from the right
                Dim contractAmount As Integer = 2 ' Amount by which the boundary should be contracted
                If square.Size.Width > contractAmount Then ' Ensure the width doesn't go negative
                    square.Size = New Size(square.Size.Width - contractAmount, square.Size.Height)

                    ' Update the DataGridView to reflect the changes
                    datagrdFid.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                    datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                    UpdateDataGridViewWithTextBoxValues(rowIndex)
                    ' Redraw PictureBox
                    PictureBox7.Invalidate()
                Else
                    MessageBox.Show("Cannot contract square further.")
                End If
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button6_Click(sender As Object, e As EventArgs) Handles Guna2Button6.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim square As Square = shapes.OfType(Of Square)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If square IsNot Nothing Then
                ' Increase the boundary from the right
                Dim increaseAmount As Integer = 2 ' Amount by which the boundary should be increased
                square.Size = New Size(square.Size.Width + increaseAmount, square.Size.Height)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(4).Value = square.TopLeft.X + square.Size.Width ' Update the X2 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({square.TopLeft.X + square.Size.Width / 2}, {square.TopLeft.Y + square.Size.Height / 2})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a square.")
            End If
        Else
            MessageBox.Show("Please select a square from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button12_Click(sender As Object, e As EventArgs) Handles Guna2Button12.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)
            If circle IsNot Nothing Then
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X, circle.Center.Y - moveAmount)
                datagrdFid.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point
                PictureBox7.Invalidate()
                UpdateDataGridViewWithTextBoxValues(rowIndex)
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button14_Click(sender As Object, e As EventArgs) Handles Guna2Button14.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Move the circle to the left
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X + moveAmount, circle.Center.Y)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button15_Click(sender As Object, e As EventArgs) Handles Guna2Button15.Click
        If datagrdFid.SelectedRows.Count > 0 Then

            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Move the circle downwards
                Dim moveAmount As Integer = 2
                circle.Center = New Point(circle.Center.X, circle.Center.Y + moveAmount)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button13_Click(sender As Object, e As EventArgs) Handles Guna2Button13.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Move the circle to the left
                Dim moveAmount As Integer = 2 ' Amount by which the circle should be moved
                circle.Center = New Point(circle.Center.X - moveAmount, circle.Center.Y)

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                datagrdFid.Rows(rowIndex).Cells(6).Value = $"({circle.Center.X}, {circle.Center.Y})" ' Update the center point
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button16_Click(sender As Object, e As EventArgs) Handles Guna2Button16.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Increase the size of the circle
                Dim increaseAmount As Integer = 2 ' Amount by which the radius should be increased
                circle.Radius += increaseAmount

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                datagrdFid.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                datagrdFid.Rows(rowIndex).Cells(4).Value = circle.Radius * 2 ' Update the width (diameter)
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub

    Private Sub Guna2Button17_Click(sender As Object, e As EventArgs) Handles Guna2Button17.Click
        If datagrdFid.SelectedRows.Count > 0 Then
            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding shape ID
            Dim selectedShapeID As Integer = rowIndex + 1

            ' Find the shape with the corresponding ID
            Dim circle As Circle = shapes.OfType(Of Circle)().FirstOrDefault(Function(s) s.ID = selectedShapeID)

            If circle IsNot Nothing Then
                ' Increase the size of the circle
                Dim increaseAmount As Integer = 2 ' Amount by which the radius should be increased
                circle.Radius -= increaseAmount

                ' Update the DataGridView to reflect the changes
                datagrdFid.Rows(rowIndex).Cells(2).Value = circle.Center.X - circle.Radius ' Update the X1 value
                datagrdFid.Rows(rowIndex).Cells(3).Value = circle.Center.Y - circle.Radius ' Update the Y1 value
                datagrdFid.Rows(rowIndex).Cells(4).Value = circle.Radius * 2 ' Update the width (diameter)
                UpdateDataGridViewWithTextBoxValues(rowIndex)
                ' Redraw PictureBox
                PictureBox7.Invalidate()
            Else
                MessageBox.Show("Selected shape is not a circle.")
            End If
        Else
            MessageBox.Show("Please select a circle from the DataGridView.")
        End If
    End Sub



    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim progname As String
        progname = txt_Sel_Prog_name.Text
        If progname = "" Then
            Return
        ElseIf (progname IsNot "") Then
            Dim isValid1 As Boolean = True
            Dim isValid As Boolean = True
            Dim fname As String = "" & ConfigurationManager.AppSettings("ReceipeFilepath").ToString().Trim()
            Dim Backupfname As String = "" & ConfigurationManager.AppSettings("BackupReceipeFilepath").ToString().Trim()
            Dim path As String = fname
            Dim Logdir As String = "" & fname
            Dim BackupLogdir As String = "" & Backupfname
            Dim ReceipeFileName As String = String.Empty
            If Not Directory.Exists(BackupLogdir) Then
                Directory.CreateDirectory(BackupLogdir)
            End If
            If Not Directory.Exists(Logdir) Then
                Directory.CreateDirectory(Logdir)
            End If
            ReceipeFileName = progname
            Dim generatedFile As String = Logdir & ReceipeFileName & ".xml"
            Dim BaackupgeneratedFile As String = BackupLogdir & ReceipeFileName & ".xml"
            Dim side As String = cmbpanelside.Text
            Dim xmDocument As XmlDocument = New XmlDocument()
            xmDocument.Load(generatedFile)
            Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/FIDUCIAL")
            MARKPOSITION.RemoveAll()
            xmDocument.Save(generatedFile)
            xmDocument.Save(BaackupgeneratedFile)
            Dim i As Integer = 0
            Dim DATA(20) As String
            For Each row As DataGridViewRow In datagrdFid.Rows
                If Not row.IsNewRow Then
                    Dim POSI As XmlElement = xmDocument.CreateElement("F")
                    MARKPOSITION.AppendChild(POSI)
                    Dim ID As XmlAttribute = xmDocument.CreateAttribute("id")
                    ID.Value = (i + 1).ToString
                    POSI.Attributes.Append(ID)
                    Dim SNO As XmlElement = xmDocument.CreateElement("SNO")
                    Dim SHAPE As XmlElement = xmDocument.CreateElement("SHAPES")
                    Dim F_X1 As XmlElement = xmDocument.CreateElement("F_X1")
                    Dim F_Y1 As XmlElement = xmDocument.CreateElement("F_Y1")
                    Dim F_RX2 As XmlElement = xmDocument.CreateElement("F_RX2")
                    Dim F_RY2 As XmlElement = xmDocument.CreateElement("F_RY2")
                    Dim F_CP As XmlElement = xmDocument.CreateElement("F_CP")
                    Dim X_OFF As XmlElement = xmDocument.CreateElement("X_OFF")
                    Dim Y_OFF As XmlElement = xmDocument.CreateElement("Y_OFF")
                    Dim F_PX As XmlElement = xmDocument.CreateElement("F_PX")
                    Dim F_PY As XmlElement = xmDocument.CreateElement("F_PY")
                    Dim THRES As XmlElement = xmDocument.CreateElement("F_THRES")
                    Dim TOLE As XmlElement = xmDocument.CreateElement("F_TOLE")
                    Dim BRIGH As XmlElement = xmDocument.CreateElement("F_BRIGH")
                    Dim SCOR As XmlElement = xmDocument.CreateElement("F_SCORE")
                    Dim SEL As XmlElement = xmDocument.CreateElement("SEL")

                    DATA(0) = datagrdFid.Rows(i).Cells(0).Value.ToString()
                    DATA(1) = datagrdFid.Rows(i).Cells(1).Value.ToString()
                    DATA(2) = datagrdFid.Rows(i).Cells(2).Value.ToString()
                    DATA(3) = datagrdFid.Rows(i).Cells(3).Value.ToString()
                    DATA(4) = datagrdFid.Rows(i).Cells(4).Value.ToString()
                    DATA(5) = datagrdFid.Rows(i).Cells(5).Value.ToString()
                    DATA(6) = datagrdFid.Rows(i).Cells(6).Value.ToString()
                    DATA(7) = datagrdFid.Rows(i).Cells(7).Value.ToString()
                    DATA(8) = datagrdFid.Rows(i).Cells(8).Value.ToString()
                    DATA(9) = datagrdFid.Rows(i).Cells(9).Value.ToString()
                    DATA(10) = datagrdFid.Rows(i).Cells(10).Value.ToString()
                    DATA(11) = datagrdFid.Rows(i).Cells(11).Value.ToString()
                    DATA(12) = datagrdFid.Rows(i).Cells(12).Value.ToString()
                    DATA(13) = datagrdFid.Rows(i).Cells(13).Value.ToString()
                    DATA(14) = datagrdFid.Rows(i).Cells(14).Value.ToString()
                    DATA(15) = datagrdFid.Rows(i).Cells(15).Value.ToString()









                    'If datagrdFid.Rows(i).Cells(11).Value = False Then
                    'DATA(11) = "False"
                    'Else
                    'D'ATA(11) = "True"
                    'End If





                    SNO.InnerText = DATA(0)
                    SHAPE.InnerText = DATA(1)
                    F_X1.InnerText = DATA(2)
                    F_Y1.InnerText = DATA(3)
                    F_RX2.InnerText = DATA(4)
                    F_RY2.InnerText = DATA(5)
                    F_CP.InnerText = DATA(6)
                    X_OFF.InnerText = DATA(7)
                    Y_OFF.InnerText = DATA(8)
                    F_PX.InnerText = DATA(9)
                    F_PY.InnerText = DATA(10)
                    THRES.InnerText = DATA(11)
                    TOLE.InnerText = DATA(12)
                    BRIGH.InnerText = DATA(13)
                    SCOR.InnerText = DATA(14)
                    SEL.InnerText = DATA(15)


                    POSI.AppendChild(SNO)
                    POSI.AppendChild(SHAPE)
                    POSI.AppendChild(F_X1)
                    POSI.AppendChild(F_Y1)
                    POSI.AppendChild(F_RX2)
                    POSI.AppendChild(F_RY2)
                    POSI.AppendChild(F_CP)
                    POSI.AppendChild(X_OFF)
                    POSI.AppendChild(Y_OFF)
                    POSI.AppendChild(F_PX)
                    POSI.AppendChild(F_PY)
                    POSI.AppendChild(THRES)
                    POSI.AppendChild(TOLE)
                    POSI.AppendChild(BRIGH)
                    POSI.AppendChild(SCOR)
                    POSI.AppendChild(SEL)
                    i = i + 1
                End If
            Next
            xmDocument.Save(generatedFile)
            xmDocument.Save(BaackupgeneratedFile)
        End If
    End Sub

    Private Sub RadioButtonTriggerOff_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTriggerOff.Click


        RadioButtonTriggerOff.Checked = True
        RadioButtonTriggerOn.Checked = False
        ButtonSoftwareOnce.Enabled = False
        RadioButtonTriggerOff.Enabled = False
        RadioButtonTriggerOn.Enabled = True
        Dim nRet As Int32 = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0)
        nRet = Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF)




    End Sub

    Private Sub RadioButtonTriggerOn_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonTriggerOn.Click


        RadioButtonTriggerOff.Enabled = True
        RadioButtonTriggerOn.Checked = True
        RadioButtonTriggerOff.Checked = False
        RadioButtonTriggerOn.Enabled = False
        ButtonSoftwareOnce.Enabled = True
        Dim nRet As Int32 = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
        nRet = Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)



    End Sub

    Private Sub ButtonSoftwareOnce_Click(sender As Object, e As EventArgs) Handles ButtonSoftwareOnce.Click


        Dim nRet As Int32
        nRet = Home_Page.FidCam1.SetCommandValue("TriggerSoftware")
        ' Dim nRet As Int32
        'nRet = Home_Page.FidCam1.SetCommandValue("TriggerSoftware")
        ' If CCamera.MV_OK <> nRet Then
        'MsgBox("Fail to software trigger once")
        'End If
        'Dim fil As String() = Directory.GetFiles("D:\Logs\fidimage\load\", "Image_w.Bmp", System.IO.SearchOption.AllDirectories)


        ' pic = New Bitmap("D:\Logs\fidimage\Image_w.Bmp")
        'System.IO.File.Delete("D:\Logs\fidimage\Image_w.Bmp")
        'PictureBox7.Image = newImg

        'If ss = True Then
        '    File.Replace("D:\Logs\fidimage\Image_w.Bmp", "D:\Logs\fidimage\load\Image_w.Bmp", "D:\Logs\fidimage\load\Image_w1.Bmp")
        '    'My.Computer.FileSystem.CopyFile("D:\Logs\fidimage\Image_w.Bmp", "D:\Logs\fidimage\load\Image_w1.Bmp")
        '    PictureBox7.Image = New Bitmap("D:\Logs\fidimage\load\Image_w1.Bmp")
        '    'File.Exists("D:\Logs\fidimage\load\Image_w.Bmp")
        '    Thread.Sleep(100)

        '    'File.Delete("D:\Logs\fidimage\load\Image_w.Bmp")
        'ElseIf ss1 = True Then
        '    'My.Computer.FileSystem.CopyFile("D:\Logs\fidimage\Image_w.Bmp", "D:\Logs\fidimage\load\Image_w.Bmp")
        '    PictureBox7.Image = New Bitmap("D:\Logs\fidimage\load\Image_w.Bmp")
        '    File.Exists("D:\Logs\fidimage\load\Image_w1.Bmp")
        '    Thread.Sleep(100)


        '    File.Delete("D:\Logs\fidimage\load\Image_w1.Bmp")
        'Else
        '    My.Computer.FileSystem.CopyFile("D:\Logs\fidimage\Image_w.Bmp", "D:\Logs\fidimage\load\Image_w.Bmp")
        '    PictureBox7.Image = New Bitmap("D:\Logs\fidimage\load\Image_w.Bmp")

        '    Thread.Sleep(100)

        'End If



    End Sub
    Private Sub Recipe_Leave(sender As Object, e As EventArgs) Handles MyBase.Leave
        Home_Page.LiveCamera1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)
        Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)
        plc.SetDevice("M247", 0)  ''''''''''' RED LIGHT
        Thread.Sleep(100)
    End Sub
    Private Sub SaveImage(shapeToSave As Shape, folderPath As String, imageName As String)

        ' Determine bounding box
        Dim boundingBox As Rectangle
        If TypeOf shapeToSave Is Square Then
            Dim square As Square = CType(shapeToSave, Square)
            ' Calculate the center point

            boundingBox = New Rectangle((square.TopLeft.X) * 4, (square.TopLeft.Y) * 4, (square.Size.Width) * 4, (square.Size.Height) * 4)
        ElseIf TypeOf shapeToSave Is Circle Then
            Dim circle As Circle = CType(shapeToSave, Circle)
            ' Scale the radius by 4
            Dim scaledRadius As Integer = circle.Radius * 4
            Dim centrX As Integer = (circle.Center.X) * 4
            Dim centrY As Integer = (circle.Center.Y) * 4

            boundingBox = New Rectangle((centrX - scaledRadius), (centrY - scaledRadius), (scaledRadius * 2), (scaledRadius * 2))
        Else
            MsgBox("Unknown shape type.")
            Return
        End If

        ' Create a bitmap and draw the image
        Dim bitmap As New Bitmap(boundingBox.Width, boundingBox.Height)
        Dim img As New Bitmap("C:\Manage Files\Load\123.Png")
        Using g As Graphics = Graphics.FromImage(bitmap)
            Dim sourceRect As New Rectangle(boundingBox.Location, boundingBox.Size)
            Dim destRect As New Rectangle(0, 0, boundingBox.Width, boundingBox.Height)
            If PictureBox7.Image IsNot Nothing Then
                g.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel)
                img.Dispose()
            Else
                MsgBox("Live camera feed is not available.")
                Return
            End If
        End Using

        ' Ensure the directory exists
        If Not Directory.Exists(folderPath) Then Directory.CreateDirectory(folderPath)

        ' Define the save path
        Dim savePath As String = Path.Combine(folderPath, imageName)

        ' Save the bitmap to the specified path
        Try
            bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png)
            MsgBox("Image saved successfully to " & savePath)
        Catch ex As Exception
            MsgBox("Error saving the image: " & ex.Message)
        End Try
    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim shapeToSave As Shape
        Try
            ' Check if any row is selected in the DataGridView
            If datagrdFid.SelectedRows.Count = 0 Then
                MsgBox("Please select a shape from the DataGridView.")
                Return
            End If

            ' Get the selected row index
            Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index

            ' Get the corresponding S.No from the selected row
            Dim selectedSNo As String = datagrdFid.Rows(rowIndex).Cells(0).Value.ToString()

            ' Get the corresponding shape ID
            Dim selectedShapeID As String = datagrdFid.Rows(rowIndex).Cells(0).Value.ToString()

            ' Find the shape with the corresponding ID
            shapeToSave = shapes.FirstOrDefault(Function(s) s.ID = (rowIndex + 1))

            If shapeToSave Is Nothing Then
                MsgBox("Selected shape not found.")
                Return
            End If

            ' Determine the bounding box of the shape
            Dim boundingBox As Rectangle
            If TypeOf shapeToSave Is Square Then
                Dim square As Square = CType(shapeToSave, Square)
                boundingBox = New Rectangle((square.TopLeft.X), (square.TopLeft.Y), (square.Size.Width), (square.Size.Height))
            ElseIf TypeOf shapeToSave Is Circle Then
                Dim circle As Circle = CType(shapeToSave, Circle)
                boundingBox = New Rectangle((circle.Center.X - circle.Radius), (circle.Center.Y - circle.Radius), (circle.Radius * 2), (circle.Radius * 2))
            Else
                MsgBox("Unknown shape type.")
                Return
            End If

            ' Create a new bitmap with the size of the bounding box
            Dim bitmap As New Bitmap(boundingBox.Width, boundingBox.Height)

            ' Draw the relevant portion of the live camera PictureBox image onto the new bitmap
            Using g As Graphics = Graphics.FromImage(bitmap)
                ' Adjust the source rectangle to match the shape's bounding box within the PictureBox
                Dim sourceRect As New Rectangle(boundingBox.Location, boundingBox.Size)
                Dim destRect As New Rectangle(0, 0, boundingBox.Width, boundingBox.Height)

                ' Check if the live camera feed image is available
                If PictureBox7.Image IsNot Nothing Then
                    ' Draw the image from Guna2PictureBox1, which contains the live camera feed
                    g.DrawImage(PictureBox7.Image, destRect, sourceRect, GraphicsUnit.Pixel)
                Else
                    MsgBox("Live camera feed is not available.")
                    Return
                End If

            End Using

            ' Construct the directory path based on TextBox3 value
            Dim folderName As String = txt_Sel_Prog_name.Text.Trim()
            Dim directoryPath As String

            If String.IsNullOrEmpty(folderName) Then
                ' If TextBox3 is empty, use the default path
                directoryPath = "D:\ff\Wrong"
            Else
                ' Use the folder name from TextBox3
                directoryPath = Path.Combine("C:\Manage Files\Fid_Image", folderName)

                ' Create the directory if it doesn't exist
                If Not Directory.Exists(directoryPath) Then
                    Directory.CreateDirectory(directoryPath)
                End If
            End If

            ' Save the new bitmap to the specified path with the S.No as filename
            Dim savePath As String = Path.Combine(directoryPath, selectedSNo & ".png")
            Try
                bitmap.Save(savePath, System.Drawing.Imaging.ImageFormat.Png)
                MsgBox("Image saved successfully to " & savePath)
            Catch ex As Exception
                MsgBox("Error saving the image: " & ex.Message)
            End Try
        Catch ex As Exception

        End Try
        'here we startted saving
        Try
            Dim folderPath As String = "C:\Manage Files\Template_Image"
            Dim selectedFileName As String = datagrdFid.SelectedRows(0).Cells(0).Value.ToString()

            ' Call the SaveImage function
            SaveImage(shapeToSave, folderPath, selectedFileName & ".Png")
        Catch ex As Exception

        End Try
        'Try
        '    If PictureBox7.Image IsNot Nothing Then
        '        Try
        '            ' Define the save path
        '            Dim savePath As String = "D:\mainimage\image.png"

        '            ' Save the image in the specified path with the .png extension
        '            PictureBox7.Image.Save(savePath, ImageFormat.Png)

        '            MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '        Catch ex As Exception
        '            MessageBox.Show("Failed to save the image. Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        '        End Try
        '    Else
        '        MessageBox.Show("No image to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        '    End If
        'Catch ex As Exception

        'End Try
        Try
            ' Check if a row is selected
            If datagrdFid.SelectedRows.Count > 0 Then
                ' Get the selected row
                Dim selectedRow As DataGridViewRow = datagrdFid.SelectedRows(0)

                ' Extract the value from column 6
                Dim column6Value As String = selectedRow.Cells(6).Value.ToString()
                Dim parts() As String = column6Value.Trim("()").Split(","c)
                'cnter = VALUE(6).Split("(", ","c)
                'Dim part As String = "45)"
                'Dim part1() As String = part.Trim("()")

                Dim trimmedPart() As String = parts(1).Split(")"c)

                Dim centx As Integer = Convert.ToInt16(parts(0))
                Dim centy As Integer = Convert.ToInt16(trimmedPart(0))
                centx = centx * 4
                centy = centy * 4
                Dim valu As String = ("(" & centx & "," & centy & ")")
                ' Extract the name for the text file from the selected row (e.g., from column 1)
                Dim fileName As String = "text" & ".txt"

                ' Define the file path
                Dim filePath As String = "C:\Manage Files\centre Point\" & fileName

                ' Write the value to the text file
                Try
                    System.IO.File.WriteAllText(filePath, valu)
                    MessageBox.Show("Value stored in " & filePath)
                Catch ex As Exception
                    MessageBox.Show("Error writing to file: " & ex.Message)
                End Try
            Else
                MessageBox.Show("Please select a row first.")
            End If
        Catch ex As Exception

        End Try
        Try
            ' Define the folder path
            Dim folderPath As String = "C:\Manage Files\image_name"

            ' Ensure the folder exists, if not, create it
            If Not System.IO.Directory.Exists(folderPath) Then
                System.IO.Directory.CreateDirectory(folderPath)
            End If

            ' Define the specific text for the filename
            Dim specificFileName As String = "text.txt"

            ' Get the selected row's cell (0) value
            Dim cellValue As String = datagrdFid.SelectedRows(0).Cells(0).Value.ToString()

            ' Construct the full file path
            Dim filePath As String = System.IO.Path.Combine(folderPath, specificFileName)

            ' Write the value to the text file
            System.IO.File.WriteAllText(filePath, cellValue)

            ' Inform the user
            MessageBox.Show("File saved successfully at " & filePath)
        Catch ex As Exception

        End Try
    End Sub

    Private imageLoadCount As Integer = 0
    Private Sub datagrdFid_CellContentDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagrdFid.CellContentDoubleClick
        ' Check if a valid row was clicked
        If e.RowIndex < 0 Then
            Exit Sub
        End If

        ' Check if TextBox3 is empty
        Dim folderName As String = txt_Sel_Prog_name.Text.Trim()
        If String.IsNullOrEmpty(folderName) Then
            MessageBox.Show("Please select a recipe first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        ' Get the S.No from the clicked row
        Dim selectedRow As DataGridViewRow = datagrdFid.Rows(e.RowIndex)
        Dim serialNumber As String = selectedRow.Cells(0).Value.ToString()

        ' Determine the directory path based on TextBox3 value
        Dim directoryPath As String = Path.Combine("C:\Manage Files\Fid_Image", folderName)

        ' Define the image file path based on S.No
        Dim imagePath As String = Path.Combine(directoryPath, serialNumber & ".png")

        ' Load the image into the PictureBox
        If imageLoadCount = 0 Then
            If File.Exists(imagePath) Then
                Guna2PictureBox1.Image = New Bitmap(imagePath)
                imageLoadCount += 1
            Else
                MessageBox.Show("Image not found for S.No " & serialNumber, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        ElseIf imageLoadCount = 1 Then
            Dim result As DialogResult = MessageBox.Show("Do you want to load the second image?", "Confirmation", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                If File.Exists(imagePath) Then
                    Guna2PictureBox2.Image = New Bitmap(imagePath)
                    imageLoadCount += 1
                Else
                    MessageBox.Show("Image not found for S.No " & serialNumber, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End If
        ElseIf imageLoadCount >= 2 Then
            Dim resetResult As DialogResult = MessageBox.Show("You have loaded two images. Do you want to reset?", "Reset", MessageBoxButtons.YesNo)
            If resetResult = DialogResult.Yes Then
                Guna2PictureBox1.Image = Nothing
                Guna2PictureBox2.Image = Nothing
                imageLoadCount = 0 ' Reset the counter
            End If
        End If
    End Sub

    Private Sub Learn_Click(sender As Object, e As EventArgs) Handles Learn.Click


        Dim rowIndex As Integer = datagrdFid.SelectedRows(0).Index
        datagrdFid.Rows(rowIndex).Cells(9).Value = X_Current.Text
        datagrdFid.Rows(rowIndex).Cells(10).Value = Y_Current.Text
        datagrdFid.Rows(rowIndex).Cells(11).Value = TextBox3.Text
        datagrdFid.Rows(rowIndex).Cells(12).Value = TextBox2.Text
        datagrdFid.Rows(rowIndex).Cells(13).Value = TextBox4.Text
        datagrdFid.Rows(rowIndex).Cells(14).Value = TextBox1.Text
    End Sub

    Private Sub PictureBox7_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox7.MouseUp

        If isDrawing Then
            ' Check the maximum current ID from the DataGridView
            'Dim maxCurrentID As Integer = GetMaxCurrentID()
            'If maxCurrentID >= currentID Then
            '    currentID = maxCurrentID + 1
            'End If
            Dim IND As Integer = datagrdFid.SelectedRows(0).Index
            'Dim 
            currentID = IND + 1
            If shapes.Count < datagrdFid.Rows.Count Then ' Subtract 1 to account for the new row placeholder
                If drawSquares Then
                    ' Draw square as before
                    Dim size As New Size(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y))
                    Dim topLeft As New Point(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y))
                    shapes.Add(New Square(topLeft, size, currentID, currentColor))
                    AddShapeToDataGridView(currentID, "Square", topLeft.X, topLeft.Y, size.Width, size.Height)
                Else
                    ' Draw circle with dynamic size
                    Dim radius As Integer = CInt(Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)) / 2)
                    Dim centerX As Integer = Math.Min(startPoint.X, endPoint.X) + radius
                    Dim centerY As Integer = Math.Min(startPoint.Y, endPoint.Y) + radius
                    shapes.Add(New Circle(New Point(centerX, centerY), radius, currentID, currentColor))

                    AddShapeToDataGridView(currentID, "Circle", centerX - radius, centerY - radius, radius * 2, 0, centerX, centerY)
                End If

                currentID += 1
                isDrawing = False
                Guna2PictureBox1.Invalidate()
            Else

                MessageBox.Show("You cannot draw more shapes than the number of rows in the DataGridView.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End If

    End Sub
    Public Sub RunPythonScript()
        Dim pythonPath As String = "C:/Users/HP/AppData/Local/Programs/Python/Python310/python.exe"
        Dim scriptPath As String = "C:\Users\HP\Downloads\test4.py"
        Dim imagePath As String = String.Empty ' Update if needed

        Dim startInfo As New ProcessStartInfo(pythonPath)
        startInfo.Arguments = """" & scriptPath & """ " & imagePath
        startInfo.UseShellExecute = False
        startInfo.RedirectStandardOutput = True
        startInfo.RedirectStandardError = True

        Dim process As New Process()
        process.StartInfo = startInfo
        process.Start()

        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim [error] As String = process.StandardError.ReadToEnd()

        process.WaitForExit()

        ' Refined regex pattern to handle optional negative signs
        Dim regex As New Regex("Offset: \((?<x>-?\d+), (?<y>-?\d+)\)")
        Dim match As Match = regex.Match(output)

        If match.Success Then
            ' Extract coordinates from regex groups
            Dim x As String = match.Groups("x").Value
            Dim y As String = match.Groups("y").Value

            ' Update UI with coordinates
            'TextBox2.Text = $"Offset: ({x}, {y})"

            datagrdFid.SelectedRows(0).Cells(7).Value = x
            datagrdFid.SelectedRows(0).Cells(8).Value = y


        Else
            TextBox2.Text = "No offset found"
        End If

        ' Display the output and error in the console for debugging
        Console.WriteLine("Output: " & output)
        Console.WriteLine("Error: " & [error])

        ' Optionally handle the disposed image if needed
        ' Dispose of the previous image if it exists
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        Try
            ' Check if a row is selected
            If datagrdFid.SelectedRows.Count > 0 Then
                'Get the selected row
                Dim selectedRow As DataGridViewRow = datagrdFid.SelectedRows(0)

                'Extract the value from column 6
                Dim column6Value As String = selectedRow.Cells(6).Value.ToString()
                Dim parts() As String = column6Value.Trim("()").Split(","c)
                'Dim cnter As String = VALUE(6).Split("(", ","c)
                'Dim part As String = "45)"
                'Dim part1() As String = parts(1).Trim("()")

                Dim trimmedPart() As String = parts(1).Split(")"c)

                Dim centx As Integer = Convert.ToInt16(parts(0))
                Dim centy As Integer = Convert.ToInt16(trimmedPart(0))
                centx = centx * 4
                centy = centy * 4
                Dim valu As String = ("(" & centx & "," & centy & ")")
                ' Extract the name for the text file from the selected row (e.g., from column 1)
                Dim fileName As String = "text" & ".txt"

                ' Define the file path
                Dim filePath As String = "C:\Manage Files\centre Point\" & fileName

                ' Write the value to the text file
                Try
                    System.IO.File.WriteAllText(filePath, valu)
                    MessageBox.Show("Value stored in " & filePath)
                Catch ex As Exception
                    MessageBox.Show("Error writing to file: " & ex.Message)
                End Try
            Else
                MessageBox.Show("Please select a row first.")
            End If
        Catch ex As Exception

        End Try




        Try
            RunPythonScript()
        Catch ex As Exception

        End Try
        Try
            If pythonProcess IsNot Nothing AndAlso Not pythonProcess.HasExited Then
                pythonProcess.Kill()
                pythonProcess.Dispose()
                pythonProcess = Nothing
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PictureBox7_Click(sender As Object, e As EventArgs) Handles PictureBox7.Click

    End Sub

    Private Sub datagrdFid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles datagrdFid.CellClick
        TextBox3.Text = datagrdFid.SelectedRows(0).Cells(11).Value
        TextBox2.Text = datagrdFid.SelectedRows(0).Cells(12).Value
        TextBox4.Text = datagrdFid.SelectedRows(0).Cells(13).Value
        TextBox1.Text = datagrdFid.SelectedRows(0).Cells(14).Value

    End Sub

    Private Sub Button24_MouseDown(sender As Object, e As MouseEventArgs) Handles Button24.MouseDown
        plc.SetDevice("M200", 1)
    End Sub

    Private Sub Button24_MouseUp(sender As Object, e As MouseEventArgs) Handles Button24.MouseUp
        plc.SetDevice("M200", 0)
    End Sub

    Private Sub Button23_MouseDown(sender As Object, e As MouseEventArgs) Handles Button23.MouseDown
        plc.SetDevice("M201", 1)
    End Sub

    Private Sub Button23_MouseUp(sender As Object, e As MouseEventArgs) Handles Button23.MouseUp
        plc.SetDevice("M201", 0)
    End Sub

    Private Sub Button21_MouseDown(sender As Object, e As MouseEventArgs) Handles Button21.MouseDown
        plc.SetDevice("M206", 1)
    End Sub

    Private Sub Button21_MouseUp(sender As Object, e As MouseEventArgs) Handles Button21.MouseUp
        plc.SetDevice("M206", 0)
    End Sub

    Private Sub Button22_MouseDown(sender As Object, e As MouseEventArgs) Handles Button22.MouseDown
        plc.SetDevice("M205", 1)
    End Sub

    Private Sub Button22_MouseUp(sender As Object, e As MouseEventArgs) Handles Button22.MouseUp
        plc.SetDevice("M205", 0)
    End Sub

    Private Sub Guna2ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox3.SelectedIndexChanged

    End Sub

    Private Sub Guna2ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Guna2ComboBox4.SelectedIndexChanged
        Dim selectedValue As String = Guna2ComboBox4.SelectedItem.ToString()
        Select Case selectedValue
            Case "HIGH"
                plc.SetDevice("D358", 3)
            Case "MEDIUM"
                plc.SetDevice("D358", 2)
            Case "LOW"
                plc.SetDevice("D358", 1)
        End Select
    End Sub



    Private Sub btLoadpos_Click(sender As Object, e As EventArgs) Handles btLoadpos.Click

    End Sub

    Private Sub RadioButton2_MouseDown(sender As Object, e As MouseEventArgs) Handles RadioButton2.MouseDown
        plc.SetDevice("M237", 1)
    End Sub

    Private Sub RadioButton2_MouseUp(sender As Object, e As MouseEventArgs) Handles RadioButton2.MouseUp
        plc.SetDevice("M237", 0)
    End Sub

    Private Sub LiveTriggerOff_CheckedChanged(sender As Object, e As EventArgs) Handles LiveTriggerOff.Click

        LiveTriggerOn.Enabled = True
        LiveTriggerOn.Checked = False
        LiveTriggerOff.Enabled = False
        LiveTriggerOff.Checked = True
        LiveTriggerOnce.Enabled = False

        Dim nRet As Int32 = Home_Page.LiveCamera1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0)
        nRet = Home_Page.LiveCamera1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF)


    End Sub

    Private Sub LiveTriggerOn_CheckedChanged(sender As Object, e As EventArgs) Handles LiveTriggerOn.Click

        LiveTriggerOn.Enabled = False
        LiveTriggerOn.Checked = True
        LiveTriggerOff.Enabled = True
        LiveTriggerOff.Checked = False
        LiveTriggerOnce.Enabled = True
        Dim nRet As Int32 = Home_Page.LiveCamera1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
        nRet = Home_Page.LiveCamera1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)


    End Sub

    Private Sub LiveTriggerOnce_Click(sender As Object, e As EventArgs) Handles LiveTriggerOnce.Click
        Dim nRet As Int32
        nRet = Home_Page.LiveCamera1.SetCommandValue("TriggerSoftware")
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        If LiveTriggerOn.Checked Then
            plc.SetDevice("M247", 1)  ''''''''''' RED LIGHT
            plc.SetDevice("M246", 0)
            'Thread.Sleep(100)
        Else
            plc.SetDevice("M247", 0)
        End If
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked Then
            plc.SetDevice("M246", 1)
            plc.SetDevice("M247", 0)
        Else
            plc.SetDevice("M246", 0)
        End If

    End Sub







    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick

        Dim ind As Integer
        Dim selectedIndex As Integer = Guna2TabControl1.SelectedIndex
        If selectedIndex = 1 Then
            lIVETeachAsync()


        ElseIf selectedIndex = 2 Then
            lIVEAsync()
        End If

    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick

        Dim selectedIndex As Integer = Guna2TabControl1.SelectedIndex
        If selectedIndex = 2 Then
            FidAsync()



        End If


    End Sub


    Private Async Sub lIVETeachAsync()
        Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        Dim nRet As Int32 = Home_Page.LiveCamera1.GetImageBuffer(stFrameOut, 1000)

        If CCamera.MV_OK = nRet Then

            If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver Then
                m_nBufSizeForDriver = stFrameOut.stFrameInfo.nFrameLen
                ReDim m_pBufForDriver(m_nBufSizeForDriver)
            End If
            m_stFrameInfoEx = stFrameOut.stFrameInfo
            Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver, 0, stFrameOut.stFrameInfo.nFrameLen)

            stDisplayInfo.hWnd = livepic1.Handle
            stDisplayInfo.pData = stFrameOut.pBufAddr
            stDisplayInfo.nDataLen = stFrameOut.stFrameInfo.nFrameLen
            stDisplayInfo.nWidth = stFrameOut.stFrameInfo.nWidth
            stDisplayInfo.nHeight = stFrameOut.stFrameInfo.nHeight
            stDisplayInfo.enPixelType = stFrameOut.stFrameInfo.enPixelType









            Home_Page.LiveCamera1.DisplayOneFrame(stDisplayInfo)
            Home_Page.LiveCamera1.FreeImageBuffer(stFrameOut)

        End If

    End Sub
    Private Async Sub lIVEAsync()
        Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        Dim nRet As Int32 = Home_Page.LiveCamera1.GetImageBuffer(stFrameOut, 1000)

        If CCamera.MV_OK = nRet Then

            If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver Then
                m_nBufSizeForDriver = stFrameOut.stFrameInfo.nFrameLen
                ReDim m_pBufForDriver(m_nBufSizeForDriver)
            End If
            m_stFrameInfoEx = stFrameOut.stFrameInfo
            Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver, 0, stFrameOut.stFrameInfo.nFrameLen)
            stDisplayInfo.hWnd = livepic.Handle

            stDisplayInfo.pData = stFrameOut.pBufAddr
            stDisplayInfo.nDataLen = stFrameOut.stFrameInfo.nFrameLen
            stDisplayInfo.nWidth = stFrameOut.stFrameInfo.nWidth
            stDisplayInfo.nHeight = stFrameOut.stFrameInfo.nHeight
            stDisplayInfo.enPixelType = stFrameOut.stFrameInfo.enPixelType








            Home_Page.LiveCamera1.DisplayOneFrame(stDisplayInfo)
            Home_Page.LiveCamera1.FreeImageBuffer(stFrameOut)

        End If

    End Sub
    Private Async Sub FidAsync()
        Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        Dim nRet1 As Int32 = Home_Page.FidCam1.GetImageBuffer(stFrameOut, 1000)
        If CCamera.MV_OK = nRet1 Then

            If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver1 Then
                m_nBufSizeForDriver1 = stFrameOut.stFrameInfo.nFrameLen
                ReDim m_pBufForDriver1(m_nBufSizeForDriver1)
            End If

            m_stFrameInfoEx = stFrameOut.stFrameInfo
            Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver1, 0, stFrameOut.stFrameInfo.nFrameLen)

            Dim stSaveImageParam As CCamera.MV_SAVE_IMG_TO_FILE_PARAM = New CCamera.MV_SAVE_IMG_TO_FILE_PARAM()
            Dim pData As IntPtr = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForDriver1, 0)
            stSaveImageParam.pData = pData
            stSaveImageParam.nDataLen = m_stFrameInfoEx.nFrameLen
            stSaveImageParam.enPixelType = m_stFrameInfoEx.enPixelType
            stSaveImageParam.nWidth = m_stFrameInfoEx.nWidth
            stSaveImageParam.nHeight = m_stFrameInfoEx.nHeight
            stSaveImageParam.enImageType = CCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Png
            stSaveImageParam.iMethodValue = 2
            stSaveImageParam.nQuality = 90
            stSaveImageParam.pImagePath = "C:\Manage Files\Load\" & "123" & ".Png"

            Thread.Sleep(5)

            nRet1 = Home_Page.FidCam1.SaveImageToFile(stSaveImageParam)


            Home_Page.FidCam1.DisplayOneFrame(stDisplayInfo)

            Home_Page.FidCam1.FreeImageBuffer(stFrameOut)
            Dim ss As Boolean = File.Exists("C:\Manage Files\Load\123.Png")
            Dim ss1 As Boolean = File.Exists("C:\Manage Files\Load\Image_w1.Png")

            If ss = True Then

                Dim img As Bitmap = New Bitmap("C:\Manage Files\Load\123.Png")

                Dim newImg As New Bitmap(PictureBox7.Width, PictureBox7.Height)

                ' Draw the resized image
                Using g As Graphics = Graphics.FromImage(newImg)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(img, 0, 0, PictureBox7.Width, PictureBox7.Height)
                End Using
                newImg.Save("C:\Manage Files\Load\Image_w1.Png")

                PictureBox7.LoadAsync("C:\Manage Files\Load\Image_w1.Png")
                newImg.Dispose()
                img.Dispose()

            End If







        End If
    End Sub

    Private Sub DATAGRID_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DATAGRID.CellContentClick

    End Sub

    Private Sub btxmin_MouseUp(sender As Object, e As MouseEventArgs) Handles btxmin.MouseUp
        plc.SetDevice("M205", 0)
    End Sub
End Class

