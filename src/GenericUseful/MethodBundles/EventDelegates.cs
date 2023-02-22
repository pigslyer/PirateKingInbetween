using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class EventDelegates
{
	public delegate void OnSomethingHappened();
	public delegate void OnSomethingHappened<T1>(T1 what1);
	public delegate void OnSomethingHappened<T1, T2>(T1 what1, T2 what2);
	public delegate void OnSomethingHappened<T1, T2, T3>(T1 what1, T2 what2, T3 what3);
}