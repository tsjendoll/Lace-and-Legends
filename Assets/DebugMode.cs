using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    [SerializeField, Tooltip("Enable debug mode")]
    private bool debugMode = false;

    [Space]

    [Header("Enemy Settings")]
    [SerializeField, Tooltip("Freeze all enemy movement")]
    private bool freezeEnemies = false;

    private void Update()
    {
        if (debugMode)
        {
            if (freezeEnemies)
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.moveSpeed = 0;
                }
            }
            else
            {
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    enemy.moveSpeed = enemy.defaultMoveSpeed;
                }
            }
        }
    }
}
