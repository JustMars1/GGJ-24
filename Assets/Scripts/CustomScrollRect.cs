using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    [NonSerialized] Coroutine _scrollToCo = null;

    public void ScrollTo(RectTransform target)
    {
        if (_scrollToCo != null)
        {
            StopCoroutine(_scrollToCo);
            _scrollToCo = null;
        }

        _scrollToCo = StartCoroutine(ScrollToCo(target));
    }

    Vector2 GetNormalizedScrollPosition(RectTransform target)
    {
        Vector2 contentSize = content.rect.size;

        Vector2 offset = content.rect.size;
        offset.Scale(content.pivot);

        Vector2 point = (Vector2)content.InverseTransformPoint(target.TransformPoint(target.rect.center)) + offset;

        contentSize.Scale(content.localScale);
        point.Scale(content.localScale);

        Vector2 viewportSize = ((RectTransform)content.parent).rect.size;

        Vector2 pos = normalizedPosition;

        if (horizontal && contentSize.x > viewportSize.x)
        {
            pos.x = Mathf.Clamp01((point.x - viewportSize.x / 2) / (contentSize.x - viewportSize.x));
        }

        if (vertical && contentSize.y > viewportSize.y)
        {
            pos.y = Mathf.Clamp01((point.y - viewportSize.y / 2) / (contentSize.y - viewportSize.y));
        }

        return pos;
    }

    IEnumerator ScrollToCo(RectTransform target)
    {
        yield return null;

        const float speed = 4.5f;
        Vector2 startPos = normalizedPosition;

        Vector2 targetPos = GetNormalizedScrollPosition(target);

        float t = 0.0f;
        while (t < 1.0f)
        {
            normalizedPosition = Vector2.LerpUnclamped(startPos, targetPos, t);
            yield return null;
            t += speed * Time.unscaledDeltaTime;
        }

        normalizedPosition = targetPos;
    }
}