using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    public static DebugMode instance;

    [Tooltip("Enable debug mode")]
    public bool debugMode = false;

    [Space]

    [Header("Enemy Settings")]
    [Tooltip("Freeze all enemy movement")]
    public bool freezeEnemies = false;

    [Header("Player Settings")]
    [Tooltip("Free player movement")]
    public bool freePlayerMovement = false;
    
    [Tooltip("Player speed when free movement is enabled")]
    public float freePlayerSpeed = 20f;

    // Handled by custom editor
    [HideInInspector, Tooltip("Adjust the game speed percentage")]
    public int gameSpeed = 100;

    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    /// <summary>
    /// Updates the debug mode state every frame.
    /// </summary>
    private void Update()
    {
        if (debugMode)
        {
            FreezeEnemies();
            FreePlayerMovement();
        }
    }

    /// <summary>
    /// Enables free player movement if the option is enabled.
    /// </summary>
    private void FreePlayerMovement()
    {
        if (freePlayerMovement)
        {
            Player player = PlayerManager.instance.player;

            player.GetComponent<Rigidbody2D>().gravityScale = 0;

            player.setZeroVelocity();
            
            if (Input.GetKey(KeyCode.W))
            {
                player.transform.position += Vector3.up * freePlayerSpeed * Time.unscaledDeltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                player.transform.position += Vector3.down * freePlayerSpeed * Time.unscaledDeltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                player.transform.position += Vector3.left * freePlayerSpeed * Time.unscaledDeltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                player.transform.position += Vector3.right * freePlayerSpeed * Time.unscaledDeltaTime;
            }
            if (Input.GetKey(KeyCode.Space)) // Press space to exit free movement
            {
                freePlayerMovement = false;
                player.stateMachine.ChangeState(player.airState);
            }

        }
    }

    /// <summary>
    /// Freezes or unfreezes all enemies based on the option.
    /// </summary>
    private void FreezeEnemies()
    {
        if (freezeEnemies)
        {
            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                enemy.moveSpeed = 0;
                enemy.anim.speed = 0;
            }
        }
        else
            ResetEnemyMoveSpeed();
    } 

    public void ResetAll()
    {
        Time.timeScale = 1; // Reset game speed
        ResetEnemyMoveSpeed(); // Reset enemy move speed
        Player player = PlayerManager.instance.player;
        player.stateMachine.ChangeState(player.airState); // Reset player state
    }

    /// <summary>
    /// Resets the move speed of all enemies to their default values./// 
    private static void ResetEnemyMoveSpeed()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.moveSpeed = enemy.defaultMoveSpeed;
            enemy.anim.speed = 1;
        }
    }
}

[CustomEditor(typeof(DebugMode))]
public class DebugScriptEditor : Editor
{
    SerializedProperty gameSpeedProp;

    /// <summary>
    /// Finds the gameSpeed property.
    /// </summary>
    private void OnEnable()
    {
        gameSpeedProp = serializedObject.FindProperty("gameSpeed");
    }

    /// <summary>
    /// Customizes the inspector GUI for the DebugMode script.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Game Settings", EditorStyles.boldLabel);

        gameSpeedProp.intValue = Mathf.RoundToInt(EditorGUILayout.Slider(new GUIContent("Game Speed %"), gameSpeedProp.intValue, 50, 200));

        serializedObject.ApplyModifiedProperties();        
    }
}
