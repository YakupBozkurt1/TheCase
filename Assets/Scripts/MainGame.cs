using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainGame : MonoBehaviour
{
    public List<int[,]> cardsInfo = new List<int[,]>();
    
    [SerializeField] private List<Sprite> sprites;
    public GameObject gameObjectPrefab;
    private Card prefabCard;
    private Image prefabCardImage;
    private TextMeshProUGUI prefabCardText;
    public GameObject handCardsGroup, handCardsGroup2, tableCardsGroup;
    public bool positionCnt = true;


    [SerializeField] private TextMeshProUGUI playerScoreText, botScoreText;
    
    private String message = "Oyun bitti! Kazanan: ";

    
    //GameRules için değişkenler
    [SerializeField] private Table _table;
    //private bool playerTurn;
    private int playerScore = 0;
    private int botScore = 0;
    public GameObject cardobj1, cardobj2;
    private Card card1, card2, card3, card4;
    private int card1Num, card2Num, cNum, cType;
    private List<GameObject> cardsonTable = new List<GameObject>();
    public bool playerWon;
    
    //BotPlays için
    private bool counter;

    public bool gameStart; 
    

    public HorizontalLayoutGroup handlayout;
    public void BotPlays()
    {
        counter = true;
        for (int i = 0; i < handCardsGroup2.transform.childCount; i++)
        {
            if (counter && _table.cardsonTable.Count >= 1)
            {
                if (_table.cardsonTable.Last().GetComponent<Card>().cardNumber ==
                    handCardsGroup2.transform.GetChild(i).gameObject.GetComponent<Card>().cardNumber ||
                    handCardsGroup2.transform.GetChild(i).gameObject.GetComponent<Card>().cardNumber == 11)
                {
                    GameObject botPlayedCard =  handCardsGroup2.transform.GetChild(i).gameObject;
                    botPlayedCard.transform.SetParent(tableCardsGroup.transform);
                    int[,] botPlayedCardInfo = { {botPlayedCard.GetComponent<Card>().cardType,botPlayedCard.GetComponent<Card>().cardNumber } };
                    botPlayedCard.GetComponent<Image>().sprite = ChooseSprite(botPlayedCardInfo);
                    botPlayedCard.GetComponentInChildren<TextMeshProUGUI>().text = ChooseText(botPlayedCardInfo);
                    counter = false;
                }
            }
        }
        if(counter)
        {
            int RanVal3 = Random.Range(0, handCardsGroup2.transform.childCount - 1); 
            GameObject botPlayedCard =  handCardsGroup2.transform.GetChild(RanVal3).gameObject;
            botPlayedCard.transform.SetParent(tableCardsGroup.transform);
            int[,] botPlayedCardInfo = { {botPlayedCard.GetComponent<Card>().cardType,botPlayedCard.GetComponent<Card>().cardNumber } };
            botPlayedCard.GetComponent<Image>().sprite = ChooseSprite(botPlayedCardInfo);
            botPlayedCard.GetComponentInChildren<TextMeshProUGUI>().text = ChooseText(botPlayedCardInfo);
        }
        if (cardsonTable.Count >= 2)
        {
            GameRules(false);
            //mainGame.CheckforGameOver();
            CheckforEmptyHand();
        }
        GroupOrganizer(tableCardsGroup);
        

    }

    public void CheckforGameOver()
    {
        if (cardsInfo.Count == 0 && _table.cardsonTable.Count == 0)
        {
            //GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200f, 200f), message);
            Debug.Log("Game Over");
        }
    }

    void TextScoreUpdate()
    {
        playerScoreText.text = "Oyuncu | " + playerScore.ToString();
        botScoreText.text = "Bilgisayar | " + botScore.ToString();
    }

    public void CheckforEmptyHand()
    {
        if (handCardsGroup.transform.childCount == 0)
            FirstHand(0);
        else if(handCardsGroup2.transform.childCount == 0)
            FirstHand(1);
    }

    public void GameRules(bool playerTurn)
    {
        Debug.Log("GameRulesCalled");
        cardsonTable = _table.cardsonTable;
        int countkeeper = cardsonTable.Count;
        cardobj1 = cardsonTable[cardsonTable.Count - 1];
        cardobj2 = cardsonTable[cardsonTable.Count - 2];
        card1 = cardobj1.GetComponent<Card>();
        card2 = cardobj2.GetComponent<Card>();
        card1Num = card1.cardNumber;
        card2Num = card2.cardNumber;
        Debug.Log(playerTurn);
        if (playerTurn)     //Oyuncu oynadıysa
        {
            if (countkeeper >= 3)
            {
                if (card1Num == card2Num || card1Num == 11)
                {
                    for (int i = 0; i < countkeeper; i++)
                    {
                        card3 = cardsonTable[i].GetComponent<Card>();
                        cNum = card3.cardNumber;
                        cType = card3.cardType;
                        card3.transform.SetParent(handCardsGroup.transform);
                        
                        int[,] keeper = {{cType, cNum}};
                        if (card3.GetComponent<Image>().sprite == null)
                        {
                            card3.GetComponent<Image>().sprite = ChooseSprite(keeper);
                            card3.GetComponentInChildren<TextMeshProUGUI>().text = ChooseText(keeper);
                        }
                        if (cNum == 1 || cNum == 11)
                            playerScore += 1;
                        else if (cNum == 2 && cType == 0)
                            playerScore += 2;
                        else if (cNum == 10 && cType == 1)
                            playerScore += 3;
                    }

                    playerWon = true;
                    //GroupOrganizer(handCardsGroup);
                    tableCardsGroup.GetComponent<HorizontalLayoutGroup>().spacing = -30; 
                    _table.cardsonTable.Clear();
                    
                }
            }
            else if(countkeeper == 2)
            {
                if (card1Num == card2Num)
                {
                    playerScore += 10;
                    for (int i = 0; i < countkeeper; i++)
                    {
                        card3 = cardsonTable[i].GetComponent<Card>();
                        cNum = card3.cardNumber;
                        cType = card3.cardType;
                        card3.transform.SetParent(handCardsGroup.transform);
                    }
                    playerWon = true;
                    //GroupOrganizer(handCardsGroup);
                    _table.cardsonTable.Clear();
                }
            }
        }
        else if(!playerTurn)    //Bot oynadıysa
        {
            Debug.Log("botagirdi");
            if (countkeeper >= 3)
            {
                Debug.Log(card1Num + " " + card2Num);
                if (card1Num == card2Num || card1Num == 11)
                {
                    Debug.Log("kazanmacond");
                    for (int i = 0; i < countkeeper; i++)
                    {
                        card3 = cardsonTable[i].GetComponent<Card>();
                        
                        cNum = card3.cardNumber;
                        cType = card3.cardType;
                        card3.transform.SetParent(handCardsGroup2.transform);
                        
                        int[,] keeper = {{cType, cNum}};
                        if (card3.GetComponent<Image>().sprite == null)
                        {
                            card3.GetComponent<Image>().sprite = ChooseSprite(keeper);
                            card3.GetComponentInChildren<TextMeshProUGUI>().text = ChooseText(keeper);
                        }
                        if (cNum == 1 || cNum == 11)
                            botScore += 1;
                        else if (cNum == 2 && cType == 0)
                            botScore += 2;
                        else if (cNum == 10 && cType == 1)
                            botScore += 3;
                    }
                    //GroupOrganizer(handCardsGroup2);
                    tableCardsGroup.GetComponent<HorizontalLayoutGroup>().spacing = -30; 
                    _table.cardsonTable.Clear();
                }
            }
            else if(countkeeper == 2)
            {
                if (card1Num == card2Num)
                {
                    Debug.Log("kazanmacondpisti");
                    botScore += 10;
                    for (int i = 0; i < countkeeper; i++)
                    {
                        card3 = cardsonTable[i].GetComponent<Card>();
                        cNum = card3.cardNumber;
                        cType = card3.cardType;
                        card3.transform.SetParent(handCardsGroup2.transform);
                    }
                    //GroupOrganizer(handCardsGroup2);
                    _table.cardsonTable.Clear();
                }
            }
        }
    }

    public void GroupOrganizer(GameObject cardsgroup)
    {
        if(cardsgroup.transform.tag == "Table")
            cardsgroup.GetComponent<HorizontalLayoutGroup>().spacing = -30 - cardsgroup.transform.childCount * 0.7f;
        else
        {
            
            if (cardsgroup.transform.childCount >= 15)
                 cardsgroup.GetComponent<HorizontalLayoutGroup>().spacing = 5 - cardsgroup.transform.childCount * 2;
             else
                 cardsgroup.GetComponent<HorizontalLayoutGroup>().spacing = 5 - cardsgroup.transform.childCount * 3;
        }
            

    }


    void CardSpawner(int[,] cardInfo, int cardHandler){
        GameObject newCardObject = gameObjectPrefab;
        prefabCard = newCardObject.GetComponent<Card>();
        prefabCardImage = newCardObject.GetComponent<Image>();
        prefabCardText = newCardObject.GetComponentInChildren<TextMeshProUGUI>();
        prefabCard.cardNumber = cardInfo[0,1];
        prefabCard.cardType = cardInfo[0,0];
        
        if (cardHandler == 0)
        {
            prefabCardImage.sprite = ChooseSprite(cardInfo);
            prefabCardText.text = ChooseText(cardInfo);
            Instantiate(newCardObject, handCardsGroup.transform);

        }
        else if (cardHandler == 1)
        {
            prefabCardImage.sprite = null;
            prefabCardText.text = null;
            Instantiate(newCardObject, handCardsGroup2.transform);
        }
        else if(cardHandler == 2)
        {
            prefabCardImage.sprite = null;
            prefabCardText.text = null;
            newCardObject.GetComponent<RectTransform>().anchoredPosition =
                tableCardsGroup.GetComponent<RectTransform>().anchoredPosition;
            Instantiate(newCardObject, tableCardsGroup.transform);
        }
        else if(cardHandler == 3)
        {
            prefabCardImage.sprite = ChooseSprite(cardInfo);
            prefabCardText.text = ChooseText(cardInfo);
            Instantiate(newCardObject,tableCardsGroup.transform );

        } 
    }
    
    void Start()
    {
        CreateCards(cardsInfo);
        FirstHand(0);
        FirstHand(1);
        FirstTableCards();
        handlayout = handCardsGroup.GetComponent<HorizontalLayoutGroup>();
        
    }

    private float timeElapsed = 0;
    private float lerpDuration = 2;
    public void HandSorter(float startValue, float endValue)
    {
        if (timeElapsed < lerpDuration)
        {
            handlayout.spacing = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
        }
        else
        {
            
            handlayout.spacing = endValue;
            timeElapsed = 0;
            playerWon = false;
            updateCard = false;
        }
    }

    public bool updateCard;
    void Update()
    {
        TextScoreUpdate();
        if (playerWon)
        {
            HandSorter(-50, 5 - handCardsGroup.transform.childCount * 2);
        }

        if (updateCard)
        {
            HandSorter(handlayout.spacing, 5 - handCardsGroup.transform.childCount * 2);
        }
        
    }
    
    void CreateCards(List<int[,]> list)
    {
        
        for (int i = 0; i < 4; i++)
        {
            for (int j = 1; j < 14; j++)
            {
                int[,] item = {{i,j}};
                list.Add(item);
            }
        }
    }
    void FirstHand(int handler)
    {
        int[,] cardInfo;
        for (int i = 0; i < 4; i++)
        {
            cardInfo = ChooseCard(cardsInfo);
            CardSpawner(cardInfo,handler);
        }
    }

    void FirstTableCards()
    {
        int[,] cardInfo;
        for (int i = 0; i < 3; i++)
        {
            cardInfo = ChooseCard(cardsInfo);
            CardSpawner(cardInfo,2);
        }
        cardInfo = ChooseCard(cardsInfo);
        CardSpawner(cardInfo,3);
    }
    
    int[,] ChooseCard(List<int[,]> cardsInfo)
    {
        int ranVal = Random.Range(0, cardsInfo.Count-1);
        
        int[,] cardInfo = cardsInfo[ranVal];
        cardsInfo.Remove(cardInfo);
        return cardInfo; 
    } 

    Sprite ChooseSprite(int[,] cardInfo)      //Kartın numarası ve tipine göre karta ait Sprite return edilir.
    {
        switch (cardInfo[0,0])
        {
            case 0: //Club
                if (1 <= cardInfo[0,1] && cardInfo[0,1]<= 10)
                    return sprites[0];
                else if (cardInfo[0,1] == 11)  //J VALE
                    return sprites[4];
                else if (cardInfo[0,1] == 12)  //Q QUEEN
                    return sprites[8];
                else if (cardInfo[0,1] == 13)  //K KING
                    return sprites[12];
                break;
            case 1: //Diamond
                if (1 <= cardInfo[0,1]&& cardInfo[0,1] <= 10)
                    return sprites[1];
                else if (cardInfo[0,1] == 11)  //J VALE
                    return sprites[5];
                else if (cardInfo[0,1] == 12)  //Q QUEEN
                    return sprites[9];
                else if (cardInfo[0,1] == 13)  //K KING
                    return sprites[13];
                break;
            case 2: //Heart
                if (1 <= cardInfo[0,1] && cardInfo[0,1] <= 10)
                    return sprites[2];
                else if (cardInfo[0,1] == 11)  //J VALE
                    return sprites[6];
                else if (cardInfo[0,1] == 12)  //Q QUEEN
                    return sprites[10];
                else if (cardInfo[0,1] == 13)  //K KING
                    return sprites[14];
                break;
            case 3: //Spade
                if (1 <= cardInfo[0,1] && cardInfo[0,1] <= 10)
                    return sprites[3];
                else if (cardInfo[0,1] == 11)  //J VALE
                    return sprites[7];
                else if (cardInfo[0,1] == 12)  //Q QUEEN
                    return sprites[11];
                else if (cardInfo[0,1] == 13)  //K KING
                    return sprites[15];
                break;
        }

        return null;
    }

    String ChooseText(int[,] cardInfo)
    {
        if (2 <= cardInfo[0,1] && cardInfo[0,1] <= 10)
            return cardInfo[0,1].ToString();
        else if (cardInfo[0,1] == 1)
            return "A";
        return null;
    }
}
