using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class VersionText : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TMP_Text>().text = "v. " + Application.version;
    }
}
