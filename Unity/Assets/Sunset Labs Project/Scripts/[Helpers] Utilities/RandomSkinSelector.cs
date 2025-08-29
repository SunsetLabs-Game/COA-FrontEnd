using UnityEngine;

public class RandomSkinSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] skinMeshes;

    public void RandomSkin()
    {
        int length = skinMeshes.Length;
        int random = Random.Range(0, length);
        for(int i = 0; i < length; i++)
        {
            bool active = (i == random);
            skinMeshes[i].SetActive(active);
        }
    }
}
