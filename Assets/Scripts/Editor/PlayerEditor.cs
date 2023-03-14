using AQEngine.Globals;
using AQEngine.Objects.KinematicObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UEditor = UnityEditor.Editor; 

namespace AQEngine.Editor
{
    [CustomEditor(typeof(Player), true)]
    public class PlayerEditor : UEditor
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
}