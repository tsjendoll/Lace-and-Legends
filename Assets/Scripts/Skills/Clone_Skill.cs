using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Text.RegularExpressions;

public class Clone_Skill : Skill
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;
    private List<RuntimeAnimatorController> cloneACS = new List<RuntimeAnimatorController>();

    private Regex regex = new Regex(@"Female(.*?)_AC");

    protected override void Start()
    {
        base.Start();
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/CloneBoots_AC"));
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/CloneCorset_AC"));
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/CloneHair_AC"));
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/ClonePantiesAndBra_AC"));
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/CloneSkirt_AC"));
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/CloneSocks_AC"));
        cloneACS.Add(Resources.Load<RuntimeAnimatorController>("Animation/Controllers/Clone/CloneSword_AC"));
    }

    public void CreateClone(Transform _clonePosition)
    {
        // Instantiate the clone
        GameObject newClone = Instantiate(clonePrefab);

        

        // Get all Animator components from the clone and the player, including children
        Animator[] cloneAnimators = newClone.GetComponentsInChildren<Animator>();
        Animator[] playerAnimators = PlayerManager.instance.player.GetComponentsInChildren<Animator>();

        // Ensure both arrays have the same length to match controllers
        if (cloneAnimators.Length != playerAnimators.Length)
        {
            Debug.LogWarning("Mismatch in the number of Animator components between the clone and the player.");
        }

        Debug.Log(cloneAnimators.Length);
        // Match each Clothing_AC to the proper clone_AC
        for (int i = 1; i < cloneAnimators.Length; i++)
        {
            Match match = regex.Match(playerAnimators[i].runtimeAnimatorController.name);
            if(match.Success)
            {

                switch (match.Groups[1].Value)
                {
                    case "Boots":
                        // Handle Boots case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[0];
                        break;

                    case "Corset":
                        // Handle Corset case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[1];
                        break;

                    case "Hair":
                        // Handle Hair case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[2];
                        break;

                    case "PantiesAndBra":
                        // Handle PantiesAndBra case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[3];
                        break;

                    case "Skirt":
                        // Handle Skirt case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[4];
                        break;

                    case "Socks":
                        // Handle Corset case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[5];
                        break;

                    case "Sword":
                        // Handle Sword case
                        cloneAnimators[i].runtimeAnimatorController = cloneACS[6];
                        break;
                }
            }
        }

        AnimatorStateInfo parentStateInfo = cloneAnimators[0].GetCurrentAnimatorStateInfo(0);

        for (int i = 0; i < cloneAnimators.Length; i++)
        {
            cloneAnimators[i].SyncAnimators(parentStateInfo);
        }

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack);

    }
}

