using BabyTurismo.Shared.Results;
using MediatR;

namespace BabyTurismo.Application.Operations.Checklists.Queries;

public sealed record GetTodayChecklistQuery(Guid VehicleId) : IRequest<Result<DailyChecklistDto?>>;
