abstract public class State<T>
{
    abstract public void Enter(T _StateUserData);
    abstract public void Update(T _StateUserData, float _DeltaTime);
    abstract public void Exit(T _StateUserData);
}

public class StateMachine<T>
{
    private T stateUserData;
    private State<T> statePrev;
    private State<T> stateCurr;

    public StateMachine()
    {
        statePrev = null;
        stateCurr = null;
    }

    public void Initialise(State<T> _NewState, T _UserData)
    {
        statePrev = null;
        stateCurr = _NewState;
        stateUserData = _UserData;

        if (null != stateCurr)
            stateCurr.Enter(stateUserData);
    }

    public void Finalise()
    {
        if (null != stateCurr)
            stateCurr.Exit(stateUserData);

        statePrev = null;
        stateCurr = null;
    }

    public void ChangeState(State<T> _NewState)
    {
        statePrev = stateCurr;

        if (null != statePrev)
            statePrev.Exit(stateUserData);

        stateCurr = _NewState;

        if (null != stateCurr)
            stateCurr.Enter(stateUserData);
    }

    public void UpdateState(float _DeltaTime)
    {
        if (null != stateCurr)
            stateCurr.Update(stateUserData, _DeltaTime);
    }

    public State<T> CurrentState
    {
        get
        {
            return stateCurr;
        }
    }
}


// Embeded member function state machine
public delegate void EnterState(StateMachine _StateMachine);
public delegate void UpdateState(StateMachine _StateMachine);
public delegate void ExitState(StateMachine _StateMachine);

public class State
{
    public State(EnterState _Enter, UpdateState _Update, ExitState _Exit, string _DebugStateName = "Unnamed")
    {
        this.Enter = _Enter;
        this.Update = _Update;
        this.Exit = _Exit;

#if (!MASTER_BUILD)
        DebugStateName = _DebugStateName;
#endif //(!MASTER_BUILD)
    }

    public readonly EnterState Enter;
    public readonly UpdateState Update;
    public readonly ExitState Exit;

#if (!MASTER_BUILD)
    public readonly string DebugStateName;
#endif //(!MASTER_BUILD)
}

public class StateMachine
{
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

#if (!MASTER_BUILD)
    public string DebugStateMachineName;
#endif //!MASTER_BUILD

    public StateMachine()
    {
    }

    public StateMachine(string _DebugStateMachineName)
    {
#if (!MASTER_BUILD)
        DebugStateMachineName = _DebugStateMachineName;
#endif //!MASTER_BUILD
    }

    public StateMachine(State _InitialState)
    {
        ChangeState(_InitialState);
    }

    public void Update()
    {
        if (CurrentState != null && CurrentState.Update != null)
            CurrentState.Update(this);
    }

    protected bool CanTransitionToState(State _State, bool _CanTransitionToSelf = true)
    {
        return (CurrentState != _State) || (_CanTransitionToSelf && CurrentState == _State);
    }

    public void ChangeState(State _State, bool _CanTransitionToSelf = true)
    {
        if (CanTransitionToState(_State, _CanTransitionToSelf))
        {
            PreviousState = CurrentState;

            if (PreviousState != null && PreviousState.Exit != null)
                PreviousState.Exit(this);

            CurrentState = _State;

#if (DEBUG_STATEMACHINE_LOG)
					UnityEngine.Debug.Log(string.Format("{0} : StateMachine PrevState {1} : CurState {2}", DebugStateMachineName, PreviousState != null ? PreviousState.DebugStateName : "null", CurrentState != null ? CurrentState.DebugStateName : "null"));
#endif //(DEBUG_STATEMACHINE_LOG)

            if (CurrentState != null && CurrentState.Enter != null)
                CurrentState.Enter(this);
        }
    }
}
