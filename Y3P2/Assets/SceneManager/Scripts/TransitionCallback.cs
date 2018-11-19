using UnityEngine;

namespace DB.MenuPack
{
    public class TransitionCallback : MonoBehaviour
    {

        public void AnimationEventTransitionComplete()
        {
            if (SceneManager.isLoadingLevel)
            {
                SceneManager.transitionComplete = true;
            }
        }
    }
}
