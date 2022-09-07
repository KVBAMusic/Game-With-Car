using System;
using UnityEngine;
using PathCreation;

public class CarBrain : MonoBehaviour
{
    GlobalInputs gInputs;

    // references in unity
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private BetterCarAIController _aiController;
    [SerializeField] private CarTimer _carTimer;
    [SerializeField] private PositionTracker _posTracker;
    [SerializeField] private CarLapTracker _lapTracker;
    [SerializeField] private ActionGateTrigger _gateTrigger;
    [SerializeField] private CarUIController _uiController;
    [SerializeField] private SpeedIndicator _speedIndicator;
    [SerializeField] private AudioSource _engineSound;

    // references in code
    public Rigidbody CRigidbody => _rigidbody;
    public PlayerMovement CMovement => _playerMovement;
    public BetterCarAIController CAIController => _aiController;
    public CarTimer CTimer => _carTimer;
    public PositionTracker CPosition => _posTracker;
    public CarLapTracker CLapTracker => _lapTracker;
    public ActionGateTrigger CGateTrigger => _gateTrigger;
    public CarUIController CUIController => _uiController;
    public SpeedIndicator CSpeedIndicator => _speedIndicator;
    public AudioSource CEngineSound => _engineSound;

    // for pathfinding, position, and respawn
    private VertexPath firstPath;
    private VertexPath currentPath;
    public VertexPath Path => currentPath;

    // global properties for the car
    public bool IsAI;
    public Constants.GameMode gameMode => (Constants.GameMode)PlayerPrefs.GetInt("Game Mode");
    public int NumLaps => gameMode == Constants.GameMode.TimeAttack ? 1 : PlayerPrefs.GetInt("Laps");

    // events
    public event EventHandler OnRespawn;
    public event EventHandler OnReset;

    //called when spawning a car (see CarLoader class)
    public void Init(bool isAI, bool isInMenu, CarStats stats)
    {
        IsAI = isAI;
        _playerMovement.stats = stats;
        _playerMovement.controlable = false;
        _rigidbody.useGravity = isInMenu;
        _rigidbody.isKinematic = !isInMenu;
        _gateTrigger.enabled = !isInMenu;
        _engineSound.enabled = !isInMenu;
        _posTracker.enabled = !isInMenu;
        _carTimer.enabled = !isInMenu;
        _lapTracker.enabled = !isInMenu;
        _uiController.enabled = !(isInMenu || isAI);
        _aiController.enabled = !isInMenu && isAI;
        if (isAI)
        {
            gInputs = FindObjectOfType<GlobalInputs>();
            switch (gameMode)
            {
                default:
                    break;
                case Constants.GameMode.SingleRace:
                    gInputs.input.Player.Reset.performed += ctx => RespawnCar();
                    break;
                case Constants.GameMode.TimeAttack:
                    gInputs.input.Player.Reset.performed += ctx => ResetCar();
                    break;
            }
        }
    }

    public void ResetCar() 
    {
        ResetPath();
        OnReset?.Invoke(this, EventArgs.Empty);
    }

    public void RespawnCar() =>
        OnRespawn?.Invoke(this, EventArgs.Empty);

    void Start()
    {
        firstPath = GameObject.FindGameObjectWithTag("First Path").GetComponent<PathCreator>().path;
        ResetPath();
        _lapTracker.OnLapStarted += NextLap;
    }

    public void StartRace()
    {
        _playerMovement.controlable = true;
        _rigidbody.isKinematic = false;
        _lapTracker.StartRace();
    }

    void NextLap(object sender, EventArgs e)
    {
        ResetPath();
    }

    public void UpdatePath(VertexPath path)
    {
        currentPath = path;
    }

    public void ResetPath()
    {
        currentPath = firstPath;
    }

    void Update()
    {

    }
}