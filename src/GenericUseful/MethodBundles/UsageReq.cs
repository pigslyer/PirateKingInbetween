/// <summary>
/// Describes degrees to which a condition must be true. This is checked with <see cref="CSharpExtensions.Evaluate(UsageReq, bool)"/>
/// <list type="bullet">
/// 	<item>
/// 		<term><see cref="UsageReq.Never"/></term>
/// 		<description> Evaluation is true if condition is false.</description>
/// 	</item>
/// 	<item>
/// 		<term><see cref="UsageReq.Required"/></term>
/// 		<description> Evaluation is true if condition is true.</description>
/// 	</item>
/// 	<item>
/// 		<term><see cref="UsageReq.Optional"/> </term>
/// 		<description> Evaluation is always true. </description>
/// 	</item>
/// </list>
/// <para>Their integer value is arranged in such a way that <see cref="UsageReq.Optional"/> is greater than both <see cref="UsageReq.Required"/> and
/// <see cref="UsageReq.Never"/>, meaning the difference between 2 already forms a natural ordering.</para>
/// </summary>
public enum UsageReq
{
	Never = 0,
	Required = 1,
	Optional = 2,
}