using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthLocation : MonoBehaviour
{
    //���� 
    public float Lat { get; set; }
    //�浵
    public float Lng { get; set; }

    public EarthLocation(float lat, float lng)
    {
        Lat = lat; Lng = lng;
    }
}
