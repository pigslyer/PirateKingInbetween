using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue.Tree.Writer
{
	public class CharacterManager : Control
	{

		#region Paths

		[Export] private NodePath __fileChooserPath;
		[Export] private NodePath __managerPath;
		[Export] private NodePath __characterListPath;
		[Export] private NodePath __characterDisplayNamePath;
		[Export] private NodePath __characterFileNamePath;
		[Export] private NodePath __characterTextureLineEditPath;
		[Export] private NodePath __characterTextureDisplayPath;
		[Export] private NodePath __currentlyEditingFileNameLabelPath;
		[Export] private NodePath __editedStarPath;
		[Export] private NodePath __saveButtonPath;
		[Export] private NodePath __discardButtonPath;
		[Export] private NodePath __newButtonPath;
		[Export] private NodePath __deleteButtonPath;
		[Export] private NodePath __selectTexturePath;

		#endregion

		private FileChooser _fileChooser;
		public String WorkingDirectory => _fileChooser.WorkingDirectory;

		private DialogueTreeResourceLoader _loader = null;
		private DialogueTreeCharacter _currentCharacter = null;
		private CanvasItem _editedStar;
		private Manager _manager;

		public override void _Ready()
		{
			base._Ready();

			_fileChooser = GetNode<FileChooser>(__fileChooserPath);
			_editedStar = GetNode<CanvasItem>(__editedStarPath);
			_manager = GetNode<Manager>(__managerPath);
		}

		public void SwitchedTo(int which)
		{
			if (which == 1)
			{
				_currentCharacter = null;
				GetNode<Label>(__currentlyEditingFileNameLabelPath).Text = "<none>";
				UpdateCurrentlySelected();
				RefreshList();
				UpdateDisabledness();
			}
		}

		private void RefreshList()
		{
			if (_loader == null)
			{
				_loader = new DialogueTreeResourceLoader(WorkingDirectory);
			}

			string[] names = _loader.GetNames();
			ItemList list = GetNode<ItemList>(__characterListPath);

			list.Clear();

			foreach (string name in names)
			{
				list.AddItem(name);
			}
		}



		public void OnSelectedItem(int index)
		{
			_currentCharacter = _loader.LoadCharacter(index);
			GetNode<Label>(__currentlyEditingFileNameLabelPath).Text = _currentCharacter.FileName;
			_unsavedTexture = _currentCharacter.PortraitFile;
			UpdateCurrentlySelected();
			UpdateDisabledness();
		}

		public void UpdateCurrentlySelected()
		{
			GetNode<LineEdit>(__characterDisplayNamePath).Text = _currentCharacter?.Name ?? "";
			GetNode<LineEdit>(__characterFileNamePath).Text = _currentCharacter?.FileName ?? "";
			UpdateTexture();
		}

		private void UpdateTexture()
		{
			GetNode<LineEdit>(__characterTextureLineEditPath).Text = _currentCharacter?.PortraitFile ?? "";
			GetNode<TextureRect>(__characterTextureDisplayPath).Texture = _currentCharacter?.CharacterPortrait;
		}

		public void OnSave()
		{
			_currentCharacter.SetName(GetNode<LineEdit>(__characterDisplayNamePath).Text);
			_currentCharacter.SetFileName(GetNode<LineEdit>(__characterFileNamePath).Text);
			_currentCharacter.SetPortrait(WorkingDirectory, _unsavedTexture = GetNode<LineEdit>(__characterTextureLineEditPath).Text);
			_loader.TrySave(_currentCharacter);
			

			GetNode<Label>(__currentlyEditingFileNameLabelPath).Text = _currentCharacter.FileName;
			RefreshList();
			UpdateDisabledness();
		}

		public void OnDiscard()
		{
			if (_currentCharacter.OldFileName == null || _currentCharacter.OldFileName == "")
			{
				_currentCharacter = null;
				GetNode<Label>(__currentlyEditingFileNameLabelPath).Text = "<none>";
			}

			UpdateCurrentlySelected();
			UpdateDisabledness();
		}

		public void OnNew()
		{
			_currentCharacter = new DialogueTreeCharacter();
			GetNode<Label>(__currentlyEditingFileNameLabelPath).Text = "<unsaved>";
			_unsavedTexture = "";
			UpdateCurrentlySelected();
			UpdateDisabledness();
		}

		public void OnDelete()
		{
			_loader.Delete(_currentCharacter);
			GetNode<Label>(__currentlyEditingFileNameLabelPath).Text = "<none>";
			_currentCharacter = null;
			RefreshList();
			UpdateDisabledness();
		}

		public async void OnSelectTexturePath()
		{
			string path = await _fileChooser.RequestFileLocation(false, WorkingDirectory, "*.png");

			if (path != null)
			{
				_currentCharacter.SetPortrait(WorkingDirectory, path.Substring(WorkingDirectory.Length));
				UpdateTexture();
			}
		}

		private bool _hasBeenEdited = false;
		private string _unsavedTexture;

		public override void _Process(float _)
		{
			base._Process(_);


			bool temp = _currentCharacter == null ||
				!(GetNode<LineEdit>(__characterDisplayNamePath).Text == _currentCharacter.Name &&
				GetNode<LineEdit>(__characterFileNamePath).Text == _currentCharacter.FileName &&
				_unsavedTexture == _currentCharacter.PortraitFile);
			
			

			if (_currentCharacter != null)
			{
				UpdateDisabledness();
			}

			_hasBeenEdited = temp;

			_editedStar.Visible = _currentCharacter != null && _hasBeenEdited;
		}

		private void UpdateDisabledness()
		{

			if (_currentCharacter == null)
			{
				GetNode<Button>(__selectTexturePath).Disabled = true;
				GetNode<Button>(__saveButtonPath).Disabled = true;
				GetNode<Button>(__discardButtonPath).Disabled = true;
				GetNode<Button>(__newButtonPath).Disabled = false;
				GetNode<Button>(__deleteButtonPath).Disabled = true;
				SetAllDisabled(GetNode<ItemList>(__characterListPath), false);
				_manager.SetQuitAllowed(Manager.QuitRequester.CharacterManager, true);
			}
			else if (_hasBeenEdited)
			{
				if (_loader.IsFileNameValid(_currentCharacter, GetNode<LineEdit>(__characterFileNamePath).Text))
				{
					GetNode<Button>(__selectTexturePath).Disabled = false;
					GetNode<Button>(__saveButtonPath).Disabled = false;
					GetNode<Button>(__discardButtonPath).Disabled = false;
					GetNode<Button>(__newButtonPath).Disabled = false;
					GetNode<Button>(__deleteButtonPath).Disabled = false;
					SetAllDisabled(GetNode<ItemList>(__characterListPath), true);
					_manager.SetQuitAllowed(Manager.QuitRequester.CharacterManager, false);
				}
				else
				{
					GetNode<Button>(__selectTexturePath).Disabled = true;
					GetNode<Button>(__saveButtonPath).Disabled = true;
					GetNode<Button>(__discardButtonPath).Disabled = false;
					GetNode<Button>(__newButtonPath).Disabled = false;
					GetNode<Button>(__deleteButtonPath).Disabled = true;
					SetAllDisabled(GetNode<ItemList>(__characterListPath), true);
					_manager.SetQuitAllowed(Manager.QuitRequester.CharacterManager, true);
				}
			}
			else
			{
				GetNode<Button>(__selectTexturePath).Disabled = false;
				GetNode<Button>(__saveButtonPath).Disabled = true;
				GetNode<Button>(__discardButtonPath).Disabled = true;
				GetNode<Button>(__newButtonPath).Disabled = false;
				GetNode<Button>(__deleteButtonPath).Disabled = false;
				SetAllDisabled(GetNode<ItemList>(__characterListPath), false);
				_manager.SetQuitAllowed(Manager.QuitRequester.CharacterManager, true);
			}
		}

		private static void SetAllDisabled(ItemList list, bool state)
		{
			list.MouseFilter = state ? MouseFilterEnum.Ignore : MouseFilterEnum.Stop;

			for (int i = 0; i < list.GetItemCount(); i++)
			{
				list.SetItemDisabled(i, state);
			}
		}
	}
}