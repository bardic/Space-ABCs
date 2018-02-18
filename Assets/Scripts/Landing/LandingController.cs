using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LandingController : MonoBehaviour
{
    public GameObject aboutUsPanel;

    public void OnStartClick()
    {
        SceneManager.LoadScene("Main");
    }
    
    public void OnGitHubLinkClick()
    {
        Application.OpenURL ("https://github.com/bardic/Space-ABCs");
    }

    public void OnAboutClick()
    {
        aboutUsPanel.SetActive(true);
    }

    public void OnAboutCloseClick()
    {
        aboutUsPanel.SetActive(false);
    }
}
