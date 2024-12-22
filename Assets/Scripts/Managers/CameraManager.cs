using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (instance !=null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
}
