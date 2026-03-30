using AutoMapper;
using EliseuBatista99.Core;
using EliseuBatista99.Tests.Mocks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace EliseuBatista99.Tests
{
    public class TestsBuilder : ITestsBuilder
    {
        private readonly Dictionary<Type, object> _mocks = new();

        protected IExecutionContext? ExecutionContextMock;


        public TestsBuilder()
        {
        }

        protected virtual void AddMockClasses()
        {

        }

        protected virtual void SetupDefaultMocks()
        {

        }

        protected virtual Profile[] GetMapperProfiles()
        {
            return [];
        }

        private void AddFrameworkMocks()
        {
            // AutoMapper

            var config = new MapperConfiguration(cfg =>
            {
                var mapperProfiles = GetMapperProfiles();

                foreach (var profile in mapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            }, NullLoggerFactory.Instance);

            IMapper mapper = config.CreateMapper();

            AddInstance(mapper);

            // IHttpContextAccessor

            var httpContextMock = new HttpContextMock();
            AddInstance(httpContextMock.Object);

        }

        protected Mock<T> AddMock<T>() where T : class
        {
            var mock = new Mock<T>();
            _mocks[typeof(T)] = mock;
            return mock;
        }

        protected void AddInstance<T>(T instance) where T : class
        {
            _mocks[typeof(T)] = instance;
        }

        public T? GetMock<T>() where T : class
        {
            if (_mocks.TryGetValue(typeof(T), out var value))
            {
                if (value is Mock<T> m)
                    return m.Object;

                return (T)value;
            }

            return null;
        }

        public IExecutionContext Build()
        {
            AddFrameworkMocks();

            AddMockClasses();

            this.ExecutionContextMock = new ExecutionContextMock(_mocks);
            return this.ExecutionContextMock;
        }
    }
}
