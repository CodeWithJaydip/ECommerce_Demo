namespace ECommerce.Application.Features.Category.DTOs.Responses;

/// <summary>
/// DTO for category response
/// </summary>
public record CategoryResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImagePath { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}
