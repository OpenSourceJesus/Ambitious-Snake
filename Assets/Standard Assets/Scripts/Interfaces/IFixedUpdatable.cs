public interface IFixedUpdatable
{
	bool PauseWhileUnfocused { get; }
	void DoFixedUpdate ();
}