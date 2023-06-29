using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public List<GameObject> cardsonTable = new List<GameObject>();
    [SerializeField] private MainGame mainGame;
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        cardsonTable.Add(other.gameObject); //? ortaya atılan kartı Tabledaki kartlar listesine ekliyorum
        
        Debug.Log("mASADAKi kartlar: " + cardsonTable.Count);
        if (other.gameObject.transform.parent.tag == "Hand")
        {
            mainGame.gameStart = true;
            other.gameObject.transform.SetParent(this.gameObject.transform);
            other.gameObject.transform.position = this.gameObject.transform.position;
            mainGame.GroupOrganizer(this.gameObject);
            //mainGame.GroupOrganizer(mainGame.handCardsGroup);
            mainGame.updateCard = true;
            if (cardsonTable.Count >= 2)
            {
                mainGame.GameRules(true);
                //mainGame.CheckforGameOver();
                mainGame.CheckforEmptyHand();
            }

            Invoke("BotPlaysforTable", 2f);
        }
        else
        {
            if (mainGame.gameStart)
            {
                if (cardsonTable.Count >= 2)
                {
                    mainGame.GameRules(false);
                    //mainGame.CheckforGameOver();
                    mainGame.CheckforEmptyHand();
                }
            }
        }
    }

    void BotPlaysforTable()
    {
        mainGame.BotPlays();
    }
}
