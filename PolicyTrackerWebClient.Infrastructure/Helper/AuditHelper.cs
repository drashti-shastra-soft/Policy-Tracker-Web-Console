using PolicyTrackerWebClient.Domain;
using System;

namespace PolicyTrackerWebClient.Infrastructure.Helper
{
    public static class AuditHelper
    {
        public static T ApplyAuditInfo<T>(T entity) where T : IAuditable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var userId = HttpContextHelper.LoginUser.Id;

            entity.CreatedBy = userId;
            entity.LastModifiedBy = userId;

            return entity;
        }
    }
}
