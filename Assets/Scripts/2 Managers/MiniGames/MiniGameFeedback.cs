using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Universal VFX Juice & SFX Trigger System for all Minigames in Dear Hiring Manager.
/// Provides snappy scale punches, directional screen shakes, color flash alerts,
/// floating celebratory/failure text popups, and radiant spark bursts.
/// </summary>
public static class MiniGameFeedback
{
    public static void PlayTap(VisualElement target = null)
    {
        AudioManager.Instance?.PlayClick();
        HapticManager.LightImpact();
        if (target != null) Punch(target, 1.08f, 100);
    }

    public static void PlaySuccess(VisualElement target = null, string popupText = "+1", VisualElement parentForPopup = null)
    {
        AudioManager.Instance?.PlayScorePop();
        HapticManager.LightImpact();
        
        if (target != null)
        {
            Punch(target, 1.18f, 160);
            Flash(target, new Color(0.18f, 0.8f, 0.44f, 0.6f), 180);
        }

        VisualElement container = parentForPopup ?? target?.parent;
        if (container != null && !string.IsNullOrEmpty(popupText))
        {
            Vector2 spawnPos = (target != null) ? target.worldBound.center : new Vector2(300, 400);
            SpawnFloatingText(container, spawnPos, popupText, new Color(0.2f, 0.95f, 0.5f));
            SpawnSparks(container, spawnPos, new Color(0.3f, 1f, 0.6f), 6);
        }
    }

    public static void PlayError(VisualElement target = null, string popupText = "MISS!", VisualElement parentForPopup = null)
    {
        AudioManager.Instance?.PlayBuzzer();
        HapticManager.MediumImpact();

        if (target != null)
        {
            Shake(target, 10f, 240);
            Flash(target, new Color(0.9f, 0.2f, 0.2f, 0.7f), 240);
        }

        VisualElement container = parentForPopup ?? target?.parent;
        if (container != null && !string.IsNullOrEmpty(popupText))
        {
            Vector2 spawnPos = (target != null) ? target.worldBound.center : new Vector2(300, 400);
            SpawnFloatingText(container, spawnPos, popupText, new Color(1f, 0.3f, 0.3f));
        }
    }

    public static void Punch(VisualElement el, float peakScale = 1.18f, int durationMs = 150)
    {
        if (el == null) return;
        el.style.scale = new Scale(new Vector2(peakScale, peakScale));
        el.schedule.Execute(() =>
        {
            if (el != null) el.style.scale = new Scale(Vector2.one);
        }).StartingIn(durationMs);
    }

    public static void Shake(VisualElement el, float intensity = 8f, int durationMs = 240)
    {
        if (el == null) return;
        int steps = durationMs / 30;
        for (int i = 0; i < steps; i++)
        {
            int stepIndex = i;
            el.schedule.Execute(() =>
            {
                if (el == null) return;
                if (stepIndex == steps - 1)
                {
                    el.style.translate = new Translate(0, 0, 0);
                }
                else
                {
                    float factor = 1f - ((float)stepIndex / steps);
                    float dx = Random.Range(-intensity, intensity) * factor;
                    float dy = Random.Range(-intensity * 0.4f, intensity * 0.4f) * factor;
                    el.style.translate = new Translate(dx, dy, 0);
                }
            }).StartingIn(stepIndex * 30);
        }
    }

    public static void Flash(VisualElement el, Color flashColor, int durationMs = 180)
    {
        if (el == null) return;
        var prevTop = el.style.borderTopColor;
        var prevBottom = el.style.borderBottomColor;
        var prevLeft = el.style.borderLeftColor;
        var prevRight = el.style.borderRightColor;
        var flashStyle = new StyleColor(flashColor);
        el.style.borderTopColor = flashStyle;
        el.style.borderBottomColor = flashStyle;
        el.style.borderLeftColor = flashStyle;
        el.style.borderRightColor = flashStyle;
        el.schedule.Execute(() =>
        {
            if (el != null)
            {
                el.style.borderTopColor = prevTop;
                el.style.borderBottomColor = prevBottom;
                el.style.borderLeftColor = prevLeft;
                el.style.borderRightColor = prevRight;
            }
        }).StartingIn(durationMs);
    }

    public static void SpawnFloatingText(VisualElement parent, Vector2 screenPos, string text, Color color, float floatDist = 55f, int durationMs = 600)
    {
        if (parent == null) return;

        Vector2 localPos = parent.WorldToLocal(screenPos);

        var lbl = new Label(text);
        lbl.style.position = Position.Absolute;
        lbl.style.left = localPos.x - 60;
        lbl.style.top = localPos.y - 25;
        lbl.style.width = 120;
        lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
        lbl.style.fontSize = 28;
        lbl.style.unityFontStyleAndWeight = FontStyle.Bold;
        lbl.style.color = color;
        lbl.pickingMode = PickingMode.Ignore;
        parent.Add(lbl);

        int steps = 15;
        int interval = durationMs / steps;
        for (int s = 0; s <= steps; s++)
        {
            int step = s;
            lbl.schedule.Execute(() =>
            {
                if (lbl == null || lbl.parent == null) return;
                float progress = (float)step / steps;
                lbl.style.top = (localPos.y - 25) - (floatDist * progress);
                lbl.style.opacity = 1f - (progress * progress);
                float sc = 1f + (Mathf.Sin(progress * Mathf.PI) * 0.25f);
                lbl.style.scale = new Scale(new Vector2(sc, sc));

                if (step == steps)
                {
                    lbl.RemoveFromHierarchy();
                }
            }).StartingIn(step * interval);
        }
    }

    public static void SpawnSparks(VisualElement parent, Vector2 screenPos, Color color, int count = 6)
    {
        if (parent == null) return;
        Vector2 localPos = parent.WorldToLocal(screenPos);

        for (int i = 0; i < count; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = Random.Range(25f, 60f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed);

            var spark = new VisualElement();
            spark.style.position = Position.Absolute;
            spark.style.left = localPos.x;
            spark.style.top = localPos.y;
            spark.style.width = 8;
            spark.style.height = 8;
            spark.style.borderTopLeftRadius = 4;
            spark.style.borderTopRightRadius = 4;
            spark.style.borderBottomLeftRadius = 4;
            spark.style.borderBottomRightRadius = 4;
            spark.style.backgroundColor = color;
            spark.pickingMode = PickingMode.Ignore;
            parent.Add(spark);

            int sparkSteps = 10;
            int sparkDuration = 350;
            int interval = sparkDuration / sparkSteps;

            for (int s = 0; s <= sparkSteps; s++)
            {
                int step = s;
                spark.schedule.Execute(() =>
                {
                    if (spark == null || spark.parent == null) return;
                    float prog = (float)step / sparkSteps;
                    spark.style.left = localPos.x + velocity.x * prog;
                    spark.style.top = localPos.y + velocity.y * prog;
                    spark.style.opacity = 1f - prog;
                    spark.style.scale = new Scale(new Vector2(1f - prog * 0.5f, 1f - prog * 0.5f));

                    if (step == sparkSteps)
                    {
                        spark.RemoveFromHierarchy();
                    }
                }).StartingIn(step * interval);
            }
        }
    }
}
