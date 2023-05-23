using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // ��������� ������ ����
    public GameObject[] prefabs;

    // Ǯ ����� �ϴ� ����Ʈ��
    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];   //pool�� list �ʱ�ȭ

        for( int i = 0; i < pools.Length; i++) {
            pools[i] = new List<GameObject> ();
        }
    }

    public GameObject Get(int index)  //���ӿ�����Ʈ�� ��ȯ
    {
        GameObject select = null;
        // ������ Ǯ�� ����ִ�(��Ȱ��ȭ ��) ���ӿ�����Ʈ ���� 
        // �߰��ϸ� select ������ �Ҵ�

        foreach (GameObject item in pools[index]) {
            if (!item.activeSelf) {       //����ִ°� �߰��ϸ� select ������ �Ҵ�
                select = item;
                select.SetActive(true);
                break;
            }
        }
        // ��ã���� ���Ӱ� �����ؼ� select ������ �Ҵ�

        if (!select) {      //select == null;
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
