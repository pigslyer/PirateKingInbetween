using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Singletons
{
	public partial class DebugDraw : Singleton<DebugDraw>
	{
		private DrawingNode2D _drawFrom = new();
		private List<DrawOperationBase> _requestedOps = new();
		private Font _font = null!;

		public override void _Ready()
		{
			base._Ready();

			AddChild(_drawFrom);
			_drawFrom.ZIndex = 1000;

			_font = ThemeDB.FallbackFont;
		}

		public static void DrawArrow(Vector2 from, Vector2 to, Color color)
		{
			Instance._requestedOps.Add(new DrawOperationArrow(from, to, color));
		}

		// set to 0 after every draw queue
		private int _drawMouseOps = 0;
		public static void DrawTextMouse(string text)
		{
			Instance._requestedOps.Add(new DrawOperationTextMouse(text, Instance._drawMouseOps++));
		}

		public override void _Process(double _)
		{
			if (_requestedOps.Count > 0)
			{
				_drawFrom.DrawQueued(_requestedOps);
				_drawMouseOps = 0;
			}
		}

		
		private partial class DrawingNode2D : Node2D
		{
			private List<DrawOperationBase> _draws = null!;

			public void DrawQueued(List<DrawOperationBase> draws)
			{
				_draws = draws;
				
				QueueRedraw();
			}

			public override void _Draw()
			{
				base._Draw();

				if (_draws != null)
				{
					foreach (var draw in _draws)
					{
						draw.Draw(this);
					}

					_draws.Clear();
				}
			}
		}

		private record DrawOperationArrow(Vector2 From, Vector2 To, Color Color) : DrawOperationBase()
		{
			private const float ARROW_LINE_WIDTH = 4;
			private const float ARROW_SIDE_WIDTH = 2;
			private const float ARROW_SIDE_LENGTH = 12;
			private static readonly float ARROW_SIDE_ANGLE = Mathf.DegToRad(30);

			public override void Draw(CanvasItem canvas)
			{
				if (From.IsEqualApprox(To))
				{
					return;
				}
				
				canvas.DrawLine(From, To, Color, width: ARROW_LINE_WIDTH);

				var angle = (-(To - From)).Angle();

				canvas.DrawLine(To, To + new Vector2(ARROW_SIDE_LENGTH, 0).Rotated(angle + ARROW_SIDE_ANGLE), Color, ARROW_SIDE_WIDTH);
				canvas.DrawLine(To, To + new Vector2(ARROW_SIDE_LENGTH, 0).Rotated(angle - ARROW_SIDE_ANGLE), Color, ARROW_SIDE_WIDTH);
			}
		}

		private record DrawOperationTextMouse(string Text, int Index) : DrawOperationBase()
		{
			private const int FONT_SIZE = 9;

			private Font Font => DebugDraw.Instance._font;
			private float Height => Font.GetHeight(FONT_SIZE);
			public override void Draw(CanvasItem canvas)
			{
				canvas.DrawString(Font, canvas.GetGlobalMousePosition() + new Vector2(0, Height * (Index + 2)), Text, fontSize: FONT_SIZE);
			}
		}

		private abstract record DrawOperationBase()
		{
			public abstract void Draw(CanvasItem canvas);
		}
	}
}