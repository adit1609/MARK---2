Imports ActUtlTypeLib
Imports Gui_Tset.My
Imports Gui_Tset.RecepieOperation
Imports Microsoft.Office.Interop.Excel
Imports MvCamCtrl.NET
Imports System.Configuration
Imports System.IO
Imports System.Net.IPAddress
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web.UI.WebControls
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Xml
Imports System.Math
Imports System.Drawing.Drawing2D
Imports Emgu.CV.ML

Public Class Operations
    Dim CHECK1 As Int32
    Dim au_start As Integer
    Dim emg As Integer
    Dim stop1 As Integer
    Dim NXT As Integer
    Dim CH As Integer
    Dim bit As Boolean
    'Dim MyCamera As CCamera = New CCamera
    'Dim nRet As Int32 = CCamera.MV_OK
    Dim m_bIsException As Boolean
    Dim m_nBufSizeForDriver As UInt32 = 1000 * 1000 * 3
    Dim m_pBufForDriver(m_nBufSizeForDriver) As Byte
    Dim m_stDeviceInfoList As CCamera.MV_CC_DEVICE_INFO_LIST = New CCamera.MV_CC_DEVICE_INFO_LIST
    Dim m_nDeviceIndex As UInt32
    Dim m_bIsGrabbing As Boolean = False
    Dim m_hGrabHandle As System.Threading.Thread
    Dim m_stFrameInfoEx As CCamera.MV_FRAME_OUT_INFO_EX = New CCamera.MV_FRAME_OUT_INFO_EX()
    Dim m_ReadWriteLock As System.Threading.ReaderWriterLock
    Dim start_m As Integer
    'Dim m_nBufSizeForDriver As UInt32 = 1000 * 1000 * 3
    'Dim m_pBufForDriver(m_nBufSizeForDriver) As Byte
    Dim m_nBufSizeForDriver1 As UInt32 = 1000 * 1000 * 3
    Dim m_pBufForDriver1(m_nBufSizeForDriver1) As Byte
    Dim AUTO As Integer
    ''' RECIPE DATA' ''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' RECIPE DATA' <summary>
    ''' RECIPE DATA' ''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' </summary>
    Dim PCB_WIDTH As Single
    Dim MARK_ID As Integer
    Dim P_XVALUE(10000) As Single
    Dim P_YVALUE(10000) As Single
    Dim P_SEL(10000) As Integer
    Dim F_TYPE(10000) As String
    Dim F_SHAP(10000) As String
    Dim F_X1(10000) As Integer
    Dim F_Y1(10000) As Integer
    Dim F_RX2(10000) As Integer
    Dim F_RY2(10000) As Integer
    Dim F_CPX(10000) As Integer
    Dim F_CPY(10000) As Integer
    Dim F_XOFF(10000) As Integer
    Dim F_YOFF(10000) As Integer
    Dim F_POSX(10000) As Single
    Dim F_POSY(10000) As Single
    Dim F_THRE(10000) As Integer
    Dim F_TOL(10000) As Integer
    Dim F_BRIG(10000) As Integer
    Dim F_SCORE(10000) As Integer
    Dim F_SEL(10000) As Integer
    Dim fid_OFFX1_T(1000) As Integer
    Dim fid_OFFY1_T(1000) As Integer
    Dim fid_OFFX1_C(1000) As Integer
    Dim fid_OFFY1_C(1000) As Integer

    Dim offsetx As Single
    Dim offsety As Single
    Dim fid_X As Integer
    Dim fid_Y As Integer
    Dim POS_LENGTH As Integer
    Dim FID_LENGTH As Integer
    Private stopCycle As Boolean = False
    Private pauseCycle As Boolean = False
    Private cycle As Process
    Private pythonProcess As Process



    Private Sub SetCtrlWhenClose()
        'ComboBoxDeviceList.Enabled = True

    End Sub

    Dim plc As New ActUtlType
    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub btStart_Click(sender As Object, e As EventArgs)
        Timer2.Start()
        Timer3.Start()



    End Sub








    Private Sub btStart_MouseDown(sender As Object, e As MouseEventArgs) Handles btStart.MouseDown
        plc.SetDevice("M202", 1)

    End Sub
    Private Sub btStart_MouseUp(sender As Object, e As MouseEventArgs) Handles btStart.MouseUp
        plc.SetDevice("M202", 0)
    End Sub
    Private Sub btStop_MouseUp(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M203", 0)
    End Sub

    Private Sub btStop_MouseDown(sender As Object, e As MouseEventArgs)
        plc.SetDevice("M203", 1)
    End Sub

    Private Sub btPass_MouseUp(sender As Object, e As MouseEventArgs) Handles btPass.MouseUp
        plc.SetDevice("M204", 0)
    End Sub

    Private Sub btPass_MouseDown(sender As Object, e As MouseEventArgs) Handles btPass.MouseDown
        plc.SetDevice("M204", 1)
    End Sub





    Private Sub Operations_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        plc.ActLogicalStationNumber = 1
        plc.Open()
        Timer1.Start()
        CheckBox5.Checked = MySettings.Default.fiducial
        If CheckBox5.Checked Then
            plc.SetDevice("M243", 1)

        Else
            plc.SetDevice("M243", 0)
        End If

        CheckBox1.Checked = MySettings.Default.MES1




        CheckBox2.Checked = MySettings.Default.BARCODE_SC
        If CheckBox2.Checked Then

            plc.SetDevice("M242", 1)
        Else

            plc.SetDevice("M242", 0)
        End If


        CheckBox3.Checked = MySettings.Default.L_MARK
        If CheckBox3.Checked Then

            plc.SetDevice("M241", 1)
        Else

            plc.SetDevice("M241", 0)
        End If


        CheckBox6.Checked = MySettings.Default.SMEMA_S
        If CheckBox6.Checked Then

            plc.SetDevice("M255", 1)
        Else

            plc.SetDevice("M255", 0)
        End If


        CheckBox7.Checked = MySettings.Default.CAM_SCA



        CheckBox4.Checked = MySettings.Default.FLIP
        If CheckBox4.Checked Then

            plc.SetDevice("M256", 1)
        Else

            plc.SetDevice("M256", 0)
        End If


        RichTextBox5.Text = MySettings.Default.Good_Count
        RichTextBox7.Text = MySettings.Default.NG_Count


















        'ComboBoxDeviceList.Items.Clear()
        'ComboBoxDeviceList.SelectedIndex = -1
        RichTextBox1.Text = My.Settings.ProgramName
        loadRecipe()
    End Sub
    Private Sub loadRecipe()
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


            'Dim files As String = Directory.GetFiles(Logdir, file1, System.IO.SearchOption.AllDirectories)
            Dim check As Boolean = File.Exists(generatedFile)

            If check = False Then
                MySettings.Default.ProgramName = ""


                Return
            End If
            RichTextBox1.Text = MySettings.Default.ProgramName

            POS_LENGTH = 1
            FID_LENGTH = 1

            Dim xmDocument As XmlDocument = New XmlDocument()
            xmDocument.Load(generatedFile)
            Dim Boardnodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/RECEIPE/BOARD")

            For Each Feeder As XmlNode In Boardnodes

                Dim P_WIDTH As String = Feeder.ChildNodes(2).InnerText
                Dim markid As String = Feeder.ChildNodes(11).InnerText
                PCB_WIDTH = Convert.ToSingle(P_WIDTH)
                MARK_ID = Convert.ToInt16(markid)

                RichTextBox15.Text = MARK_ID


                plc.SetDevice("D374", MARK_ID)

                Dim SIDEnodes As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/SIDE")
                Dim FRONT As String

                FRONT = SIDEnodes.ChildNodes(0).InnerText



                RichTextBox9.Text = FRONT
                Dim MARKPOSITION As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/MARKPOSITION")
                Dim Nodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/MARKPOSITION/POSI")
                Dim i As Integer
                For Each Node As XmlNode In Nodes

                    Dim xvalue As String = Node.ChildNodes(0).InnerText

                    Dim yvalue As String = Node.ChildNodes(1).InnerText

                    Dim che As Boolean = Node.ChildNodes(2).InnerText
                    'Dim dd As Integer = Convert.ToInt16(xvalue)
                    'Dim sd As Single = Convert.ToSingle(dd)
                    Dim x As Single = CSng(xvalue)
                    Dim y As Single = CSng(yvalue)
                    P_XVALUE(i) = x
                    P_YVALUE(i) = y

                    If che = True Then
                        P_SEL(i) = 1
                    ElseIf che = False Then
                        P_SEL(i) = 0
                    End If


                    i += 1

                    POS_LENGTH += 1

                Next











                'Next

            Next

            Dim FIDUCIAL As XmlNode = xmDocument.SelectSingleNode("JOBList/JOB/TAGTYPE/FIDUCIAL")
            Dim FNodes As XmlNodeList = xmDocument.SelectNodes("JOBList/JOB/TAGTYPE/FIDUCIAL/F")
            Dim d As Integer
            For Each Node As XmlNode In FNodes
                'Dim d As Integer = Convert.ToInt16(FNodes.Count)
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

                F_TYPE(d) = VALUE(0)
                F_SHAP(d) = VALUE(1)
                F_X1(d) = Convert.ToInt16(VALUE(2))
                F_Y1(d) = Convert.ToInt16(VALUE(3))
                F_RX2(d) = Convert.ToInt16(VALUE(4))
                F_RY2(d) = Convert.ToInt16(VALUE(5))
                Dim parts() As String = VALUE(6).Trim("()").Split(","c)
                'cnter = VALUE(6).Split("(", ","c)
                'Dim part As String = "45)"
                'Dim part1() As String = part.Trim("()")

                Dim trimmedPart() As String = parts(1).Split(")"c)


                F_CPX(d) = Convert.ToInt16(parts(0))
                F_CPY(d) = Convert.ToInt16(trimmedPart(0))
                If VALUE(7) IsNot "" Then
                    F_XOFF(d) = Convert.ToInt16(VALUE(7))
                End If
                If VALUE(8) IsNot "" Then
                    F_YOFF(d) = Convert.ToInt16(VALUE(8))
                End If
                If VALUE(9) IsNot "" Then
                    F_POSX(d) = Convert.ToSingle(VALUE(9))
                End If
                If VALUE(10) IsNot "" Then
                    F_POSY(d) = Convert.ToSingle(VALUE(10))
                End If
                If VALUE(11) IsNot "" Then
                    F_THRE(d) = Convert.ToInt16(VALUE(11))
                End If
                If VALUE(12) IsNot "" Then
                    F_TOL(d) = Convert.ToInt16(VALUE(12))
                End If
                If VALUE(13) IsNot "" Then
                    F_BRIG(d) = Convert.ToInt16(VALUE(13))
                End If
                If VALUE(14) IsNot "" Then
                    F_SCORE(d) = Convert.ToInt16(VALUE(14))
                End If


                If VALUE(15) = "True" Then
                    F_SEL(d) = 1
                ElseIf VALUE(15) = "False" Then
                    F_SEL(d) = 0
                End If

                d += 1
                FID_LENGTH += 1
            Next

        End If

    End Sub
    Dim length As Integer
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
        Thread.Sleep(200)
        ' Refined regex pattern to handle optional negative signs
        Dim regex As New Regex("Offset: \((?<x>-?\d+), (?<y>-?\d+)\)")
        Dim match As Match = regex.Match(output)

        If match.Success Then
            ' Extract coordinates from regex groups
            Dim x As String = match.Groups("x").Value
            Dim y As String = match.Groups("y").Value

            ' Update UI with coordinates
            'TextBox2.Text = $"Offset: ({x}, {y})"

            fid_X = Convert.ToInt16(x)
            fid_Y = Convert.ToInt16(y)


        Else
            MsgBox("Offset is not found")
            plc.SetDevice("M304", 1)
            stopCycle = True
            bit = False
            Return

        End If

        ' Display the output and error in the console for debugging
        Console.WriteLine("Output: " & output)
        Console.WriteLine("Error: " & [error])

        ' Optionally handle the disposed image if needed
        ' Dispose of the previous image if it exists
    End Sub


    Private Async Function SAVE_PIC() As Task
        plc.SetDevice("M247", 1)
        Await Task.Delay(700)
        Dim nRet As Int32 = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0)
        'Dim nRet As Int32 = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
        nRet = Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF)

        Await Task.Delay(300)
        Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        Dim nRet1 = Home_Page.FidCam1.GetImageBuffer(stFrameOut, 1000)
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
            stSaveImageParam.iMethodValue = 1
            stSaveImageParam.nQuality = 90
            stSaveImageParam.pImagePath = "C:\Manage Files\Load\" & "123" & ".Png"

            Thread.Sleep(30)


            'File.Delete("D:\Logs\fidimage")
            nRet1 = Home_Page.FidCam1.SaveImageToFile(stSaveImageParam)


            If (CCamera.MV_OK <> nRet1) Then
                'MsgBox("Save Image fail!")
            Else

                ' PictureBox7.Load("D:\Logs\fidimage\Image_w" & ".jpg")



            End If
            Home_Page.FidCam1.DisplayOneFrame(stDisplayInfo)

            Home_Page.FidCam1.FreeImageBuffer(stFrameOut)
            nRet = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
            nRet = Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)
        Else
            'plc.SetDevice("M247", 0)  ''''''''''' RED LIGHT
            Thread.Sleep(100)
        End If
        plc.SetDevice("M247", 0)
    End Function

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick




        Dim value As Integer
        plc.GetDevice("M224", value)
        If (value = 1) And bit = False Then
            bt_Gateopenl.Enabled = False
            bt_Gateopenr.Enabled = False
            bt_Pcbstopper.Enabled = False
            btLoadpos.Enabled = False
            btUnloadpos.Enabled = False
            Button11.Enabled = False
            bt_Pcbload.Enabled = False
            bt_Pcbunload.Enabled = False
            Button13.Enabled = False
            Button3.Enabled = False
            bt_Pcbclamp.Enabled = False
            bt_Pcbunclamp.Enabled = False
            btStart.BackColor = Color.Green


            'live_camera()
            Sendin_DATA()
        ElseIf (value = 0) And bit = True Then
            plc.SetDevice("M300", 0)
            plc.SetDevice("M247", 0)  ''''''''''' RED LIGHT
            Thread.Sleep(100)
            bt_Gateopenl.Enabled = True
            bt_Gateopenr.Enabled = True
            bt_Pcbstopper.Enabled = True
            btLoadpos.Enabled = True
            btUnloadpos.Enabled = True
            Button11.Enabled = True
            bt_Pcbload.Enabled = True
            bt_Pcbunload.Enabled = True
            Button13.Enabled = True
            Button3.Enabled = True
            bt_Pcbclamp.Enabled = True
            bt_Pcbunclamp.Enabled = True
            btStart.BackColor = Color.White

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



    Private Sub Operations_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed


        Timer2.Stop()
        Timer3.Stop()
        Dim nRet As Int32 = Home_Page.FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
        nRet = Home_Page.FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)
    End Sub
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick


    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs)
        Timer2.Stop()
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick











    End Sub

    Private Sub CheckBox2_Click(sender As Object, e As EventArgs)
        Timer1.Start()
    End Sub


    Private Sub bt_Pcbstopper_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Pcbstopper.MouseDown
        plc.SetDevice("M219", 1)
    End Sub

    Private Sub bt_Pcbstopper_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Pcbstopper.MouseUp
        plc.SetDevice("M219", 0)
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

    Private Sub btLoadpos_MouseDown(sender As Object, e As MouseEventArgs) Handles btLoadpos.MouseDown
        plc.SetDevice("M233", 1)
    End Sub

    Private Sub btLoadpos_MouseUp(sender As Object, e As MouseEventArgs) Handles btLoadpos.MouseUp
        plc.SetDevice("M233", 0)
    End Sub

    Private Sub btUnloadpos_MouseDown(sender As Object, e As MouseEventArgs) Handles btUnloadpos.MouseDown
        plc.SetDevice("M234", 1)
    End Sub

    Private Sub btUnloadpos_MouseUp(sender As Object, e As MouseEventArgs) Handles btUnloadpos.MouseUp
        plc.SetDevice("M234", 0)
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

    Private Sub Button9_KeyDown(sender As Object, e As KeyEventArgs)
        plc.SetDevice("M239", 1)
    End Sub

    Private Sub bt_Pcbclamp_KeyDown(sender As Object, e As KeyEventArgs) Handles bt_Pcbclamp.KeyDown
        plc.SetDevice("M239", 1)
    End Sub

    Private Sub bt_Pcbclamp_MouseDown(sender As Object, e As MouseEventArgs) Handles bt_Pcbclamp.MouseDown
        plc.SetDevice("M248", 1)
    End Sub

    Private Sub Button3_MouseDown(sender As Object, e As MouseEventArgs) Handles Button3.MouseDown
        plc.SetDevice("M239", 1)
    End Sub

    Private Sub Button3_MouseUp(sender As Object, e As MouseEventArgs) Handles Button3.MouseUp
        plc.SetDevice("M239", 0)
    End Sub

    Private Sub TableLayoutPanel3_Paint(sender As Object, e As PaintEventArgs) Handles TableLayoutPanel3.Paint

    End Sub

    Private Sub Button13_MouseDown(sender As Object, e As MouseEventArgs) Handles Button13.MouseDown
        plc.SetDevice("M237", 1)
    End Sub

    Private Sub Button13_MouseUp(sender As Object, e As MouseEventArgs) Handles Button13.MouseUp
        plc.SetDevice("M237", 0)
    End Sub

    Private Sub Button11_MouseDown(sender As Object, e As MouseEventArgs) Handles Button11.MouseDown
        plc.SetDevice("M252", 1)
    End Sub

    Private Sub Button11_MouseUp(sender As Object, e As MouseEventArgs) Handles Button11.MouseUp
        plc.SetDevice("M252", 0)
    End Sub

    Private Sub btStart_Click_1(sender As Object, e As EventArgs) Handles btStart.Click
        Timer2.Start()
        Timer3.Start()



    End Sub
    Private Sub live_camera()
        Dim nRet1 As Int32 = Home_Page.LiveCamera1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_LINE0)
        nRet1 = Home_Page.LiveCamera1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_OFF)

        Dim stFrameOut As CCamera.MV_FRAME_OUT = New CCamera.MV_FRAME_OUT()
        Dim stDisplayInfo As CCamera.MV_DISPLAY_FRAME_INFO = New CCamera.MV_DISPLAY_FRAME_INFO()
        Dim nRet = Home_Page.LiveCamera1.GetImageBuffer(stFrameOut, 1000)

        If CCamera.MV_OK = nRet Then

            If stFrameOut.stFrameInfo.nFrameLen > m_nBufSizeForDriver Then
                m_nBufSizeForDriver = stFrameOut.stFrameInfo.nFrameLen
                ReDim m_pBufForDriver(m_nBufSizeForDriver)
            End If
            m_stFrameInfoEx = stFrameOut.stFrameInfo
            Marshal.Copy(stFrameOut.pBufAddr, m_pBufForDriver, 0, stFrameOut.stFrameInfo.nFrameLen)
            stDisplayInfo.hWnd = PictureBox1.Handle
            stDisplayInfo.pData = stFrameOut.pBufAddr
            stDisplayInfo.nDataLen = stFrameOut.stFrameInfo.nFrameLen
            stDisplayInfo.nWidth = stFrameOut.stFrameInfo.nWidth
            stDisplayInfo.nHeight = stFrameOut.stFrameInfo.nHeight
            stDisplayInfo.enPixelType = stFrameOut.stFrameInfo.enPixelType
            Home_Page.LiveCamera1.DisplayOneFrame(stDisplayInfo)
            Home_Page.LiveCamera1.FreeImageBuffer(stFrameOut)

        End If
    End Sub

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        If CheckBox5.Checked Then
            MySettings.Default.fiducial = True
            MySettings.Default.Save()
            plc.SetDevice("M243", 1)
        Else
            MySettings.Default.fiducial = False
            MySettings.Default.Save()
            plc.SetDevice("M243", 0)
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            MySettings.Default.MES1 = True
            MySettings.Default.Save()
        Else
            MySettings.Default.MES1 = False
            MySettings.Default.Save()
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged_1(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            MySettings.Default.BARCODE_SC = True
            MySettings.Default.Save()
            plc.SetDevice("M242", 1)
        Else
            MySettings.Default.BARCODE_SC = False
            MySettings.Default.Save()
            plc.SetDevice("M242", 0)
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked Then
            MySettings.Default.L_MARK = True
            MySettings.Default.Save()
            plc.SetDevice("M241", 1)
        Else
            MySettings.Default.L_MARK = False
            MySettings.Default.Save()
            plc.SetDevice("M241", 0)
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked Then
            MySettings.Default.SMEMA_S = True
            MySettings.Default.Save()
            plc.SetDevice("M255", 1)
        Else
            MySettings.Default.SMEMA_S = False
            MySettings.Default.Save()
            plc.SetDevice("M255", 0)
        End If
    End Sub

    Private Sub CheckBox7_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox7.CheckedChanged
        If CheckBox7.Checked Then
            MySettings.Default.CAM_SCA = True
            MySettings.Default.Save()

        Else
            MySettings.Default.CAM_SCA = False
            MySettings.Default.Save()

        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs)
        If CheckBox4.Checked Then
            MySettings.Default.FLIP = True
            MySettings.Default.Save()
            plc.SetDevice("M256", 1)
        Else
            MySettings.Default.FLIP = False
            MySettings.Default.Save()
            plc.SetDevice("M256", 0)
        End If
    End Sub
    Public Async Sub Sendin_DATA()
        bit = True
        stopCycle = False
        Try
            plc.GetDevice("M900", AUTO)
            Dim ddd As Integer

            If stopCycle Then Exit Sub
            Await Task.Run(Sub() PauseCheck())

            plc.SetDevice("M202", 1)
            Await Task.Delay(100) ' Replaces Thread.Sleep
            Await Task.Delay(200) ' Replaces Thread.Sleep

            If CheckBox5.Checked Then
                Await ProcessCheckBox5OperationsAsync(ddd = 1)
            End If

            plc.SetDevice("M247", 0)
            plc.SetDevice("M303", 1)
            Await ProcessLoopOperationsAsync()

            plc.SetDevice("M301", 1)
            plc.GetDevice("M900", AUTO)


        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
        bit = False
    End Sub

    Private Async Function ProcessCheckBox5OperationsAsync(ByVal ddd As Integer) As Task
        Dim L1 As Integer = 0


        For J = 0 To 1

            Do Until (F_SEL(L1) = 1)
                If stopCycle Then Exit Function
                Await Task.Run(Sub() PauseCheck())
                L1 += 1
            Loop


            If stopCycle Then Exit Function
            Await Task.Run(Sub() PauseCheck())
            If stopCycle Then Exit Function
            Await Task.Run(Sub() PauseCheck())
            plc.SetDevice("M247", 1)
            Dim wordsX() As Integer = ConvertFloatToWord(F_POSX(L1))
            plc.SetDevice("D370", wordsX(0))
            plc.SetDevice("D371", wordsX(1))

            Dim wordsY() As Integer = ConvertFloatToWord(F_POSY(L1))
            plc.SetDevice("D372", wordsY(0))
            plc.SetDevice("D373", wordsY(1))
            plc.SetDevice("M302", 1)
            Await WaitForPLCAsync(CHECK1)

            ' Red light and capture image

            Await Task.Delay(200)
            Dim nRet1 As Int32 = Home_Page.FidCam1.SetCommandValue("TriggerSoftware")
            SAVE_PIC()
            If stopCycle Then Exit Function
            ' Image processing
            Await ProcessImageAsync("C:\Manage Files\Load\123.Png")

            ' Write to file
            Dim centx As Integer = Convert.ToInt16(F_CPX(L1)) * 4
            Dim centy As Integer = Convert.ToInt16(F_CPY(L1)) * 4
            Dim valu As String = $"({centx},{centy})"
            WriteToFile("C:\Manage Files\centre Point\text.txt", valu)
            WriteToFile("C:\Manage Files\image_name\text.txt", F_TYPE(L1))

            ' Run Python script
            Await RunPythonScriptAsync()

            L1 += 1

        Next
        ddd = 1

    End Function

    Private Async Function WaitForPLCAsync(ByVal CHECK2 As Int32) As Task
        Do
            If stopCycle Then Exit Function
            Await Task.Run(Sub() PauseCheck())
            plc.SetDevice("M302", 1)
            Await Task.Delay(200)
            plc.GetDevice("M352", CHECK2)
        Loop Until CHECK2 = 1
    End Function

    Private Async Function ProcessLoopOperationsAsync() As Task
        Dim i As Int32
        Dim ST As Integer

        For i = 0 To (POS_LENGTH - 1)
            If stopCycle Then Exit Function
            Await Task.Run(Sub() PauseCheck())

            If P_SEL(i) = 1 Then
                Await WaitForPLCAsync1(ST)

                plc.SetDevice("M353", 0)
                Await Task.Delay(3000)

                ' Set device with coordinates
                SetPLCCoordinates(i)

                plc.SetDevice("M300", 1)
                Await Task.Delay(3000)

                If CheckBox2.Checked Then
                    Await ProcessBarcodeCheckAsync(i)

                    If RichTextBox4.Text = RichTextBox2.Text Then
                        MySettings.Default.Good_Count += 1
                        MySettings.Default.Save()
                        RichTextBox5.Text = MySettings.Default.Good_Count
                    Else
                        MySettings.Default.NG_Count += 1
                        MySettings.Default.Save()
                        RichTextBox7.Text = MySettings.Default.NG_Count
                    End If

                End If
            End If
        Next
    End Function
    Private Async Function WaitForPLCAsync1(ByVal st As Int32) As Task


        Do
                If stopCycle Then Exit Function
                Await Task.Run(Sub() PauseCheck())
                plc.GetDevice("M224", au_start)
                plc.GetDevice("M300", stop1)
                plc.GetDevice("M353", NXT)
                plc.GetDevice("M303", st)
                plc.GetDevice("X13", emg)


                If (emg = 1) Then
                    Return
                End If

            Loop Until (NXT = 1 And au_start = 1 And stop1 = 0 And st = 0)

    End Function
    Private Sub SetPLCCoordinates(ByVal index As Integer)
        Dim floatValueX As Single
        If Single.TryParse(P_XVALUE(index), floatValueX) Then
            Dim wordsX() As Integer = ConvertFloatToWord(floatValueX)
            plc.SetDevice("D370", wordsX(0))
            plc.SetDevice("D371", wordsX(1))
        End If

        Dim floatValueY As Single
        If Single.TryParse(P_YVALUE(index).ToString(), floatValueY) Then
            Dim wordsY() As Integer = ConvertFloatToWord(floatValueY)
            plc.SetDevice("D372", wordsY(0))
            plc.SetDevice("D373", wordsY(1))
        End If
    End Sub

    Private Async Function ProcessBarcodeCheckAsync(ByVal index As Integer) As Task
        Do
            If stopCycle Then Exit Function
            Await Task.Run(Sub() PauseCheck())
            plc.GetDevice("M224", au_start)
            plc.GetDevice("M300", stop1)
            plc.GetDevice("M353", NXT)

            plc.GetDevice("X13", emg)
            Await barcode_scannerAsync()
            Await laser_barcodeAsync()

        Loop Until (NXT = 1 And au_start = 1 And stop1 = 0) ' Define the appropriate exit condition
    End Function
    Private Async Function barcode_scannerAsync() As Task
        Dim startRegister As Integer = 280
        Dim endRegister As Integer = 299
        Dim numRegisters As Integer = endRegister - startRegister + 1


        Dim words(numRegisters - 1) As Integer


        For i As Integer = 0 To numRegisters - 1
            plc.GetDevice("D" & (startRegister + i).ToString(), words(i))
        Next

        ' Create a byte array to hold the combined byte values
        Dim bytes(numRegisters * 2 - 1) As Byte

        ' Convert the 16-bit integers to a byte array
        For i As Integer = 0 To words.Length - 1
            Dim wordBytes() As Byte = BitConverter.GetBytes(words(i))
            bytes(i * 2) = wordBytes(0)
            bytes(i * 2 + 1) = wordBytes(1)
        Next

        ' Convert the byte array to a string
        Dim strValue As String = System.Text.Encoding.ASCII.GetString(bytes)

        ' Display the string in the RichTextBox
        RichTextBox4.Text = strValue.TrimEnd(Chr(0)) ' Remove any trailing null characters

    End Function
    Private Async Function laser_barcodeAsync() As Task
        Dim startRegister As Integer = 401
        Dim endRegister As Integer = 421
        Dim numRegisters As Integer = endRegister - startRegister + 1


        Dim words(numRegisters - 1) As Integer


        For i As Integer = 0 To numRegisters - 1
            plc.GetDevice("D" & (startRegister + i).ToString(), words(i))
        Next

        ' Create a byte array to hold the combined byte values
        Dim bytes(numRegisters * 2 - 1) As Byte

        ' Convert the 16-bit integers to a byte array
        For i As Integer = 0 To words.Length - 1
            Dim wordBytes() As Byte = BitConverter.GetBytes(words(i))
            bytes(i * 2) = wordBytes(0)
            bytes(i * 2 + 1) = wordBytes(1)
        Next

        ' Convert the byte array to a string
        Dim strValue As String = System.Text.Encoding.ASCII.GetString(bytes)

        ' Display the string in the RichTextBox
        RichTextBox2.Text = strValue.TrimEnd(Chr(0)) ' Remove any trailing null characters

    End Function
    Private Async Function ProcessImageAsync(ByVal imagePath As String) As Task
        If File.Exists(imagePath) Then
            Using img As New Bitmap(imagePath)
                Dim newImg As New Bitmap(PictureBox3.Width, PictureBox3.Height)

                Using g As Graphics = Graphics.FromImage(newImg)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.DrawImage(img, 0, 0, PictureBox3.Width, PictureBox3.Height)
                End Using

                newImg.Save("C:\Manage Files\Load\Image_w1.Png")
                Await Task.Delay(100)

                PictureBox3.Invoke(Sub() PictureBox3.LoadAsync("C:\Manage Files\Load\Image_w1.Png"))

                Await Task.Delay(50)
            End Using
        End If
    End Function

    Private Sub PauseCheck()
        Do While pauseCycle
            If stopCycle Then Exit Do
            Thread.Sleep(100)
        Loop
    End Sub



    Private Sub WriteToFile(ByVal filePath As String, ByVal content As String)
        Try
            System.IO.File.WriteAllText(filePath, content)
        Catch ex As Exception
            MessageBox.Show("Error writing to file: " & ex.Message)
        End Try
    End Sub

    Private Async Function RunPythonScriptAsync() As Task
        Try
            RunPythonScript()
        Catch ex As Exception
            ' Handle exception
        End Try

        Try
            If pythonProcess IsNot Nothing AndAlso Not pythonProcess.HasExited Then
                pythonProcess.Kill()
                pythonProcess.Dispose()
                pythonProcess = Nothing
            End If
        Catch ex As Exception
            ' Handle exception
        End Try
    End Function


    Private Sub btStop_Click_1(sender As Object, e As EventArgs) Handles btStop.Click
        stopCycle = True
    End Sub
    Private Sub btPass_Click(sender As Object, e As EventArgs) Handles btPass.Click
        pauseCycle = Not pauseCycle ' Toggle pause state
    End Sub

    Private Sub bt_Pcbclamp_Click(sender As Object, e As EventArgs) Handles bt_Pcbclamp.Click

    End Sub

    Private Sub bt_Pcbclamp_MouseUp(sender As Object, e As MouseEventArgs) Handles bt_Pcbclamp.MouseUp
        plc.SetDevice("M248", 0)
    End Sub
    Public Mode_work As Integer
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Mode_work = Mode_work + 2
        If Mode_work = 2 Then
            plc.SetDevice("M244", 1)

            Button4.BackColor = Color.Green
        Else
            Mode_work = 0
            plc.SetDevice("M244", 0)
            Button4.BackColor = Color.White
        End If
    End Sub

    Private Async Function Button5_MouseDown(sender As Object, e As MouseEventArgs) As Task Handles Button5.MouseDown
        plc.SetDevice("M253", 1)
    End Function

    Private Async Function Button5_MouseUp(sender As Object, e As MouseEventArgs) As Task Handles Button5.MouseUp
        plc.SetDevice("M253", 0)
    End Function

    Private Async Function Button6_MouseDown(sender As Object, e As MouseEventArgs) As Task Handles Button6.MouseDown
        plc.SetDevice("M225", 1)
        Button6.BackColor = Color.Green
    End Function

    Private Async Function Button6_MouseUp(sender As Object, e As MouseEventArgs) As Task Handles Button6.MouseUp
        plc.SetDevice("M225", 0)
        Button6.BackColor = Color.Transparent
    End Function

    Private Sub Button7_MouseDown(sender As Object, e As MouseEventArgs) Handles Button7.MouseDown
        plc.SetDevice("M226", 1)
        Button7.BackColor = Color.Green
    End Sub

    Private Sub Button7_MouseUp(sender As Object, e As MouseEventArgs) Handles Button7.MouseUp
        plc.SetDevice("M226", 0)
        Button7.BackColor = Color.Transparent
    End Sub
End Class