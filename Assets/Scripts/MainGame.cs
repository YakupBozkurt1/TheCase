using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainGame : MonoBehaviour
{
    private List<int[,]> cardsInfo = new List<int[,]>();    //The 52 card deck list
    [SerializeField] private List<Sprite> sprites;      //List of sprites for the cards
    [SerializeField] private TextMeshProUGUI playerScoreText, botScoreText;
    [SerializeField] private GameObject gameOver;
    private TextMeshProUGUI gameOverText; 
    public GameObject handCardsGroup, handCardsGroup2, tableCardsGroup; //Groups to keep cards as their children
    public bool gameStart;
    private int ranVal;     //Random value for choosing a card from the deck

    public HorizontalLayoutGroup tablelayout; //Used to reach the spacing of the table
    
    private int[,] cardInfo;        //Used in functions to reach a card from the deck
    
    //Variables for CardSpawner
    public GameObject gameObjectPrefab;     //Variables to get Prefab components
    private GameObject newCardObject;
    private Card prefabCard;
    private Image prefabCardImage;
    private TextMeshProUGUI prefabCardText;
    
    //Variables for GameRules
    private int playerScore, botScore;                //Score of the player and bot  
    private int wonCardsPlayer,wonCardsBot;     //Total number of gained card of the player and bot
    private int countkeeper;  //Keeps the number of cards on table
   
    
    [SerializeField] private Table _table;          //Table variable to reach list of the cards on table
    private List<GameObject> cardsonTable = new List<GameObject>();   //List which i equalize the list of the cards on table from the table script
    private GameObject cardobj1, cardobj2;           //The last played card and the card one before the last played card 
    private Card card1, card2, card3, card4;        //Keeps the components of the card objects used in GameRules
    private int card1Num, card2Num, cNum, cType;    //keeps the number and types of the cards

    //Variables for BotPlays
    private bool counter;
    private GameObject botPlayedCard;   //The chosen bot's card to play
    private int RanVal3;    //To keep the random value
    
    //Variables for HandSorter
    public bool handSorter;             //Used to call the HandSorter function
    private float timeElapsed = 0;      //keeps the past time
    private float lerpDuration;         //Animation duration
    public HorizontalLayoutGroup handlayout;    //To reach the spacing of the Player's hand
    
    
    void Start()
    {
        //First values for variables
        newCardObject = gameObjectPrefab;                       //equalizes a new object to the prefab
        prefabCard = newCardObject.GetComponent<Card>();        //Gets the prefab's components
        prefabCardImage = newCardObject.GetComponent<Image>();
        prefabCardText = newCardObject.GetComponentInChildren<TextMeshProUGUI>();
        
        playerScore = 0;
        botScore = 0;
        wonCardsPlayer = 0;
        wonCardsBot = 0;
        lerpDuration = 0.5f; 
        handlayout = handCardsGroup.GetComponent<HorizontalLayoutGroup>();
        tablelayout = tableCardsGroup.GetComponent<HorizontalLayoutGroup>();
        gameOverText = gameOver.GetComponentInChildren<TextMeshProUGUI>();
        
        CreateCards(cardsInfo);     //Creates the 52 card deck
        DrawCards(0);         //Draw cards for the player
        DrawCards(1);         //Draw cards for the bot
        FirstTableCards();          //Draw cards for the table

    }
    
    void Update()
    {
        TextScoreUpdate();  //Calls to update the scores
        if (handSorter)     //If the player gets four cards from the deck
        {
            HandSorter(-50, 5 - handCardsGroup.transform.childCount * 2);
        }
        
    }
    
    void CreateCards(List<int[,]> list)     //Creates the 52 card deck at start
    {
        for (int i = 0; i < 4; i++)     //Loop for the card types
        {
            for (int j = 1; j < 14; j++)    //Loop for the card numbers
            {
                int[,] cardInfo = {{i,j}};
                list.Add(cardInfo);             //Adding card number and type to the 2 dimensional card list
            }
        }
    }
    
    void CardSpawner(int[,] cardInfo, int cardHandler)          //Instantiates a gameobject from the card prefab
    {
        prefabCard.cardNumber = cardInfo[0,1];      //Determines the number of the Object
        prefabCard.cardType = cardInfo[0,0];        //Determines the type of the Object (club, diamond, heart or spade)
        
        if (cardHandler == 0)   //For the player's cards
        {
            prefabCardImage.sprite = ChooseSprite(cardInfo);        //arranges the image
            prefabCardText.text = ChooseText(cardInfo);             //arranges the text
            Instantiate(newCardObject, handCardsGroup.transform);   //Instantiates the new object as a child of Player's hand (handCardsGroup)

        }
        else if (cardHandler == 1)  //Fot the bot's cards
        { 
            prefabCardImage.sprite = null;      //arranges the image as white because it shouldnt be seen
            prefabCardText.text = null;         //arranges the text as null because it shouldnt be seen
            Instantiate(newCardObject, handCardsGroup2.transform);  //Instantiates the new object as a child of Bot's hand (handCardsGroup2)
        }
        else if(cardHandler == 2)   //For the table's cards but the cards are closed
        {
            prefabCardImage.sprite = null;      //arranges the image as white because it shouldnt be seen
            prefabCardText.text = null;         //arranges the text as null because it shouldnt be seen
            newCardObject.GetComponent<RectTransform>().anchoredPosition =
                tableCardsGroup.GetComponent<RectTransform>().anchoredPosition;     //Arranges the anchored position to place card on the middle
            Instantiate(newCardObject, tableCardsGroup.transform);                  //Instantiates the new object as a child of Table (tableCardsGroup)
        }
        else if(cardHandler == 3)   //For the table's cards but the cards are open
        {
            prefabCardImage.sprite = ChooseSprite(cardInfo);        //arranges the image
            prefabCardText.text = ChooseText(cardInfo);             //arranges the text
            Instantiate(newCardObject,tableCardsGroup.transform );  //Instantiates the new object as a child of Table (tableCardsGroup)
        } 
    }
    
    void DrawCards(int handler)     //Draws 4 cards from the deck for the player or bot
    {
        if (cardsInfo.Count != 0)   //Checks if the deck is out of cards
        {
            for (int i = 0; i < 4; i++)
            {
                cardInfo = ChooseCard(cardsInfo);   //Choose a random card from the deck
                CardSpawner(cardInfo,handler);      //Instantiate a card as a child of bot or player based on the handler
            }
        }
        if(handler == 0)
            handSorter = true;      //If the player gets the cards activate the cards lineup animation
    }

    void FirstTableCards()  //Draws 4 cards from the deck for the table at the beginning
    {
        for (int i = 0; i < 3; i++)
        {
            cardInfo = ChooseCard(cardsInfo);   //Chooses a random card from the deck
            CardSpawner(cardInfo,2);   //Instantiates a card as a child of table but the cards are closed
        }
        cardInfo = ChooseCard(cardsInfo);     //Chooses a random card from the deck
        CardSpawner(cardInfo,3);    //Instantiates a card as a child of table but the cards are closed
    }
    
    int[,] ChooseCard(List<int[,]> cardsInfo)   //Chooses a random card from the deck
    {
        ranVal = Random.Range(0, cardsInfo.Count-1);    //Chooses a random value from range 0 to number of cards on the deck
        cardInfo = cardsInfo[ranVal];                   //Equalizes the cardInfo to a card from the deck which is chosen randomly
        cardsInfo.Remove(cardInfo);                     //The chosen card gets removed from the deck
        return cardInfo; 
    } 

    Sprite ChooseSprite(int[,] cardInfo1)      //Based on the number and the type of the card, the sprite of the object's image is determined
    {
        switch (cardInfo1[0,0])                 //Checks the Card type
        {
            case 0: //If it is Club
                if (1 <= cardInfo1[0,1] && cardInfo1[0,1]<= 10) 
                    return sprites[0];          //If it is ace or it has a number from 2 to 10, the default club sprite is chosen 
                else if (cardInfo1[0,1] == 11)  
                    return sprites[4];          //If it is Jack(Vale or J), the jack club sprite is chosen 
                else if (cardInfo1[0,1] == 12)  
                    return sprites[8];          //If it is Queen(Q), the queen club sprite is chosen 
                else if (cardInfo1[0,1] == 13)  
                    return sprites[12];         //If it is King(K), the king club sprite is chosen 
                break;
            case 1: //Diamond   Same principles applies for the other cases
                if (1 <= cardInfo1[0,1]&& cardInfo1[0,1] <= 10)
                    return sprites[1];
                else if (cardInfo1[0,1] == 11)  //J JACK
                    return sprites[5];
                else if (cardInfo1[0,1] == 12)  //Q QUEEN
                    return sprites[9];
                else if (cardInfo1[0,1] == 13)  //K KING
                    return sprites[13];
                break;
            case 2: //Heart
                if (1 <= cardInfo1[0,1] && cardInfo1[0,1] <= 10)
                    return sprites[2];
                else if (cardInfo1[0,1] == 11)  //J JACK
                    return sprites[6];
                else if (cardInfo1[0,1] == 12)  //Q QUEEN
                    return sprites[10];
                else if (cardInfo1[0,1] == 13)  //K KING
                    return sprites[14];
                break;
            case 3: //Spade
                if (1 <= cardInfo1[0,1] && cardInfo1[0,1] <= 10)
                    return sprites[3];
                else if (cardInfo1[0,1] == 11)  //J JACK
                    return sprites[7];
                else if (cardInfo1[0,1] == 12)  //Q QUEEN
                    return sprites[11];
                else if (cardInfo1[0,1] == 13)  //K KING
                    return sprites[15];
                break;
        }
        return null;
    }

    String ChooseText(int[,] cardInfo1)
    {
        if (2 <= cardInfo1[0,1] && cardInfo1[0,1] <= 10)    //If the card has a number it returns that number as a text
            return cardInfo1[0,1].ToString();
        else if (cardInfo1[0,1] == 1)                       //If the card is ace it returns 'A' as a text
            return "A";
        return null;                                        //Other sprites already have the letters on it so in other conditions, the text is null
    }
    
    public void GameRules(bool playerTurn)      //When the player or the bot playes it gets triggered and checks for the points earning condition
    {

        cardsonTable = _table.cardsonTable;                     //Equalize the List from the table to the list from this script
        countkeeper = cardsonTable.Count;                       //Number of cards on table
        cardobj1 = cardsonTable[countkeeper - 1];        //Last played card object 
        cardobj2 = cardsonTable[countkeeper - 2];        //Card object which is one before the last played card object
        card1 = cardobj1.GetComponent<Card>();
        card2 = cardobj2.GetComponent<Card>();
        card1Num = card1.cardNumber;
        card2Num = card2.cardNumber;
 
        if (playerTurn)     //The Player played
        {
            if (countkeeper >= 3)      //If there are more than 3 cards in the table
            {
                if (card1Num == card2Num || card1Num == 11)     //Checks if the last two cards have the same number or the last played card is Jack(Vale)
                {
                    for (int i = 0; i < countkeeper; i++)       //Tours the every card on the table and calculate the score
                    {
                        card3 = cardsonTable[i].GetComponent<Card>();
                        cNum = card3.cardNumber;
                        cType = card3.cardType;
                        if (cNum == 1 || cNum == 11)        //If one of the cards on table is Ace (As veya A) player gains 1 point 
                            playerScore += 1;
                        else if (cNum == 2 && cType == 0)   //If one of the cards on table is Club 2 (Sinek 2li) player gains 2 point 
                            playerScore += 2;
                        else if (cNum == 10 && cType == 1)  //If one of the cards on table is Diamond 10 (Karo 10'lu) player gains 3 point 
                            playerScore += 3;
                        
                        Destroy(tableCardsGroup.transform.GetChild(i).gameObject);      //Destroys the cards on table
                        wonCardsPlayer++;       //The number of gained cards increases one
                    }
                    
                    TableSpacingOrganizer();        //Organize the table spacing
                    _table.cardsonTable.Clear();    //The list of cards on table gets cleared
                    
                }
            }
            else if(countkeeper == 2)       //If there are 2 cards on the table, the pisti scneario is called
            {
                if (card1Num == card2Num)  //If the two cards have the same number
                {
                    playerScore += 10;
                    for (int i = 0; i < countkeeper; i++)
                    {
                        Destroy(tableCardsGroup.transform.GetChild(i).gameObject);  
                        wonCardsPlayer++;           //The number of gained cards increases one
                    }
                    handSorter = true;              //HandSorter animation is activated             
                    TableSpacingOrganizer();        //Organize the table spacing
                    _table.cardsonTable.Clear();    //The list of cards on table gets cleared
                }
                else if (card1Num == 11)    //If the last played card is Jack(Vale)
                {
                    playerScore += 1;
                    for (int i = 0; i < countkeeper; i++)
                    {
                        Destroy(tableCardsGroup.transform.GetChild(i).gameObject);  ////Destroys the cards on table
                        wonCardsPlayer++;                                           //The number of gained cards increases one
                    }
                    handSorter = true;              //HandSorter animation is activated             
                    TableSpacingOrganizer();        //Organize the table spacing
                    _table.cardsonTable.Clear();    //The list of cards on table gets cleared
                }
            }
        }
        else if(!playerTurn)    //If the bot played, same events applied for bot
        {
            if (countkeeper >= 3)
            {
                Debug.Log(card1Num + " " + card2Num);
                if (card1Num == card2Num || card1Num == 11)
                {
                    for (int i = 0; i < countkeeper; i++)
                    {
                        card3 = cardsonTable[i].GetComponent<Card>();
                        
                        cNum = card3.cardNumber;
                        cType = card3.cardType;
                        wonCardsBot++;
                        if (cNum == 1 || cNum == 11)
                            botScore += 1;
                        else if (cNum == 2 && cType == 0)
                            botScore += 2;
                        else if (cNum == 10 && cType == 1)
                            botScore += 3;
                        Destroy(tableCardsGroup.transform.GetChild(i).gameObject);
                    }
                    TableSpacingOrganizer();
                    _table.cardsonTable.Clear();    
                }
            }
            else if(countkeeper == 2)
            {
                if (card1Num == card2Num)
                {
                    botScore += 10;
                    for (int i = 0; i < countkeeper; i++)
                    {
                        Destroy(tableCardsGroup.transform.GetChild(i).gameObject);
                        wonCardsBot++;
                    }
                    TableSpacingOrganizer();        
                    _table.cardsonTable.Clear();   
                }
                else if (card1Num == 11)
                {
                    botScore += 1;
                    for (int i = 0; i < countkeeper; i++)
                    {
                        Destroy(tableCardsGroup.transform.GetChild(i).gameObject);
                        wonCardsBot++;
                    }
                    TableSpacingOrganizer();        
                    _table.cardsonTable.Clear();    
                }
            }
        }
    }
    
    public void CheckforEmptyHand() //Checks for if the player or bot is out of cards
    {
        if (handCardsGroup.transform.childCount == 0)       //If player has 0 cards
            DrawCards(0);                             //Draw 4 Cards from the deck
        else if(handCardsGroup2.transform.childCount == 0)  //If bot has 0 cards
            DrawCards(1);                             //Draw 4 Cards from the deck for the bot
    }
    
    public void BotPlays()      //When it's bot's turn, this function gets called and make the bot play
    {
        counter = true;
        for (int i = 0; i < handCardsGroup2.transform.childCount; i++)      //Checks all the cards of bot's hand
        {
            if (counter && _table.cardsonTable.Count > 0)              //If the number of the cards on table is bigger than 0
            {
                if (_table.cardsonTable.Last().GetComponent<Card>().cardNumber ==
                    handCardsGroup2.transform.GetChild(i).gameObject.GetComponent<Card>().cardNumber ||
                    handCardsGroup2.transform.GetChild(i).gameObject.GetComponent<Card>().cardNumber == 11)     //Checks if the bot has the card which has the same number with the last card on the table or checks if the bot has the card which is jack(J)
                {
                    botPlayedCard =  handCardsGroup2.transform.GetChild(i).gameObject;       
                    botPlayedCard.transform.SetParent(tableCardsGroup.transform);       //Chosen card gets into the group of cards on table
                    
                    int[,] botPlayedCardInfo = { {botPlayedCard.GetComponent<Card>().cardType,botPlayedCard.GetComponent<Card>().cardNumber } };
                    botPlayedCard.GetComponent<Image>().sprite = ChooseSprite(botPlayedCardInfo);                   //Determines the sprite of the played card
                    botPlayedCard.GetComponentInChildren<TextMeshProUGUI>().text = ChooseText(botPlayedCardInfo);   //Determines the text of the played card
                    counter = false;
                }
            }
        }
        if(counter)     //If the bot doesn't have a card to win a point, a random card gets chosen and played
        {
            RanVal3 = Random.Range(0, handCardsGroup2.transform.childCount);            //Chooses a random value between 0 and the number of cards in bot's hand
            botPlayedCard =  handCardsGroup2.transform.GetChild(RanVal3).gameObject;    //Determines the card object which has the random index
            botPlayedCard.transform.SetParent(tableCardsGroup.transform);               //The chosen card gets into the group of cards on table
            
            int[,] botPlayedCardInfo = { {botPlayedCard.GetComponent<Card>().cardType,botPlayedCard.GetComponent<Card>().cardNumber } };
            botPlayedCard.GetComponent<Image>().sprite = ChooseSprite(botPlayedCardInfo);                   //Determines the sprite of the played card
            botPlayedCard.GetComponentInChildren<TextMeshProUGUI>().text = ChooseText(botPlayedCardInfo);   //Determines the text of the played card
        }
 
        TableSpacingOrganizer();  //Organizes the spacing of the cards on table
    }
    
    void TextScoreUpdate()  //Updates the Players' score texts on the screen
    {
        playerScoreText.text = "Oyuncu | " + playerScore.ToString();
        botScoreText.text = "Bilgisayar | " + botScore.ToString();
    }
    
    public void TableSpacingOrganizer()     //Organizes the spacing of the table
    {
        tablelayout.spacing  = -30 - tableCardsGroup.transform.childCount * 0.7f;
    }
    
    public void HandSorter(float startValue, float endValue)
    {
        if (timeElapsed < lerpDuration)     //Checks if the animation time is reached
        {
            handlayout.spacing = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);      //makes the card spacing change during time between the start and end values
            timeElapsed += Time.deltaTime;  //elapsed time measured
        }
        else
        {
            handlayout.spacing = endValue;
            timeElapsed = 0;
            handSorter = false;
        }
    }
    
    public void CheckforGameOver()  //Checks for the game over
    {
        if (cardsInfo.Count == 0 && handCardsGroup.transform.childCount == 0 && handCardsGroup2.transform.childCount == 0)  //if there is no card in the players' hands and in the deck, game is over
        {
            if (wonCardsPlayer > wonCardsBot)
                playerScore += 3;
            else if (wonCardsPlayer < wonCardsBot)
                botScore += 3;
            
            gameOver.SetActive(true);
            if (playerScore > botScore)
                gameOverText.text = "Oyun Bitti! Kazandiniz!";
            else if(playerScore < botScore)
                gameOverText.text = "Oyun Bitti! Kaybettiniz!";
            else
                gameOverText.text = "Oyun Bitti! Berabere!";
            
        }
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
