using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Overarching.Debug
{
	public partial class DebugContainer : Node
	{
		public record DebugConfiguration(string menuTitle, IList<DebugUIElement> UiElements);
		public record DebugUIElement(string? Title, Control Control);
		public delegate DebugConfiguration GetDebugUI();

		[Export] private PackedScene _defaultSceneRoot = null!;

		[Export] private Window _debugPopup = null!;
		[Export] private TabContainer _debugUIRoot = null!;
		private PackedScene _debugItemScene = null!;

		private Node _root = null!;

		private IList<IDebugPanel> _debugPanelCreators = null!;

		public override void _Ready()
		{
			base._Ready();

			_debugItemScene = Pigslyer.PirateKingInbetween.Util.Reflection.PathAttribute.LoadResource<DebugItemScene>();

			_debugPopup.CloseRequested += () => 
			{
				_debugPopup.Hide();
				GetTree().Paused = false;
			};

			_root = _defaultSceneRoot.Instantiate();
			AddChild(_root);

			_debugPanelCreators = CreateEditors();

			SpawnEditors();
		}

		private static IList<IDebugPanel> CreateEditors()
		{
			List<IDebugPanel> ret = new();

			ret.Add(new Editors.LogDisplay());
			ret.Add(new Editors.LogSettings());

			return ret;
		}

		private void SpawnEditors()
		{
			foreach (IDebugPanel panel in _debugPanelCreators)
			{
				AddToDebugUI(panel);
			}
		}

		private void AddToDebugUI(IDebugPanel panel)
		{
			DebugConfiguration conf = panel.GetPanelData();

			VBoxContainer uiRoot = new()
			{
				Name = conf.menuTitle
			};

			DebugItemScene itemRoot;

			foreach (DebugUIElement item in conf.UiElements)
			{
				uiRoot.AddChild(itemRoot = _debugItemScene.Instantiate<DebugItemScene>());
				itemRoot.SetData(item.Title, item.Control);
			}

			_debugUIRoot.AddChild(uiRoot);
		}

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);

			if (@event.IsActionPressed(InputActions.ToggleDebugUI.GetAction()))
			{
				_debugPopup.Visible = true;
				GetTree().Paused = true;
			}
		}
	}
}