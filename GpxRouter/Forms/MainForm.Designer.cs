﻿namespace GpxRouter.Forms
{
    partial class MainForm
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
            this.groupBoxWaypoints = new System.Windows.Forms.GroupBox();
            this.buttonReverse = new System.Windows.Forms.Button();
            this.buttonPaste = new System.Windows.Forms.Button();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.dataGridViewWaypoints = new System.Windows.Forms.DataGridView();
            this.buttonSaveAs = new System.Windows.Forms.Button();
            this.groupBoxFiles = new System.Windows.Forms.GroupBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.groupBoxWaypoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWaypoints)).BeginInit();
            this.groupBoxFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxWaypoints
            // 
            this.groupBoxWaypoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxWaypoints.Controls.Add(this.buttonReverse);
            this.groupBoxWaypoints.Controls.Add(this.buttonPaste);
            this.groupBoxWaypoints.Controls.Add(this.buttonMoveDown);
            this.groupBoxWaypoints.Controls.Add(this.buttonMoveUp);
            this.groupBoxWaypoints.Controls.Add(this.buttonDelete);
            this.groupBoxWaypoints.Controls.Add(this.dataGridViewWaypoints);
            this.groupBoxWaypoints.Location = new System.Drawing.Point(10, 65);
            this.groupBoxWaypoints.Name = "groupBoxWaypoints";
            this.groupBoxWaypoints.Size = new System.Drawing.Size(342, 292);
            this.groupBoxWaypoints.TabIndex = 3;
            this.groupBoxWaypoints.TabStop = false;
            this.groupBoxWaypoints.Text = "Waypoints";
            // 
            // buttonReverse
            // 
            this.buttonReverse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonReverse.Location = new System.Drawing.Point(6, 234);
            this.buttonReverse.Name = "buttonReverse";
            this.buttonReverse.Size = new System.Drawing.Size(76, 23);
            this.buttonReverse.TabIndex = 5;
            this.buttonReverse.Text = "Reverse";
            this.buttonReverse.UseVisualStyleBackColor = true;
            this.buttonReverse.Click += new System.EventHandler(this.buttonReverse_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPaste.Location = new System.Drawing.Point(180, 263);
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(75, 23);
            this.buttonPaste.TabIndex = 8;
            this.buttonPaste.Text = "Paste";
            this.buttonPaste.UseVisualStyleBackColor = true;
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveDown.Location = new System.Drawing.Point(47, 263);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(35, 23);
            this.buttonMoveDown.TabIndex = 7;
            this.buttonMoveDown.Text = "▼";
            this.buttonMoveDown.UseVisualStyleBackColor = true;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMoveUp.Location = new System.Drawing.Point(6, 263);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(35, 23);
            this.buttonMoveUp.TabIndex = 6;
            this.buttonMoveUp.Text = "▲";
            this.buttonMoveUp.UseVisualStyleBackColor = true;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDelete.Location = new System.Drawing.Point(261, 263);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 9;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // dataGridViewWaypoints
            // 
            this.dataGridViewWaypoints.AllowDrop = true;
            this.dataGridViewWaypoints.AllowUserToDeleteRows = false;
            this.dataGridViewWaypoints.AllowUserToResizeRows = false;
            this.dataGridViewWaypoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewWaypoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewWaypoints.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridViewWaypoints.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewWaypoints.MultiSelect = false;
            this.dataGridViewWaypoints.Name = "dataGridViewWaypoints";
            this.dataGridViewWaypoints.RowHeadersVisible = false;
            this.dataGridViewWaypoints.RowHeadersWidth = 50;
            this.dataGridViewWaypoints.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewWaypoints.Size = new System.Drawing.Size(330, 209);
            this.dataGridViewWaypoints.TabIndex = 4;
            this.dataGridViewWaypoints.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewWaypoints_CellDoubleClick);
            this.dataGridViewWaypoints.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewWaypoints_CellValueChanged);
            this.dataGridViewWaypoints.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewWaypoints_KeyDown);
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSaveAs.Location = new System.Drawing.Point(261, 19);
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveAs.TabIndex = 2;
            this.buttonSaveAs.Text = "Save As...";
            this.buttonSaveAs.UseVisualStyleBackColor = true;
            this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // groupBoxFiles
            // 
            this.groupBoxFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFiles.Controls.Add(this.buttonOpen);
            this.groupBoxFiles.Controls.Add(this.buttonSaveAs);
            this.groupBoxFiles.Location = new System.Drawing.Point(10, 11);
            this.groupBoxFiles.Name = "groupBoxFiles";
            this.groupBoxFiles.Size = new System.Drawing.Size(342, 48);
            this.groupBoxFiles.TabIndex = 0;
            this.groupBoxFiles.TabStop = false;
            this.groupBoxFiles.Text = "Files";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpen.Location = new System.Drawing.Point(180, 19);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(75, 23);
            this.buttonOpen.TabIndex = 1;
            this.buttonOpen.Text = "Open...";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 368);
            this.Controls.Add(this.groupBoxFiles);
            this.Controls.Add(this.groupBoxWaypoints);
            this.Name = "MainForm";
            this.Text = "GpxRouter";
            this.groupBoxWaypoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewWaypoints)).EndInit();
            this.groupBoxFiles.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxWaypoints;
        private System.Windows.Forms.DataGridView dataGridViewWaypoints;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonSaveAs;
        private System.Windows.Forms.GroupBox groupBoxFiles;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonReverse;
    }
}

