using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarScript : MonoBehaviour
{
    private Animator animator;
    float maxHealth = 50;
    float health;

    public GameObject healthbar;
    
    // Start is called before the first frame update
    void Start()
    {
        healthbar = Instantiate(healthbar, transform.position, Quaternion.identity);

        animator = healthbar.GetComponent<Animator>();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Health", health / maxHealth);
    }
}
