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

		private r2vect checkcolision(r2vect ball, r2vect ballv, r2vect player, r2vect playerv, int rsum)
		{
			if (pointdist(player, ballv, ball) < rsum)
			{
				double d = pointdist(player, ballv, ball);
				r2vect foot = -d * (ballv.orthocomp().normalise()) + player;
				r2vect travel = ball - foot;
				if (travel.l2norm() > ballv.l2norm()) return null;
				return (Math.Sqrt(Math.Pow(rsum, 2) + Math.Pow(d, 2))) * (-1 * ballv.normalise()) + foot;
			}
			return null;
		}
		private void bounce(r2vect player, r2vect colpoint, r2vect ballv)
		{
			r2vect normal = (colpoint - player).normalise();
			this.ballv = (ballv - 2 * ballv.innerprod(normal) * normal);
		}

		r2vect player = new(1000, 360);
		int playerv = 15;
		r2vect enemy = new(280, 360);

		r2vect ball = new(640, 360);
		r2vect ballv = new(0, 0);

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
			/*do
			{
				ang = RNG.Next(0, 360);
			}
			while (!((ang > 60 && ang < 120) || (ang > 240 && ang < 300)));
			ballv = new(Math.Cos(ang), Math.Sin(ang));*/
			ballv = new(1, 0);
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
				ballshoot();
				skglControl1.Invalidate();
				return;
			}
			if (nballx > 1280 && (nbally > 240 && nbally < 480))
			{
				ballshoot();
				skglControl1.Invalidate();
				return;
			}
			r2vect col = checkcolision(ball, ballv, player, null, 45);
			if (col != null)
			{
				bounce(player, col, ballv);
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
			else
			{
				ball = ball + ballv;
			}

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

			canvas.FillColor = Colors.White;
			canvas.FillCircle((int)(ball.x * scale), (int)(ball.y * scale), 15 * scale);
			canvas.FillColor = Colors.Red;
			canvas.FillCircle((int)(player.x * scale), (int)(player.y * scale), 30 * scale);
			canvas.FillColor = Colors.Blue;
			canvas.FillCircle((int)(enemy.x * scale), (int)(enemy.y * scale), 30 * scale);
		}

		private void GameWindow_MouseDown(object sender, MouseEventArgs e)
		{
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