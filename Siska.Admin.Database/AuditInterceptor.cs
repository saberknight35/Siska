using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Siska.Admin.Model.Entities;
using Siska.Admin.Utility;

namespace Siska.Admin.Database
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly List<AuditEntry> _auditEntries;
        private readonly IExtractUser _extractUser;
        public AuditInterceptor(List<AuditEntry> auditEntries, IExtractUser extractUser) 
        {
            _auditEntries = auditEntries;
            _extractUser = extractUser;
        }

        #region async
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            var startTime = DateTime.UtcNow;

            var auditEntries = eventData.Context.ChangeTracker
                .Entries()
                .Where(x => x.Entity is not AuditEntry && x.Entity is not Users
                             &&
                             (x.State is EntityState.Added || x.State is EntityState.Modified || x.State is EntityState.Deleted))
                .Select(x => new AuditEntry
                {
                    Id = Guid.NewGuid(),
                    StartTime = startTime,
                    Metadata = x.DebugView.LongView,
                    ActionTable = x.Metadata.Name,
                    ActionTaken = x.State.ToString(),
                    ActionBy = _extractUser.Username
                }).ToList();

            if (auditEntries.Count == 0)
            {
                return await base.SavingChangesAsync(eventData, result, cancellationToken);
            }

            _auditEntries.AddRange(auditEntries);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null || _auditEntries.Count == 0)
            {
                return await base.SavedChangesAsync(eventData, result, cancellationToken);
            }

            var endTime = DateTime.UtcNow;

            foreach (var auditEntry in _auditEntries)
            {
                auditEntry.EndTime = endTime;
                auditEntry.Succeeded = true;
            }

            if (_auditEntries.Count > 0)
            {
                await eventData.Context.Set<AuditEntry>().AddRangeAsync(_auditEntries);
                _auditEntries.Clear();
                await eventData.Context.SaveChangesAsync(cancellationToken);
            }

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null || _auditEntries.Count == 0)
            {
                await base.SaveChangesFailedAsync(eventData, cancellationToken);
            }

            var endTime = DateTime.UtcNow;

            foreach (var auditEntry in _auditEntries)
            {
                auditEntry.EndTime = endTime;
                auditEntry.Succeeded = false;
                auditEntry.ErrorMessage = eventData.Exception.Message + "\n" + eventData.Exception.StackTrace;
            }

            if (_auditEntries.Count > 0)
            {
                await eventData.Context.Set<AuditEntry>().AddRangeAsync(_auditEntries);
                _auditEntries.Clear();
                await eventData.Context.SaveChangesAsync(cancellationToken);
            }

            await base.SaveChangesFailedAsync(eventData, cancellationToken);
        }
        #endregion async

        #region sync
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context == null)
            {
                return base.SavingChanges(eventData, result);
            }

            var startTime = DateTime.UtcNow;

            var auditEntries = eventData.Context.ChangeTracker
                .Entries()
                .Where(x => x.Entity is not AuditEntry
                             &&
                             x.State is EntityState.Added || x.State is EntityState.Modified || x.State is EntityState.Deleted)
                .Select(x => new AuditEntry
                {
                    Id = Guid.NewGuid(),
                    StartTime = startTime,
                    Metadata = x.DebugView.LongView,
                    ActionTable = x.Metadata.Name,
                    ActionTaken = x.State.ToString(),
                    ActionBy = _extractUser.Username
                }).ToList();

            if (auditEntries.Count == 0)
            {
                return base.SavingChanges(eventData, result);
            }

            _auditEntries.AddRange(auditEntries);

            return base.SavingChanges(eventData, result);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (eventData.Context == null || _auditEntries.Count == 0)
            {
                return base.SavedChanges(eventData, result);
            }

            var endTime = DateTime.UtcNow;

            foreach (var auditEntry in _auditEntries)
            {
                auditEntry.EndTime = endTime;
                auditEntry.Succeeded = true;
            }

            if (_auditEntries.Count > 0)
            {
                eventData.Context.Set<AuditEntry>().AddRange(_auditEntries);
                _auditEntries.Clear();
                eventData.Context.SaveChanges();
            }

            return base.SavedChanges(eventData, result);
        }

        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            if (eventData.Context == null || _auditEntries.Count == 0)
            {
                base.SaveChangesFailed(eventData);
            }

            var endTime = DateTime.UtcNow;

            foreach (var auditEntry in _auditEntries)
            {
                auditEntry.EndTime = endTime;
                auditEntry.Succeeded = false;
                auditEntry.ErrorMessage = eventData.Exception.Message + "\n" + eventData.Exception.StackTrace;
            }

            if (_auditEntries.Count > 0)
            {
                eventData.Context.Set<AuditEntry>().AddRange(_auditEntries);
                _auditEntries.Clear();
                eventData.Context.SaveChanges();
            }

            base.SaveChangesFailed(eventData);
        }
        #endregion sync
    }
}
