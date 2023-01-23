using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue.Tree.Writer
{
	public class DialogueTreeWriter : MarginContainer
	{
		#region Paths
		[Export] private NodePath __dialoguePlayerPath = null;
		[Export] private NodePath __dialogueEditorPath = null;
		[Export] private NodePath __currentFileLabelPath = null;
		[Export] private NodePath __fileChooserPath = null;
		[Export] private NodePath __unsavedChangesStarPath = null;
		[Export] private NodePath __managerPath = null;

		#endregion

		private FileChooser _fileChooser;
		private TextEdit _dialogueEditor;
		private Label _currentFilePathLabel;
		private CanvasItem _unsavedChangesStar;
		private Manager _manager;
		private string WorkingDirectory => _fileChooser.WorkingDirectory;
		
		private string __currentFile = null;

		private string _currentFile
		{
			get => __currentFile;
			set => __currentFile = _currentFilePathLabel.Text = value;
		}

		public override void _Ready()
		{
			base._Ready();

			_fileChooser = GetNode<FileChooser>(__fileChooserPath);
			_dialogueEditor = GetNode<TextEdit>(__dialogueEditorPath);
			_currentFilePathLabel = GetNode<Label>(__currentFileLabelPath);
			_unsavedChangesStar = GetNode<CanvasItem>(__unsavedChangesStarPath);
			_manager = GetNode<Manager>(__managerPath);
		}

		public void Play()
		{
			GetNode<DialoguePlayer>(__dialoguePlayerPath).Play(DialogueTree.StringToTree(CurrentText.Split("\n"), WorkingDirectory));
		}

		public async void CreateNewFile()
		{
			var result = await _fileChooser.RequestFileLocation(save : true, WorkingDirectory, "*.dial");

			if (result != null)
			{
				_currentFile = result;
				CurrentText = "";
			}
		}

		public void Save()
		{
			if (_currentFile == null)
			{
				SaveAs();
			}
			else
			{
				FileHelper.SaveToLocation(AppendDial(_currentFile), CurrentText);
				ShowStar(false);
			}
		}

		public async void SaveAs()
		{
			var result = await _fileChooser.RequestFileLocation(save: true, _currentFile ?? WorkingDirectory, "*.dial");
			if (result != null)
			{
				_currentFile = result;
				Save();
			}
		}

		public async void Load()
		{
			var result = await _fileChooser.RequestFileLocation(save: false, WorkingDirectory, "*.dial");
			if (result != null)
			{
				_currentFile = result;
				CurrentText = FileHelper.LoadFromLocation(AppendDial(_currentFile));
				ShowStar(false);
			}
		}

		public void OnTextChanged()
		{
			if (_currentFile != null)
			{
				ShowStar(true);
			}
		}

		private string CurrentText
		{
			get => _dialogueEditor.Text;
			set => _dialogueEditor.Text = value;
		}

		private string AppendDial(string str)
		{
			if (str.Extension() != "dial")
			{
				return $"{str}.dial";
			}
			return str;
		}

		private void ShowStar(bool state)
		{
			_unsavedChangesStar.Visible = state;
			_manager.SetQuitAllowed(Manager.QuitRequester.Writer, !state);
		}
	}
}