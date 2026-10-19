namespace ErrorProofing
{
    partial class AddMaterialID
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
            this.lbSuggest = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_CellID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Material = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbSuggest
            // 
            this.lbSuggest.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbSuggest.FormattingEnabled = true;
            this.lbSuggest.ItemHeight = 30;
            this.lbSuggest.Location = new System.Drawing.Point(310, 84);
            this.lbSuggest.Name = "lbSuggest";
            this.lbSuggest.Size = new System.Drawing.Size(136, 94);
            this.lbSuggest.TabIndex = 17;
            this.lbSuggest.Visible = false;
            this.lbSuggest.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbSuggest_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(13, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 30);
            this.label1.TabIndex = 14;
            this.label1.Text = "物料位置";
            // 
            // tb_CellID
            // 
            this.tb_CellID.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_CellID.Location = new System.Drawing.Point(156, 24);
            this.tb_CellID.Margin = new System.Windows.Forms.Padding(4);
            this.tb_CellID.Multiline = true;
            this.tb_CellID.Name = "tb_CellID";
            this.tb_CellID.ReadOnly = true;
            this.tb_CellID.Size = new System.Drawing.Size(198, 37);
            this.tb_CellID.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(13, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 30);
            this.label5.TabIndex = 4;
            this.label5.Text = "物料号";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(419, 60);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(163, 161);
            this.button2.TabIndex = 8;
            this.button2.Text = "修改";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 18;
            this.label2.Text = "物料入库";
            // 
            // tb_Material
            // 
            this.tb_Material.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_Material.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.tb_Material.Location = new System.Drawing.Point(156, 87);
            this.tb_Material.Multiline = true;
            this.tb_Material.Name = "tb_Material";
            this.tb_Material.Size = new System.Drawing.Size(198, 40);
            this.tb_Material.TabIndex = 19;
            this.tb_Material.TextChanged += new System.EventHandler(this.tb_Material_TextChanged);
            // 
            // AddMaterialID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 364);
            this.Controls.Add(this.tb_Material);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbSuggest);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tb_CellID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AddMaterialID";
            this.Text = "AddMaterialID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddMaterialID_FormClosing);
            this.Load += new System.EventHandler(this.AddMaterialID_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_CellID;
        private System.Windows.Forms.ListBox lbSuggest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Material;
    }
}