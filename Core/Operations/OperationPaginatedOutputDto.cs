using System.Diagnostics.CodeAnalysis;


namespace EliseuBatista99.Core.Operations
{
    [ExcludeFromCodeCoverage]
    public class OperationPaginatedOutputDto : OperationOutputDto
    {
        public bool? HasMorePages { get; set; }
    }
}
