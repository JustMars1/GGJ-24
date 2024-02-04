using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomDropdown : TMP_Dropdown
{
    public override void OnSelect(BaseEventData eventData)
    {
        if (!IsHighlighted())
        {
            var scroll = GetComponentInParent<CustomScrollRect>();
            if (scroll != null)
            {
                scroll.ScrollTo((RectTransform)transform);
            }
            
            AudioController.Play(AudioController.globalAudioClips.selectClip, AudioGroup.Sound, ignorePause: true);
        }

        base.OnSelect(eventData);
    }
    
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (IsInteractable() && currentSelectionState != SelectionState.Selected && currentSelectionState != SelectionState.Pressed)
        {
            AudioController.Play(AudioController.globalAudioClips.selectClip, AudioGroup.Sound, ignorePause: true);
        }
        
        base.OnPointerEnter(eventData);
    }
}