using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    [Tooltip("The type of sword skill currently selected. Determines behavior such as bouncing or piercing.")]
    public SwordType swordType = SwordType.Regular;

    [Header("Bounce Info")]
    [SerializeField, Tooltip("The number of times the sword can bounce off surfaces or enemies.")]
    private int bounceAmount;

    [SerializeField, Tooltip("The gravity effect applied when the sword is in bouncing mode.")]
    private float bounceGravity;

    [SerializeField, Tooltip("The speed of the bouncing sword when traveling between targets")]
    private float bounceSpeed;

    [Header("Pierce Info")]
    [SerializeField, Tooltip("The amount of pierce this object has. Determines how many targets it can penetrate.")]
    private int piearceAmount;

    [SerializeField, Tooltip("The gravity effect applied when the sword is in piercing mode.")]
    private float pierceGravity;

    [Header("Spin Info")]
    [SerializeField, Tooltip("How often the sword can hit enemies.")]
    private float hitCooldown = .33f;
    [SerializeField, Tooltip("The distance the sword should travel away from player before stopping in place")]
    private float maxTravelDistance = 7;
    [SerializeField, Tooltip("The amount of time the sword should remain spinning before is returns to player")]
    private float spinDuration;
    [SerializeField, Tooltip("The gravity effect applied when the sword is in spinning mode.")]
    private float spinGravity = 1f;

    [Header("Skill Info")]
    [SerializeField, Tooltip("The prefab used to instantiate the sword object.")]
    private GameObject swordPrefab;

    [SerializeField, Tooltip("The speed at which the sword will return to player when called back.")]
    private float returnSpeed;

    [SerializeField, Tooltip("The force applied to the sword when launched.")]
    private Vector2 launchForce;

    [SerializeField, Tooltip("The default gravity effect applied to the sword.")]
    private float defaultSwordGravity;
    
    [SerializeField, Tooltip("The duration the enemy will remain frozen after being hit by the sword.")]
    private float freezeDuration;

    [Header("Aim Dots")]
    [SerializeField, Tooltip("The number of dots used to visualize the sword's trajectory.")]
    private int numberOfDots;

    [SerializeField, Tooltip("The spacing between the dots in the trajectory visualization.")]
    private float spaceBetweenDots;

    [SerializeField, Tooltip("The prefab used to instantiate the trajectory dots.")]
    private GameObject dotPrefab;

    [SerializeField, Tooltip("The parent transform under which all trajectory dots are organized.")]
    private Transform dotsParent;

    private Vector2 finalDir;
    private GameObject[] dots;
    private bool areDotsActive;
    private float swordGravity;

    protected override void Start()
    {
        base.Start();

        SetupGravity();

        GenerateDots();

    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if(swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if(swordType == SwordType.Spin)
            swordGravity = spinGravity;
        else if(swordType == SwordType.Regular)
            swordGravity = defaultSwordGravity;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

        if (Input.GetKey(KeyCode.Mouse1))
        {
            SetupGravity();

            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
        }
    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        else if(swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(piearceAmount);
        else if(swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);

        SetupGravity();

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim Region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - playerPosition;

        return direction;
    }

    public void DotsActive(bool _isActive)
    {
        areDotsActive = _isActive;
        for (int i = 1; i < dots.Length; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x * launchForce.x,
            AimDirection().normalized.y * launchForce.y) * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion
}
