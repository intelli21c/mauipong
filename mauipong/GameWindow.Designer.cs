namespace mauipong
{
	partial class GameWindow
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.skglControl1 = new SkiaSharp.Views.Desktop.SKGLControl();
			this.maintimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// skglControl1
			// 
			this.skglControl1.BackColor = System.Drawing.Color.Black;
			this.skglControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.skglControl1.Location = new System.Drawing.Point(0, 0);
			this.skglControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.skglControl1.Name = "skglControl1";
			this.skglControl1.Size = new System.Drawing.Size(1262, 673);
			this.skglControl1.TabIndex = 0;
			this.skglControl1.VSync = true;
			this.skglControl1.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skglControl1_PaintSurface);
			// 
			// maintimer
			// 
			this.maintimer.Interval = 16;
			// 
			// GameWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1262, 673);
			this.Controls.Add(this.skglControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.Name = "GameWindow";
			this.Text = "MauiPong";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameWindow_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameWindow_KeyUp);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameWindow_MouseDown);
			this.ResumeLayout(false);

		}

		#endregion

		private SkiaSharp.Views.Desktop.SKGLControl skglControl1;
		private System.Windows.Forms.Timer maintimer;
	}
}