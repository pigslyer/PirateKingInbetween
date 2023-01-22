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
		private const string DATA_STORAGE = "user://WriterWorkingLocation.txt";
		public string WorkingDirectory { get; private set;}

		public override void _Ready()
		{
			base._Ready();

			GetCloseButton().Hide();

			if (FileHelper.TryLoadFromLocation(DATA_STORAGE, out string workingDirectory))
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
			Mode = ModeEnum.OpenDir;
			WindowTitle = "Choose your working directory";
			ClearFilters();
			Popup_();
			
			var result = await ToSignal(this, "dir_selected");
			WorkingDirectory = (string) result[0];
			FileHelper.SaveToLocation(DATA_STORAGE, WorkingDirectory);
		}

		public async Task<string> RequestFileLocation(bool save, string from = null, params string[] extenisons)
		{
			Mode = save ? ModeEnum.SaveFile : ModeEnum.OpenFile;
			WindowTitle = $"Select which file you want to {(save ? "save" : "load")}.";

			CurrentDir = from ?? WorkingDirectory;
			Filters = extenisons;

			Popup_();

			return (string)(await ToSignal(this, "file_selected"))[0];
		}
	}
}