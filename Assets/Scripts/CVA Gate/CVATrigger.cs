using UnityEngine;
using UnityEngine.VFX;
using System;

public class CVATrigger : MonoBehaviour
{
    Rigidbody rb;
    PlayerMovement pm;
    Vector3 euler;
    bool active;
    Transform temp;
    AudioController ac;
    [SerializeField] VisualEffect effect;
    CameraTarget ct;

    GlobalInputs input;

    private void Awake()
    {
        input = FindObjectOfType<GlobalInputs>();
    }

    void Start()
    {
        ct = FindObjectOfType<CameraTarget>();
        rb = GetComponent<Rigidbody>();
        ac = FindObjectOfType<AudioController>();
        pm = GetComponent<PlayerMovement>();
        input.input.Player.Reset.performed += ctx => ResetCar();
    }

    private void FixedUpdate()
    {

        if (active)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, temp.rotation, Time.fixedDeltaTime * 2.5f);
            rb.velocity = 50 * temp.forward;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void BoostGateHit(Transform t, Vector3 e)
    {
        if (!pm.isAI) ac.PlaySound("CVA");
        effect.Play();
        euler = e + new Vector3(0, 90, 0);
        temp = t;
        temp.eulerAngles = euler;
        transform.position += transform.up * 1;
        active = true;
    }

    public void AGGateHit(bool activate, GameObject vfx)
    {
        pm.isOnAntigrav = activate;
        vfx.transform.position = transform.position;
        var burst = vfx.GetComponent<VisualEffect>();
        burst.Play();
        rb.velocity *= 1.1f;
        if (!pm.isAI) ac.PlaySound("AG");
    }

    public void TeleportGateHit(Transform dest)
    { 
        transform.position = dest.position;
        transform.rotation = dest.rotation;
        rb.velocity = 40 * dest.forward;
        if (!pm.isAI)
        {
            ct.gameObject.transform.rotation = dest.rotation;
            ct.targetRotation = dest.rotation;
            ac.PlaySound("Teleport");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IActionGate gate;
        try
        {
            gate = other.gameObject.GetComponent<IActionGate>();
        }
        catch (NullReferenceException)
        {
            gate = null;
        }
        if (gate != null)
        {
            gate.Action(gameObject);
        }
        else
        {
            if (other.gameObject.CompareTag("CVA Disable"))
            {
                active = false;
                effect.Stop();
            }
        }
    }

    public void ResetCar()
    {
        if (effect != null && active) effect.Stop();
        active = false;
    }
}
