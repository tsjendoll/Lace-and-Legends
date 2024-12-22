using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{    
    public static PlayerManager instance;
    public Player player;

    public Animator playerAnimator;
    public Animator topAnimator;
    public Animator bottomAnimator;

    

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
}
