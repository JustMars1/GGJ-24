using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomDropdown : TMP_Dropdown
{
    public override void OnSelect(BaseEventData eventData)
    {
        var scroll = GetComponentInParent<CustomScrollRect>();
        if (scroll != null)
        {
            scroll.ScrollTo((RectTransform)transform);
        }
        base.OnSelect(eventData);
    }
}