using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public List<GameObject> cardsonTable = new List<GameObject>();
    [SerializeField] private MainGame mainGame;
    
    //I added a box collider to the items so if a card is played to the table, OnTrigger will be activated
    private void OnTriggerEnter2D(Collider2D other)  
    {
        cardsonTable.Add(other.gameObject); //Adding the played card to the table cards list whether it is played by bot or the player.
        
        if (other.gameObject.transform.parent.tag == "Hand")    //If the card is played by the player
        {
            mainGame.gameStart = true;          //This is for the bot. At line 34 gamerules for the bot is called but it needs to be called only when the bot plays so i put the gameStart condition and made it true here  
            other.gameObject.transform.SetParent(this.gameObject.transform);            //Arranges the played card's parent as table
            other.gameObject.transform.position = this.gameObject.transform.position;   //Arranges the position
            mainGame.TableSpacingOrganizer();   //Organizes the table spacing
            if (cardsonTable.Count >= 2)        
            {
                mainGame.GameRules(true);
            }
            mainGame.CheckforEmptyHand();
            Invoke("BotPlaysforTable", 0.7f);  //After the player's turn, bot plays
        }
        else    //If the parent of the object which triggers is not the player 
        {
            if (mainGame.gameStart) //When the game starts, 4 cards appear on the table and they trigger the function. Their parent is not hand and gamerules shouldn't be applied for them so i use the gameStart bool.
                                    //It is false at the start so starting cards will not enter this condition and other played cards by bot will enter
            {
                if (cardsonTable.Count >= 2)
                {
                    mainGame.GameRules(false);
                }
                mainGame.CheckforGameOver();
                mainGame.CheckforEmptyHand();
            }
        }
    }

    void BotPlaysforTable() //To use invoke, i created a function to call the botplays from another script
    {
        mainGame.BotPlays();
    }
}
