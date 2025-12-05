using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Core.Services;

public class BadgeEvaluator : IBadgeEvaluator
{
    public BadgeStatus Evaluate(DateTime? issueDate, DateTime? expiryDate, int badgeValidityMonths, int badgeExpiryWarningDays, DateTime now)
    {
        if (badgeValidityMonths > 0 && !expiryDate.HasValue && issueDate.HasValue)
        {
            expiryDate = issueDate.Value.Date.AddMonths(badgeValidityMonths);
        }

        var isExpired = expiryDate.HasValue && expiryDate.Value.Date < now.Date;
        var isWarning = false;

        if (!isExpired && expiryDate.HasValue && badgeExpiryWarningDays > 0)
        {
            var warningThreshold = now.Date.AddDays(badgeExpiryWarningDays);
            isWarning = expiryDate.Value.Date <= warningThreshold;
        }

        return new BadgeStatus(issueDate, expiryDate, isExpired, isWarning, null);
    }
}
