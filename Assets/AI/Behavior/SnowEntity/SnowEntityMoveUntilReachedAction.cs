using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Snow Entity Move Until Reached",
        description: "Agent moves to its current target and returns Success only when it reaches it.",
        category: "Polar Night/Snow Entity",
        id: "de4f97d5e4fb44e4ba275f5c164a7080")]
    public partial class SnowEntityMoveUntilReachedAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            return bridge.MoveToCurrentTarget() ? Status.Running : Status.Failure;
        }

        protected override Status OnUpdate()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            if (!bridge.CanMove())
            {
                return Status.Failure;
            }

            if (bridge.HasReachedTarget())
            {
                return Status.Success;
            }

            return Status.Running;
        }

        private SnowEntityBehaviourBridge GetBridge()
        {
            GameObject agentObject = Agent != null && Agent.Value ? Agent.Value : GameObject;

            if (!agentObject)
            {
                return null;
            }

            return agentObject.GetComponent<SnowEntityBehaviourBridge>();
        }
    }
}