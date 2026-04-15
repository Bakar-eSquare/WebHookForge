using System.Text.RegularExpressions;
using WebhookForge.Application.Common.Helpers;
using WebhookForge.Application.Common.Interfaces;
using WebhookForge.Application.Common.Models;
using WebhookForge.Application.DTOs.MockRule;
using WebhookForge.Domain.Entities;

namespace WebhookForge.Application.Services;

/// <summary>
/// Handles mock rule CRUD, activation, reordering, and request matching.
/// Matching logic: active rules are evaluated in ascending Priority order;
/// the first rule whose method, path, and body expression all match wins.
/// Depends on: IUnitOfWork.
/// </summary>
public class MockRuleService : IMockRuleService
{
    private readonly IUnitOfWork _uow;

    public MockRuleService(IUnitOfWork uow) => _uow = uow;

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<MockRuleDto>>> GetByEndpointAsync(Guid endpointId, Guid userId, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, endpointId, userId, ct);
        if (!access.IsSuccess) return Result<IReadOnlyList<MockRuleDto>>.Forbidden(access.Error!);

        var rules = await _uow.MockRules.FindAsync(r => r.EndpointId == endpointId, ct);
        return Result<IReadOnlyList<MockRuleDto>>.Success(rules.OrderBy(r => r.Priority).Select(ToDto).ToList());
    }

    /// <inheritdoc/>
    public async Task<Result<MockRuleDto>> GetByIdAsync(Guid ruleId, Guid userId, CancellationToken ct = default)
    {
        var rule = await _uow.MockRules.GetByIdAsync(ruleId, ct);
        if (rule is null) return Result<MockRuleDto>.NotFound("Mock rule not found.");

        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, rule.EndpointId, userId, ct);
        if (!access.IsSuccess) return Result<MockRuleDto>.Forbidden(access.Error!);

        return Result<MockRuleDto>.Success(ToDto(rule));
    }

    /// <inheritdoc/>
    public async Task<Result<MockRuleDto>> CreateAsync(CreateMockRuleDto dto, Guid endpointId, Guid userId, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, endpointId, userId, ct);
        if (!access.IsSuccess) return Result<MockRuleDto>.Forbidden(access.Error!);

        var rule = new MockRule
        {
            EndpointId          = endpointId,
            Name                = dto.Name,
            Priority            = dto.Priority,
            MatchMethod         = dto.MatchMethod,
            MatchPath           = dto.MatchPath,
            MatchBodyExpression = dto.MatchBodyExpression,
            ResponseStatus      = dto.ResponseStatus,
            ResponseBody        = dto.ResponseBody,
            ResponseHeaders     = JsonHelper.Serialize(dto.ResponseHeaders),
            DelayMs             = dto.DelayMs
        };

        await _uow.MockRules.AddAsync(rule, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<MockRuleDto>.Success(ToDto(rule), 201);
    }

    /// <inheritdoc/>
    public async Task<Result<MockRuleDto>> UpdateAsync(Guid ruleId, UpdateMockRuleDto dto, Guid userId, CancellationToken ct = default)
    {
        var rule = await _uow.MockRules.GetByIdAsync(ruleId, ct);
        if (rule is null) return Result<MockRuleDto>.NotFound("Mock rule not found.");

        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, rule.EndpointId, userId, ct);
        if (!access.IsSuccess) return Result<MockRuleDto>.Forbidden(access.Error!);

        rule.Name                = dto.Name;
        rule.Priority            = dto.Priority;
        rule.MatchMethod         = dto.MatchMethod;
        rule.MatchPath           = dto.MatchPath;
        rule.MatchBodyExpression = dto.MatchBodyExpression;
        rule.ResponseStatus      = dto.ResponseStatus;
        rule.ResponseBody        = dto.ResponseBody;
        rule.ResponseHeaders     = JsonHelper.Serialize(dto.ResponseHeaders);
        rule.DelayMs             = dto.DelayMs;
        rule.IsActive            = dto.IsActive;
        rule.UpdatedAt           = DateTime.UtcNow;

        await _uow.MockRules.UpdateAsync(rule, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<MockRuleDto>.Success(ToDto(rule));
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(Guid ruleId, Guid userId, CancellationToken ct = default)
    {
        var rule = await _uow.MockRules.GetByIdAsync(ruleId, ct);
        if (rule is null) return Result.NotFound("Mock rule not found.");

        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, rule.EndpointId, userId, ct);
        if (!access.IsSuccess) return Result.Forbidden(access.Error!);

        await _uow.MockRules.DeleteAsync(rule, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result<MockRuleDto>> ToggleAsync(Guid ruleId, Guid userId, CancellationToken ct = default)
    {
        var rule = await _uow.MockRules.GetByIdAsync(ruleId, ct);
        if (rule is null) return Result<MockRuleDto>.NotFound("Mock rule not found.");

        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, rule.EndpointId, userId, ct);
        if (!access.IsSuccess) return Result<MockRuleDto>.Forbidden(access.Error!);

        rule.IsActive  = !rule.IsActive;
        rule.UpdatedAt = DateTime.UtcNow;

        await _uow.MockRules.UpdateAsync(rule, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<MockRuleDto>.Success(ToDto(rule));
    }

    /// <inheritdoc/>
    public async Task<Result> ReorderAsync(Guid endpointId, ReorderRulesDto dto, Guid userId, CancellationToken ct = default)
    {
        var access = await AccessGuard.RequireEndpointAccessAsync(_uow, endpointId, userId, ct);
        if (!access.IsSuccess) return Result.Forbidden(access.Error!);

        var rules = await _uow.MockRules.FindAsync(r => r.EndpointId == endpointId, ct);
        var ruleMap = rules.ToDictionary(r => r.Id);

        // Assign priority in steps of 10 so rules can be inserted between existing ones later
        for (var i = 0; i < dto.RuleIds.Count; i++)
        {
            if (!ruleMap.TryGetValue(dto.RuleIds[i], out var rule)) continue;
            rule.Priority  = (i + 1) * 10;
            rule.UpdatedAt = DateTime.UtcNow;
            await _uow.MockRules.UpdateAsync(rule, ct);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<MockRuleDto?> MatchRuleAsync(Guid endpointId, string method, string? path, string? body, CancellationToken ct = default)
    {
        var rules = await _uow.MockRules.GetActiveByEndpointIdAsync(endpointId, ct);

        return rules
            .FirstOrDefault(r =>
                (string.IsNullOrEmpty(r.MatchMethod) || r.MatchMethod.Equals(method, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(r.MatchPath)   || Regex.IsMatch(path ?? string.Empty, r.MatchPath)) &&
                (string.IsNullOrEmpty(r.MatchBodyExpression) || Regex.IsMatch(body ?? string.Empty, r.MatchBodyExpression)))
            .Let(ToDto);
    }

    // ── Mapping ──────────────────────────────────────────────────

    private static MockRuleDto ToDto(MockRule r) => new()
    {
        Id                  = r.Id,
        EndpointId          = r.EndpointId,
        Name                = r.Name,
        Priority            = r.Priority,
        MatchMethod         = r.MatchMethod,
        MatchPath           = r.MatchPath,
        MatchBodyExpression = r.MatchBodyExpression,
        ResponseStatus      = r.ResponseStatus,
        ResponseBody        = r.ResponseBody,
        ResponseHeaders     = JsonHelper.TryDeserialize<Dictionary<string, string>>(r.ResponseHeaders),
        DelayMs             = r.DelayMs,
        IsActive            = r.IsActive,
        CreatedAt           = r.CreatedAt
    };

    // MatchRuleAsync uses the Let() extension below to handle the null case.
}

// Small extension to avoid a separate local variable in MatchRuleAsync
file static class Extensions
{
    public static TOut? Let<TIn, TOut>(this TIn? input, Func<TIn, TOut> map) where TIn : class
        => input is null ? default : map(input);
}
