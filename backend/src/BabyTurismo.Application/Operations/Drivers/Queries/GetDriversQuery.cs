using BabyTurismo.Shared.Pagination;
using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Drivers.Queries;

public sealed record GetDriversQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string? Status) : IRequest<Result<PagedResult<DriverDto>>>;
