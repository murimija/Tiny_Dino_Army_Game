using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    private void Awake()
    {
        if (instance != null)
            return;

        instance = this;
    }

    [SerializeField] private GameObject curtainForScenesTransition;
    private string nextScene;
    private static readonly int ChangeScene = Animator.StringToHash("changeScene");
    private const float timeOfAnimation = 1f;

    private void Start()
    {
        Invoke(nameof(ternOffCurtain), timeOfAnimation);
    }

    public void GoToScene(string nameOfScene)
    {
        nextScene = nameOfScene;
        curtainForScenesTransition.SetActive(true);
        curtainForScenesTransition.GetComponent<Animator>().SetTrigger(ChangeScene);
        Invoke(nameof(onTransitionComplete), timeOfAnimation);
    }

    // ReSharper disable once UnusedMember.Global
    private void onTransitionComplete()
    {
        SceneManager.LoadScene(nextScene);
    }

    private void ternOffCurtain()
    {
        curtainForScenesTransition.SetActive(false);
    }
}