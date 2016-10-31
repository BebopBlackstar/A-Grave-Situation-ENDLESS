using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Xml.Serialization;
using System.IO;



//[SerializeField]
[System.Serializable()]
public class saves
{
    private int m_saveSlots = 3;
    public List<playerStats> saveslots;
    public saves()
    {
        saveslots = new List<playerStats>(m_saveSlots);
        saveslots.Add(new playerStats("Player1"));
        saveslots.Add(new playerStats("Player2"));
        saveslots.Add(new playerStats("Player3"));
    }
}
public class playerStats
{
    public string name;
    public int moneh;
    public int lvl;

    public playerStats()
    {

    }
    public playerStats(int moneyCurrent)
    {
        moneh = moneyCurrent;
    }
    public playerStats(string _name)
    {
        name = _name;
    }

    
}

public class MenuSystem : MonoBehaviour
{

    [SerializeField]
    public bool testExperiment = false; //Will it save infomation during runtime?
    public Scene scenes;
    public int Checktest = 3;
    int num_loadGame = 0;
    int levelCount = 0;
    saves stats;
    public int moneyCurrent = 2000;
    public string myName = "Default";

    void Start()
    {
        stats = new saves();
//       stats.saveslots[0] = new playerStats("super jeffry");
        stats.saveslots[0].moneh = moneyCurrent;
        //quit();
    }

    public void newGame() //load up the first level for training
    {
        stats.saveslots[0] = new playerStats(myName);
        SceneManager.LoadScene(1);
    }

    public void resume()
    { }

    public void saveGame()
    { }

    public void loadGame()
    { }

    //public void nextLevel(SerializationInfo info, StreamingContext ctxt)
    //{
    //    info.AddValue("GameSlot: ", (num_loadGame));
    //    //info.AddValue("foundGem1", (foundGem1));
    //    info.AddValue("SaveMoney: ", moneyCurrent);
    //    info.AddValue("CurrentLevel: ", levelCount);
    //}

    public void leaderboards()
    { }

    public void options()
    { }

    public void credits()
    { }

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
}
