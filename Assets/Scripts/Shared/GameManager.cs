using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game state and sets up the scene.
/// </summary>
public class GameManager : MonoBehaviour
{
  public static GameManager instance = null;
  [SerializeField] private bool _doingSetup = true;

  private LevelManager _levelManager;

  void Awake()
  {
    if (instance == null) instance = this;
    else if (instance != this) Destroy(gameObject);

    _levelManager = GetComponent<LevelManager>();
  }

  void Start()
  {
    InitGame();
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  /// <summary>
  /// Initializes the game by setting up the scene with the given level generator.
  /// </summary>
  void InitGame()
  {
    _levelManager.SetupScene();
  }

  /// <summary>
  /// Marks the end of the setup phase, set by the level generator.
  /// </summary>
  public void FinishSetup()
  {
    _doingSetup = false;
  }
}
