using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Snow Entity Is Captured",
        description: "Checks if Agent is captured.",
        category: "Polar Night/Snow Entity",
        id: "7c1f3d6e6f3f4b0d9a8e8c31d2b5a901")]
    public partial class SnowEntityIsCapturedAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            return bridge.IsCaptured() ? Status.Success : Status.Failure;
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