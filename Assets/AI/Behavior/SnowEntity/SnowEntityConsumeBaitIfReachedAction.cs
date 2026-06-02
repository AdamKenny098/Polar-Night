using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Snow Entity Consume Bait If Reached",
        description: "Agent consumes current bait if close enough.",
        category: "Polar Night/Snow Entity",
        id: "a0d5bdb7e0e2439f8987a29ee91bc3bb")]
    public partial class SnowEntityConsumeBaitIfReachedAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            return bridge.ConsumeBaitIfReached() ? Status.Success : Status.Failure;
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