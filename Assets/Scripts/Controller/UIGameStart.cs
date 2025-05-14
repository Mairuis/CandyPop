using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameStart : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene("GameScene"); });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
