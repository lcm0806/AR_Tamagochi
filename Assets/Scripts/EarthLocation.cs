using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthLocation : MonoBehaviour
{
    //위도 
    public float Lat { get; set; }
    //경도
    public float Lng { get; set; }

    public EarthLocation(float lat, float lng)
    {
        Lat = lat; Lng = lng;
    }
}
