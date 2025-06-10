//  - EmissionMaterialGlassTorchFadeOut - Script by Marcelli Michele

// This script is attached to child light of the Electric Torch
// and takes the emission material in conjunction with the light

using Photon.Pun;
using UnityEngine;

public class EmissionMaterialGlassTorchFadeOut : MonoBehaviourPun, IPunObservable
{
    private Renderer _mat;
    private float _intensity = 0;

    Color _alphaStart;

    ElectricTorchOnOff _torchOnOff;

    private void Start()
    {
        _mat = GetComponent<Renderer>();
        _alphaStart = _mat.material.color;

        GameObject _torchLight = GameObject.Find("Torch Light");

        if (_torchLight != null) {_torchOnOff = _torchLight.GetComponent<ElectricTorchOnOff>();}
        if (_torchLight == null) {Debug.Log("Cannot find 'ElectricTorchOnOff' script");}
    }

    private void Update()
    {
        _intensity = _torchOnOff.intensityLight;
    }

    public void TimeEmission(float t)
    {
        if (photonView.IsMine)
        {
            _intensity -= t * Time.deltaTime;
            photonView.RPC("UpdateEmissionIntensity", RpcTarget.All, _intensity);
        }
    }

    [PunRPC]
    private void UpdateEmissionIntensity(float intensity)
    {
        _intensity = intensity;
        if (_intensity < 0)
        {
            _intensity = 0;
        }
        _mat.material.SetColor("_EmissionColor", _alphaStart * _intensity);
    }

    public void OffEmission()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetEmissionOff", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SetEmissionOff()
    {
        _mat.material.SetColor("_EmissionColor", _alphaStart * Color.black);
    }    

    public void OnEmission()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("SetEmissionOn", RpcTarget.All, _torchOnOff.intensityLight);
        }
    }

    [PunRPC]
    private void SetEmissionOn(float intensity)
    {
        _mat.material.SetColor("_EmissionColor", _alphaStart * intensity);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_intensity);
        }
        else
        {
            _intensity = (float)stream.ReceiveNext();
            _mat.material.SetColor("_EmissionColor", _alphaStart * _intensity);
        }
    }
}
