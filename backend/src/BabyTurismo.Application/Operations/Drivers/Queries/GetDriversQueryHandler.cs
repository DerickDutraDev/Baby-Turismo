using BabyTurismo.Application.Common.Interfaces;
using BabyTurismo.Shared.Pagination;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Queries;

internal sealed class GetDriversQueryHandler : IRequestHandler<GetDriversQuery, Result<PagedResult<DriverDto>>>
{
    private readonly IDriverRepository _driverRepository;

    public GetDriversQueryHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Result<PagedResult<DriverDto>>> Handle(GetDriversQuery request, CancellationToken cancellationToken)
    {
        var result = await _driverRepository.GetPaginatedDriversAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.Status,
            cancellationToken);

        return Result.Success(result);
    }
}
