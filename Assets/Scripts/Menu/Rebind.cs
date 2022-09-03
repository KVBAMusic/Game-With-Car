using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebind : MonoBehaviour
{
    GlobalInputs inputs;
    enum Action { Vertical, Horizontal, Reset, Drift, AddCheckpoint, RemoveCheckpoint, Camera };
    [SerializeField] Action action;
    [SerializeField] int binding;
    [SerializeField] Button button;
    [SerializeField] Text text;
    public InputAction[] actions;
    private void Start()
    {
        inputs = FindObjectOfType<GlobalInputs>();
        actions = new InputAction[7]
        {
            inputs.input.Player.MoveForwBack,
            inputs.input.Player.MoveLeftRight,
            inputs.input.Player.Reset,
            inputs.input.Player.Drift,
            inputs.input.Player.CheckpointAdd,
            inputs.input.Player.CheckpointRemove,
            inputs.input.Player.ChangeCamera,
        };
        SetText();
    }

    void SetText()
    {
        text.text = InputControlPath.ToHumanReadableString(actions[(uint)action].bindings[binding].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }


    public void RebindAction()
    {
        button.enabled = false;
        actions[(uint)action].Disable();
        var rebindOperation = actions[(uint)action].PerformInteractiveRebinding()
            .WithControlsExcluding("<Keyboard>/escape")
            .WithControlsExcluding("Mouse")
            .WithTargetBinding(binding)
            .OnMatchWaitForAnother(.1f)
            .OnComplete(operation => OnRebind(operation))
            .Start();
    }

    void OnRebind(InputActionRebindingExtensions.RebindingOperation operation)
    {
        SetText();
        button.enabled = true;
        operation.Dispose();
        inputs.SaveBindingOverrides();
        actions[(uint)action].Enable();
    }

}
