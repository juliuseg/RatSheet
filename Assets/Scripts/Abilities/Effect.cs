using UnityEngine;

public class Effect
{
    private float duration; // If 0, effect is one time only. Durration is handled elsewhere

    private float healthEffect;
    private float speedEffect;
    private float defenseEffect;

    public Effect(float _duration, float _effectValue)
    {
        duration = _duration;
    }

}