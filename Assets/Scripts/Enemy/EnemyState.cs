using System;
using System.Threading;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public abstract class EnemyState
{
    public static Action DealtDamage;

    public abstract void Enter(EnemyStateMachine context);
    public abstract void UpdateDestination(EnemyStateMachine context);
    public abstract bool NeedsDestinationUpdate(EnemyStateMachine context);
    public abstract void CheckForStateChange(EnemyStateMachine context);
    public abstract void Exit(EnemyStateMachine context);
}

public class Patrol : EnemyState
{
    public override void Enter(EnemyStateMachine context)
    {
        context.StopTimer(); //reset the timer
        context.StartTimer();
        UpdateDestination(context);

    }

    public override void CheckForStateChange(EnemyStateMachine context)
    {
        if (context.IsInRange())
        {
            context.ChangeState(new MoveTowardsPlayer());
        }
        else if ((int)context.GetTime() % 10 == 0) //every 5 seconds the patrolling enemy will randomly decide if it should go to idle
        {
            var rand = UnityEngine.Random.Range(0, 15);
            if (context.GetTime() > rand)
            {
                context.ChangeState(new Idle());
            }
            else if(context.GetTime() > rand/2)
            {
                UpdateDestination(context);
            }
        }
        // or just wait..
    }

    public override bool NeedsDestinationUpdate(EnemyStateMachine context)
    {
        CheckForStateChange(context);

        if(context.IsNextToEdge() || context.IsNextToWall()) //if youre next to a ledge, turn around and go the other way
        {
            context.ChangeFacingDirection();
            UpdateDestination(context);
            return true;
        }
        return false;
    }

    public override void UpdateDestination(EnemyStateMachine context)
    {
        var random = UnityEngine.Random.Range(2, 10);
        if (context.GetIsFacingLeft())
        {
            context.ChangeFacingDirection();
            context.SetDestination(new Vector2(context.transform.position.x + random, 0));
        }
        else
        {
            context.ChangeFacingDirection();
            context.SetDestination(new Vector2(context.transform.position.x - random, 0));
        }
    }

    public override void Exit(EnemyStateMachine context)
    {

    }
}
public class Idle : EnemyState
{
    public override void Enter(EnemyStateMachine context)
    {
        context.SetDestination(context.transform.position);
        context.ChangeSpeed(0);
        context.StopTimer(); //reset the timer
        context.StartTimer();
    }
    public override void CheckForStateChange(EnemyStateMachine context)
    {
        if(context.IsInRange())
        {
            context.ChangeState(new MoveTowardsPlayer());
        }
        else if ((int)context.GetTime() % 3 ==0) //every 2 seconds the idle enemy will randomly decide if it should go to patrol
        {
            if(context.GetTime() > UnityEngine.Random.Range(0,15))
            {
                context.ChangeState(new Patrol());
            }
        }
        // or just wait..
    }

    public override bool NeedsDestinationUpdate(EnemyStateMachine context)
    {
        //if the enemy isnt moving, it should never need a destination update
        CheckForStateChange (context);
        return false;
    }

    public override void UpdateDestination(EnemyStateMachine context)
    {
        context.SetDestination(context.transform.position);
    }

    public override void Exit(EnemyStateMachine context)
    {
        context.ChangeSpeed(context.GetNormalSpeed());
    }
}
public class MoveTowardsPlayer : EnemyState
{
    public override void Enter(EnemyStateMachine context)
    {
        //do anything you want when you first enter the state
        context.SpotPlayer();
        CheckForStateChange(context);
    }
    public override void CheckForStateChange(EnemyStateMachine context)
    {
        //are you in sight range?
        if(!context.IsInRange())
        {
            context.ChangeState(new Idle());
        }
        else if(context.GetNextToPlayer())
        {
            context.ChangeState(new Attack());
        }
    }

    public override bool NeedsDestinationUpdate(EnemyStateMachine context)
    {
        if (!context.IsFacingPlayer())
        {
            context.ChangeFacingDirection();
        }
        if (!context.IsNextToEdge() && !context.IsNextToWall())
        {
            var distance = Mathf.Abs(context.GetDestination().x - context.GetPlayerLocation().x);
            //If the enemy is within a not so absurd distance so the states dont flicker as often
            if (distance > 0.45f && distance < context.GetChaseDistance())
            {
                return true;
            }
        }
        
        CheckForStateChange(context);
        return false;
    }

    public override void UpdateDestination(EnemyStateMachine context)
    {
        context.SetDestination(context.GetPlayerLocation());
    }

    public override void Exit(EnemyStateMachine context)
    {
        //implement things you want to do before reliquishing control to the next state
        //context.ChangeState(new Idle());
    }
}

public class Attack : EnemyState
{
    public override void Enter(EnemyStateMachine context)
    {
        context.ToggleIsAttacking(true);
        CheckForStateChange(context);
        context.SetDestination(context.transform.position);
        context.ChangeSpeed(0);
        context.StopTimer(); //reset the timer
        context.StartTimer();
    }

    public override void CheckForStateChange(EnemyStateMachine context)
    {
        if(!context.GetNextToPlayer())
        {
            context.ChangeState(new MoveTowardsPlayer());
        }
        else
        {
            if((int)context.GetTime() % 3 ==0)
            {
                // TODO: do attack here
                DealtDamage?.Invoke();
                Debug.Log("ATTACK");
            }
        }
    }

    public override bool NeedsDestinationUpdate(EnemyStateMachine context)
    {
        CheckForStateChange(context);

        if (!context.IsFacingPlayer())
        {
            context.ChangeFacingDirection();
        }
        return false;
    }

    public override void UpdateDestination(EnemyStateMachine context)
    {
        //never needed for attack
    }
    public override void Exit(EnemyStateMachine context)
    {
        context.ToggleIsAttacking(false);
    }
}
public class Died : EnemyState
{
    public override void CheckForStateChange(EnemyStateMachine context)
    {
    }

    public override void Enter(EnemyStateMachine context)
    {
        context.gameObject.SetActive(false);
    }

    public override void Exit(EnemyStateMachine context)
    {
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