using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// An <see cref="AnimatedSprite"/> which generates its own <see cref="SpriteFrames"/>.
/// It does so by recursively going through the subdirectories of the directory provided to it, looking for .pngs and config.anim files.
/// <para>
/// If a .anim is provided to it, it'll load all frames in lexicographic order.
/// </para> <para>
/// The .anim should contain 3 fields under its [anim] section: name=[animationName], speed=[FPS], loop=[bool]
/// </para> </summary>
[Tool]
public class SpriteFramesGenerator : AnimatedSprite
{

	const string IMAGE_EXTENSIONS = "png";

	[Export(PropertyHint.Dir)] private string _spritePath = "";

	[Export] private bool _update
	{
		set => OnUpdatePushed();
		get => true;
	}

	private void OnUpdatePushed()
	{
		if (TryGetFolderData(_spritePath, out FolderData folder))
		{
			GD.Print($"Folder: {folder}");
			UpdateSpriteFrames(folder);
		}
	}

	private bool TryGetFolderData(string newPath, out FolderData data)
	{
		Directory dir = new Directory();
		
		if (!dir.DirExists(newPath))
		{
			GD.PushError($"Folder {newPath} does not exist.");
			data = null;
			return false;
		}

		data = new FolderData(newPath);
		return true;
	}

	private void UpdateSpriteFrames(FolderData folders)
	{
		if (Frames == null)
		{
			Frames = new SpriteFrames();
		}

		Dictionary<string, string> _pathToAnimName = new Dictionary<string, string>();

		try
		{

			foreach (string anim in Frames.GetAnimationNames())
			{

				if (Frames.GetFrameCount(anim) > 0)
				{
					_pathToAnimName.Add(Frames.GetFrame(anim, 0).ResourcePath.GetBaseDir(), anim);
				}
				else
				{
					_pathToAnimName.Add(anim, anim);
				}
			}
		} 
		catch (ArgumentException)
		{
			GD.Print("Exception");
			GD.Print(string.Join(", ", _pathToAnimName.Keys.Select(k => $"{k} : {_pathToAnimName[k]}")));
		}

		string animName;

		return;

		foreach (FolderData folder in folders)
		{
			GD.Print(folder.Path);

			if (!_pathToAnimName.TryGetValue(folder.Path, out animName))
			{
				animName = folder.Path;			
				Frames.AddAnimation(animName);
			}
			
			if (folder.Textures.Count == 0)
			{
				Frames.RemoveAnimation(animName);
				continue;
			}

			while (Frames.GetFrameCount(animName) > 0)
			{
				Frames.RemoveFrame(animName, 0);
			}

			foreach (Texture tex in folder.Textures)
			{
				Frames.AddFrame(animName, tex);
			}
		}

		if (Frames.GetAnimationNames().Contains("default") && Frames.GetAnimationNames().Length > 1)
		{
			Frames.RemoveAnimation("default");
			Animation = Frames.GetAnimationNames()[0];
		}
	}
	

	private class FolderData : IEnumerable<FolderData>
	{
		public readonly string Path;
		public IReadOnlyCollection<Texture> Textures => _textures;
		private SortedSet<Texture> _textures = new SortedSet<Texture>(new Comparer());
		private List<FolderData> _subfolders = new List<FolderData>();

		public FolderData(string path)
		{
			Path = path;

			IEnumerator<string> iter = path.ListDir(true);
			Directory dir = new Directory();
			dir.ChangeDir(path);

			while (iter.MoveNext())
			{
				if (iter.Current.Extension() == IMAGE_EXTENSIONS)
				{
					_textures.Add(ResourceLoader.Load<Texture>($"{path}/{iter.Current}"));
				}
				else if (dir.DirExists(iter.Current))
				{
					_subfolders.Add(new FolderData($"{path}/{iter.Current}"));
				}
			}
		}

		public override string ToString()
		{
			return $"Path: {Path}, Textures: {Textures.Count}, Subfolders: {string.Join(", ", _subfolders)}";
		}

		private class Comparer : IComparer<Texture>
		{
			public int Compare(Texture x, Texture y) => GetNumberFromEnd(x.ResourcePath.BaseName()).CompareTo(GetNumberFromEnd(y.ResourcePath.BaseName()));

			private static int GetNumberFromEnd(string from)
			{
				int pos;
				for (pos = from.Length - 1; from[pos] >= '0' && from[pos] <= '9'; pos--)
				{ }

				return int.Parse(from.Substring(pos + 1));
			}
		}

		private List<FolderData> ToList()
		{
			List<FolderData> ret = new List<FolderData>();
			ToList(ret, this);
			return ret;
		}

		private static void ToList(List<FolderData> list, FolderData what)
		{
			list.Add(what);
			what._subfolders.ForEach(f => ToList(list, f));
		}

		public IEnumerator<FolderData> GetEnumerator() => ToList().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ToList().GetEnumerator();

	}
}