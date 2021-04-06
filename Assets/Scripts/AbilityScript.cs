using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{
    PlayerScript parentScript;
    GameObject abilityManager;
    _VariableManager _VariableManager;
    public GameObject dashEffect;

    /* Abilities
     * Dash (Cooldown) - makes you invulnerable
     * Rapid Fire (Cooldown) - scales shooting rate
     * Fire Rate (Passive Scale) - scales shooting rate
     * 
     * 
     * 
     * 
     * 
     * Slo-mo maybe
     */

    float dashTime = 15;
    float tempDash = 0;
    float initialSpeed;
    float newSpeed;
    bool earlyQueue = false;
    bool dashQueued = false;

    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponent<PlayerScript>();

        abilityManager = GameObject.Find("VariableManager");

        if (abilityManager)
        {
            _VariableManager = abilityManager.GetComponent<_VariableManager>();
        }
        else
        {
            Debug.Log("Ability Manager Not Found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!abilityManager)
        {
            abilityManager = GameObject.Find("VariableManager");

            if (abilityManager)
            {
                _VariableManager = abilityManager.GetComponent<_VariableManager>();
                Debug.Log("Ability Manager Found");
            }
        }
        else
        {
            if (_VariableManager.dashCooldown < _VariableManager.dashCount)
            {
                _VariableManager.dashCooldown++;
            }

            if (parentScript.dash)
            {
                tempDash++;
                parentScript.moveSpeed = ((newSpeed - initialSpeed) * (1 - (tempDash / dashTime))) + initialSpeed;

                Instantiate(dashEffect, transform.position + new Vector3(0, -0.2f, 0), Quaternion.identity);

                if (tempDash >= dashTime)
                {
                    tempDash = 0;
                    parentScript.dash = false;
                    parentScript.moveSpeed = initialSpeed;
                    parentScript.invincible = false;
                    parentScript.movement.x = Input.GetAxisRaw("Horizontal"); //Input for x movement
                    parentScript.movement.y = Input.GetAxisRaw("Vertical"); //Input for y movement
                }

                //Dash Queue
                if (Input.GetKeyDown("space"))
                {
                    earlyQueue = true;
                }
            }

            //Dash Mechanism
            if ((Input.GetKeyDown("space") || dashQueued) && !parentScript.dash)
            {
                dashQueued = false;

                //Get player's current velocity
                float xMov = parentScript.movement.x;
                float yMov = parentScript.movement.y;

                initialSpeed = parentScript.moveSpeed;

                //Stop player
                if (xMov != 0 || yMov != 0)
                {
                    parentScript.dash = true;

                    //Set player velocity
                    parentScript.movement.x = xMov;
                    parentScript.movement.y = yMov;
                    parentScript.moveSpeed *= 6;
                    newSpeed = parentScript.moveSpeed;
                    parentScript.invincible = true;
                }
            }

            if (earlyQueue)
            {
                earlyQueue = false;
                dashQueued = true;
            }
        }
    }
}
