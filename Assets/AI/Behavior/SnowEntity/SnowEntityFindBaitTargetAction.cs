using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Snow Entity Find Bait Target",
        description: "Agent finds nearest active entity bait and sets it as target.",
        category: "Polar Night/Snow Entity",
        id: "7e7ed3940a9b40e5a9a3210b648e7750")]
    public partial class SnowEntityFindBaitTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            return bridge.FindNearestBaitTarget() ? Status.Success : Status.Failure;
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