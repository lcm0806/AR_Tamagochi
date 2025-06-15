using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapboxPlane : MonoBehaviour
{
    // 인스펙터에서 할당해야 합니다.
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

    private int mapWidth = 1024; // 예시: Plane에 적합한 해상도로 설정 (2의 거듭제곱 권장)
    private int mapHeight = 1024; // 예시: Plane에 적합한 해상도로 설정

    private string[] styleStr = new string[] { "light-v10", "dark-v10", "streets-v11", "outdoors-v11", "satellite-v9", "satellite-streets-v11" };
    private string url = "";
    private bool mapIsLoading = false;

    // 업데이트 감지를 위한 이전 값 변수들
    private string accessTokenLast;
    private float centerLatitudeLast = -33.8873f;
    private float centerLongitudeLast = 151.2189f;
    private float zoomLast = 12.0f;
    private int bearingLast = 0;
    private int pitchLast = 0;
    private style mapStyleLast = style.Streets;

    private bool updateMap = true; // 지도 업데이트 필요 여부 플래그

    // Material을 저장할 변수
    private Material mapMaterial;

    void Start()
    {
        // MeshRenderer가 할당되었는지 확인
        if (targetMeshRenderer == null)
        {
            Debug.LogError("Target Mesh Renderer is not assigned! Please assign a Mesh Renderer to the 'Target Mesh Renderer' field in the Inspector.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        // Material 인스턴스 생성 및 할당 (공유 Material이 아닌 고유한 Material을 사용하기 위함)
        if (targetMeshRenderer.sharedMaterial != null)
        {
            mapMaterial = new Material(targetMeshRenderer.sharedMaterial);
        }
        else
        {
            mapMaterial = new Material(Shader.Find("Standard"));
        }
        targetMeshRenderer.material = mapMaterial; // Plane에 새 Material 인스턴스 할당

        // GPS Manager 할당 여부 확인 및 이벤트 구독
        if (gpsManager != null)
        {
            // 위치 업데이트 이벤트를 구독합니다.
            gpsManager.OnLocationUpdated.AddListener(OnGpsLocationUpdated);
            Debug.Log("GPS 위치 업데이트를 구독했습니다.");

            // 선택적으로, GPS가 이미 활성화된 경우 초기 위치를 가져옵니다.
            if (Input.location.status == LocationServiceStatus.Running)
            {
                OnGpsLocationUpdated(gpsManager.CurrentLocation);
            }
        }
        else
        {
            Debug.LogWarning("MapboxPlane에 GPS Manager가 할당되지 않았습니다. 지도는 기본 좌표를 사용합니다.");
            // GPS Manager가 할당되지 않은 경우, 기본값으로 지도를 로드합니다.
            StartCoroutine(GetMapbox());
        }

        // 초기 값 저장
        SaveCurrentSettingsAsLast();
    }

    void OnDisable()
    {
        // 객체가 비활성화될 때 메모리 누수를 방지하기 위해 이벤트 구독을 취소합니다.
        if (gpsManager != null)
        {
            gpsManager.OnLocationUpdated.RemoveListener(OnGpsLocationUpdated);
            Debug.Log("GPS 위치 업데이트 구독을 취소했습니다.");
        }
    }

    void Update()
    {
        // 설정이 변경되었는지 확인 (GPS 업데이트도 이 조건에 포함됨)
        if (updateMap && HasSettingsChanged())
        {
            StartCoroutine(GetMapbox());
        }
    }

    /// <summary>
    /// GpsManager가 위치를 업데이트할 때 호출됩니다.
    /// </summary>
    /// <param name="location">새로운 EarthLocation 데이터.</param>
    private void OnGpsLocationUpdated(EarthLocation location)
    {
        // MapboxPlane의 중심 좌표를 업데이트합니다.
        centerLatitude = location.Lat;
        centerLongitude = location.Lng;

        Debug.Log($"MapboxPlane이 GPS 업데이트를 받았습니다: 위도={centerLatitude}, 경도={centerLongitude}");

        // 다음 Update 사이클에서 지도 새로 고침을 트리거하도록 updateMap을 true로 설정합니다.
        updateMap = true;
    }

    // 설정 변경 여부 확인 함수
    private bool HasSettingsChanged()
    {
        return accessTokenLast != accessToken ||
               !Mathf.Approximately(centerLatitudeLast, centerLatitude) ||
               !Mathf.Approximately(centerLongitudeLast, centerLongitude) ||
               !Mathf.Approximately(zoomLast, zoom) || // float 비교는 Approximate 사용
               bearingLast != bearing ||
               pitchLast != pitch ||
               mapStyleLast != mapStyle;
    }

    // 현재 설정을 last 변수에 저장하는 함수
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
        // 이미 로딩 중이면 중복 요청 방지
        if (mapIsLoading)
        {
            yield break;
        }

        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" +
              centerLongitude + "," + centerLatitude + "," + zoom + "," +
              bearing + "," + pitch + "/" +
              mapWidth + "x" + mapHeight + "?access_token=" + accessToken;

        mapIsLoading = true;
        updateMap = false; // 새로운 요청이 시작되었으므로 updateMap 플래그를 false로 설정

        Debug.Log($"Mapbox URL 요청: {url}"); // 디버깅을 위해 URL을 로그합니다.

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("WWW 오류: " + www.error);
            mapIsLoading = false;
            updateMap = true; // 실패했으므로 다시 업데이트 시도 가능하게
        }
        else
        {
            mapIsLoading = false;
            Texture2D downloadedTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            // 다운로드된 텍스처를 Material의 mainTexture에 적용
            if (mapMaterial != null)
            {
                mapMaterial.mainTexture = downloadedTexture;
                mapMaterial.mainTextureScale = new Vector2(1, 1);
                Debug.Log("지도 텍스처가 성공적으로 적용되었습니다.");
            }
            else
            {
                Debug.LogError("지도 Material이 null입니다! 텍스처를 적용할 수 없습니다.");
            }

            // 현재 설정 값을 last 변수들에 저장
            SaveCurrentSettingsAsLast();
            updateMap = true; // 로딩 성공 후 updateMap 플래그를 다시 true로 설정
        }
    }
}