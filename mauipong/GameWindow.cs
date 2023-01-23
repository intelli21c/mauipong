using Microsoft.Maui.Graphics.Skia;
using Microsoft.Maui.Graphics;

namespace mauipong
{
	public partial class GameWindow : Form
	{
		class r2vect
		{
			public double x, y;
			public r2vect()
			{
			}
			public r2vect(double x, double y)
			{
				this.x = x;
				this.y = y;
			}
			public static r2vect operator +(r2vect tis, r2vect tat)
			{
				return new r2vect(tis.x + tat.x, tis.y + tat.y);
			}
			public static r2vect operator -(r2vect tis, r2vect tat)
			{
				return new r2vect(tis.x - tat.x, tis.y - tat.y);
			}
			public static r2vect operator *(double d, r2vect tis)
			{
				return new r2vect(d * tis.x, d * tis.y);
			}

			public double l2norm()
			{
				return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
			}
			public r2vect normalise()
			{
				return new r2vect((x / l2norm()), (y / l2norm()));
			}
			public r2vect orthocomp()
			{
				return new r2vect(-y, x);
			}

			public double innerprod(r2vect v)
			{
				return this.x * v.x + this.y * v.y;
			}
		}
		private double pointdist(r2vect point, r2vect linegrad, r2vect lineorg)
		{
			return Math.Abs((linegrad.y * point.x - linegrad.x * point.y + linegrad.x * lineorg.y - linegrad.y * lineorg.x)) / linegrad.l2norm();
		}

		private double pointdistsigned(r2vect point, r2vect linegrad, r2vect lineorg)
		{
			return (linegrad.y * point.x - linegrad.x * point.y + linegrad.x * lineorg.y - linegrad.y * lineorg.x) / linegrad.l2norm();
		}

		private r2vect checkcolision(r2vect ball, r2vect ballv, r2vect player, r2vect playerv, int rsum)
		{
			double d = pointdistsigned(player, ballv, ball);
			if (Math.Abs(d) < 45)
			{
				r2vect foot = d * (ballv.orthocomp().normalise()) + player;
				r2vect travel = ball - foot;
				if ((travel.l2norm() - (Math.Sqrt(Math.Pow(rsum, 2) - Math.Pow(d, 2)))) > ballv.l2norm()) return null;
				if ((travel.l2norm() - (Math.Sqrt(Math.Pow(rsum, 2) - Math.Pow(d, 2)))) < ballv.l2norm() &&
					(travel.l2norm() + (Math.Sqrt(Math.Pow(rsum, 2) - Math.Pow(d, 2)))) > ballv.l2norm())
				{
					this.ball = (Math.Sqrt(Math.Pow(rsum, 2) - Math.Pow(d, 2))) * (-1 * ballv.normalise()) + foot;
				}
				return 30 * (((Math.Sqrt(Math.Pow(rsum, 2) - Math.Pow(d, 2))) * (-1 * ballv.normalise()) + foot) - player).normalise() + player;
			}
			return null;
		}


		private void bounce(r2vect playr, r2vect colpoint) //r2vect ballv)
		{
			r2vect normal = (colpoint - playr).normalise();
			ballv = (ballv - 2 * ballv.innerprod(normal) * normal);
		}

		r2vect player = new(1000, 360);
		r2vect playerv = new(0, 0);
		r2vect enemy = new(280, 360);
		r2vect enemyv = new(0, 0);

		r2vect ball = new(640, 360);
		r2vect ballv = new(0, 0);

		int playerscore = 0;
		int enemyscore = 0;

		bool wpress = false;
		bool apress = false;
		bool spress = false;
		bool dpress = false;
		bool shftpress = false;

		void ballshoot()
		{
			ball = new(640, 360);
			var RNG = new Random();
			double ang = 0;
			do
			{
				ang = RNG.Next(0, 360);
			}
			while (!((ang > 60 && ang < 120) || (ang > 240 && ang < 300)));
			ballv = new(Math.Cos(ang), Math.Sin(ang));
			ballv = 15 * ballv;
		}

		private void mainupdate(object? sender, EventArgs e)
		{
			double compvel = 0;
			int playervel = shftpress ? 7 : 15;
			compvel = playervel;
			if ((wpress || spress) && (apress || dpress)) compvel = playervel / Math.Sqrt(2);
			if (wpress)
			{
				if (player.y - 10 - 30 > 0)
					player.y -= compvel;
			}
			if (apress)
			{
				if (player.x - 10 - 30 > (1280 / 2))
					player.x -= compvel;
			}
			if (spress)
			{
				if (player.y + 10 + 30 < 720)
					player.y += compvel;
			}
			if (dpress)
			{
				if (player.x + 10 + 30 < 1280)
					player.x += compvel;
			}



			int nballx = (int)(ball.x + ballv.x);
			int nbally = (int)(ball.y + ballv.y);
			if (nballx < 0 && (nbally > 240 && nbally < 480))
			{
				playerscore++;
				ballshoot();
				skglControl1.Invalidate();
				return;
			}
			if (nballx > 1280 && (nbally > 240 && nbally < 480))
			{
				enemyscore++;
				ballshoot();
				skglControl1.Invalidate();
				return;
			}
			r2vect col = checkcolision(ball, ballv, player, null, 45);
			if (col != null)
			{
				bounce(player, col);//, ballv);
				ball = ball + 2 * ballv; //not really...
				skglControl1.Invalidate();
				return;
			}
			r2vect cole = checkcolision(ball, ballv, enemy, null, 45);
			if (cole != null)
			{
				bounce(enemy, cole);//, ballv);
				ball = ball + 2 * ballv; //not really...
				skglControl1.Invalidate();
				return;
			}

			if (nballx < 0 && (nbally < 240 || nbally > 480))
			{
				ballv.x = -ballv.x;
				skglControl1.Invalidate();
				return;
			}
			if (nballx > 1280 && (nbally < 240 || nbally > 480))
			{
				ballv.x = -ballv.x;
				skglControl1.Invalidate();
				return;
			}
			if (nbally < 0 || nbally > 720)
			{
				ballv.y = -ballv.y;
				skglControl1.Invalidate();
				return;
			}

			ball = ball + ballv;
			skglControl1.Invalidate();
		}

