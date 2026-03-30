using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace EliseuBatista99.Tests.Mocks
{
    public class HttpContextMock : Mock<IHttpContextAccessor>
    {
        private DefaultHttpContext _context = new DefaultHttpContext();

        public HttpContextMock()
        {
            this.HttpContext(new DefaultHttpContext());
        }

        public HttpContextMock HttpContext(DefaultHttpContext outputMock)
        {
            _context = outputMock;

            this.Setup(x => x.HttpContext).Returns(_context);

            return this;
        }

        public HttpContextMock User(ClaimsPrincipal outputMock)
        {
            _context.User = outputMock;

            this.HttpContext(_context);

            return this;
        }
    }
}
