using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[SerializeField]
public class playerStats
{
    public int saveSlots = 3;
    //public Button[] UI_saveSlots;
    float[] Save_money = {0, 0, 0};
    int[] Save_Level = {0, 0, 0};
}

public class MenuSystem : MonoBehaviour {

    [SerializeField]
    public bool testExperiment = false; //Will it save infomation during runtime?
    public Scene scenes;
    public int Checktest = 3;
    int num_loadGame = 0;
    
    void Start()
    { /*GetComponent<MenuSystem>().GetComponent<playerStats>().UI_saveSlots;*/ }

    public void newGame() //load up the first level for training
    {

        SceneManager.LoadScene(1);
    }

    public void resume()
    { }

    public void saveGame()
    { }

    public void loadGame()
    {    }

    public void leaderboards()
    {    }

    public void options()
    {    }

    public void credits()
    {    }

    public void quit()
    {    }
}
