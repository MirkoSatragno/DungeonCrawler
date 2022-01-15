using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField, Range(0.5f, 10f)]
    protected float disappearingDuration = 2;

    public delegate void RemoveObstacleDelegate();
    public static RemoveObstacleDelegate RemoveObstacle;

    protected SpriteRenderer sprite;
    private bool vanishing;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Debug.Assert(sprite, "Obstacle: spriteRenderer component not found");

        vanishing = false;
    }

    private void OnEnable()
    {
        RemoveObstacle += onRemoveObstacle;
    }

    private void Update()
    {
        if (vanishing)
        {
            Color spriteColor = sprite.color;
            spriteColor.a = Mathf.Max(0f, spriteColor.a - Time.deltaTime / disappearingDuration);
            sprite.color = spriteColor;

            if (spriteColor.a == 0)
            {
                SkilledPlayer.EndPower();
                Destroy(gameObject);
            }
                

        }
    }

    private void OnDisable()
    {
        RemoveObstacle -= onRemoveObstacle;
    }



    private void onRemoveObstacle()
    {
        vanishing = true;
        GetComponent<AudioSource>().Play();
    }
}
