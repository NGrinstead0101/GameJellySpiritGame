using UnityEngine;

public abstract class EnemyState
{
    public abstract void Enter(EnemyStateMachine context);
    public abstract void UpdateDestination(EnemyStateMachine context);
    public abstract bool NeedsDestinationUpdate(EnemyStateMachine context);

    public abstract void CheckForStateChange(EnemyStateMachine context);

    public abstract void Exit(EnemyStateMachine context);
}

public class Idle : EnemyState
{
    public override void CheckForStateChange(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public override void Enter(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public override void Exit(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public override bool NeedsDestinationUpdate(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateDestination(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }
}
public class MoveTowardsPlayer : EnemyState
{
    public override void Enter(EnemyStateMachine context)
    {
        //do anything you want when you first enter the state
        CheckForStateChange(context);
    }
    public override void CheckForStateChange(EnemyStateMachine context)
    {
        //are you in range?
        // are you by a ledge?
    }

    public override bool NeedsDestinationUpdate(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateDestination(EnemyStateMachine context)
    {
        throw new System.NotImplementedException();
    }

    public override void Exit(EnemyStateMachine context)
    {
        //implement things you want to do before reliquishing control to the next state
    }
}