namespace ValidationService.Core.Interfaces.Events.Processors
{
    public interface IEventProcessor<T>
    {
         void Process(T data);
    }
}