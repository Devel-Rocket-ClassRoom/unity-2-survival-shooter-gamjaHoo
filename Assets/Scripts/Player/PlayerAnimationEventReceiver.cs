using UnityEngine;

public class PlayerAnimationEventReceiver : MonoBehaviour
{
    public void RestartLevel()
    {
        GameManager.instance?.RestartLevel();
    }
}