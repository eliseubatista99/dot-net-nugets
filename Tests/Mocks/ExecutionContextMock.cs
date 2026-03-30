
using EliseuBatista99.Core;

namespace EliseuBatista99.Tests.Mocks
{
    public class ExecutionContextMock : IExecutionContext
    {
        private readonly Dictionary<Type, object> _dependencies;

        public ExecutionContextMock(Dictionary<Type, object> dependencies)
        {
            _dependencies = dependencies;
        }

        public T GetService<T>() where T : notnull
        {
            return (T)_dependencies[typeof(T)];
        }
    }
}
