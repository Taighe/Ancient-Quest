using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Player), true)]
public class PlayerEditor : Editor 
{
    [InitializeOnEnterPlayMode]
    static void InitializeOnPlay()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        var obj = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //Init Player variables
        obj.Editor_SetHP(obj.HP);
        obj.Editor_SetMaxHP(obj.MaxHP);
        obj.Editor_RemoveAllPowerUps();
        if (obj.SlingPowerUp)
            obj.AddPowerUp(PowerUps.Sling);

        if (obj.SwordPowerUp)
            obj.AddPowerUp(PowerUps.Sword);

        if (obj.ShieldPowerUp)
            obj.AddPowerUp(PowerUps.Shield);
    }
}