using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue
{
	public class DialogueLine : PanelContainer
	{
		private const string GROUP = "DIALOGUE_LINE_GROUP";

		#region Paths
		[Export] private NodePath _speakerLabelPath = null;
		[Export] private NodePath _lineLabelPath = null;
		#endregion

		public void Initialize(string speaker, string text, bool rightSide)
		{
			Label speakerLabel = GetNode<Label>(_speakerLabelPath);
			Label textLabel = GetNode<Label>(_lineLabelPath);

			speakerLabel.Text = speaker;
			textLabel.Text = text;

			speakerLabel.Align = textLabel.Align = rightSide ? Label.AlignEnum.Right : Label.AlignEnum.Left;
		}

		public static void FreeAll()
		{
			Autoloads.Global.Instance.GetTree().CallGroup(GROUP, "queue_free");
		}
	}
}