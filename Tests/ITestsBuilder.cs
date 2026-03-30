using EliseuBatista99.Core;

namespace EliseuBatista99.Tests

{
    public interface ITestsBuilder
    {
        public T? GetMock<T>() where T : class;

        public IExecutionContext Build();
    }
}

