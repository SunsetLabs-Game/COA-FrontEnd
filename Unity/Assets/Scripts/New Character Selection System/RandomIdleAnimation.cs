using UnityEngine;

namespace Chidera_Nwosu
{
    public class RandomIdleAnimation : StateMachineBehaviour
    {
        private bool isBored;
        private float idleTime;
        private int randomHash;
        private int boredAnimation;

        [Header("Details")]
        [SerializeField] private float timeUntilBored;
        [SerializeField] private int numberOfBoredAnimations;


        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ResetBoredom();
            if(randomHash == 0 )
            {
                randomHash = Animator.StringToHash("randomAnimation");
            }
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (isBored == false)
            {
                idleTime += Time.deltaTime;
                if (idleTime >= timeUntilBored)
                {
                    isBored = true;
                    boredAnimation = Random.Range(1, numberOfBoredAnimations + 1);
                    boredAnimation = boredAnimation * 2 - 1;

                    animator.SetFloat(randomHash, boredAnimation - 1);
                }
            }
            else if (stateInfo.normalizedTime % 1 >= 0.98f)
            {
                ResetBoredom();
            }
            animator.SetFloat(randomHash, boredAnimation, 0.2f, Time.deltaTime);
        }

        private void ResetBoredom()
        {
            if (isBored)
            {
                boredAnimation--;
            }
            isBored = false;
            idleTime = 0.0f;
        }
    }
}
