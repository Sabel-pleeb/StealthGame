using UnityEngine;
using System.Collections.Generic;
using Pathfinding.BehaviourTrees;
using UnityEngine.AI;


public class Entity 
{
        public EntityMovement Movement;
        public EntityVision Vision;
        public EAnimStates Anims;
        public EntityInteract Interact;
        public EntityDetection Detection;
        public ELightDetection lightDetection;
        public GeneratorManager Generators;
        public PlayerHiding playerHiding;
        public Transform Transform;
    public UIManager uiManager;
}
