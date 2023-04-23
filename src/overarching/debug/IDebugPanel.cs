using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pigslyer.PirateKingInbetween.Overarching.Debug
{
	public interface IDebugPanel
	{
		DebugContainer.DebugConfiguration GetPanelData();
	}
}