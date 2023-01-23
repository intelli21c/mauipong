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

		private double pointdistr(r2vect point, r2vect linegrad, r2vect lineorg)
		{
			return (linegrad.y * point.x - linegrad.x * point.y + linegrad.x * lineorg.y - linegrad.y * lineorg.x) / linegrad.l2norm();
		}

		private r2vect checkcolision(r2vect ball, r2vect ballv, r2vect player, r2vect playerv, int rsum)
		{
			double d = pointdist(player, ballv, ball);
			if (d < 45)
			{
				d = pointdistr(player, ballv, ball);
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

		r2vect vbef;
		r2vect vaft;
		private void bounce(r2vect player, r2vect colpoint) //r2vect ballv)
		{
			vbef = new(ballv.x, ballv.y);
			balll = new(ball.x, ball.y);
			r2vect normal = (colpoint - player).normalise();
			ballv = (ballv - 2 * ballv.innerprod(normal) * normal);
			vaft = new(ballv.x, ballv.y);
		}

		r2vect player = new(640, 360);
		r2vect playerv = new(0, 0);
		r2vect enemy = new(280, 360);
		r2vect enemyv = new(0, 0);

		r2vect ball = new(640, 360);
		r2vect ballv = new(0, 0);

		bool wpress = false;
		bool apress = false;
		bool spress = false;
		bool dpress = false;
		bool shftpress = false;

		void ballshoot()
		{
			ball = new(cursor.x, cursor.y);
			ballv = new(Math.Cos(sang * Math.PI / 180), Math.Sin(sang * Math.PI / 180));
			ballv = 15 * ballv;
		}

		r2vect col;
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
			col = checkcolision(ball, ballv, player, null, 45);
			if (col != null)
			{
				bounce(player, col);//, ballv);
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
			else
			{
				ball = ball + ballv;
			}

			skglControl1.Invalidate();
		}

		double cang = 0;
		double sang = 180;
		r2vect cursor = new(940, 360);
		private void testupdate(object? sender, EventArgs e)
		{
			double dang = shftpress ? 0.5 : 2;
			if (wpress) cang -= dang;
			if (spress) cang += dang;
			if (apress) sang -= dang;
			if (dpress) sang += dang;
			cursor = new(640 + 300 * Math.Cos(cang * Math.PI / 180), 360 + 300 * Math.Sin(cang * Math.PI / 180));
			col = checkcolision(ball, ballv, player, null, 45);
			if (col != null)
			{
				bounce(player, col);//, ballv);
				ball = ball + 2 * ballv; //not really...
				skglControl1.Invalidate();
				return;
			}
			int nballx = (int)(ball.x + ballv.x);
			int nbally = (int)(ball.y + ballv.y);
			if (nballx < 0 || nbally < 0 || nballx > 1280 || nbally > 720) ballv = new(0, 0);
			ball = ball + ballv;
			skglControl1.Invalidate();
		}

		public GameWindow()
		{
			InitializeComponent();
			maintimer.Tick += testupdate;
			maintimer.Enabled = true;
		}

		r2vect coll = new(0, 0);
		r2vect playl = new(0, 0);
		r2vect balll = new(0, 0);
		r2vect bvp = new(0, 0);
		r2vect bva = new(0, 0);
		private void skglControl1_PaintSurface_orig(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
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

			if (col != null)
			{
				coll = new(col.x, col.y);
				playl = new(player.x, player.y);
				balll = new(ball.x, ball.y);
				bvp = col - vbef;
				bva = col + vaft;
			}

			canvas.FillColor = Colors.Orange;
			canvas.FillCircle((int)(playl.x * scale), (int)(playl.y * scale), 3 * scale);
			canvas.FillColor = Colors.OrangeRed;
			canvas.FillCircle((int)(balll.x * scale), (int)(balll.y * scale), 3 * scale);
			canvas.StrokeColor = Colors.Blue;
			canvas.StrokeSize = 3;
			canvas.DrawLine((float)bvp.x * scale, (float)bvp.y * scale, (float)coll.x * scale, (float)coll.y * scale);
			canvas.StrokeColor = Colors.Cyan;
			canvas.DrawLine((float)coll.x * scale, (float)coll.y * scale, (float)bva.x * scale, (float)bva.y * scale);
			canvas.FillColor = Colors.Yellow;
			canvas.FillCircle((int)(coll.x * scale), (int)(coll.y * scale), 3 * scale);
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
			canvas.StrokeSize = 5 * scale;
			canvas.DrawCircle(1280 * scale / 2, 720 * scale / 2, 300 * scale);
			canvas.StrokeColor = Colors.Black;
			canvas.StrokeSize = 3;
			canvas.DrawCircle((float)cursor.x * scale, (float)cursor.y * scale, 10);
			canvas.DrawLine((float)cursor.x * scale, (float)cursor.y * scale,
				(float)(cursor.x + 500 * Math.Cos(sang * Math.PI / 180)) * scale, (float)(cursor.y + 500 * Math.Sin(sang * Math.PI / 180)) * scale);

			canvas.FillColor = Colors.White;
			canvas.FillCircle((int)(ball.x * scale), (int)(ball.y * scale), 15 * scale);
			canvas.FillColor = Colors.Red;
			canvas.FillCircle((int)(player.x * scale), (int)(player.y * scale), 30 * scale);

			if (col != null)
			{
				coll = new(col.x, col.y);
				playl = new(player.x, player.y);
				bvp = col - vbef;
				bva = col + vaft;
			}

			canvas.FillColor = Colors.Orange;
			canvas.FillCircle((int)(playl.x * scale), (int)(playl.y * scale), 3 * scale);
			canvas.FillColor = Colors.OrangeRed;
			canvas.FillCircle((int)(balll.x * scale), (int)(balll.y * scale), 3 * scale);
			canvas.StrokeColor = Colors.Blue;
			canvas.StrokeSize = 3;
			canvas.DrawLine((float)bvp.x * scale, (float)bvp.y * scale, (float)coll.x * scale, (float)coll.y * scale);
			canvas.StrokeColor = Colors.Cyan;
			canvas.DrawLine((float)coll.x * scale, (float)coll.y * scale, (float)bva.x * scale, (float)bva.y * scale);
			canvas.FillColor = Colors.Yellow;
			canvas.FillCircle((int)(coll.x * scale), (int)(coll.y * scale), 3 * scale);
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
			if (e.KeyCode == Keys.T)
			{
				if (maintimer.Enabled)
					maintimer.Enabled = false;
				else
					maintimer.Enabled = true;
			}
			if (e.KeyCode == Keys.Space)
			{
				testupdate(null, null);
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