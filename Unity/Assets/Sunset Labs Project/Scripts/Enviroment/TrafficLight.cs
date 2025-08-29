using UnityEngine;
using UnityEngine.AI;

public class TrafficLight : MonoBehaviour
{
    private bool isRedLight;
    private float countDownTimer;

    [Header("Parameters")]
    [Range(15f, 50f)] private float changeColorTimer;
    [SerializeField] private NavMeshObstacle roadBlock;
    [SerializeField] private GameObject[] trafficLights;

    private void Start()
    {
        isRedLight = false;
        countDownTimer = changeColorTimer;
    }

    private void HandleTrafficLight_Action()
    {
        GameObject light = (isRedLight) ? trafficLights[0] : trafficLights[2];
        countDownTimer -= Time.deltaTime;
        if(countDownTimer <= 0)
        {
            isRedLight = !isRedLight;
            countDownTimer = (isRedLight) ? changeColorTimer - 20 : changeColorTimer;
        }

        roadBlock.gameObject.SetActive(isRedLight);
        if (countDownTimer == 10)
        {
            SwitchLight(trafficLights[1]);
            return;
        }
        SwitchLight(light);
    }

    private void SwitchLight(GameObject light)
    {
        foreach(var l in trafficLights)
        {
            //Set True if Light is Equals to L
            l.SetActive(l == light);
        }
    }
}
