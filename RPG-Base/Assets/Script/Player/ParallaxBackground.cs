using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // 배경 이미지의 시작 위치와 길이를 저장할 변수
    private float length, startpos;

    // 카메라 게임 오브젝트
    public GameObject cam;

    // 패럴랙스 효과 강도
    public float parallaxEffect;

    void Start()
    {
        // 현재 배경 이미지의 x 위치를 시작 위치로 설정
        startpos = transform.position.x;

        cam = GameObject.Find("Main Camera");
        // 배경 이미지의 길이를 가져옴 (SpriteRenderer의 bounds를 통해)
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 카메라의 x 위치를 기준으로 배경이 이동해야 할 거리를 계산
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        // 카메라의 x 위치에 패럴랙스 효과를 적용한 거리
        float dist = (cam.transform.position.x * parallaxEffect);

        // 배경의 새로운 위치를 설정 (y와 z는 그대로 유지)
        transform.position = new Vector3(startpos + dist, transform.position.y);

        // 배경이 화면 오른쪽으로 나가면 시작 위치를 오른쪽으로 이동
        if (temp > startpos + length) startpos += length;
        // 배경이 화면 왼쪽으로 나가면 시작 위치를 왼쪽으로 이동
        else if (temp < startpos - length) startpos -= length;
    }
}
