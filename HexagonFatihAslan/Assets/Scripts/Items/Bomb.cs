using UnityEngine;
using UnityEngine.UI;

public class Bomb : Item
{
    [SerializeField]
    int MinHealth,MaxHealth;
    int FullHealth=0;
    int Health;
    int SpawnedAt=Global.Move;
    Text HealthText;

    void Start()
    {
        HealthText=transform.GetChild(0).GetChild(0).GetComponent<Text>();
        FullHealth=Random.Range(MinHealth,MaxHealth+1);
    }

    void LateUpdate()
    {        
        Health=SpawnedAt-Global.Move+FullHealth;
        HealthText.text=""+Health;
        if(Health<=0)
            Global.GameOver=true;
    }
}