		public GameWindow()
		{
			InitializeComponent();
			maintimer.Tick += mainupdate;
			maintimer.Enabled = true;
		}

		private void skglControl1_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			float scale = skglControl1.Height / 720f;
			ICanvas canvas = new SkiaCanvas() { Canvas = e.Surface.Canvas };
			e.Surface.Canvas.Clear();
			canvas.FillColor = Colors.Grey;
			canvas.FillRectangle(0, 0, Width, Height);

			//draw field
			canvas.StrokeColor = Colors.White;
			canvas.StrokeSize = 30;
			canvas.DrawRectangle(0, 0, 1280 * scale, 720 * scale);
			canvas.StrokeColor = Colors.Black;
			canvas.StrokeSize = 15;
			canvas.DrawLine((0 + 7) * scale, 240 * scale, (0 + 7) * scale, 480 * scale);
			canvas.DrawLine((1280 - 7) * scale, 240 * scale, (1280 - 7) * scale, 480 * scale);
			canvas.StrokeColor = Colors.White;
			canvas.StrokeDashPattern = new float[] { 2 * scale, 2 * scale };
			canvas.DrawLine(640 * scale, 0 * scale, 640 * scale, 720 * scale);
			canvas.StrokeDashPattern = null;

			canvas.Font = Microsoft.Maui.Graphics.Font.Default;
			canvas.FontColor = Colors.White;
			canvas.FontSize = 50 * scale;
			canvas.DrawString(playerscore.ToString(), (640 + 50) * scale, 0 * scale, 100, 100,
				Microsoft.Maui.Graphics.HorizontalAlignment.Center, Microsoft.Maui.Graphics.VerticalAlignment.Center);
			canvas.DrawString(enemyscore.ToString(), (640 - 150) * scale, 0 * scale, 100, 100,
				Microsoft.Maui.Graphics.HorizontalAlignment.Center, Microsoft.Maui.Graphics.VerticalAlignment.Center);


			canvas.FillColor = Colors.White;
			canvas.FillCircle((int)(ball.x * scale), (int)(ball.y * scale), 15 * scale);
			canvas.FillColor = Colors.Red;
			canvas.FillCircle((int)(player.x * scale), (int)(player.y * scale), 30 * scale);
			canvas.FillColor = Colors.Blue;
			canvas.FillCircle((int)(enemy.x * scale), (int)(enemy.y * scale), 30 * scale);
		}

		private void GameWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F)
			{
				if (this.FormBorderStyle == FormBorderStyle.None &&
				this.WindowState == FormWindowState.Maximized)
				{
					this.FormBorderStyle = FormBorderStyle.FixedSingle;
					this.WindowState = FormWindowState.Normal;
					this.Width = 1280;
					this.Height = 720;
					return;
				}
				this.FormBorderStyle = FormBorderStyle.None;
				this.WindowState = FormWindowState.Maximized;
			}
			if (e.KeyCode == Keys.R)
			{
				ballshoot();
			}
			if (e.KeyCode == Keys.G)
			{
				playerscore = 0;
				enemyscore = 0;
				ball = new(640, 360);
				ballv = new(0, 0);
			}
			if (!wpress && e.KeyCode == Keys.W)
			{
				wpress = true;
			}
			if (!apress && e.KeyCode == Keys.A)
			{
				apress = true;
			}
			if (!spress && e.KeyCode == Keys.S)
			{
				spress = true;
			}
			if (!dpress && e.KeyCode == Keys.D)
			{
				dpress = true;
			}
			if (!shftpress && e.KeyCode == Keys.ShiftKey)
			{
				shftpress = true;
			}
		}

		private void GameWindow_KeyUp(object sender, KeyEventArgs e)
		{
			if (wpress && e.KeyCode == Keys.W)
			{
				wpress = false;
			}
			if (apress && e.KeyCode == Keys.A)
			{
				apress = false;
			}
			if (spress && e.KeyCode == Keys.S)
			{
				spress = false;
			}
			if (dpress && e.KeyCode == Keys.D)
			{
				dpress = false;
			}
			if (shftpress && e.KeyCode == Keys.ShiftKey)
			{
				shftpress = false;
			}
		}
	}
}