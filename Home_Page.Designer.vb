<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Home_Page
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Home_Page))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.pnDock = New System.Windows.Forms.Panel()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button9 = New System.Windows.Forms.Button()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btProg = New System.Windows.Forms.Button()
        Me.btoper = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btReturn = New System.Windows.Forms.Button()
        Me.btsetup = New System.Windows.Forms.Button()
        Me.btmain = New System.Windows.Forms.Button()
        Me.btUser = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.pnDock)
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(720, 847)
        Me.Panel1.TabIndex = 1
        '
        'pnDock
        '
        Me.pnDock.BackColor = System.Drawing.Color.IndianRed
        Me.pnDock.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnDock.Location = New System.Drawing.Point(0, 90)
        Me.pnDock.Name = "pnDock"
        Me.pnDock.Size = New System.Drawing.Size(720, 696)
        Me.pnDock.TabIndex = 2
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.SystemColors.ButtonFace
        Me.Panel3.Controls.Add(Me.Label2)
        Me.Panel3.Controls.Add(Me.Panel4)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(720, 90)
        Me.Panel3.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.OrangeRed
        Me.Label2.Location = New System.Drawing.Point(627, 13)
        Me.Label2.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(192, 21)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "lblDateTime"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Panel4
        '
        Me.Panel4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Panel4.Controls.Add(Me.Button2)
        Me.Panel4.Controls.Add(Me.Button9)
        Me.Panel4.Controls.Add(Me.Button8)
        Me.Panel4.Location = New System.Drawing.Point(379, 46)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(337, 41)
        Me.Panel4.TabIndex = 0
        '
        'Button2
        '
        Me.Button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Button2.BackColor = System.Drawing.Color.Maroon
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button2.Location = New System.Drawing.Point(130, 0)
        Me.Button2.Margin = New System.Windows.Forms.Padding(2)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(69, 41)
        Me.Button2.TabIndex = 8
        Me.Button2.Text = "MES"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Button9
        '
        Me.Button9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Button9.BackColor = System.Drawing.Color.Transparent
        Me.Button9.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button9.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button9.Image = CType(resources.GetObject("Button9.Image"), System.Drawing.Image)
        Me.Button9.Location = New System.Drawing.Point(199, 0)
        Me.Button9.Margin = New System.Windows.Forms.Padding(2)
        Me.Button9.Name = "Button9"
        Me.Button9.Size = New System.Drawing.Size(69, 41)
        Me.Button9.TabIndex = 7
        Me.Button9.UseVisualStyleBackColor = False
        '
        'Button8
        '
        Me.Button8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Button8.BackColor = System.Drawing.Color.Transparent
        Me.Button8.Dock = System.Windows.Forms.DockStyle.Right
        Me.Button8.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.Button8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button8.Image = Global.Gui_Tset.My.Resources.Resources.icons8_logout_50
        Me.Button8.Location = New System.Drawing.Point(268, 0)
        Me.Button8.Margin = New System.Windows.Forms.Padding(2)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(69, 41)
        Me.Button8.TabIndex = 4
        Me.Button8.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        Me.Button8.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.IndianRed
        Me.Panel2.Controls.Add(Me.btProg)
        Me.Panel2.Controls.Add(Me.btoper)
        Me.Panel2.Controls.Add(Me.Button1)
        Me.Panel2.Controls.Add(Me.btReturn)
        Me.Panel2.Controls.Add(Me.btsetup)
        Me.Panel2.Controls.Add(Me.btmain)
        Me.Panel2.Controls.Add(Me.btUser)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel2.Location = New System.Drawing.Point(0, 786)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(720, 61)
        Me.Panel2.TabIndex = 0
        '
        'btProg
        '
        Me.btProg.BackColor = System.Drawing.Color.Silver
        Me.btProg.Dock = System.Windows.Forms.DockStyle.Left
        Me.btProg.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btProg.Location = New System.Drawing.Point(384, 0)
        Me.btProg.Margin = New System.Windows.Forms.Padding(2)
        Me.btProg.Name = "btProg"
        Me.btProg.Size = New System.Drawing.Size(128, 61)
        Me.btProg.TabIndex = 5
        Me.btProg.Text = "PROGRAMMING"
        Me.btProg.UseVisualStyleBackColor = False
        '
        'btoper
        '
        Me.btoper.BackColor = System.Drawing.Color.Silver
        Me.btoper.Dock = System.Windows.Forms.DockStyle.Left
        Me.btoper.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btoper.Location = New System.Drawing.Point(256, 0)
        Me.btoper.Margin = New System.Windows.Forms.Padding(2)
        Me.btoper.Name = "btoper"
        Me.btoper.Size = New System.Drawing.Size(128, 61)
        Me.btoper.TabIndex = 4
        Me.btoper.Text = "OPERATION"
        Me.btoper.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.Silver
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Button1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(128, 0)
        Me.Button1.Margin = New System.Windows.Forms.Padding(2)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(128, 61)
        Me.Button1.TabIndex = 9
        Me.Button1.Text = "HOMING "
        Me.Button1.UseVisualStyleBackColor = False
        '
        'btReturn
        '
        Me.btReturn.Dock = System.Windows.Forms.DockStyle.Right
        Me.btReturn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btReturn.Location = New System.Drawing.Point(454, 0)
        Me.btReturn.Margin = New System.Windows.Forms.Padding(2)
        Me.btReturn.Name = "btReturn"
        Me.btReturn.Size = New System.Drawing.Size(10, 61)
        Me.btReturn.TabIndex = 8
        Me.btReturn.Text = "RETURN"
        Me.btReturn.UseVisualStyleBackColor = True
        '
        'btsetup
        '
        Me.btsetup.BackColor = System.Drawing.Color.Silver
        Me.btsetup.Dock = System.Windows.Forms.DockStyle.Right
        Me.btsetup.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btsetup.Location = New System.Drawing.Point(464, 0)
        Me.btsetup.Margin = New System.Windows.Forms.Padding(2)
        Me.btsetup.Name = "btsetup"
        Me.btsetup.Size = New System.Drawing.Size(128, 61)
        Me.btsetup.TabIndex = 7
        Me.btsetup.Text = "SETUP"
        Me.btsetup.UseVisualStyleBackColor = False
        '
        'btmain
        '
        Me.btmain.BackColor = System.Drawing.Color.Silver
        Me.btmain.Dock = System.Windows.Forms.DockStyle.Right
        Me.btmain.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btmain.Location = New System.Drawing.Point(592, 0)
        Me.btmain.Margin = New System.Windows.Forms.Padding(2)
        Me.btmain.Name = "btmain"
        Me.btmain.Size = New System.Drawing.Size(128, 61)
        Me.btmain.TabIndex = 6
        Me.btmain.Text = "MAINTENANCE"
        Me.btmain.UseVisualStyleBackColor = False
        '
        'btUser
        '
        Me.btUser.BackColor = System.Drawing.Color.Silver
        Me.btUser.Dock = System.Windows.Forms.DockStyle.Left
        Me.btUser.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btUser.Location = New System.Drawing.Point(0, 0)
        Me.btUser.Margin = New System.Windows.Forms.Padding(2)
        Me.btUser.Name = "btUser"
        Me.btUser.Size = New System.Drawing.Size(128, 61)
        Me.btUser.TabIndex = 3
        Me.btUser.Text = "USER"
        Me.btUser.UseVisualStyleBackColor = False
        '
        'Timer1
        '
        '
        'Timer2
        '
        '
        'Timer3
        '
        '
        'Home_Page
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(720, 847)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Home_Page"
        Me.Text = "LASER MARKING MACHINE"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel1.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel4.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents Button9 As Button
    Friend WithEvents Button8 As Button
    Friend WithEvents btReturn As Button
    Friend WithEvents btsetup As Button
    Friend WithEvents btmain As Button
    Friend WithEvents btProg As Button
    Friend WithEvents btoper As Button
    Friend WithEvents btUser As Button
    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents pnDock As Windows.Forms.Panel
    Friend WithEvents Panel3 As Windows.Forms.Panel
    Friend WithEvents Panel4 As Windows.Forms.Panel
    Friend WithEvents Panel2 As Windows.Forms.Panel
    Friend WithEvents Timer1 As Timer
    Friend WithEvents Button1 As Button
    Friend WithEvents Timer2 As Timer
    Friend WithEvents Timer3 As Timer
    Friend WithEvents Button2 As Button
End Class
