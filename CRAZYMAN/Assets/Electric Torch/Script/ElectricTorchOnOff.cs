using Photon.Pun;
using UnityEngine;

public class ElectricTorchOnOff : MonoBehaviourPun, IPunObservable
{
    private PhotonView photonView;
    private Light _light;
    EmissionMaterialGlassTorchFadeOut _emissionMaterialFade;
    BatteryPowerPickup _batteryPower;

    public enum LightChoose
    {
        noBattery,
        withBattery
    }

    public LightChoose modoLightChoose;
    public bool _PowerPickUp = false;
    public float intensityLight = 2.5f;
    private bool _flashLightOn = true;
    [SerializeField] float _lightTime = 0.05f;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        _batteryPower = FindObjectOfType<BatteryPowerPickup>();
        _light = GetComponent<Light>();
    }

    void Start()
    {
        _emissionMaterialFade = GetComponentInChildren<EmissionMaterialGlassTorchFadeOut>();
        if (_emissionMaterialFade == null)
        {
            Debug.LogError("Cannot find 'EmissionMaterialGlassTorchFadeOut' script");
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            InputKey();
            UpdateLightLogic();
        }

        _ApplyFlashLightState();
    }

    void UpdateLightLogic()
    {
        if (_flashLightOn && modoLightChoose == LightChoose.withBattery)
        {
            intensityLight -= Time.deltaTime * _lightTime;
            if (intensityLight < 0) intensityLight = 0;

            if (_PowerPickUp)
                intensityLight = _batteryPower.PowerIntensityLight;

            _emissionMaterialFade.TimeEmission(_lightTime);
        }
    }

    void _ApplyFlashLightState()
    {
        if (_flashLightOn)
        {
            _light.intensity = intensityLight;
            if (_emissionMaterialFade != null)
                _emissionMaterialFade.OnEmission();
        }
        else
        {
            _light.intensity = 0.0f;
            if (_emissionMaterialFade != null)
                _emissionMaterialFade.OffEmission();

            if (_PowerPickUp)
                intensityLight = _batteryPower.PowerIntensityLight;
        }
    }

    void InputKey()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _flashLightOn = !_flashLightOn;
            photonView.RPC("SetFlashLightState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
            photonView.RPC("SetEmissionState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
        }
    }

    [PunRPC]
    public void SetEmissionState(bool state, float intensity)
    {
        if (_emissionMaterialFade != null)
        {
            if (state)
                _emissionMaterialFade.OnEmission();
            else
                _emissionMaterialFade.OffEmission();
        }
    }

    [PunRPC]
    public void SetFlashLightState(bool state, float intensity)
    {
        _flashLightOn = state;
        intensityLight = intensity;
        _ApplyFlashLightState();
    }

    // Photon 상태 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어가 상태를 전송
            stream.SendNext(_flashLightOn);
            stream.SendNext(intensityLight);
        }
        else
        {
            // 원격 플레이어가 상태를 수신
            _flashLightOn = (bool)stream.ReceiveNext();
            intensityLight = (float)stream.ReceiveNext();
            _ApplyFlashLightState();
        }
    }
}