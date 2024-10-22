<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BoardDetailForm
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BoardDetailForm))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.DataGridViewBoardDetails = New System.Windows.Forms.DataGridView()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.PictureBoxPCBA = New System.Windows.Forms.PictureBox()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridViewBoardDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        CType(Me.PictureBoxPCBA, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.DataGridViewBoardDetails)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.FlowLayoutPanel1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(850, 1061)
        Me.Panel1.TabIndex = 0
        '
        'DataGridViewBoardDetails
        '
        Me.DataGridViewBoardDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridViewBoardDetails.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridViewBoardDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewBoardDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridViewBoardDetails.Location = New System.Drawing.Point(0, 646)
        Me.DataGridViewBoardDetails.Name = "DataGridViewBoardDetails"
        Me.DataGridViewBoardDetails.Size = New System.Drawing.Size(850, 415)
        Me.DataGridViewBoardDetails.TabIndex = 2
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.PictureBoxPCBA)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel2.Location = New System.Drawing.Point(0, 116)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(850, 530)
        Me.Panel2.TabIndex = 1
        '
        'PictureBoxPCBA
        '
        Me.PictureBoxPCBA.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBoxPCBA.Image = CType(resources.GetObject("PictureBoxPCBA.Image"), System.Drawing.Image)
        Me.PictureBoxPCBA.Location = New System.Drawing.Point(0, 0)
        Me.PictureBoxPCBA.Name = "PictureBoxPCBA"
        Me.PictureBoxPCBA.Size = New System.Drawing.Size(850, 530)
        Me.PictureBoxPCBA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxPCBA.TabIndex = 0
        Me.PictureBoxPCBA.TabStop = False
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(850, 116)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'BoardDetailForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(850, 1061)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "BoardDetailForm"
        Me.Text = "BoardDetailForm"
        Me.Panel1.ResumeLayout(False)
        CType(Me.DataGridViewBoardDetails, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        CType(Me.PictureBoxPCBA, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Windows.Forms.Panel
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents DataGridViewBoardDetails As DataGridView
    Friend WithEvents Panel2 As Windows.Forms.Panel
    Friend WithEvents PictureBoxPCBA As PictureBox
End Class
