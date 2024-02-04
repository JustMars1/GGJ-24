using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : Toggle
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

    public override void OnPointerClick(PointerEventData eventData)
    {
        AudioController.Play(AudioController.globalAudioClips.pressClip, AudioGroup.Sound, ignorePause: true);
        base.OnPointerClick(eventData);
    }
    
    public override void OnSubmit(BaseEventData eventData)
    {
        AudioController.Play(AudioController.globalAudioClips.pressClip, AudioGroup.Sound, ignorePause: true);
        base.OnSubmit(eventData);
    }
}