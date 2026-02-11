namespace EliseuBatista99.Core
{
    public interface IExecutionContext
    {
        public T GetService<T>() where T : notnull;
    }
}
