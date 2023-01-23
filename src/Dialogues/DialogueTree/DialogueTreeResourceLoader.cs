using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace PirateInBetween.Game.Dialogue.Tree
{
	public class DialogueTreeResourceLoader : IDialogueResourceLoader
	{
		public const string CHARACTER_DIRECTORY = "/characters/";
		public const string CHARACTER_FILE_EXTENSION = ".char";

		private string _workingDirectory;
		private string _workingDirectoryCharacter => _workingDirectory + CHARACTER_DIRECTORY;
		private List<string> _names = new List<string>();

		public DialogueTreeResourceLoader(string workingDirectory)
		{
			_workingDirectory = workingDirectory;

			RefreshNames();
		}

		private void RefreshNames()
		{
			_names.Clear();

			Directory dir = new Directory();
			dir.MakeDirRecursive(_workingDirectoryCharacter);
			dir.Open(_workingDirectoryCharacter);

			dir.ListDirBegin(skipNavigational : true);

			for (string name = dir.GetNext(); name != ""; name = dir.GetNext())
			{
				_names.Add(name.BaseName());
			}
		}

		public string[] GetNames() => _names.ToArray();

		public DialogueTreeCharacter LoadCharacter(int index) => LoadCharacterFromFile(_names[index]);
		public DialogueCharacter LoadCharacter(string name) => LoadCharacterFromFile(name);

		private const string SECTION = "";

		private DialogueTreeCharacter LoadCharacterFromFile(string filename)
		{
			ConfigFile file = new ConfigFile();
			file.Load(GetPath(filename));
			
			DialogueTreeCharacter ret = new DialogueTreeCharacter(
					filename : filename, 
					name : (string)file.GetValue(SECTION, "displayName", ""), 
					workingDirectory : _workingDirectory, 
					pathToTextureRelativeToWorkingDirectory : (string)file.GetValue(SECTION, "portraitPath", "")
			);
			
			return ret;
		}

		public bool TrySave(DialogueTreeCharacter character)
		{
			if (character.FileName != character.OldFileName && _names.Contains(character.FileName))
			{
				return false;
			}

			if (character.FileName != character.OldFileName && character.OldFileName != null)
			{
				Delete(character);
			}

			if (character.FileName != character.OldFileName)
			{
				_names.Add(character.FileName);
				character.UpdateOldFilename();
			}

			ConfigFile file = new ConfigFile();
			file.SetValue(SECTION, "displayName", character.Name);
			file.SetValue(SECTION, "portraitPath", character.PortraitFile);
			
			file.Save(GetPath(character.FileName));

			return true;
		}

		public void Delete(DialogueTreeCharacter character)
		{
			OS.MoveToTrash(GetPath(character.OldFileName));
			_names.Remove(character.OldFileName);
		}

		public bool IsFileNameValid(DialogueTreeCharacter character, string name)
		{
			return name != null && name != "" && (name == character.OldFileName || !_names.Contains(name));
		}

		private string GetPath(string filename) => _workingDirectoryCharacter + filename + CHARACTER_FILE_EXTENSION;

	}
}