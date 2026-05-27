using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Snow Entity Pick Wander Target",
        description: "Agent picks a random wander target.",
        category: "Polar Night/Snow Entity",
        id: "c710ed9bc8374650a8719eb3de7481e2")]
    public partial class SnowEntityPickWanderTargetAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            return bridge.PickRandomWanderPosition() ? Status.Success : Status.Failure;
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