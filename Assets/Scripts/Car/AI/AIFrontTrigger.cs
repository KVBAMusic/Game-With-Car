using UnityEngine;

public class AIFrontTrigger : MonoBehaviour
{
    [SerializeField] CarAIController carAI;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Car")
        {
            carAI.needsToReverse = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        carAI.needsToReverse = false;
    }
}
