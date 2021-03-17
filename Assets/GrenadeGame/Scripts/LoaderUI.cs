using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoaderUI : MonoBehaviour
{
    public void SetLoading(bool isLoading)
    {
        gameObject.SetActive(isLoading);
    }
}
