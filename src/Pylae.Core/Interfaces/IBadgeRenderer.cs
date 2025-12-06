using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IBadgeRenderer
{
    Task<byte[]> RenderBadgeAsync(Member member, CancellationToken cancellationToken = default);
    Task<byte[]> RenderBatchBadgesAsync(IEnumerable<Member> members, CancellationToken cancellationToken = default);
}
