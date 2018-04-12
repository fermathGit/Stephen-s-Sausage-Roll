/// <summary>
/// Basic of all state in entity.
/// </summary>
public interface IFSMState<T>
{
    /// <summary>
    /// Enter the specified stateEnt.
    /// </summary>
    /// <param name="stateEnt">State ent.</param>
    void Enter(T stateEnt);

    /// <summary>
    /// Execute the specified stateEnt.
    /// </summary>
    /// <param name="stateEnt">State ent.</param>
    void Execute(T stateEnt);

    /// <summary>
    /// Exit the specified stateEnt.
    /// </summary>
    /// <param name="stateEnt">State ent.</param>
    void Exit(T stateEnt);
}
