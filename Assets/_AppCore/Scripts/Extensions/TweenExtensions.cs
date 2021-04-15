using UnityEngine;
using DG.Tweening;
using System;

namespace AppCore
{
    public static class TweenExtensions
    {
        public static bool KillIfPlaying(this Tween tween, bool complete = false)
        {
            if (tween != null && tween.IsPlaying())
            {
                tween.Kill(complete);

                return true;
            }

            return false;
        }
    }
}