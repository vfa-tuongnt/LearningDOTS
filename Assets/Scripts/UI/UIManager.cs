using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Entities;
using Unity.Scenes;
using Unity.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private Text playerNumberText;
    [SerializeField] private Button changeSceneBtn, increaseBtn, decreaseBtn, clearPlayers;
    public Action IncreaseAction, DecreaseAction, LoadToNormalScene;
    public Action ClearPlayersRequest;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        changeSceneBtn.onClick.AddListener(() =>
        {
            if(SceneManager.GetActiveScene().name == "NormalScene")
            {
                DefaultWorldInitialization.Initialize("EntityWorld", false); // Create world again
                SceneManager.LoadScene("SampleScene");
            }
            else
            {
                LoadToNormalScene?.Invoke();

                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            }
        });
        
        increaseBtn.onClick.AddListener(() => IncreaseAction?.Invoke());
        decreaseBtn.onClick.AddListener(() => DecreaseAction?.Invoke());
        clearPlayers.onClick.AddListener(() => ClearPlayersRequest?.Invoke());
    }

    public void UpdateUI(int playerNumber)
    {
        playerNumberText.text = playerNumber.ToString();
    }
}
