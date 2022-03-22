using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#pragma warning disable 1998

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] Image targetImage;
    [SerializeField] Animator animator;
    [SerializeField] GameObject enemy;
    [SerializeField] Text timeText;
    [SerializeField] Text startText;
    [SerializeField] Text leafretText;
    [SerializeField] Text over, clear;
    [SerializeField] Text result;
    [SerializeField] Button quit, retry;
    [SerializeField] GameObject resporn;
    [SerializeField] GameObject pauseMenu;

    float time;
    [System.NonSerialized] public bool isPause;
    int count;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        targetImage.gameObject.SetActive(false);
        startText.gameObject.SetActive(true);
        leafretText.gameObject.SetActive(true);
        over.gameObject.SetActive(false);
        clear.gameObject.SetActive(false);
        result.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
        retry.gameObject.SetActive(false);
        pauseMenu.SetActive(false);
        isPause = false;
        count = 0;
        time = 0;
        CreateEnemy();

        StartCoroutine(Init());
    }

    void Update()
    {
        // 照準
        if(animator.GetBool("IsDrawAim") && !animator.GetBool("IsDead")) targetImage.gameObject.SetActive(true);
        else targetImage.gameObject.SetActive(false);

        // 一時停止
        if (Input.GetKey(KeyCode.P))
        {
            GameManager.instance.Pause();
            isPause = true;
        }
        if (Input.GetKey(KeyCode.R))
        {
            GameManager.instance.Resume();
            isPause = false;
        }

        // 時間表示
        time += Time.deltaTime;
        timeText.text = time.ToString("N2");
    }

    // 敵を生成
    public async void CreateEnemy()
    {
        count++;
        switch(count)
        {
            case 1:
                Instantiate(enemy, resporn.transform.position, Quaternion.identity);
            break;
            case 2:
                Instantiate(enemy, resporn.transform.position, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 3, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 3, Quaternion.identity);
            break;
            case 5:
                Instantiate(enemy, resporn.transform.position, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 3, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 5, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 3, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 5, Quaternion.identity);
            break;
            case 10:
                Instantiate(enemy, resporn.transform.position, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 8, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 6, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 4, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position + Vector3.right * 2, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 2, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 4, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 6, Quaternion.identity);
                Instantiate(enemy, resporn.transform.position - Vector3.right * 8, Quaternion.identity);
            break;
            case 19:
                Crear();
            break;
        }
    }

    // ゲーム開始
    IEnumerator Init()
    {
        yield return new WaitForSeconds(3f);

        startText.gameObject.SetActive(false);
    }

    // ゲームクリア
    void Crear()
    {
        clear.gameObject.SetActive(true);
        result.text = "Time : " + timeText.text;
        result.gameObject.SetActive(true);
        quit.gameObject.SetActive(true);
        retry.gameObject.SetActive(true);
        timeText.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    // ゲームオーバー
    public void GameOver()
    {
        over.gameObject.SetActive(true);
        quit.gameObject.SetActive(true);
        retry.gameObject.SetActive(true);
        timeText.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    // ポーズ
    public void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        leafretText.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    // 再開
    public void Resume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        leafretText.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 終了
    public void OnClickQuit()
    {
        Application.Quit();
    }

    // リトライ
    public void OnClickRetry()
    {
        Scene loadScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadScene.name);
    }
}
