using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

// should i add a chase/lose Player and moveToGenerator strat
namespace Pathfinding.BehaviourTrees  // file for executing strategies in game
{
    public interface IStrategy
    {
        Nodes.Status Process();  // returns a node status eg if strategy succeeds node will return 'Success'
        void Reset()   // incase the strategy has a state, reset this (strat linked to leaf and leaf means reset BHT after leaf behaviour completed ?)
        {
            // can override if neccassary, noop = no operation
        } 
    }

    public class HasStoppedStrategy : IStrategy
    {
        readonly NavMeshAgent agent;
        readonly float stopDuration;

        float stopStartTime;
        bool timerStarted;

        public HasStoppedStrategy(NavMeshAgent agent, float stopDuration = 2f)
        {
            this.agent = agent;
            this.stopDuration = stopDuration;
        }

        public Nodes.Status Process()
        {
            // Haven't reached the destination yet.
            if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                timerStarted = false;
                return Nodes.Status.Running;
            }

            // Start the timer once we've arrived.
            if (!timerStarted)
            {
                stopStartTime = Time.time;
                timerStarted = true;
            }

            // Wait until enough time has passed.
            if (Time.time - stopStartTime >= stopDuration)
            {
                return Nodes.Status.Success;
            }

            return Nodes.Status.Running;
        }

        public void Reset()
        {
            timerStarted = false;
        }
    }

    public class ReturnToClosestPatrolPointStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;

        Transform targetPoint;

        public ReturnToClosestPatrolPointStrategy(Transform entity,NavMeshAgent agent,List<Transform> patrolPoints)
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
        }

        public Nodes.Status Process()
        {
            if (patrolPoints.Count == 0)
                return Nodes.Status.Failure;

            if (targetPoint == null)
            {
                float closestDistance = Mathf.Infinity;

                foreach (Transform point in patrolPoints)
                {
                    float distance = Vector3.Distance(
                        entity.position,
                        point.position);

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        targetPoint = point;
                    }
                }

                agent.SetDestination(targetPoint.position);
            }

            if (!agent.pathPending &&
                agent.remainingDistance <= 0.2f)
            {
                return Nodes.Status.Success;
            }

            return Nodes.Status.Running;
        }

        public void Reset()
        {
            targetPoint = null;
            agent.ResetPath();
        }
    }

    public class ChasePlayerStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform player;
        readonly float stoppingDistance;

        public ChasePlayerStrategy(Transform entity, NavMeshAgent agent,Transform player, float stoppingDistance = 1.5f)
        {
            this.entity = entity;
            this.agent = agent;
            this.player = player;
            this.stoppingDistance = stoppingDistance;
        }

        public Nodes.Status Process()
        {
            agent.SetDestination(player.position);

            if (!agent.pathPending &&
                agent.remainingDistance <= stoppingDistance)
            {
                return Nodes.Status.Success;
            }

            return Nodes.Status.Running;
        }

        public void Reset()
        {
            agent.ResetPath();
        }
    }

    public class ActionStrategy : IStrategy  //simple fire and forget 
    {
        readonly Action doSomething;

        public ActionStrategy(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Nodes.Status Process()
        {
            doSomething();
            return Nodes.Status.Success;
        }
    }

    public class Condition : IStrategy  //Logical IF
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)  //evaluate if Func is true or false - whether met condition or not
        {
            this.predicate = predicate;
        }

        public Nodes.Status Process() => predicate() ? Nodes.Status.Success : Nodes.Status.Failure;
    }

    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;  
        readonly NavMeshAgent agent;
        readonly List<Transform> patrolPoints;  //waypoints manually set ?
        readonly float patrolSpeed;
        int currentIndex;  //note of patrol point index?
        bool isPathCalculated;

        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, float patrolSpeed = 5f)  //constructor
        {
            this.entity = entity;
            this.agent = agent;
            this.patrolPoints = patrolPoints;
            this.patrolSpeed = patrolSpeed;
        }

        public Nodes.Status Process()
        {
            if (patrolPoints.Count == 0) return Nodes.Status.Failure;

           // entity.LookAt(target.position.With(y:entity.position.y));  //looking at destination

            if(!isPathCalculated)
            {
                agent.SetDestination(patrolPoints[currentIndex].position);
                isPathCalculated = true;
            }

           // if(isPathCalculated && agent.remainingDistance < 0.2f)  // distance check to next point, if close enough go to next patrol point index
            if (!agent.pathPending && agent.remainingDistance <= 0.2f)
            {
               // currentIndex++;  // is this a sequence ? iterating to next patrol point 
                currentIndex = (currentIndex + 1) % patrolPoints.Count;
                isPathCalculated = false;  //reset path bool

                Debug.Log($"Moving to waypoint {currentIndex}");
            }

            return Nodes.Status.Running;  //agent is still moving so process is running
        }

        public void Reset() => currentIndex = 0;  // set index of waypoints back to 0 if process isnt running ? (original waypoint)
    }

    public class MoveToTarget : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform target;
        bool isPathCalculated;

        public MoveToTarget(Transform entity, NavMeshAgent agent, Transform target)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
        }

        public Nodes.Status Process()
        {
            if (Vector3.Distance(entity.position, target.position) < 1f)
            {
                return Nodes.Status.Success;
            }

            agent.SetDestination(target.position);
           // entity.LookAt(target.position.With(y: entity.position.y));

            if (agent.pathPending)
            {
                isPathCalculated = true;
            }
            return Nodes.Status.Running;
        }

        public void Reset() => isPathCalculated = false;
    }
}
