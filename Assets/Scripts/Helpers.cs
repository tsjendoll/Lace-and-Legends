using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public static class Helpers 
{
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    public static void SyncAnimators(this Animator _animator, AnimatorStateInfo _parentStateInfo) {
        float parentAnimatorTime = _parentStateInfo.normalizedTime % 1;

        _animator.SetBool("Idle", true);
        _animator.Play(_parentStateInfo.fullPathHash, -1, parentAnimatorTime);
        _animator.Update(0);

    }

    /// <summary>
    /// Returns an array of colliders within a specified circle, filtered by the entities facing direction.
    /// </summary>
    /// <param name="position">The center position of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="facingDir">
    /// The direction the player is facing:
    /// <list type="bullet">
    /// <item><description>1 for right</description></item>
    /// <item><description>-1 for left</description></item>
    /// </list>
    /// </param>
    /// <returns>An array of colliders within the specified half of the circle based on the facing direction.</returns>
    public static Collider2D[] GetFilteredColliders(Vector2 position, float radius, int facingDir)
    {
        Collider2D[] allColliders = Physics2D.OverlapCircleAll(position, radius);
        List<Collider2D> filteredColliders = new List<Collider2D>();

        foreach (Collider2D collider in allColliders)
        {
            float relativeX = collider.transform.position.x - position.x;

            // Filter based on the facing direction
            if ((facingDir == -1 && relativeX <= 0) ||  // Left half if facing left
                (facingDir == 1 && relativeX >= 0))     // Right half if facing right
            {
                filteredColliders.Add(collider);
            }
        }

        return filteredColliders.ToArray();
    }
}
