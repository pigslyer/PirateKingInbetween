using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Util.Editor
{
	public partial class SpriteFramesGenerator : Node
	{
		const string OPTION_GROUP = "Options";

		[Export] private TabContainer _optionParent = null!;
		[Export] private OptionButton _options = null!;
		[Export] private AnimatedSprite2D _sprite = null!;


		private SpriteFrames? _activeFrames = null;
		private int _activeIndex;

		public void OnFrameChanged()
		{
			Texture2D cur = _sprite.SpriteFrames.GetFrameTexture(_sprite.Animation, _sprite.Frame);
			Vector2 parentSize = _sprite.GetParent<Control>().Size;

			_sprite.Scale = parentSize / cur.GetSize();
		}

		public void OnTabChanged(int idx)
		{
			if (_activeFrames == null)
			{
				return;
			}

			_activeIndex = idx;
			_options.Clear();

			if (idx == 0)
			{
				_activeFrames.GetAnimationNames().ForEach(str => {
					_options.AddItem(str);
				});
			}
			else
			{
				AnimationSettings cur = _settings[idx - 1];

				_activeFrames.GetAnimationNames().Where(name => name.StartsWith(cur.AnimationName)).ForEach(str => {
					_options.AddItem(str.TrimPrefix($"{cur.AnimationName}_"));
				});
			}

			OnAnimationSelected(0);
		}

		public void OnAnimationSelected(int item)
		{
			if (_activeFrames == null)
			{
				return;
			}

			if (_activeIndex == 0)
			{
				_sprite.Play(_activeFrames.GetAnimationNames()[item]);
			}
			else
			{
				AnimationSettings cur = _settings[_activeIndex - 1];

				_sprite.Play(_activeFrames.GetAnimationNames().Where(name => name.StartsWith(cur.AnimationName)).ToArray()[item]);
			}
		}

		private List<AnimationSettings> _settings = new();

		private void LoadOptions(string path, bool openSprite)
		{
			Clear();

			if (openSprite)
			{
				LoadSprite(_activeFrames = ResourceLoader.Load<SpriteFrames>(path));
			}
			else
			{
				LoadDir(_activeFrames = new(), path);
				_activeFrames.RemoveAnimation("default");
			}

			_sprite.SpriteFrames = _activeFrames;

			OnTabChanged(0);
		}

		private void LoadSprite(SpriteFrames frames)
		{
			IEnumerable<string> anims = frames.GetAnimationNames();

			List<string> animBases = new();

			while (anims.FirstOrDefault() != null)
			{
				string first = anims.First().Split("_")[0];
				animBases.Add(first);

				anims = anims.Where(name => !name.StartsWith(first));
			}

			
			foreach (var anim in animBases)
			{
				var settings = AnimationSettings.GenerateFromFrames(frames, anim);
				_settings.Add(settings);

				settings.Generate(_optionParent);
			}
		}

		private void LoadDir(SpriteFrames frames, string path)
		{
			var dir = DirAccess.Open(path);
			dir.IncludeHidden = dir.IncludeNavigational = false;

			foreach (var anim in dir.GetDirectories().Select(d => $"{path}/{d}"))
			{
				var settings = AnimationSettings.GenerateFromDir(frames, anim, path);
				_settings.Add(settings);

				settings.Generate(_optionParent);
			}
		}

		private void SupplementFrames(string path)
		{
			if (_activeFrames == null)
			{
				return;
			}

			var dir = DirAccess.Open(path);
			dir.IncludeHidden = dir.IncludeNavigational = false;

			foreach (var animPath in dir.GetDirectories().Select(d => $"{path}/{d}"))
			{
				string curName = GetAnimName(path, animPath);
				AnimationSettings? curSettings = _settings.Where(anim => anim.AnimationName == curName).FirstOrDefault();

				if (curSettings == null)
				{
					curSettings = AnimationSettings.GenerateFromDir(_activeFrames, animPath, path);
					_settings.Add(curSettings);

					curSettings.Generate(_optionParent);
				}
				else
				{
					curSettings.UpdateFrom(animPath, path);

					curSettings.Generate(_optionParent);
				}
			}
		}

		private void SaveOptions(string path)
		{
			if (_activeFrames == null)
			{
				return;
			}

			ResourceSaver.Save(_activeFrames, path);
		}

		private void Clear()
		{
			_sprite.SpriteFrames = _activeFrames = null;
			_activeIndex = 0;
			_settings.Clear();
			_options.Clear();
			GetTree().CallGroup(OPTION_GROUP, "queue_free");
		}

		private static string[] GetSubAnimPath(string rootPath)
		{
			var dir = DirAccess.Open(rootPath);
			dir.IncludeHidden = dir.IncludeNavigational = false;

			string[] ret = dir.GetFiles().Length == 0 ? Array.Empty<string>() : new string[] {rootPath};

			foreach (var file in dir.GetDirectories().Select(d => $"{rootPath}/{d}"))
			{
				ret = ret.Concat(GetSubAnimPath(file)).ToArray();
			}

			return ret;
		}

		private static string[] GetAnimFilePaths(string rootPath)
		{
			var dir = DirAccess.Open(rootPath);
			dir.IncludeHidden = dir.IncludeNavigational = false;

			return dir.GetFiles().Where(f => !f.EndsWith(".import")).ToArray();
		}

		private static string GetAnimName(string rootPath, string animPath)
		{
			return animPath.TrimPrefix($"{rootPath}/").Replace("/", "_");
		}

		private class AnimationSettings
		{
			private readonly SpriteFrames _frames;

			public readonly string AnimationName;
			private  bool _loop;
			private double _FPS;
			private float[] _perFrameSpeed;

			public static AnimationSettings GenerateFromDir(SpriteFrames from, string animRootDir, string rootDir)
			{
				GetSubAnimPath(animRootDir).ForEach(anim => {
					var name = GetAnimName(rootDir, anim);
					from.AddAnimation(name);

					AddAnimation(from, anim, name);
				});

				return new(
					from,
					GetAnimName(rootDir, animRootDir),
					true,
					5,
					Enumerable.Range(0, from.GetFrameCount(GetAnimName(rootDir, GetSubAnimPath(animRootDir)[0]))).Select(d => 1.0f).ToArray()
				);
			}

			public static AnimationSettings GenerateFromFrames(SpriteFrames from, string animName)
			{
				string firstName = from.GetAnimationNames().Where(name => name.StartsWith(animName)).First();

				return new(
					from,
					animName,
					from.GetAnimationLoop(firstName),
					from.GetAnimationSpeed(firstName),
					Enumerable.Range(0, from.GetFrameCount(firstName)).Select(idx => from.GetFrameDuration(firstName, idx)).ToArray()
				);
			}

			private AnimationSettings(SpriteFrames frames, string animName, bool loop, double fps, float[] perFrameSpeed)
			{ 
				_frames = frames; AnimationName = animName; _loop = loop; _FPS = fps; _perFrameSpeed = perFrameSpeed;
			}

			public void UpdateFrom(string animPath, string rootDir)
			{
				foreach (var subAnim in GetSubAnimPath(animPath))
				{
					var animName = GetAnimName(rootDir, subAnim);

					_frames.RemoveAnimation(animName);
					_frames.AddAnimation(animName);

					AddAnimation(_frames, subAnim, animName);

					int count = _frames.GetFrameCount(animName);
					if (count != _perFrameSpeed.Length)
					{
						float[] temp = new float[count];
						Array.Copy(_perFrameSpeed, temp, Math.Min(_perFrameSpeed.Length, count));
						_perFrameSpeed = temp;
					}
				}
			}

			private static void AddAnimation(SpriteFrames from, string animFolder, string animName)
			{
				var dir = DirAccess.Open(animFolder);
				dir.IncludeHidden = dir.IncludeNavigational = false;

				dir.GetFiles().Where(f => !f.EndsWith(".import")).Select(f => $"{animFolder}/{f}").ForEach(img =>
				{
					from.AddFrame(animName, ResourceLoader.Load<Texture2D>(img));
				});
			}

			private Node? _generatedRoot = null;

			public void Generate(Node root)
			{

				var animNames = _frames.GetAnimationNames().Where(name => name.StartsWith(AnimationName));

				var vbox = new VBoxContainer()
				{
					Name = AnimationName,
					AnchorLeft = 0.1f,
					AnchorRight = 0.9f,
					AnchorTop = 0.1f,
					AnchorBottom = 0.9f,
				};

				vbox.AddToGroup(OPTION_GROUP);

				if (_generatedRoot != null)
				{
					_generatedRoot.Name = Guid.NewGuid().ToString();
					_generatedRoot.AddSibling(vbox);
					_generatedRoot.QueueFree();
				}
				else
				{
					root.AddChild(vbox);
				}

				_generatedRoot = vbox;

				var loopbox = new CheckBox()
				{
					Text = "Loop",
				};

				loopbox.SetPressedNoSignal(_loop);

				loopbox.Toggled += state => 
				{
					_loop = state;
					animNames.ForEach(name => _frames.SetAnimationLoop(name, state));
				};

				vbox.AddChild(loopbox);

				var fpsedit = new LineEdit()
				{
					Text = _FPS.ToString()
				};

				fpsedit.TextSubmitted += text => {
					if (double.TryParse(text, out var val))
					{
						_FPS = val;
						animNames.ForEach(name => _frames.SetAnimationSpeed(name, val));
					}
					else
					{
						fpsedit.Clear();
					}
				};

				vbox.AddChild(fpsedit);

				var perframelabel = new Label()
				{
					Text = "Frame speeds:"
				};

				vbox.AddChild(perframelabel);

				for (int i = 0; i < _perFrameSpeed.Length; i++)
				{
					var hbox = new HBoxContainer();

					var count = new Label()
					{
						Text = $"{i}:	"
					};

					hbox.AddChild(count);

					var frameset = new LineEdit()
					{
						Text = _perFrameSpeed[i].ToString(),
						SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
					};

					var idx = i;
					frameset.TextSubmitted += text => {
						if (float.TryParse(text, out var val))
						{
							_perFrameSpeed[idx] = val;
							animNames.ForEach(name => _frames.SetFrame(name, idx, _frames.GetFrameTexture(name, idx), val));
						}
						else
						{
							frameset.Clear();
						}
					};

					hbox.AddChild(frameset);

					vbox.AddChild(hbox);
				}
			}
		}
	}
}