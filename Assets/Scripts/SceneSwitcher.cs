using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
	public void GoToMainScene()
	{
		SceneManager.LoadScene("MainScene");
	}
	public void GotoMenuScene()
	{
		SceneManager.LoadScene("MenuScene");
	}
}
