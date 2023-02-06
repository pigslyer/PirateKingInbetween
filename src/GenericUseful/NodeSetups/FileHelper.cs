using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class FileHelper
{
	public static void SaveToLocation(string path, string contents)
	{
		File file = new File();
		file.Open(path, File.ModeFlags.Write);

		file.StoreString(contents);

		file.Close();
	}

	public static string LoadFromLocation(string path)
	{
		File file = new File();
		file.Open(path, File.ModeFlags.Read);

		string ret = file.GetAsText();

		file.Close();

		return ret;
	}

	public static bool TryLoadFromLocation(string path, out string contents)
	{
		contents = "";
		File file = new File();
		
		if (file.FileExists(path))
		{
			contents = LoadFromLocation(path);
			return true;
		}
		return false;
	}
}