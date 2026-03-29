using DataFuse.Adapters.Abstraction;

namespace DataFuse.Integration.Impl;

public class EventPublisher
{
    private readonly ISubscriber<ExecutorResultArgs> subscriber;

    public EventPublisher(ISubscriber<ExecutorResultArgs> subscriber)
    {
        this.subscriber = subscriber;
    }

    public void PublishEvent(IDataContext context, ExecutorResultArgs args) => subscriber.OnEventHandler(context, args);
}

public class ExecutorResultArgs : EventArgs
{
    public ExecutorResultArgs(IEnumerable<IQueryResult> result)
    {
        Result = result;
    }

    public IEnumerable<IQueryResult> Result { get; }
}

public interface ISubscriber<TArgs>
{
    void OnEventHandler(IDataContext context, TArgs e);
}
