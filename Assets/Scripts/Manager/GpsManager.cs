using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class GpsManager : MonoBehaviour
{
    [SerializeField] private float _refreshCycle = 1;
    [SerializeField] private int _maxRefreshWait = 20;

    public EarthLocation CurrentLocation { get; private set; }

    public UnityEvent<EarthLocation> OnLocationUpdated = new();

    private WaitForSeconds _cycle;
    private Coroutine _routine;

    public int RefreshCount { get; private set; }


    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        StartGps();
    }

    private void OnDisable()
    {
        StopGps();
    }

    private void Init()
    {
        _cycle = new WaitForSeconds(_refreshCycle);
    }

    private void StartGps()
    {
        if (_routine != null) StopCoroutine(_routine);

        _routine = StartCoroutine(GpsLoop());
    }

    private void StopGps()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (Input.location.status != LocationServiceStatus.Stopped)
        {
            Input.location.Stop();
        }
    }

    private IEnumerator GpsLoop()
    {
        // ������ ����� ���� ����� ī���� ����
        RefreshCount = 0;
        //GPS�� Ȱ��ȭ �Ǿ��ִ°�? �Ǿ� ���� ������ �������
        bool isGpsActive =
            !Input.location.isEnabledByUser ||
            !Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        if (isGpsActive)
        {
            yield break;
        }
        // ���� ����

        Input.location.Start();

        //GPS�� ���� ������ ������ �Ȱ�������
        // ��� ī��Ʈ�� 0 �ʰ���� 
        // ����Ŭ �ֱ�� ī��Ʈ�� ����, �ݺ��ϸ� ���
        int count = _maxRefreshWait;
        while (Input.location.status == LocationServiceStatus.Initializing && count > 0)
        {
            count--;
            yield return _cycle;
        }
        // ī��Ʈ�� 0���ϰų�(ī��Ʈ �ٵ�) Gps���°� ������ ���
        //�����.
        if (count <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            yield break;
        }

        //���� ���󱸵� �Ǿ����� ������.
        while (true)
        {
            LocationInfo data = Input.location.lastData;
            CurrentLocation = new(data.latitude, data.longitude);

            RefreshCount++;

            //���� ���� ���� ��, �����ڿ��� ��ȣ�� �Ѹ� �� �ִ� �̺�Ʈ�� ������ ������
            OnLocationUpdated?.Invoke(CurrentLocation);

            yield return _cycle;
        }
    }
}
