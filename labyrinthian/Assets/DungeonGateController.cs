using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedScripts
{
   
    public class DungeonGateController : MonoBehaviour
    {
        bool AnimatorIsPlaying(Animator animator)
        {
            return animator.GetCurrentAnimatorStateInfo(0).length >
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public enum GateState
        {
            Opening,
            Opened,
            Closing,
            Closed
        }

        public GateState gateState;

        [SerializeField]
        private Animator gateAnimator;

        void Start()
        {
            switch (gateState)
            {
                
                case GateState.Opened:
                    ChangeState(GateState.Opening);
                    break;               
                case GateState.Closed:
                    ChangeState(GateState.Closing);
                    break;
               
                default:
                    break;
            }
            

        }

        void Update()
        {

        }

        public void ChangeState(GateState state)
        {
            gateState = state;

            StartCoroutine(SwitchGate(state));
        }

           
        private IEnumerator SwitchGate(GateState state)
        {
            switch (state)
            {
                case GateState.Opening:
                    gateAnimator.Play("DoorOpen");

                    break;

                case GateState.Closing:
                    gateAnimator.Play("DoorClose");
                    break;

            }
            

            while((gateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f && gateAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f))
            {
                yield return new WaitForEndOfFrame();
            }

            if (state == GateState.Opening)
            {
                gateState = GateState.Opened;
                Debug.Log("GATE OPENED");
            }
            else if(state == GateState.Closing)
            {
                gateState = GateState.Closed;
                Debug.Log("GATE CLOSED");
            }
        }

    }
}