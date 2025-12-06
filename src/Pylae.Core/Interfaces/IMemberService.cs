using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

/// <summary>
/// Service for managing members (visitors/staff) in the system.
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// Retrieves a member by their unique ID.
    /// </summary>
    /// <param name="memberId">The unique member ID (GUID).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The member if found, null otherwise.</returns>
    Task<Member?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an active member by their member number (badge number).
    /// </summary>
    /// <param name="memberNumber">The member number.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active member if found, null otherwise.</returns>
    Task<Member?> GetActiveByMemberNumberAsync(int memberNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for members by name or member number.
    /// </summary>
    /// <param name="query">Search query (name or member number).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of matching members.</returns>
    Task<IReadOnlyCollection<Member>> SearchAsync(string? query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next available member number, recycling from inactive members.
    /// Finds the lowest unused number starting from 1.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The next available member number.</returns>
    Task<int> GetNextAvailableMemberNumberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new member in the system.
    /// If MemberNumber is 0 or not set, auto-assigns the next available number.
    /// </summary>
    /// <param name="member">The member to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created member with generated ID and MemberNumber.</returns>
    /// <exception cref="InvalidOperationException">Thrown if member number is already in use.</exception>
    Task<Member> CreateAsync(Member member, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing member's information.
    /// </summary>
    /// <param name="member">The member with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated member.</returns>
    /// <exception cref="InvalidOperationException">Thrown if member not found or member number conflict.</exception>
    Task<Member> UpdateAsync(Member member, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes a member and their associated photo.
    /// </summary>
    /// <param name="memberId">The member ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a member as inactive (soft delete).
    /// </summary>
    /// <param name="memberId">The member ID to deactivate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task MarkInactiveAsync(string memberId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Issues or reissues a badge for a member with validity period.
    /// </summary>
    /// <param name="memberId">The member ID.</param>
    /// <param name="issueDate">The badge issue date.</param>
    /// <param name="badgeValidityMonths">Validity period in months (-1 for no expiry, >0 for expiry).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The member with updated badge information.</returns>
    Task<Member> IssueBadgeAsync(string memberId, DateTime issueDate, int badgeValidityMonths, CancellationToken cancellationToken = default);
}
