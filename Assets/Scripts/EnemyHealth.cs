
using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int curHealth = 100;
    public float healthBarLength;
    // Use this for initialization
    void Start()
    {
        healthBarLength = Screen.width / 2;
    }

    // Update is called once per frame


    void OnGUI()
    {
        GUI.Box(new Rect(10, 40, healthBarLength, 20), curHealth + "/" + maxHealth);
    }

    public void AdjustCurrentHealth(int adjust)
    {
        curHealth -= adjust;
        healthBarLength = (Screen.width / 4) * (curHealth / (float)maxHealth);
        if (curHealth <= 0)
        {
            Destroy(this.gameObject);
        }
      
    }
}
