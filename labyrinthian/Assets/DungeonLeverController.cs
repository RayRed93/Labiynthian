//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Sends UnityEvents for basic hand interactions
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Valve.VR.InteractionSystem;

namespace RedScripts
{
    [RequireComponent(typeof(Interactable))]
    public class DungeonLeverController : MonoBehaviour
    {
        public enum LeverState
        {
            Up,
            Down
        }

        [SerializeField]
        private LeverState leverState;

        [SerializeField]
        private DungeonGateController dungeonGate;


        private Animation leverAnim;

        private void Start()
        {
            leverAnim = this.GetComponent<Animation>();
        }

        private void HandHoverUpdate(Hand hand)
        {
            if (hand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
            {
                Debug.Log("OPRN GATE");
                if (leverState == LeverState.Down)
                {
                    StartCoroutine(SwithLever(LeverState.Up));
                }
                else
                {
                    StartCoroutine(SwithLever(LeverState.Down));
                }
                    
            }
        }

        private IEnumerator SwithLever(LeverState state)
        {
            if ((dungeonGate.gateState != DungeonGateController.GateState.Closing && dungeonGate.gateState != DungeonGateController.GateState.Opening))
            {
                switch (state)
                {
                    case LeverState.Up:
                        leverAnim.Play("lever_up", PlayMode.StopAll);
                        break;
                    case LeverState.Down:
                        leverAnim.Play("lever_down", PlayMode.StopAll);
                        break;
                }

                while (leverAnim.isPlaying)
                {
                    yield return new WaitForEndOfFrame();
                }

                leverState = state;

                if (state == LeverState.Down)
                {
                    dungeonGate.ChangeState(DungeonGateController.GateState.Opening);
                }
                else
                {
                    dungeonGate.ChangeState(DungeonGateController.GateState.Closing);
                } 
            }
        }
      

    }
}