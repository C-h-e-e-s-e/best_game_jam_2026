using System.Collections;
using UnityEngine;

public class RandomTeleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportInterval = 60f; // Time in seconds (60s = 1 minute)
    public Transform[] teleportPoints; // Drag your empty point transforms here

    private Transform player;

    void Start()
    {
        // Find the player automatically by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (teleportPoints == null || teleportPoints.Length == 0)
        {
            Debug.LogWarning("No teleport points assigned to RandomTeleporter!");
            return;
        }

        // Start the timer loop
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(teleportInterval);
            TeleportPlayerRandomly();
        }
    }

    void TeleportPlayerRandomly()
    {
        if (player == null || teleportPoints.Length == 0) return;

        // Pick a random teleport point
        int randomIndex = Random.Range(0, teleportPoints.Length);
        Transform targetPoint = teleportPoints[randomIndex];

        // Optional: If you use a Rigidbody2D, reset velocity so momentum doesn't carry over
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Teleport the player
        player.position = targetPoint.position;
        Debug.Log("Player was teleported to a random location!");
    }
}