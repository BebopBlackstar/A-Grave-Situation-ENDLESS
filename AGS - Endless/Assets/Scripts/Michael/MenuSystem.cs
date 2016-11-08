using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;



//[SerializeField]
[System.Serializable()]
public class saves //Save files and games data.
{
    private int m_saveSlots = 3;
    public List<playerStats> saveslots;
    public saves()
    {
        saveslots = new List<playerStats>(m_saveSlots);
        saveslots.Add(new playerStats("Player1", 5, 2));
        saveslots.Add(new playerStats("Player2", 5, 2));
        saveslots.Add(new playerStats("Player3", 5, 2));
    }
}
public class playerStats
{
    public string name;
    public int money;
    public int lvl;

    public playerStats()
    {

    }
    public playerStats(int moneyCurrent, int level)
    {
        money = moneyCurrent;
        lvl = level;
    }
    public playerStats(string _name)
    {
        name = _name;
    }
    public playerStats(string _name, int moneyCurrent, int level)
    {
        name = _name;
        money = moneyCurrent;
        lvl = level;
    }


    
}

public class MenuSystem : MonoBehaviour
{

    [SerializeField]
    public bool testExperiment = false; //Will it save infomation during runtime?
    public Scene scenes;
    public int Checktest = 3;
    int num_CurrentSlot = 0;
    public int levelCount = 0;
    saves stats;
    public int moneyCurrent = 2000;
    public string myName = "Default";
    public bool isMainMenu = false;


    public List<GameObject> buttons_FirstSet = new List<GameObject>();
    public List<GameObject> buttons_SecondSet = new List<GameObject>();

    public List<string> firstSet = new List<string>();
    public string command = "";
    public bool comGo = false;

    bool enter = true;
    //bool exit = false;
    //public List<Button> new_Off = new List<Button>();
    //public List<Button> save_Off = new List<Button>();
    //public List<Button> load_Off = new List<Button>();
    //public List<Button> options_Off = new List<Button>();
    //public List<Button> credits_Off = new List<Button>();



    void Start()
    {
        stats = new saves();
//       stats.saveslots[0] = new playerStats("super jeffry");
        stats.saveslots[num_CurrentSlot].money = moneyCurrent;
        //quit();
        firstSet.Add("resume"); //0
        firstSet.Add("new"); //1
        firstSet.Add("save"); //2
        firstSet.Add("load"); //3
        firstSet.Add("options"); //4
        firstSet.Add("leaderboard"); //5
        firstSet.Add("credits"); //6

    }

   

    public void resume()
    { SceneManager.LoadScene(levelCount); }

    public void slots(int slotSelect)
    {
        Debug.Log("Slots active! ");
        //int i = 0;
        if (enter)
        {
            foreach (GameObject but in buttons_FirstSet)
            { but.SetActive(false); }

        }

        if(slotSelect != 0)
        {
            num_CurrentSlot = slotSelect - 1;
            //Time.timeScale = 1;
            if (command.Contains("new"))
            {
               
                stats.saveslots[slotSelect] = new playerStats(myName);
                levelCount = 5;
                //Time.timeScale = 1;
                Time.timeScale = 1;
                SceneManager.LoadScene(levelCount);

            }
            if (command.Contains("save"))
            {
                //                //stats.saveslots[num_CurrentSlot](myName, moneyCurrent, levelCount);
             
                System.Type type = typeof(saves);
                XmlSerializer serilizer = new XmlSerializer(type);
                StreamWriter writer = new StreamWriter("../ProfileInfomation");
                Debug.Log("Writing Information");
                serilizer.Serialize(writer, stats);
                writer.Close();
            }
            if (command.Contains("load"))
            {
               
                levelCount = stats.saveslots[num_CurrentSlot].lvl;
                moneyCurrent = stats.saveslots[num_CurrentSlot].money;
                myName = stats.saveslots[num_CurrentSlot].name;
                Time.timeScale = 1;
                SceneManager.LoadScene(levelCount);
            }
        }



    }


    public void leaderboards()
    {
        Debug.Log("Leaderboards called.");
        if (enter)
        {
            foreach (GameObject but in buttons_FirstSet)
            { but.SetActive(false); }
            buttons_SecondSet[1].gameObject.SetActive(true);
            buttons_SecondSet[1].SetActive(true);
            enter = false;
        }
    }

    public void options()
    {
        if (enter)
        {
            foreach (GameObject but in buttons_FirstSet)
            { but.SetActive(false); }
            buttons_SecondSet[2].gameObject.SetActive(true);
            buttons_SecondSet[2].SetActive(true);
            enter = false;
        }
    }

    public void credits()
    {
        if (enter)
        {
            foreach (GameObject but in buttons_FirstSet)
            { but.SetActive(false); }
            buttons_SecondSet[3].gameObject.SetActive(true);
            buttons_SecondSet[3].SetActive(true);
            enter = false;
        } //DEFAULT Load new buttons and unload old from first and second.
    }

