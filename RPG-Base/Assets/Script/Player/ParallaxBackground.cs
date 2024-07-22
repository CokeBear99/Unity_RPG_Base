using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // ��� �̹����� ���� ��ġ�� ���̸� ������ ����
    private float length, startpos;

    // ī�޶� ���� ������Ʈ
    public GameObject cam;

    // �з����� ȿ�� ����
    public float parallaxEffect;

    void Start()
    {
        // ���� ��� �̹����� x ��ġ�� ���� ��ġ�� ����
        startpos = transform.position.x;

        cam = GameObject.Find("Main Camera");
        // ��� �̹����� ���̸� ������ (SpriteRenderer�� bounds�� ����)
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // ī�޶��� x ��ġ�� �������� ����� �̵��ؾ� �� �Ÿ��� ���
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        // ī�޶��� x ��ġ�� �з����� ȿ���� ������ �Ÿ�
        float dist = (cam.transform.position.x * parallaxEffect);

        // ����� ���ο� ��ġ�� ���� (y�� z�� �״�� ����)
        transform.position = new Vector3(startpos + dist, transform.position.y);

        // ����� ȭ�� ���������� ������ ���� ��ġ�� ���������� �̵�
        if (temp > startpos + length) startpos += length;
        // ����� ȭ�� �������� ������ ���� ��ġ�� �������� �̵�
        else if (temp < startpos - length) startpos -= length;
    }
}
