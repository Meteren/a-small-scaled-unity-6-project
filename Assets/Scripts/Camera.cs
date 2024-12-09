using Cinemachine;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin channel;
    void Start()
    {
        channel = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        GameManager.instance.blackBoard.SetValue("Channel", channel);
    }

   
}
