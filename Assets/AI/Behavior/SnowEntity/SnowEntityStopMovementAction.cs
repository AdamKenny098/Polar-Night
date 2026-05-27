using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace Unity.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Snow Entity Stop Movement",
        description: "Agent stops moving.",
        category: "Polar Night/Snow Entity",
        id: "b53ac279fd7b4cf58f96d97d2fd05903")]
    public partial class SnowEntityStopMovementAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            SnowEntityBehaviourBridge bridge = GetBridge();

            if (!bridge)
            {
                return Status.Failure;
            }

            bridge.StopMovement();
            return Status.Success;
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