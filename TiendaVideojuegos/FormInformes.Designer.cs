namespace TiendaVideojuegos
{
    partial class FormInformes
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.reportViewerVentas = new Microsoft.Reporting.WinForms.ReportViewer();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTotalInformeVentas = new System.Windows.Forms.Label();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dtpHasta = new System.Windows.Forms.DateTimePicker();
            this.dtpDesde = new System.Windows.Forms.DateTimePicker();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnRefrescarStock = new System.Windows.Forms.Button();
            this.dgvInformeStock = new System.Windows.Forms.DataGridView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInformeStock)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(22, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(826, 483);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.reportViewerVentas);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.lblTotalInformeVentas);
            this.tabPage1.Controls.Add(this.btnBuscar);
            this.tabPage1.Controls.Add(this.dtpHasta);
            this.tabPage1.Controls.Add(this.dtpDesde);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(818, 457);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Informe de Ventas";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // reportViewerVentas
            // 
            this.reportViewerVentas.LocalReport.ReportEmbeddedResource = "TiendaVideojuegos.InformeVentas.rdlc";
            this.reportViewerVentas.Location = new System.Drawing.Point(24, 59);
            this.reportViewerVentas.Name = "reportViewerVentas";
            this.reportViewerVentas.ServerReport.BearerToken = null;
            this.reportViewerVentas.Size = new System.Drawing.Size(754, 350);
            this.reportViewerVentas.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(314, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Hasta:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(21, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Desde:";
            // 
            // lblTotalInformeVentas
            // 
            this.lblTotalInformeVentas.AutoSize = true;
            this.lblTotalInformeVentas.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalInformeVentas.Location = new System.Drawing.Point(558, 412);
            this.lblTotalInformeVentas.Name = "lblTotalInformeVentas";
            this.lblTotalInformeVentas.Size = new System.Drawing.Size(124, 24);
            this.lblTotalInformeVentas.TabIndex = 1;
            this.lblTotalInformeVentas.Text = "Total: 0,00 €";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(589, 30);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(109, 23);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Generar Informe";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dtpHasta
            // 
            this.dtpHasta.Location = new System.Drawing.Point(366, 30);
            this.dtpHasta.Name = "dtpHasta";
            this.dtpHasta.Size = new System.Drawing.Size(200, 20);
            this.dtpHasta.TabIndex = 1;
            // 
            // dtpDesde
            // 
            this.dtpDesde.Location = new System.Drawing.Point(78, 30);
            this.dtpDesde.Name = "dtpDesde";
            this.dtpDesde.Size = new System.Drawing.Size(200, 20);
            this.dtpDesde.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnRefrescarStock);
            this.tabPage2.Controls.Add(this.dgvInformeStock);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(818, 457);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Informe de Stock Crítico";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnRefrescarStock
            // 
            this.btnRefrescarStock.Location = new System.Drawing.Point(271, 18);
            this.btnRefrescarStock.Name = "btnRefrescarStock";
            this.btnRefrescarStock.Size = new System.Drawing.Size(137, 23);
            this.btnRefrescarStock.TabIndex = 1;
            this.btnRefrescarStock.Text = "Actualizar Informe";
            this.btnRefrescarStock.UseVisualStyleBackColor = true;
            this.btnRefrescarStock.Click += new System.EventHandler(this.btnRefrescarStock_Click);
            // 
            // dgvInformeStock
            // 
            this.dgvInformeStock.AllowUserToAddRows = false;
            this.dgvInformeStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInformeStock.Location = new System.Drawing.Point(18, 47);
            this.dgvInformeStock.Name = "dgvInformeStock";
            this.dgvInformeStock.ReadOnly = true;
            this.dgvInformeStock.Size = new System.Drawing.Size(691, 344);
            this.dgvInformeStock.TabIndex = 0;
            // 
            // FormInformes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(899, 519);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormInformes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Informes y Estadísticas";
            this.Load += new System.EventHandler(this.FormInformes_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInformeStock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DateTimePicker dtpHasta;
        private System.Windows.Forms.DateTimePicker dtpDesde;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTotalInformeVentas;
        private System.Windows.Forms.Button btnRefrescarStock;
        private System.Windows.Forms.DataGridView dgvInformeStock;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewerVentas;
    }
}