<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Start
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.BOX1 = New System.Windows.Forms.RichTextBox()
        Me.btsrt = New System.Windows.Forms.Button()
        Me.btrs = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btrs)
        Me.Panel1.Controls.Add(Me.btsrt)
        Me.Panel1.Controls.Add(Me.BOX1)
        Me.Panel1.Controls.Add(Me.Button1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(295, 297)
        Me.Panel1.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Button1.Font = New System.Drawing.Font("Microsoft Tai Le", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.Location = New System.Drawing.Point(46, 182)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(172, 68)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "count"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'BOX1
        '
        Me.BOX1.Location = New System.Drawing.Point(46, 134)
        Me.BOX1.Name = "BOX1"
        Me.BOX1.Size = New System.Drawing.Size(102, 42)
        Me.BOX1.TabIndex = 1
        Me.BOX1.Text = ""
        '
        'btsrt
        '
        Me.btsrt.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btsrt.Font = New System.Drawing.Font("Microsoft Tai Le", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btsrt.Location = New System.Drawing.Point(154, 134)
        Me.btsrt.Name = "btsrt"
        Me.btsrt.Size = New System.Drawing.Size(64, 44)
        Me.btsrt.TabIndex = 2
        Me.btsrt.Text = "SET"
        Me.btsrt.UseVisualStyleBackColor = True
        '
        'btrs
        '
        Me.btrs.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btrs.Font = New System.Drawing.Font("Microsoft Tai Le", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btrs.Location = New System.Drawing.Point(154, 84)
        Me.btrs.Name = "btrs"
        Me.btrs.Size = New System.Drawing.Size(64, 44)
        Me.btrs.TabIndex = 3
        Me.btrs.Text = "RST"
        Me.btrs.UseVisualStyleBackColor = True
        '
        'Start
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(295, 297)
        Me.Controls.Add(Me.Panel1)
        Me.MaximumSize = New System.Drawing.Size(311, 336)
        Me.Name = "Start"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Start"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents Button1 As Button
    Friend WithEvents btrs As Button
    Friend WithEvents btsrt As Button
    Friend WithEvents BOX1 As RichTextBox
End Class
