using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Queries;

public sealed record GetChecklistReportQuery(
    Guid? VehicleId,
    string? StartDate,
    string? EndDate) : IRequest<Result<IReadOnlyList<ChecklistReportRowDto>>>;
