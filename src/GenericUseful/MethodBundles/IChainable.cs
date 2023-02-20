
/// <summary>
/// Represents a monadic(?) class which can form a single link in a chain.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IChainable<T>
{
	IChainable<T> Chain(T data);
}