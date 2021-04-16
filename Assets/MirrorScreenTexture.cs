using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ruccho.GraphicsCapture;
using System.Linq;
using System;

public class MirrorScreenTexture : MonoBehaviour
{
    [SerializeField]
    GameObject _middleEyeScreen;
    [SerializeField]
    GameObject _leftEyeScreen;
    [SerializeField]
    GameObject _rightEyeScreen;

    private CaptureClient client = new CaptureClient();

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<ICaptureTarget> monitors = Utils.GetMonitors();
        client.SetTarget(monitors.First());
        Application.onBeforeRender += OnBeforeRender;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFromSource();
    }

    private void UpdateFromSource() {
        Texture2D captureTex = client.GetTexture();
        if (captureTex != null) {
            captureTex.filterMode = FilterMode.Point;
        }

        if (_middleEyeScreen.GetComponent<Renderer>().material.mainTexture != captureTex) {
            _middleEyeScreen.GetComponent<Renderer>().material.mainTexture = captureTex;
            _middleEyeScreen.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f, -1f);
        }

        if (_leftEyeScreen.GetComponent<Renderer>().material.mainTexture != captureTex) {
            _leftEyeScreen.GetComponent<Renderer>().material.mainTexture = captureTex;
            _leftEyeScreen.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, -1f);
        }

        if (_rightEyeScreen.GetComponent<Renderer>().material.mainTexture != captureTex) {
            _rightEyeScreen.GetComponent<Renderer>().material.mainTexture = captureTex;
            _rightEyeScreen.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, -1f);
            _rightEyeScreen.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.5f, 0f);
        }
    }

    public void SetTarget(ICaptureTarget target) {
        try {
            client.SetTarget(target);
        } catch (CreateCaptureException e) {
            Debug.LogWarning("Could not capture target. Error was: " + e.Message);
        }
    }

    public void OnBeforeRender() {
        UpdateFromSource();
    }

    private void OnDestroy() {
        client?.Dispose();
    }

    public void SetSBS3D(bool isEnabled) {
        _leftEyeScreen.SetActive(isEnabled);
        _rightEyeScreen.SetActive(isEnabled);
        _middleEyeScreen.SetActive(!isEnabled);
    }
}
