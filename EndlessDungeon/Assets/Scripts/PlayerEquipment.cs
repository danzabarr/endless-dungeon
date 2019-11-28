using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    public Mesh testMesh;
    public Vector3 testPosition, testRotation;
    public string testHead, testBody, testHands, testFeet;

    [SerializeField]
    private GameObject mainHandWeapon, offHandWeapon;

    [SerializeField]
    private GameObject[] head, body, hands, feet;

    public void OnValidate()
    {
        SetMainHand(testMesh, testPosition, testRotation);
        SetOffHand(testMesh, testPosition, testRotation);
        SetHead(testHead);
        SetBody(testBody);
        SetHands(testHands);
        SetFeet(testFeet);
    }

    public void SetMainHand(Mesh mesh, Vector3 position, Vector3 eulerRotation)
    {
        mainHandWeapon.transform.localPosition = position;
        mainHandWeapon.transform.localRotation = Quaternion.Euler(eulerRotation);
        foreach (MeshFilter filter in mainHandWeapon.GetComponentsInChildren<MeshFilter>())
            filter.sharedMesh = mesh;
    }

    public void SetOffHand(Mesh mesh, Vector3 position, Vector3 eulerRotation)
    {
        offHandWeapon.transform.localPosition = new Vector3(position.x, position.y, -position.z);
        offHandWeapon.transform.localRotation = Quaternion.Euler(-eulerRotation.x, -eulerRotation.y, eulerRotation.z);
        foreach (MeshFilter filter in offHandWeapon.GetComponentsInChildren<MeshFilter>())
            filter.sharedMesh = mesh;
    }

    public void SetHead(string name)
    {
        foreach (GameObject obj in head)
            obj.SetActive(obj.name == name);
    }

    public void SetBody(string name)
    {
        foreach (GameObject obj in body)
            obj.SetActive(obj.name == name);
    }

    public void SetHands(string name)
    {
        foreach (GameObject obj in hands)
            obj.SetActive(obj.name == name);
    }

    public void SetFeet(string name)
    {
        foreach (GameObject obj in feet)
            obj.SetActive(obj.name == name);
    }
}
