using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework.Interfaces;

namespace Pathfinding.BehaviourTrees  //namespace for organsing group related functionailty, eg can use this BHT for all enemies 
{   // add more node ideas 
    //Repeat, repeat node x number of times 
    public class UntilFail : Nodes  // continue running unless fail 
    {
        public UntilFail(string name) : base(name) { }

        public override Status Process()
        {
            if (children[0].Process() == Status.Failure)
            {
                Reset();
                return Status.Failure;
            }

            return Status.Running;
        }

    }

    public class UntilSuccess : Nodes  // continue running unless succeed 
    {
        public UntilSuccess(string name) : base(name) { }

        public override Status Process()
        {
            if (children[0].Process() == Status.Success)
            {
                Reset();
                return Status.Success;
            }

            return Status.Running;
        }

    }

    public class Inverter : Nodes  // logical NOT, returns opposite of what it's child returns (only works on one child)
    {
        public Inverter(string name) : base(name) { }

        public override Status Process()
        {
            switch (children[0].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    return Status.Success;
                default:
                    return Status.Failure;
            }
        }
    }

    public class RandomSelector : PrioritySelector // overrides and randomises priority slector 
    {
        protected override List<Nodes> SortChildren() => children.Shuffle().ToList();

        public RandomSelector(string name) : base(name) { }
    }

    public class PrioritySelector : Selector  // like selector but can change order instead of playing in order of being added
    { // inherits from selector
        List<Nodes> sortedChildren;
        List<Nodes> SortedChildren => sortedChildren ??= SortChildren();

        protected virtual List<Nodes> SortChildren() => children.OrderByDescending(child => child.priority).ToList();

        public PrioritySelector(string name) : base(name) { }

        public override void Reset()
        {
            base.Reset();  
            sortedChildren = null;
        }

        public override Status Process() // try to get one of the children to succeed in order of priority
        {
            foreach (var child in SortedChildren)
            {

              var status = child.Process();

                if (status == Status.Success)
                {
                    return Status.Success;
                }

                if (status == Status.Running)
                {
                    return Status.Running;
                }
            } 
            return Status.Failure;
        }
    }

    public class Selector : Nodes  // Logical OR, similar to Sequence but only needs one successful child
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if(currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:  // if one child is successful, return sucess
                        Reset();
                        return Status.Success;
                    default:  // if there was a failure just move to next child 
                        currentChild++;
                        return Status.Running;
                }
            }
            Reset();
            return Status.Failure;  // if get through all children and none succeeded, return failure 
        }
    }

    public class Sequence : Nodes  // logical AND, makes sure every child get's executed properly or its a failure
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if (currentChild < children.Count)  // still in change of children as we are iterating through them
            {
                switch (children[currentChild].Process())  // evaluate the return of each childs process method
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();  // reset whole node if one child/part of sequence fails 
                        return Status.Failure;
                    /*  default:
                          currentChild++;  // next child until changes from running to success 
                          return currentChild == children.Count ? Status.Success : Status.Running; */
                    case Status.Success:
                        currentChild++;

                        if (currentChild == children.Count)
                        {
                            Reset();
                            return Status.Success;
                        }

                        return Status.Running;
                }
            }
            Reset(); 
            return Status.Success;  // if did not make it inside if condition
        }
    }

    public class Leaf : Nodes  // leaf = node that doesn't have any children, only has behaviour to execute, inherits from Nodes ?
    {
        // encapsulate behaviour in a 'strategy'
        readonly IStrategy strategy;

        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)  //constructor chaining, takes leaf name and passes to base class (Nodes class below ?)
        {
            //Preconditions.CheckNotNull(strategy); //from prev vid idk
            this.strategy = strategy;
          //  Debug.Log(name + "JUST RAN");
        }

        public override Status Process() => strategy.Process();  //execute the strategy then reset
        public override void Reset() => strategy.Reset();
    }

    public class Nodes  //BHTree made of nodes, nodes need to report on status 
    {
        public enum Status
        {
            Success,
            Failure,
            Running
        }

        public readonly string name;  // name of each node 
        public readonly int priority;

        public readonly List<Nodes> children = new();  // each node can have 0 or more children (leaf)
        protected int currentChild; // keep track of current child

        public Nodes(string name = "Node", int priority = 0) // default name and priority 0
        {
            this.name = name;
            this.priority = priority;
        }

        public void AddChild(Nodes child) => children.Add(child);  //public method to add child to node 

        public virtual Status Process() => children[currentChild].Process();  //Process method, 'virtual' so can be overridden, call CHILD at INDEX and run THAT process method

        public virtual void Reset()   // virtual so can be overridden, reset entire tree by iterating through all children
        {
            currentChild = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }


    public class BehaviourTree : Nodes  // in the case of BHT being children of BHT (Node is parent ?)
    {
        public BehaviourTree(string name) : base(name) { }  //pass child behaviour tree to main base class

        public override Status Process()  //override process method
        {
         /*    while (currentChild < children.Count)  // interate through all the children and run their process method
             {
                 var status = children[currentChild].Process();
                 if (status != Status.Success)  //[1a]
                 {
                     return status;
                 }
                 currentChild++;  //if running current child process was success [1a] , keep iterating 
             }
           //  return Status.Success;  //if all children are have processed, return success 
         //  return children[0].Process(); */

            if (children.Count == 0)
                return Status.Failure;

            return children[0].Process();
        }
    }
}
