using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _VariableManager : MonoBehaviour
{
    public static _VariableManager Instance { get; private set; }

    //Variables///////////////////////////

    //Dash Ability
    public int dashCount;
    public int dashCooldown;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //Initialize all variable values////////////////////////

            //Dash ability
            dashCount = 0;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}