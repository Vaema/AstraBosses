namespace AstraBosses.Core.Music;

public enum BossMusicEasing
{
    Linear,
    SmoothStep,
    EaseIn,
    EaseOut,
    EaseInOut
}

public static class BossMusicTween
{
    public static float Ease(float progress, BossMusicEasing easing)
    {
        progress = Math.Clamp(progress, 0f, 1f);
        return easing switch
        {
            BossMusicEasing.SmoothStep => progress * progress * (3f - 2f * progress),
            BossMusicEasing.EaseIn => progress * progress,
            BossMusicEasing.EaseOut => 1f - (1f - progress) * (1f - progress),
            BossMusicEasing.EaseInOut => progress < 0.5f
                ? 2f * progress * progress
                : 1f - Pow(-2f * progress + 2f, 2f) / 2f,
            _ => progress
        };
    }

    public static float Lerp(float from, float to, float progress, BossMusicEasing easing = BossMusicEasing.Linear)
    {
        return from + (to - from) * Ease(progress, easing);
    }

    public static T Lerp<T>(T from, T to, float progress, Func<T, T, float, T> lerp, BossMusicEasing easing = BossMusicEasing.Linear)
    {
        ArgumentNullException.ThrowIfNull(lerp);
        return lerp(from, to, Ease(progress, easing));
    }

    public static float ClipProgress(BossMusicTimelineClip clip, BossMusicClockSnapshot snapshot, BossMusicTrack track)
    {
        ArgumentNullException.ThrowIfNull(clip);
        ArgumentNullException.ThrowIfNull(track);

        double beat = snapshot.ElapsedSeconds / track.SecondsPerBeat;
        double position = (beat - clip.StartBeat) % clip.DurationBeats;
        if (position < 0d)
            position += clip.DurationBeats;

        return (float)Math.Clamp(position / clip.DurationBeats, 0d, 1d);
    }
}
