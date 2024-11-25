﻿'
Imports System.Configuration
Imports System.Drawing.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Threading
Imports ActUtlTypeLib
Imports Gui_Tset.My
Imports MvCamCtrl.NET
Imports OfficeOpenXml




Public Class Home_Page
    Dim dval As String
    Dim prevDval As String = "000"
    Public Checkagain As String = "000"
    Private alarmForm As Alarms = Nothing
    Dim plc As New ActUtlType
    Dim check As Integer

    Dim previousAlarmValue As Integer = -1
    Dim serialNumber As Integer = 1
    Private alarmMessageBoxShown As Boolean = False
    Private blinking As Boolean = False
    Private normalColor As Color
    Public FidCam1 As CCamera = New CCamera
    Public LiveCamera1 As CCamera = New CCamera
    Private currentChildform As Form
    Dim fidValue As UInt32
    Dim LiveValue As UInt32
    Dim BarValue As UInt32
    Dim fidcheck As Boolean
    Dim livecheck As Boolean
    Dim m_nBufSizeForDriver As UInt32 = 1000 * 1000 * 3
    Dim m_pBufForDriver(m_nBufSizeForDriver) As Byte
    Dim m_nBufSizeForDriver1 As UInt32 = 1000 * 1000 * 3
    Dim m_pBufForDriver1(m_nBufSizeForDriver1) As Byte
    Dim m_stDeviceInfoList As CCamera.MV_CC_DEVICE_INFO_LIST = New CCamera.MV_CC_DEVICE_INFO_LIST
    Dim m_nDeviceIndex As UInt32
    Dim m_stFrameInfoEx As CCamera.MV_FRAME_OUT_INFO_EX = New CCamera.MV_FRAME_OUT_INFO_EX()
    Public Sub Home_Page(_childfrm As Form)
        If currentChildform IsNot Nothing Then
            currentChildform.Close()
        End If
        currentChildform = _childfrm

        _childfrm.TopLevel = False
        _childfrm.FormBorderStyle = FormBorderStyle.None
        _childfrm.Dock = DockStyle.Fill
        pnDock.Controls.Add(_childfrm)
        _childfrm.BringToFront()
        _childfrm.Show()

    End Sub

    Private Sub Home_Page_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        plc.ActLogicalStationNumber = 1
        ' Set Timer2 interval to 1 second
        Timer2.Stop()
        Home_Page(New userlogin2)
        Label2.Text = DateTime.Now.ToString("dd MMM HH:mm:ss")


        check = plc.Open()
        btReturn.Hide()

        plc.SetDevice("M247", 1)
        'FidCamConnect()
        'LiveCamConnect()


        'Label3.Text = My.Settings.ProgramName

        Timer1.Start()
        Alarm.InitializeAlarms()
        plc.SetDevice("M246", 0)
        plc.SetDevice("M247", 0)

        Timer5.Start()







    End Sub
    Private Sub FidCamConnect()
        Dim nRet As Int32 = CCamera.MV_OK
        Dim stDeviceInfoList As CCamera.MV_CC_DEVICE_INFO_LIST = New CCamera.MV_CC_DEVICE_INFO_LIST

        fidcheck = False
        ' ch:枚举设备 | en:Enum device
        nRet = CCamera.EnumDevices((CCamera.MV_GIGE_DEVICE), stDeviceInfoList)
        If CCamera.MV_OK <> nRet Then
            Console.WriteLine("Enum Device failed:{0:x8}", nRet)

        End If

        If (0 = stDeviceInfoList.nDeviceNum) Then
            Console.WriteLine("No Find Gige | Usb Device !")

        End If

        '  ch:打印设备信息 | en:Print device info
        Dim i As Int32
        For i = 0 To stDeviceInfoList.nDeviceNum - 1
            Dim stDeviceInfo As CCamera.MV_CC_DEVICE_INFO = New CCamera.MV_CC_DEVICE_INFO
            stDeviceInfo = CType(Marshal.PtrToStructure(stDeviceInfoList.pDeviceInfo(i), GetType(CCamera.MV_CC_DEVICE_INFO)), CCamera.MV_CC_DEVICE_INFO)
            If (CCamera.MV_GIGE_DEVICE = stDeviceInfo.nTLayerType) Then
                Dim stGigeInfoPtr As IntPtr = Marshal.AllocHGlobal(216)
                Marshal.Copy(stDeviceInfo.stSpecialInfo.stGigEInfo, 0, stGigeInfoPtr, 216)
                Dim stGigeInfo As CCamera.MV_GIGE_DEVICE_INFO
                stGigeInfo = CType(Marshal.PtrToStructure(stGigeInfoPtr, GetType(CCamera.MV_GIGE_DEVICE_INFO)), CCamera.MV_GIGE_DEVICE_INFO)
                Dim nIpByte1 As UInt32 = (stGigeInfo.nCurrentIp And &HFF000000) >> 24
                Dim nIpByte2 As UInt32 = (stGigeInfo.nCurrentIp And &HFF0000) >> 16
                Dim nIpByte3 As UInt32 = (stGigeInfo.nCurrentIp And &HFF00) >> 8
                Dim nIpByte4 As UInt32 = (stGigeInfo.nCurrentIp And &HFF)


                If (nIpByte4 = 51) Then

                    nRet = FidCam1.CreateDevice(stDeviceInfo)
                    FidCam1.CloseDevice()
                    FidCam1.DestroyDevice()
                    nRet = FidCam1.CreateDevice(stDeviceInfo)
                    If CCamera.MV_OK <> nRet Then
                        MsgBox("Fail to create handle")
                        fidcheck = True
                        Return
                    End If

                    nRet = FidCam1.OpenDevice()
                    If CCamera.MV_OK <> nRet Then
                        FidCam1.DestroyDevice()

                        MsgBox("Open device failed")
                        fidcheck = True
                        Return
                    End If
                    If stDeviceInfo.nTLayerType = CCamera.MV_GIGE_DEVICE Then
                        Dim nPacketSize As Int32
                        nPacketSize = FidCam1.GIGE_GetOptimalPacketSize()
                        If nPacketSize > 0 Then
                            nRet = FidCam1.SetIntValueEx("GevSCPSPacketSize", nPacketSize)
                            If 0 <> nRet Then
                                MsgBox("Set Packet Size failed")
                            End If
                        Else
                            MsgBox("Get Packet Size failed")
                        End If
                    End If
                    'FidCam1.SetGrabStrategy(1)
                    Dim stTriggerMode As CCamera.MVCC_ENUMVALUE = New CCamera.MVCC_ENUMVALUE

                    nRet = FidCam1.SetEnumValue("TriggerSource", CCamera.MV_CAM_TRIGGER_SOURCE.MV_TRIGGER_SOURCE_SOFTWARE)
                    nRet = FidCam1.SetEnumValue("TriggerMode", CCamera.MV_CAM_TRIGGER_MODE.MV_TRIGGER_MODE_ON)


                    nRet = FidCam1.GetEnumValue("TriggerMode", stTriggerMode)
                    If CCamera.MV_OK <> nRet Then
                        MsgBox("Fail to acquire trigger mode")
                        Return
                    End If




                    Dim stExposureTime As CCamera.MVCC_FLOATVALUE = New CCamera.MVCC_FLOATVALUE
                    Dim fExposureTime As Single
                    fExposureTime = 5000.0
                    nRet = FidCam1.SetFloatValue("ExposureTime", fExposureTime)

                    If CCamera.MV_OK <> nRet Then
                        MsgBox("Fail to acquire exposure time")
                    End If

                    'Dim stGain As CCamera.MVCC_FLOATVALUE = New CCamera.MVCC_FLOATVALUE
                    'nRet = FidCam1.SetFloatValue("Gain", 5)
                    'If CCamera.MV_OK <> nRet Then
                    'MsgBox("Fail to acquire gain")
                    'End If

                    'Dim stFrameRate As CCamera.MVCC_FLOATVALUE = New CCamera.MVCC_FLOATVALUE
                    'FidCam1.SetBoolValue("AcquisitionFrameRateEnable", True)
                    'nRet = FidCam1.SetFloatValue("AcquisitionFrameRate", 56)
                    'nRet = FidCam1.SetFloatValue("ResultingFrameRate", 56)
                    'If CCamera.MV_OK <> nRet Then
                    'M 'sgBox("Fail to acquire frame rate")
                    'End If


                End If
            Else
                Dim stUsbInfoPtr As IntPtr = Marshal.AllocHGlobal(540)
                Marshal.Copy(stDeviceInfo.stSpecialInfo.stUsb3VInfo, 0, stUsbInfoPtr, 540)
                Dim stUsbInfo As CCamera.MV_USB3_DEVICE_INFO
                stUsbInfo = CType(Marshal.PtrToStructure(stUsbInfoPtr, GetType(CCamera.MV_USB3_DEVICE_INFO)), CCamera.MV_USB3_DEVICE_INFO)

                Console.WriteLine("[Dev " + Convert.ToString(i) + "]:")
                Console.WriteLine("SerialNumber:" + stUsbInfo.chSerialNumber)
                Console.WriteLine("UserDefinedName:" + stUsbInfo.chUserDefinedName)
                Console.WriteLine("")
            End If
        Next







        Threading.Thread.Sleep(5)


        nRet = FidCam1.StartGrabbing()
    End Sub
    Private Sub LiveCamConnect()
        Dim nRet As Int32 = CCamera.MV_OK
        Dim stDeviceInfoList As CCamera.MV_CC_DEVICE_INFO_LIST = New CCamera.MV_CC_DEVICE_INFO_LIST

        livecheck = False
        ' ch:枚举设备 | en:Enum device
        nRet = CCamera.EnumDevices((CCamera.MV_GIGE_DEVICE), stDeviceInfoList)
        If CCamera.MV_OK <> nRet Then
            Console.WriteLine("Enum Device failed:{0:x8}", nRet)

        End If

        If (0 = stDeviceInfoList.nDeviceNum) Then
            Console.WriteLine("No Find Gige | Usb Device !")

        End If

        '  ch:打印设备信息 | en:Print device info
        Dim i As Int32
        For i = 0 To stDeviceInfoList.nDeviceNum - 1
            Dim stDeviceInfo As CCamera.MV_CC_DEVICE_INFO = New CCamera.MV_CC_DEVICE_INFO
            stDeviceInfo = CType(Marshal.PtrToStructure(stDeviceInfoList.pDeviceInfo(i), GetType(CCamera.MV_CC_DEVICE_INFO)), CCamera.MV_CC_DEVICE_INFO)
            If (CCamera.MV_GIGE_DEVICE = stDeviceInfo.nTLayerType) Then
                Dim stGigeInfoPtr As IntPtr = Marshal.AllocHGlobal(216)
                Marshal.Copy(stDeviceInfo.stSpecialInfo.stGigEInfo, 0, stGigeInfoPtr, 216)
                Dim stGigeInfo As CCamera.MV_GIGE_DEVICE_INFO
                stGigeInfo = CType(Marshal.PtrToStructure(stGigeInfoPtr, GetType(CCamera.MV_GIGE_DEVICE_INFO)), CCamera.MV_GIGE_DEVICE_INFO)
                Dim nIpByte1 As UInt32 = (stGigeInfo.nCurrentIp And &HFF000000) >> 24
                Dim nIpByte2 As UInt32 = (stGigeInfo.nCurrentIp And &HFF0000) >> 16
                Dim nIpByte3 As UInt32 = (stGigeInfo.nCurrentIp And &HFF00) >> 8
                Dim nIpByte4 As UInt32 = (stGigeInfo.nCurrentIp And &HFF)


                If (nIpByte4 = 50) Then

                    nRet = LiveCamera1.CreateDevice(stDeviceInfo)
                    LiveCamera1.CloseDevice()
                    LiveCamera1.DestroyDevice()
                    nRet = LiveCamera1.CreateDevice(stDeviceInfo)
                    If CCamera.MV_OK <> nRet Then

                        livecheck = True
                        LiveCamera1.DestroyDevice()
                        MsgBox("Fail to create handle")
                        Return
                    End If
                    nRet = LiveCamera1.OpenDevice()
                    If CCamera.MV_OK <> nRet Then
                        livecheck = True
                        LiveCamera1.DestroyDevice()
                        MsgBox("Open device failed")
                        Return
                    End If
                    If stDeviceInfo.nTLayerType = CCamera.MV_GIGE_DEVICE Then
                        Dim nPacketSize1 As Int32
                        nPacketSize1 = 55

                        nRet = LiveCamera1.SetIntValueEx("GevSCPSPacketSize", nPacketSize1)

                    End If
                    Dim stTriggerMode1 As CCamera.MVCC_ENUMVALUE = New CCamera.MVCC_ENUMVALUE
                    nRet = LiveCamera1.GetEnumValue("TriggerMode", stTriggerMode1)
                    If CCamera.MV_OK <> nRet Then
                        MsgBox("Fail to acquire trigger mode")
                        Return
                    End If

                    Dim stTriggerSource1 As CCamera.MVCC_ENUMVALUE = New CCamera.MVCC_ENUMVALUE
                    nRet = LiveCamera1.GetEnumValue("TriggerSource", stTriggerSource1)
                    If CCamera.MV_OK <> nRet Then
                        MsgBox("Fail to acquire trigger source")
                        Return
                    End If
                    'Dim stExposureTime1 As CCamera.MVCC_FLOATVALUE = New CCamera.MVCC_FLOATVALUE
                    'Dim fExposureTime As Single
                    ' fExposureTime = 31000.0
                    'nRet = LiveCamera1.SetFloatValue("ExposureTime", fExposureTime)
                    'If CCamera.MV_OK <> nRet Then
                    'MsgBox("Fail to acquire exposure time")
                    'End If

                    'Dim stGain1 As CCamera.MVCC_FLOATVALUE = New CCamera.MVCC_FLOATVALUE
                    ' nRet = LiveCamera1.GetFloatValue("Gain", stGain1)
                    ' If CCamera.MV_OK <> nRet Then
                    'MsgBox("Fail to acquire gain")
                    'End If

                    'Dim stFrameRate1 As CCamera.MVCC_FLOATVALUE = New CCamera.MVCC_FLOATVALUE
                    ' nRet = LiveCamera1.GetFloatValue("ResultingFrameRate", stFrameRate1)
                    'If CCamera.MV_OK <> nRet Then
                    'MsgBox("Fail to acquire frame rate")
                    'End If

                End If



            Else
                Dim stUsbInfoPtr As IntPtr = Marshal.AllocHGlobal(540)
                Marshal.Copy(stDeviceInfo.stSpecialInfo.stUsb3VInfo, 0, stUsbInfoPtr, 540)
                Dim stUsbInfo As CCamera.MV_USB3_DEVICE_INFO
                stUsbInfo = CType(Marshal.PtrToStructure(stUsbInfoPtr, GetType(CCamera.MV_USB3_DEVICE_INFO)), CCamera.MV_USB3_DEVICE_INFO)

                Console.WriteLine("[Dev " + Convert.ToString(i) + "]:")
                Console.WriteLine("SerialNumber:" + stUsbInfo.chSerialNumber)
                Console.WriteLine("UserDefinedName:" + stUsbInfo.chUserDefinedName)
                Console.WriteLine("")
            End If
        Next





        Threading.Thread.Sleep(5)

        nRet = LiveCamera1.StartGrabbing()

    End Sub
    Private Sub btUser_Click(sender As Object, e As EventArgs) Handles btUser.Click
        Home_Page(New userlogin2)
        btUser.Hide()
        btoper.Hide()
        btProg.Hide()
        btmain.Hide()
        Button1.Hide()
        btsetup.Hide()
        btReturn.Size = btUser.Size
        btReturn.Dock = DockStyle.Right
        btReturn.Show()


    End Sub

    Private Sub btoper_Click(sender As Object, e As EventArgs) Handles btoper.Click
        Home_Page(New Operations)
        btUser.Hide()
        btoper.Hide()
        btProg.Hide()
        btmain.Hide()
        Button1.Hide()
        btsetup.Hide()
        btReturn.Dock = DockStyle.Right
        btReturn.Size = btUser.Size
        btReturn.Show()

    End Sub

    Private Sub btProg_Click(sender As Object, e As EventArgs) Handles btProg.Click
        Home_Page(New NewRec)
        btUser.Hide()
        btoper.Hide()
        btProg.Hide()
        btmain.Hide()
        Button1.Hide()
        btsetup.Hide()
        btReturn.Dock = DockStyle.Right
        btReturn.Size = btUser.Size
        btReturn.Show()

    End Sub

    Private Sub btmain_Click(sender As Object, e As EventArgs) Handles btmain.Click
        Home_Page(New Maintenance)
        btUser.Hide()
        btoper.Hide()
        btProg.Hide()
        Button1.Hide()
        btmain.Hide()
        btsetup.Hide()
        btReturn.Dock = DockStyle.Right
        btReturn.Size = btUser.Size
        btReturn.Show()

    End Sub

    Private Sub btReturn_Click(sender As Object, e As EventArgs) Handles btReturn.Click
        Home_Page(New Starting)
        btUser.Show()
        btoper.Show()
        btProg.Show()
        btmain.Show()
        Button1.Show()
        btsetup.Show()
        btReturn.Dock = DockStyle.Right
        btReturn.Hide()

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label2.Text = DateTime.Now.ToString("dd MMM HH:mm:ss")
        Dim startRegister As Integer = 204
        Dim endRegister As Integer = 205
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
        RichTextBox1.Text = strValue.TrimEnd(Chr(0))
        dval = strValue.TrimEnd(Chr(0))

        ' Check if dval has changed from the previous value
        If dval <> prevDval Then
            prevDval = dval ' Update the previous value
            If dval <> "000" Then
                ' Start asynchronous processing
                DisplayAlarmAsync(dval)
            End If
        ElseIf dval = prevDval AndAlso Checkagain = "000" Then
            ' If dval is the same as before and Checkagain is 0, display the alarm again
            If dval <> "000" Then
                DisplayAlarmAsync(dval)
            End If
        End If
    End Sub




    Private Sub btsetup_Click(sender As Object, e As EventArgs) Handles btsetup.Click
        ' Hide buttons and setup UI as needed
        btUser.Hide()
        btoper.Hide()
        btProg.Hide()
        btmain.Hide()
        btsetup.Hide()
        Button1.Hide()
        btReturn.Dock = DockStyle.Right
        btReturn.Size = btUser.Size
        btReturn.Show()
        Recipe.Button7.PerformClick()

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        btUser.Hide()
        btoper.Hide()
        btProg.Hide()
        btmain.Hide()
        btsetup.Hide()
        btReturn.Dock = DockStyle.Right
        btReturn.Size = btUser.Size
        btReturn.Show()

        normalColor = Button1.BackColor ' Store the normal color of the button
        blinking = True
        Timer2.Start()
        Timer4.Start()

    End Sub

    ' Method to accept the parameter and update Label3




    Private Sub Button1_MouseDown(sender As Object, e As MouseEventArgs) Handles Button1.MouseDown
        plc.SetDevice("M219", 1)
    End Sub

    Private Async Function Timer2_Tick(sender As Object, e As EventArgs) As Task Handles Timer2.Tick


        Dim m222Value As Integer
        Dim result As Integer = plc.GetDevice("M222", m222Value)

        If result = 0 AndAlso m222Value = 1 Then
            Timer2.Stop() ' Stop the timer to prevent further checking
            Timer4.Stop()
            plc.SetDevice("M238", 0)
            blinking = False
            Button1.BackColor = normalColor ' Revert to the normal color
            btReturn.PerformClick()
        Else
            ' Toggle the color between green and normal color to create a blinking effect
            If blinking Then
                If Button1.BackColor = normalColor Then
                    Button1.BackColor = Color.Green
                Else
                    Button1.BackColor = normalColor
                End If
            End If
        End If
    End Function

    Private Sub Button1_MouseUp(sender As Object, e As MouseEventArgs) Handles Button1.MouseUp
        plc.SetDevice("M219", 0)
    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Me.Close()
    End Sub


    Private Sub Home_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        FidCam1.CloseDevice()
        FidCam1.DestroyDevice()
        LiveCamera1.CloseDevice()
        LiveCamera1.DestroyDevice()
        plc.SetDevice("M247", 0)
    End Sub

    Private Sub Panel3_Paint(sender As Object, e As PaintEventArgs) Handles Panel3.Paint

    End Sub
    Dim Bulbcheck As Integer = 0
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Bulbcheck = Bulbcheck + 1
        If Bulbcheck = 1 Then
            plc.SetDevice("M238", 1)
        Else
            Bulbcheck = 0
            plc.SetDevice("M238", 0)
        End If
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs)

        ShowLoginPanel()
    End Sub

    Private Sub ShowLoginPanel()
        ' Create labels and textboxes for login
        Dim lblUsername As New Label()
        lblUsername.Text = "Username:"
        lblUsername.Location = New Point(50, 50)
        lblUsername.Size = New Size(100, 30)
        Me.Controls.Add(lblUsername)

        Dim txtUsername As New TextBox()
        txtUsername.Name = "txtUsername"
        txtUsername.Location = New Point(150, 50)
        txtUsername.Size = New Size(150, 30)
        Me.Controls.Add(txtUsername)

        Dim lblPassword As New Label()
        lblPassword.Text = "Password:"
        lblPassword.Location = New Point(50, 100)
        lblPassword.Size = New Size(100, 30)
        Me.Controls.Add(lblPassword)

        Dim txtPassword As New TextBox()
        txtPassword.Name = "txtPassword"
        txtPassword.Location = New Point(150, 100)
        txtPassword.Size = New Size(150, 30)
        txtPassword.PasswordChar = "*"c
        Me.Controls.Add(txtPassword)

        Dim btnLogin As New Button()
        btnLogin.Text = "Login"
        btnLogin.Location = New Point(150, 150)
        btnLogin.Size = New Size(100, 30)
        'AddHandler btnLogin.Click, AddressOf Login
        Me.Controls.Add(btnLogin)
    End Sub

    'Private Sub Login(sender As Object, e As EventArgs)
    '    Dim username As String = CType(Me.Controls("txtUsername"), TextBox).Text
    '    Dim password As String = CType(Me.Controls("txtPassword"), TextBox).Text

    '    ' Load XML file and validate login
    '    Dim doc As XDocument = XDocument.Load("UserCredentials.xml")
    '    Dim user = doc.Descendants("User").FirstOrDefault(Function(u) u.Element("Username").Value = username AndAlso u.Element("Password").Value = password)

    '    If user IsNot Nothing Then
    '        MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '        ShowSPCModuleForm()
    '    Else
    '        MessageBox.Show("Invalid username or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End If
    'End Sub

    Private Sub ShowSPCModuleForm()
        ' Remove existing login controls
        Me.Controls.Clear()

        ' Add SPC-specific controls here
        Dim lblSPC As New Label()
        lblSPC.Text = "SPC Dashboard"
        lblSPC.Location = New Point(50, 50)
        lblSPC.Size = New Size(200, 30)
        Me.Controls.Add(lblSPC)

        ' Add a Logout button
        Dim btnLogout As New Button()
        btnLogout.Text = "Logout"
        btnLogout.Location = New Point(50, 100)
        btnLogout.Size = New Size(100, 30)
        AddHandler btnLogout.Click, AddressOf Logout
        Me.Controls.Add(btnLogout)
    End Sub

    Private Sub Logout(sender As Object, e As EventArgs)
        ' Clear the SPC controls
        Me.Controls.Clear()

        ' Optionally, you could reinitialize the HomeForm or bring it back to its original state
        InitializeComponent()
    End Sub
    'Private Sub SaveAlarmToExcel(alarmName As String, alarmCode As String, time As String, recipe As String, user As String)
    '    ' Retrieve the default path from app.config
    '    Dim folderPath As String = ConfigurationManager.AppSettings("Warn")

    '    ' Ensure folderPath is not null or empty
    '    If String.IsNullOrEmpty(folderPath) Then
    '        MessageBox.Show("The folder path is not set in the configuration file.")
    '        Return
    '    End If

    '    ' Create the directory if it doesn't exist
    '    If Not Directory.Exists(folderPath) Then
    '        Directory.CreateDirectory(folderPath)
    '    End If

    '    ' Define the Excel file path with the current date as the filename
    '    Dim fileName As String = Path.Combine(folderPath, DateTime.Now.ToString("yyyy-MM-dd") & ".xlsx")
    '    Dim fileInfo As New FileInfo(fileName)

    '    ' Create or open the Excel file
    '    Using package As New ExcelPackage(fileInfo)
    '        Dim worksheet As ExcelWorksheet

    '        ' Check if any worksheets exist
    '        If package.Workbook.Worksheets.Count = 0 Then
    '            ' Add a new worksheet if none exists
    '            worksheet = package.Workbook.Worksheets.Add("Alarms")
    '            ' Add headers to the worksheet
    '            worksheet.Cells("A1").Value = "S.No"
    '            worksheet.Cells("B1").Value = "Alarm Name"
    '            worksheet.Cells("C1").Value = "Alarm Code"
    '            worksheet.Cells("D1").Value = "Time"
    '            worksheet.Cells("E1").Value = "Recipe"
    '            worksheet.Cells("F1").Value = "User"
    '            'worksheet.Cells("G1").Value = "REMEDY"
    '            'worksheet.Cells("H1").Value = "CATEGORY"
    '            'worksheet.Cells("I1").Value = "ALARM DURATION"
    '        Else
    '            ' Access the first worksheet
    '            worksheet = package.Workbook.Worksheets(1)
    '        End If

    '        ' Find the next available row
    '        Dim nextRow As Integer
    '        If worksheet.Dimension IsNot Nothing Then
    '            nextRow = worksheet.Dimension.End.Row + 1
    '        Else
    '            nextRow = 2 ' Start from row 2 if no data exists
    '        End If

    '        ' Write the data to the worksheet
    '        worksheet.Cells(nextRow, 1).Value = nextRow - 1 ' S.No
    '        worksheet.Cells(nextRow, 2).Value = alarmName
    '        worksheet.Cells(nextRow, 3).Value = alarmCode
    '        worksheet.Cells(nextRow, 4).Value = time
    '        worksheet.Cells(nextRow, 5).Value = recipe
    '        worksheet.Cells(nextRow, 6).Value = user

    '        ' Save the changes to the Excel file
    '        package.Save()
    '    End Using
    'End Sub
    Private Async Sub DisplayAlarmAsync(currentDval As String)
        ' Ensure that the alarm form is not already open
        If alarmForm Is Nothing OrElse alarmForm.IsDisposed Then
            ' Run the background processing on a different thread
            Dim alarm As Alarm = Await Task.Run(Function()
                                                    Return Alarm.Alarms.FirstOrDefault(Function(a) a.Number = currentDval)
                                                End Function)

            ' Invoke the display logic on the UI thread
            Me.Invoke(Sub()
                          If alarmForm IsNot Nothing AndAlso Not alarmForm.IsDisposed Then
                              ' If the form is already open, just update the labels
                              alarmForm.Label2.Text = currentDval.ToString()
                              alarmForm.Label3.Text = If(alarm IsNot Nothing, alarm.Name, "Unknown Alarm Code!")
                          Else
                              ' Otherwise, create a new instance of the form
                              alarmForm = New Alarms()
                              alarmForm.Label2.Text = currentDval.ToString()
                              alarmForm.Label3.Text = If(alarm IsNot Nothing, alarm.Name, "Unknown Alarm Code!")
                              alarmForm.StartPosition = FormStartPosition.CenterParent

                              ' Show the form, centered in ExcelForm
                              alarmForm.BringToFront()
                              alarmForm.Show()
                              alarmForm.TopMost = True

                          End If
                          'SaveAlarmToExcel(alarmForm.Label3.Text, alarmForm.Label2.Text, Label2.Text, Label3.Text, "user")
                      End Sub)
        End If
    End Sub

    Private Sub Home_Page_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        btReturn.PerformClick()
    End Sub
    Private isBlinking As Boolean = False
    'Private Async Function Timer4_Tick(sender As Object, e As EventArgs) As Task Handles Timer4.Tick
    '    ' Toggle blinking state between True and False
    '    isBlinking = Not isBlinking

    '    ' Use the blinking state to set the PLC device values
    '    If isBlinking Then
    '        plc.SetDevice("M238", 1) ' Turn ON
    '    Else
    '        plc.SetDevice("M238", 0) ' Turn OFF
    '    End If
    'End Function

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick

    End Sub

    Private Sub Timer5_Tick(sender As Object, e As EventArgs) Handles Timer5.Tick





    End Sub



End Class
