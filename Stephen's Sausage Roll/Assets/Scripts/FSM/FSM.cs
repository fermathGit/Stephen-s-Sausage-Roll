using UnityEngine;
using System.Collections;

/// <summary>
/// FSM state machine
/// </summary>
public class FSM<T>
{
    private T _stateEntity;
    private IFSMState<T> _currentState;             // 当前状态
    private IFSMState<T> _previousState;            // 上一状态
    private IFSMState<T> _autoChangeState;        // 下一自动切换状态

    /// <summary>
    /// Initializes a new instance of the <see cref="FSM`1"/> class.
    /// </summary>
    /// <param name="inst">Inst.</param>
    public FSM(T inst)
    {
        _stateEntity = inst;
        _autoChangeState = null;
    }

    /// <summary>
    /// Sets the state of the current.For init FSM.
    /// </summary>
    /// <param name="state">State.</param>
    public void SetCurrentState(IFSMState<T> state)
    {
        _currentState = state;
    }

    public void SetPreviousState(IFSMState<T> state)
    {
        _previousState = state;
    }

    /// <summary>
    /// called by T.
    /// </summary>
    public void Update()
    {
        if (_autoChangeState != null)
        {
            ChangeState(_autoChangeState);
            _autoChangeState = null;
            return;
        }
        if (_currentState != null)
        {
            _currentState.Execute(_stateEntity);
        }
    }

    /// <summary>
    /// Autos the state of the change.
    /// </summary>
    /// <param name="state">State.</param>
    public void AutoChangeState(IFSMState<T> state)
    {
        _autoChangeState = state;
    }

    /// <summary>
    /// Changes the state.
    /// </summary>
    /// <param name="state">State.</param>
    public void ChangeState(IFSMState<T> state)
    {
        if (state == null)
        {
            return;
        }
        _autoChangeState = null;
        if (_currentState != state)
        {
            _previousState = _currentState;
        }

        if (_currentState != null)
        {
            _currentState.Exit(_stateEntity);
        }

        _currentState = state;
        _currentState.Enter(_stateEntity);
    }

    /// <summary>
    /// Revert the state of the to previous.
    /// </summary>
    public void RevertToPreviousState()
    {
        ChangeState(_previousState);
    }

    /// <summary>
    /// Determines whether this instance is in state the specified state.
    /// </summary>
    public bool IsInState(IFSMState<T> state)
    {
        return _currentState.GetType() == state.GetType();
    }

    public void Dispose()
    {
        _currentState = null;
        _previousState = null;
        _autoChangeState = null;
    }
}