    public void quit()
    {
        {//SAVE GAME
            System.Type type = typeof(saves);
            XmlSerializer serilizer = new XmlSerializer(type);
            StreamWriter writer = new StreamWriter("../ProfileInfomation");
            Debug.Log("Writing Information");
            serilizer.Serialize(writer, stats);
            writer.Close();
        }//SAVE GAME
         //SceneManager.UnloadScene(0);
        Debug.Log("Quit");
        //Application.Quit();
        //SceneManager.UnloadScene(levelCount+1);
        //Application.EditorApplication.isPlaying = false;
        Application.Quit();


    }
    public void ButtonActive(string name)
    {

        if(name != "")
        {
            int i = 0;
            foreach(string function in firstSet)
            {
                if(name == function)
                { command = function; }
                
                i++;
            }
            comGo = false;
        }
        //buttons_FirstSet
        //buttons_SecondSet
        


    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (enter == false)
            { command = ""; }
            else if(levelCount != 0)
            {
                SceneManager.LoadScene(levelCount);
                Time.timeScale = 1;
            }
        }

        if (command != "" && comGo != true)
        {
            Debug.Log("Check comGo!");
            int i = 0;
            foreach (string com in firstSet)
            {
                if (command == com)//Slots screen via (new/load/save)
                { Debug.Log("Check return; " + i); break; }
                i++;
            }
            Debug.Log("Do i get here? " + i);
            if (firstSet[i] == firstSet[1] || firstSet[i] == firstSet[2] || firstSet[i] == firstSet[3])
            { // (new/load/save)
                //if(firstSet is == firstSet[1])
                if (firstSet[i] == firstSet[1])
                { command = "slots/new"; }//"slots/new"
                if (firstSet[i] == firstSet[2])
                { command = "slots/save"; } //"slots/save"
                if (firstSet[i] == firstSet[3])
                { command = "slots/load"; } //"slots/load
            }
            if(command.Contains(firstSet[5]))
            { leaderboards(); }
            if(command.Contains(firstSet[4]))
            { options(); }
            if (command.Contains(firstSet[6]))
            { credits(); }

            //Initilize functions to load up a new screen or infomation to the game.
            comGo = true;
        }
        else if (buttons_FirstSet[3].activeInHierarchy != true && command == "")
        {

            foreach (GameObject but in buttons_FirstSet)
            { but.SetActive(true); }

            buttons_SecondSet[0].gameObject.SetActive(false);
            buttons_SecondSet[0].SetActive(false);

//<<<<<<< HEAD
            buttons_SecondSet[0].gameObject.SetActive(false);
//=======
            buttons_SecondSet[0].SetActive(false);
//>>>>>>> 11b62b4153a5a73ae465d1c3bcae2aa5dfa87580


            foreach(GameObject but in buttons_SecondSet)
            { but.SetActive(false); }
            //buttons_SecondSet[0].gameObject.SetActive(false);
            //buttons_SecondSet[0].SetActive(false);
            //buttons_SecondSet[]

            enter = true;
        }

        //int i = 0;
        //foreach (GameObject check in buttons_FirstSet)
        //{
        //    if (check.GetComponent<Button>.)
        //        i++;
        //}

        if (command.Contains("slots/"))
        {
            slots(0);
        }
        

    }
}







//////public void newGame() //load up the first level for training
//////   {
//////       stats.saveslots[num_CurrentSlot] = new playerStats(myName);
//////       levelCount = 5;
//////       SceneManager.LoadScene(levelCount);
//////   }
//////   public void saveGame()
//////   {
//////       // stats.saveslots[num_CurrentSlot] = new playerStats(lvl)
//////       stats.saveslots[num_CurrentSlot] = new playerStats(myName, moneyCurrent, levelCount);
//////       {//SAVE GAME
//////           System.Type type = typeof(saves);
//////           XmlSerializer serilizer = new XmlSerializer(type);
//////           StreamWriter writer = new StreamWriter("../ProfileInfomation");
//////           Debug.Log("Writing Information");
//////           serilizer.Serialize(writer, stats);
//////           writer.Close();
//////       }//SAVE GAME
//////   }

//////   public void loadGame()
//////   {
//////       //stats.saveslots[num_CurrentSlot](myName, moneyCurrent, levelCount);
//////       levelCount = stats.saveslots[num_CurrentSlot].lvl;
//////       moneyCurrent = stats.saveslots[num_CurrentSlot].money;
//////       myName = stats.saveslots[num_CurrentSlot].name;
//////       SceneManager.LoadScene(levelCount);

//////       //info.AddValue("GameSlot: ", (num_loadGame));
//////       //info.AddValue("foundGem1", (foundGem1));
//////       //info.AddValue("SaveMoney: ", moneyCurrent);
//////       //info.AddValue("CurrentLevel: ", levelCount);
//////   }

//public void nextLevel(SerializationInfo info, StreamingContext ctxt)
//{
//    info.AddValue("GameSlot: ", (num_loadGame));
//    info.AddValue("foundGem1", (foundGem1));
//    info.AddValue("SaveMoney: ", moneyCurrent);
//    info.AddValue("CurrentLevel: ", levelCount);
//}