using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapboxPlane : MonoBehaviour
{
    // �ν����Ϳ��� �Ҵ��ؾ� �մϴ�.
    public GpsManager gpsManager;

    public string accessToken;
    public float centerLatitude = -33.8873f;
    public float centerLongitude = 151.2189f;
    public float zoom = 12.0f;
    public int bearing = 0;
    public int pitch = 0;
    public enum style { Light, Dark, Streets, Outdoors, Satellite, SatelliteStreets };
    public style mapStyle = style.Streets;

    public MeshRenderer targetMeshRenderer;

    private int mapWidth = 1024; // ����: Plane�� ������ �ػ󵵷� ���� (2�� �ŵ����� ����)
    private int mapHeight = 1024; // ����: Plane�� ������ �ػ󵵷� ����

    private string[] styleStr = new string[] { "light-v10", "dark-v10", "streets-v11", "outdoors-v11", "satellite-v9", "satellite-streets-v11" };
    private string url = "";
    private bool mapIsLoading = false;

    // ������Ʈ ������ ���� ���� �� ������
    private string accessTokenLast;
    private float centerLatitudeLast = -33.8873f;
    private float centerLongitudeLast = 151.2189f;
    private float zoomLast = 12.0f;
    private int bearingLast = 0;
    private int pitchLast = 0;
    private style mapStyleLast = style.Streets;

    private bool updateMap = true; // ���� ������Ʈ �ʿ� ���� �÷���

    // Material�� ������ ����
    private Material mapMaterial;

    void Start()
    {
        // MeshRenderer�� �Ҵ�Ǿ����� Ȯ��
        if (targetMeshRenderer == null)
        {
            Debug.LogError("Target Mesh Renderer is not assigned! Please assign a Mesh Renderer to the 'Target Mesh Renderer' field in the Inspector.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        // Material �ν��Ͻ� ���� �� �Ҵ� (���� Material�� �ƴ� ������ Material�� ����ϱ� ����)
        if (targetMeshRenderer.sharedMaterial != null)
        {
            mapMaterial = new Material(targetMeshRenderer.sharedMaterial);
        }
        else
        {
            mapMaterial = new Material(Shader.Find("Standard"));
        }
        targetMeshRenderer.material = mapMaterial; // Plane�� �� Material �ν��Ͻ� �Ҵ�

        // GPS Manager �Ҵ� ���� Ȯ�� �� �̺�Ʈ ����
        if (gpsManager != null)
        {
            // ��ġ ������Ʈ �̺�Ʈ�� �����մϴ�.
            gpsManager.OnLocationUpdated.AddListener(OnGpsLocationUpdated);
            Debug.Log("GPS ��ġ ������Ʈ�� �����߽��ϴ�.");

            // ����������, GPS�� �̹� Ȱ��ȭ�� ��� �ʱ� ��ġ�� �����ɴϴ�.
            if (Input.location.status == LocationServiceStatus.Running)
            {
                OnGpsLocationUpdated(gpsManager.CurrentLocation);
            }
        }
        else
        {
            Debug.LogWarning("MapboxPlane�� GPS Manager�� �Ҵ���� �ʾҽ��ϴ�. ������ �⺻ ��ǥ�� ����մϴ�.");
            // GPS Manager�� �Ҵ���� ���� ���, �⺻������ ������ �ε��մϴ�.
            StartCoroutine(GetMapbox());
        }

        // �ʱ� �� ����
        SaveCurrentSettingsAsLast();
    }

    void OnDisable()
    {
        // ��ü�� ��Ȱ��ȭ�� �� �޸� ������ �����ϱ� ���� �̺�Ʈ ������ ����մϴ�.
        if (gpsManager != null)
        {
            gpsManager.OnLocationUpdated.RemoveListener(OnGpsLocationUpdated);
            Debug.Log("GPS ��ġ ������Ʈ ������ ����߽��ϴ�.");
        }
    }

    void Update()
    {
        // ������ ����Ǿ����� Ȯ�� (GPS ������Ʈ�� �� ���ǿ� ���Ե�)
        if (updateMap && HasSettingsChanged())
        {
            StartCoroutine(GetMapbox());
        }
    }

    /// <summary>
    /// GpsManager�� ��ġ�� ������Ʈ�� �� ȣ��˴ϴ�.
    /// </summary>
    /// <param name="location">���ο� EarthLocation ������.</param>
    private void OnGpsLocationUpdated(EarthLocation location)
    {
        // MapboxPlane�� �߽� ��ǥ�� ������Ʈ�մϴ�.
        centerLatitude = location.Lat;
        centerLongitude = location.Lng;

        Debug.Log($"MapboxPlane�� GPS ������Ʈ�� �޾ҽ��ϴ�: ����={centerLatitude}, �浵={centerLongitude}");

        // ���� Update ����Ŭ���� ���� ���� ��ħ�� Ʈ�����ϵ��� updateMap�� true�� �����մϴ�.
        updateMap = true;
    }

    // ���� ���� ���� Ȯ�� �Լ�
    private bool HasSettingsChanged()
    {
        return accessTokenLast != accessToken ||
               !Mathf.Approximately(centerLatitudeLast, centerLatitude) ||
               !Mathf.Approximately(centerLongitudeLast, centerLongitude) ||
               !Mathf.Approximately(zoomLast, zoom) || // float �񱳴� Approximate ���
               bearingLast != bearing ||
               pitchLast != pitch ||
               mapStyleLast != mapStyle;
    }

    // ���� ������ last ������ �����ϴ� �Լ�
    private void SaveCurrentSettingsAsLast()
    {
        accessTokenLast = accessToken;
        centerLatitudeLast = centerLatitude;
        centerLongitudeLast = centerLongitude;
        zoomLast = zoom;
        bearingLast = bearing;
        pitchLast = pitch;
        mapStyleLast = mapStyle;
    }

    IEnumerator GetMapbox()
    {
        // �̹� �ε� ���̸� �ߺ� ��û ����
        if (mapIsLoading)
        {
            yield break;
        }

        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" +
              centerLongitude + "," + centerLatitude + "," + zoom + "," +
              bearing + "," + pitch + "/" +
              mapWidth + "x" + mapHeight + "?access_token=" + accessToken;

        mapIsLoading = true;
        updateMap = false; // ���ο� ��û�� ���۵Ǿ����Ƿ� updateMap �÷��׸� false�� ����

        Debug.Log($"Mapbox URL ��û: {url}"); // ������� ���� URL�� �α��մϴ�.

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("WWW ����: " + www.error);
            mapIsLoading = false;
            updateMap = true; // ���������Ƿ� �ٽ� ������Ʈ �õ� �����ϰ�
        }
        else
        {
            mapIsLoading = false;
            Texture2D downloadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            // �ٿ�ε�� �ؽ�ó�� Material�� mainTexture�� ����
            if (mapMaterial != null)
            {
                mapMaterial.mainTexture = downloadedTexture;
                mapMaterial.mainTextureScale = new Vector2(1, 1);
                Debug.Log("���� �ؽ�ó�� ���������� ����Ǿ����ϴ�.");
            }
            else
            {
                Debug.LogError("���� Material�� null�Դϴ�! �ؽ�ó�� ������ �� �����ϴ�.");
            }

            // ���� ���� ���� last �����鿡 ����
            SaveCurrentSettingsAsLast();
            updateMap = true; // �ε� ���� �� updateMap �÷��׸� �ٽ� true�� ����
        }
    }
}