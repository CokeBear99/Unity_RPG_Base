using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private KeyCode myHotkey;
    private TextMeshProUGUI myText;

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackhole;

    public void SetupHotkey(KeyCode _myHotkey, Transform _myEnemy, Blackhole_Skill_Controller _myBlackhole)
    {
        sr = GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myEnemy = _myEnemy;
        blackhole = _myBlackhole;

        myHotkey = _myHotkey;
        myText.text = _myHotkey.ToString();
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(myHotkey))
        {
            blackhole.AddEnemyToList(myEnemy);

            //키가 눌려졌을때 Text Visible Off
            myText.color = Color.clear;
            sr.color = Color.clear;

        }
            

    }



}
