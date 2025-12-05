using Pylae.Core.Models;

namespace Pylae.Core.Interfaces;

public interface IBadgeEvaluator
{
    BadgeStatus Evaluate(DateTime? issueDate, DateTime? expiryDate, int badgeValidityMonths, int badgeExpiryWarningDays, DateTime now);
}
