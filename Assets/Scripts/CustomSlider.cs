using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(CustomSlider))]
[CanEditMultipleObjects]
public class CustomSliderEditor : SliderEditor
{
    SerializedProperty _audioGroup;
    SerializedProperty _slideClipOverride;

    protected override void OnEnable()
    {
        base.OnEnable();
        _audioGroup = serializedObject.FindProperty("audioGroup");
        _slideClipOverride = serializedObject.FindProperty("slideClipOverride");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(_audioGroup);
        EditorGUILayout.PropertyField(_slideClipOverride);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public class CustomSlider : Slider
{
    public AudioClip slideClipOverride = null;
    public AudioGroup audioGroup = AudioGroup.None;

    bool isHorizontal => direction == Direction.LeftToRight || direction == Direction.RightToLeft;

    AudioClip slideClip => slideClipOverride == null ? AudioController.globalAudioClips.slideClip : slideClipOverride;

    AudioSource _slideSound = null;
    
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

    public override void OnPointerUp(PointerEventData eventData)
    {
        PlaySlideSound();
        base.OnPointerUp(eventData);
    }

    public override void OnMove(AxisEventData eventData)
    {
        switch (eventData.moveDir)
        {
            case MoveDirection.Left:
            case MoveDirection.Right:
            {
                if (isHorizontal)
                {
                    PlaySlideSound();
                }

                break;
            }
            case MoveDirection.Down:
            case MoveDirection.Up:
            {
                if (!isHorizontal)
                {
                    PlaySlideSound();
                }

                break;
            }
            case MoveDirection.None:
            default:
                break;
        }

        base.OnMove(eventData);
    }

    void PlaySlideSound()
    {
        if (_slideSound != null && _slideSound.isPlaying && _slideSound.time < _slideSound.clip.length / 2)
        {
            return;
        }
        _slideSound = AudioController.Play(slideClip, audioGroup, ignorePause: true);
    }
}