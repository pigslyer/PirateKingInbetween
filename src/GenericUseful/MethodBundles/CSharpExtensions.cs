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

	public static int TrailingZeroCount(this int value)
	{
		int ret = 0;

		for (int i = 1; i < 32 && (value & i) == 0; i <<= 1)
		{
			ret++;
		}
		
		return ret;
	}

	public static float MoveTowards(this float what, float to, float delta) => Mathf.MoveToward(what, to, delta);

	public static float Sqr(this float what) => what * what;

	/// <summary>
	/// Returns 1 if <paramref name="of"/> is true, otherwise -1.
	/// </summary>
	/// <param name="of"></param>
	/// <returns></returns>
	public static int Sign(this bool of) => of ? 1 : -1;
}