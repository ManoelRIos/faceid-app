
namespace Marerial_design_elements
{
    partial class UC_Info
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_Info));
            this.infoText = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.SuspendLayout();
            // 
            // infoText
            // 
            this.infoText.AutoSize = false;
            this.infoText.BackColor = System.Drawing.Color.Transparent;
            this.infoText.Font = new System.Drawing.Font("Poppins", 9F);
            this.infoText.Location = new System.Drawing.Point(0, -1);
            this.infoText.Name = "infoText";
            this.infoText.Size = new System.Drawing.Size(359, 448);
            this.infoText.TabIndex = 0;
            this.infoText.Text = resources.GetString("infoText.Text");
            // 
            // UC_Info
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.infoText);
            this.Name = "UC_Info";
            this.Size = new System.Drawing.Size(359, 371);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel infoText;
    }
}
