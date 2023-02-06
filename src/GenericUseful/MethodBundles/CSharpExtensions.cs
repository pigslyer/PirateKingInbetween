using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class CSharpExtensions
{
	/// <summary>
	/// Describes degrees to which a condition must be true.
	/// </summary>
	/// <returns>
	/// Based on <paramref name="usageRequirement"/>...
	/// <list type="bullet">
	/// 	<item>
	/// 		<term><see cref="UsageReq.Never"/></term>
	/// 		<description> True if <paramref name="condition"/> is false.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term><see cref="UsageReq.Required"/></term>
	/// 		<description> True if <paramref name="condition"/> is true.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term><see cref="UsageReq.Optional"/> </term>
	/// 		<description> True. </description>
	/// 	</item>
	/// </list>
	/// Otherwise it returns false.
	/// </returns>
	public static bool Evaluate(this UsageReq usageRequirement, bool condition) => usageRequirement == UsageReq.Optional || (condition != (usageRequirement == UsageReq.Never));
}