namespace AstraBosses.Core.Music;

public sealed class BossMusicTimelineController<T>
{
    private BossMusicClockSnapshot previousSnapshot;

    public BossMusicTimelineController(BossMusicTimeline<T> timeline)
    {
        Timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
    }

    public BossMusicTimeline<T> Timeline { get; private set; }

    public bool IsInitialized { get; private set; }

    public void Select(BossMusicTimeline<T> timeline, BossMusicClockSnapshot snapshot)
    {
        Timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
        Reset(snapshot);
    }

    public void Reset(BossMusicClockSnapshot snapshot)
    {
        previousSnapshot = snapshot;
        IsInitialized = true;
    }

    public BossMusicTimelineSample<T> Sample(BossMusicClockSnapshot snapshot, BossMusicTrack track)
    {
        if (!IsInitialized)
            Reset(snapshot);

        return Timeline.Sample(snapshot, track);
    }

    public IReadOnlyList<BossMusicTimelineClip> Update(BossMusicClockSnapshot snapshot, BossMusicTrack track)
    {
        if (!IsInitialized)
        {
            Reset(snapshot);
            return [];
        }

        IReadOnlyList<BossMusicTimelineClip> entered = Timeline.GetEnteredClips(previousSnapshot, snapshot, track);
        previousSnapshot = snapshot;
        return entered;
    }
}
