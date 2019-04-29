using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private GameObject curtainForScenesTransition;
    private string nextScene;
    private static readonly int ChangeScene = Animator.StringToHash("changeScene");
    private static float timeOfAnimation = 1f;

    private void Start()
    {
        Invoke("ternOffCurtain", timeOfAnimation);
    }

    public void GoToScene(string nameOfScene)
    {
        nextScene = nameOfScene;
        curtainForScenesTransition.SetActive(true);
        curtainForScenesTransition.GetComponent<Animator>().SetTrigger(ChangeScene);
        Invoke("onTransitionComplete", timeOfAnimation);
    }

    // ReSharper disable once UnusedMember.Global
    public void onTransitionComplete()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void ternOffCurtain()
    {
        curtainForScenesTransition.SetActive(false);
    }
}