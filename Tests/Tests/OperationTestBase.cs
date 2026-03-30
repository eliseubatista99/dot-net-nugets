using EliseuBatista99.Core;

namespace EliseuBatista99.Tests
{
    public class OperationTestBase
    {
        private TestsBuilder Builder;
        protected IExecutionContext ExecutionContext;

        protected OperationTestBase()
        {
            if (Builder == null)
            {
                Builder = CreateBuilder();
            }

            ExecutionContext = Builder.Build();

            BeforeAll();
        }

        protected virtual TestsBuilder CreateBuilder()
        {
            return new TestsBuilder();
        }

        protected virtual T GetBuilder<T>() where T : TestsBuilder
        {
            return (this.Builder as T)!;
        }

        protected virtual void BeforeAll()
        {

        }

        protected virtual void InitializeTest()
        {
        }
    }
}
