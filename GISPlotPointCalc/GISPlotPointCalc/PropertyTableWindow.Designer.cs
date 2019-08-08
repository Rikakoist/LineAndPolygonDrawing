namespace GISPlotPointCalc
{
    partial class PropertyTableWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyTableWindow));
            this.PointsDataGridView = new System.Windows.Forms.DataGridView();
            this.PointNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DelButton = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.PointsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // PointsDataGridView
            // 
            this.PointsDataGridView.AllowUserToAddRows = false;
            this.PointsDataGridView.AllowUserToDeleteRows = false;
            this.PointsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.HotTrack;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.DarkSlateGray;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.PointsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.PointsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PointsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PointNo,
            this.X,
            this.Y,
            this.DelButton});
            this.PointsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PointsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.PointsDataGridView.Name = "PointsDataGridView";
            this.PointsDataGridView.ReadOnly = true;
            this.PointsDataGridView.RowTemplate.Height = 23;
            this.PointsDataGridView.Size = new System.Drawing.Size(214, 361);
            this.PointsDataGridView.TabIndex = 0;
            this.PointsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.SaveCurrentValue);
            // 
            // PointNo
            // 
            this.PointNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.PointNo.Frozen = true;
            this.PointNo.HeaderText = "点号";
            this.PointNo.MaxInputLength = 10;
            this.PointNo.MinimumWidth = 20;
            this.PointNo.Name = "PointNo";
            this.PointNo.ReadOnly = true;
            this.PointNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PointNo.Width = 35;
            // 
            // X
            // 
            this.X.HeaderText = "X";
            this.X.MaxInputLength = 3;
            this.X.MinimumWidth = 60;
            this.X.Name = "X";
            this.X.ReadOnly = true;
            this.X.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.X.Width = 60;
            // 
            // Y
            // 
            this.Y.HeaderText = "Y";
            this.Y.MaxInputLength = 3;
            this.Y.MinimumWidth = 60;
            this.Y.Name = "Y";
            this.Y.ReadOnly = true;
            this.Y.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Y.Width = 60;
            // 
            // DelButton
            // 
            this.DelButton.HeaderText = "";
            this.DelButton.MinimumWidth = 40;
            this.DelButton.Name = "DelButton";
            this.DelButton.ReadOnly = true;
            this.DelButton.Text = "删除";
            this.DelButton.UseColumnTextForButtonValue = true;
            this.DelButton.Visible = false;
            this.DelButton.Width = 40;
            // 
            // PropertyTableWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(214, 361);
            this.ControlBox = false;
            this.Controls.Add(this.PointsDataGridView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.MaximizeBox = false;
            this.Name = "PropertyTableWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "属性表";
            ((System.ComponentModel.ISupportInitialize)(this.PointsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.DataGridView PointsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn X;
        private System.Windows.Forms.DataGridViewTextBoxColumn Y;
        private System.Windows.Forms.DataGridViewButtonColumn DelButton;
    }
}