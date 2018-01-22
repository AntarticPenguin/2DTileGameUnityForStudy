using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameScene : MonoBehaviour
{
    public MainGameUI GameUI;
    public TileMap _tileMap;


    // Use this for initialization
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        MessageSystem.Instance.ProcessMessage();
    }


    void Init()
    {
        _tileMap.Init();
        GameManager.Instance.SetMap(_tileMap);

        Character player = CreateCharacter("Player", "character01");
        Character monster = CreateCharacter("Monster", "character02");
        player.BecomeViewer();
    }

    Character CreateCharacter(string fileName, string resourceName)
    {
        string filePath = "Prefabs/CharacterFrame/Character";
        GameObject characterPrefabs = Resources.Load<GameObject>(filePath);

        GameObject characterGameObject = GameObject.Instantiate(characterPrefabs);
        characterGameObject.transform.SetParent(_tileMap.transform);
        characterGameObject.transform.localPosition = Vector3.zero;

        Character character = null;
        switch(fileName)
        {
            case "Player":
                character = characterGameObject.AddComponent<Player>();
                break;
            case "Monster":
                character = characterGameObject.AddComponent<Monster>();
                break;

        }
        character.Init(resourceName);

        Slider hpGuage = GameUI.CreateHPSlider();
        character.LinkHPGuage(hpGuage);

        Slider cooltimeGuage = GameUI.CrateCooltimeSlider();
        character.LinkCooltimeGuage(cooltimeGuage);

        return character;
    }
}
