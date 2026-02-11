using System.Diagnostics.CodeAnalysis;


namespace EliseuBatista99.Core.Operations
{
    [ExcludeFromCodeCoverage]
    public class OperationPaginatedInputDto : OperationInputDto
    {
        public int? Page { get; set; }

        public int? PageSize { get; set; }
    }
}
