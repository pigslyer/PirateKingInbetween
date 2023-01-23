using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PirateInBetween.Game.Dialogue.Tree
{
	public class DialogueTreeCharacter : DialogueCharacter
	{
		public string FileName { get; private set; }
		public string OldFileName { get; private set; }
		public string PortraitFile { get; private set; } = "";

		public DialogueTreeCharacter()
		{ }

		public DialogueTreeCharacter(string filename, string name)
		{
			OldFileName = filename;
			SetFileName(filename);
			SetName(name);
		}

		public DialogueTreeCharacter(string filename, string name, string workingDirectory, string pathToTextureRelativeToWorkingDirectory) : this(filename, name)
		{
			SetPortrait(workingDirectory, pathToTextureRelativeToWorkingDirectory);
		}


		public void SetFileName(string name)
		{
			FileName = name;
		}

		public void UpdateOldFilename()
		{
			OldFileName = FileName;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public void SetPortrait(string workingDirectory, string pathRelativeToWorkingDirectory)
		{
			if (pathRelativeToWorkingDirectory != "")
			{
				Image image = new Image();
				ImageTexture texture = new ImageTexture();

				image.Load(workingDirectory + pathRelativeToWorkingDirectory);
				texture.CreateFromImage(image);

				CharacterPortrait = texture;
			}
			PortraitFile = pathRelativeToWorkingDirectory;
		}
	}
}