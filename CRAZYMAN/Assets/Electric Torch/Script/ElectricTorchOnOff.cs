using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ElectricTorchOnOff : MonoBehaviourPun, IPunObservable
{
    private PhotonView photonView;
    private Light _light;
    EmissionMaterialGlassTorchFadeOut _emissionMaterialFade;
    //BatteryPowerPickup _batteryPower;

    private MentalGauge mentalGauge;

    // 손전등 배터리 UI 슬라이더 변수 추가
    [SerializeField] private Slider _flashlightBatterySlider;

    public enum LightChoose
    {
        noBattery,
        withBattery
    }

    public LightChoose modoLightChoose;
    public bool _PowerPickUp = false;
    [SerializeField] float _maxIntensity = 2.5f; // 최대 밝기 (배터리 만충 값)
    [SerializeField] public float intensityLight = 2.5f;
    private bool _flashLightOn = true;
    public bool IsFlashLightOn { get { return _flashLightOn; } }
    [SerializeField] float _lightTime = 0.05f;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        //_batteryPower = FindObjectOfType<BatteryPowerPickup>();
        _light = GetComponent<Light>();

        // MentalGauge 스크립트 찾기 (플레이어 오브젝트에 붙어있다고 가정)
        mentalGauge = GetComponent<MentalGauge>();
        if (mentalGauge == null)
        {
            mentalGauge = FindObjectOfType<MentalGauge>();
            if (mentalGauge == null)
            {
                Debug.LogWarning("[ElectricTorchOnOff] Awake: MentalGauge script is not found in the scene or on this GameObject! Mental health drain may not work correctly.");
            }
        }

    }

    void Start()
    {
        _emissionMaterialFade = GetComponentInChildren<EmissionMaterialGlassTorchFadeOut>();
        if (_emissionMaterialFade == null)
        {
            Debug.LogError("Cannot find 'EmissionMaterialGlassTorchFadeOut' script");
        }

        StartCoroutine(InitializeUI());

        // 슬라이더가 연결되었다면 최대값 설정 및 초기값 업데이트
        if (_flashlightBatterySlider != null)
        {
            _flashlightBatterySlider.maxValue = _maxIntensity; // 슬라이더 최대값을 배터리 최대치로 설정
            _flashlightBatterySlider.value = intensityLight; // 현재 배터리 값으로 슬라이더 초기화
            Debug.Log($"[ElectricTorchOnOff] Initialized Flashlight Battery Slider. Max: {_maxIntensity}, Current: {intensityLight}");
        }
        else
        {
            Debug.LogWarning("[ElectricTorchOnOff] _flashlightBatterySlider is null in Start. Cannot initialize slider.");
        }

        _ApplyFlashLightState();
    }

    private IEnumerator InitializeUI()
    {
        yield return new WaitForSeconds(0.1f); // 또는 yield return null;

        GameObject ui = GameObject.Find("UIInGame(Clone)");

        if (ui != null)
        {
            Slider[] sliders = ui.GetComponentsInChildren<Slider>(true);
            foreach (var s in sliders)
            {
                if (s.name == "FlashLight_Slider")
                {
                    _flashlightBatterySlider = s;
                    break;
                }
            }
        }

        if (_flashlightBatterySlider == null)
            Debug.LogError("[FlashLightSystem] FlashLight_Slider 연결 실패!");
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            InputKey();
            UpdateLightLogic();
        }

        // 슬라이더가 null인 경우 다시 시도
        if (_flashlightBatterySlider == null)
        {
            TryFindBatterySlider();
        }

        // --- 손전등 배터리 슬라이더 값 업데이트 ---
        // 로컬 플레이어의 UI만 업데이트하면 되므로 IsMine 체크 안에 넣는 것이 효율적입니다.
        if (photonView.IsMine && _flashlightBatterySlider != null)
        {
            _flashlightBatterySlider.value = intensityLight; // 현재 배터리 값으로 슬라이더 업데이트
            _light.intensity = _flashLightOn ? intensityLight : 0f;

            Debug.Log($"[ElectricTorchOnOff] Slider Updated: {intensityLight}");
        }
    }
    private void TryFindBatterySlider()
    {
        GameObject uiInGameInstance = GameObject.Find("UIInGame"); // 또는 "UIInGame(Clone)"
        if (uiInGameInstance != null)
        {
            Slider[] sliders = uiInGameInstance.GetComponentsInChildren<Slider>(true); // 비활성 오브젝트 포함
            foreach (Slider s in sliders)
            {
                if (s.name == "FlashLight_Slider")
                {
                    _flashlightBatterySlider = s;
                    Debug.Log("[ElectricTorchOnOff] Flashlight Battery Slider 연결됨!");
                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("[ElectricTorchOnOff] UIInGame을 찾을 수 없음.");
        }
    }

    void UpdateLightLogic()
    {
        if (_flashLightOn && modoLightChoose == LightChoose.withBattery)
        {
            intensityLight -= Time.deltaTime * _lightTime;

            _flashlightBatterySlider.value = intensityLight;
            _light.intensity = intensityLight;
            if (intensityLight < 0)
            {
                intensityLight = 0;
                // 배터리 소모로 인해 손전등이 꺼지는 경우
                if (_flashLightOn) // 켜져있었는데 배터리 0으로 꺼지는 경우에만 RPC 호출
                {
                    _flashLightOn = false; // 상태 변경
                    // RPC를 호출하여 다른 클라이언트에게 상태 변경 알림
                    photonView.RPC("SetFlashLightState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
                    photonView.RPC("SetEmissionState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
                    Debug.Log("[ElectricTorchOnOff] Battery depleted. Flashlight turned off.");
                }
            }

            /*if (_PowerPickUp)
                intensityLight = _batteryPower.PowerIntensityLight;*/

            _emissionMaterialFade.TimeEmission(_lightTime);
        }
    }

    void _ApplyFlashLightState()
    {
        // 현재 배터리 잔량(_intensityLight)을 실제 손전등 밝기에 적용
        // 손전등이 꺼져있으면 밝기는 0
        _light.intensity = _flashLightOn ? intensityLight : 0.0f;

        // 손전등 상태에 따라 Emission 켜고 끄기
        if (_emissionMaterialFade != null)
        {
            if (_flashLightOn)
                _emissionMaterialFade.OnEmission();
            else
                _emissionMaterialFade.OffEmission();
        }
        Debug.Log($"[ElectricTorchOnOff] Applied Flashlight State: On={_flashLightOn}, Intensity={intensityLight}");

        if (photonView.IsMine && _flashlightBatterySlider != null)
        {
            _flashlightBatterySlider.value = intensityLight;
        }
        /*if (_flashLightOn)
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

            *//*if (_PowerPickUp)
                intensityLight = _batteryPower.PowerIntensityLight;*//*
        }*/
    }

    void InputKey()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // 현재 상태의 반대로 변경 시도
            bool nextFlashLightState = !_flashLightOn;

            // 끄는 건 언제나 가능
            if (!nextFlashLightState)
            {
                _flashLightOn = nextFlashLightState;
                // RPC를 호출하여 다른 클라이언트에게 상태 변경 알림
                photonView.RPC("SetFlashLightState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
                photonView.RPC("SetEmissionState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
                Debug.Log("[ElectricTorchOnOff] Flashlight turned off by input.");
            }
            // 켜는 경우, 배터리가 0보다 커야 함
            else // nextFlashLightState == true (켜는 시도)
            {
                if (intensityLight > 0)
                {
                    _flashLightOn = nextFlashLightState;
                    // RPC를 호출하여 다른 클라이언트에게 상태 변경 알림
                    photonView.RPC("SetFlashLightState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
                    photonView.RPC("SetEmissionState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
                    Debug.Log("[ElectricTorchOnOff] Flashlight turned on by input.");
                }
                else
                {
                    Debug.Log("[ElectricTorchOnOff] Cannot turn on flashlight. Battery is empty.");
                }
            }

            /*_flashLightOn = !_flashLightOn;
            photonView.RPC("SetFlashLightState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);
            photonView.RPC("SetEmissionState", RpcTarget.AllBuffered, _flashLightOn, intensityLight);*/
        }
    }

    public void AddBattery(float amount)
    {
        if (!photonView.IsMine) return; // 로컬 플레이어만 배터리 충전 가능

        intensityLight += amount;
        // 최대 밝기(_maxIntensity)를 넘지 않도록 제한
        intensityLight = Mathf.Clamp(intensityLight, 0, _maxIntensity);

        Debug.Log($"[ElectricTorchOnOff] Battery charged by {amount}. Current battery: {intensityLight}");

        // 다른 클라이언트에게 변경된 배터리 잔량 동기화
        photonView.RPC("SyncBatteryLevel", RpcTarget.AllBuffered, intensityLight);

        // 배터리가 충전되어서 0보다 커졌다면, 꺼져있던 손전등을 다시 켤 수 있게 됨
        // (자동으로 켜지는 건 아니고, 우클릭으로 다시 켤 수 있게 되는 것)
    }

    // 배터리 잔량 동기화를 위한 RPC
    [PunRPC]
    public void SyncBatteryLevel(float newIntensity)
    {
        intensityLight = newIntensity;
        // RPC 수신 후 상태 적용 함수 호출!
        _ApplyFlashLightState();
        Debug.Log($"[ElectricTorchOnOff] Synced Battery Level: {intensityLight}");
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