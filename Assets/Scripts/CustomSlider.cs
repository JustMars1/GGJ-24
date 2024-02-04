using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : Slider
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