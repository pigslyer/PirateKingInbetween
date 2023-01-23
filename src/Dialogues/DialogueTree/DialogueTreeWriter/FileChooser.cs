using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue.Tree.Writer
{
	public class FileChooser : FileDialog
	{
		[Export] private string WorkingDirectorySaveLocation = "user://workingdirectory.txt";
		public string WorkingDirectory { get; private set;}

		public override void _Ready()
		{
			base._Ready();

			if (FileHelper.TryLoadFromLocation(WorkingDirectorySaveLocation, out string workingDirectory))
			{
				WorkingDirectory = workingDirectory;
			}
			else
			{
				RequestWorkingDirectory();
			}
		}

		public async void RequestWorkingDirectory()
		{
			GetCloseButton().Hide();

			Mode = ModeEnum.OpenDir;
			WindowTitle = "Choose your working directory";
			ClearFilters();
			Popup_();

			WorkingDirectory = (string)(await ToSignal(this, "dir_selected"))[0];
			FileHelper.SaveToLocation(WorkingDirectorySaveLocation, WorkingDirectory);
			
			GetCloseButton().Show();
		}

		public async Task<string> RequestFileLocation(bool save, string from = null, params string[] extenisons)
		{
			async Task<string> UntilFileSelected()
			{
				return (string)(await ToSignal(this, "file_selected"))[0];
			}

			async Task<string> UntilThisClosed()
			{
				await ToSignal(this, "hide");
				return null;
			}

			Mode = save ? ModeEnum.SaveFile : ModeEnum.OpenFile;
			WindowTitle = $"Select which file you want to {(save ? "save" : "load")}.";

			CurrentDir = from ?? WorkingDirectory;
			Filters = extenisons;

			Popup_();

			return (await Task.WhenAny(UntilFileSelected(), UntilThisClosed())).Result;
		}

		public override void _Input(InputEvent ev)
		{
			base._Input(ev);

			if (ev.IsActionPressed("ui_cancel") && GetCloseButton().Visible)
			{
				Hide();
			}
		}
	}
}