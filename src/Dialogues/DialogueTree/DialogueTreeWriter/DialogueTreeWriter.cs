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

		#endregion

		private FileChooser _fileChooser;
		private TextEdit _dialogueEditor;
		private Label _currentFilePathLabel;
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
		}

		public void Play()
		{
			GetNode<DialoguePlayer>(__dialoguePlayerPath).Play(DialogueTree.StringToTree(CurrentText.Split("\n")));
		}

		public async void CreateNewFile()
		{
			_currentFile = await _fileChooser.RequestFileLocation(save : true, WorkingDirectory, "*.dial");
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
			}
		}

		public async void SaveAs()
		{
			_currentFile = await _fileChooser.RequestFileLocation(save: true, _currentFile ?? WorkingDirectory, "*.dial");
			Save();
		}

		public async void Load()
		{
			_currentFile = await _fileChooser.RequestFileLocation(save: false, WorkingDirectory, "*.dial");
			CurrentText = FileHelper.LoadFromLocation(AppendDial(_currentFile));
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
	}
}