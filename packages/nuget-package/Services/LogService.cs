using System;
using System.Linq;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Astroboard.Services
{

    public class LogService : LogServiceBase, ILogService
    {
        public LogService(IScopeProvider scopeProvider, AppCaches caches) : base(scopeProvider, caches)
        {
        }

        public DateTime? GetLogDateByPageIdAndHeader(int pageId, string logHeader)
        {
            using (var scope = this.scopeProvider.CreateScope(autoComplete: true))
            {
                return scope.Database
                    .Query<DateTime?>($"SELECT Datestamp FROM umbracoLog WHERE NodeId = @0 AND LogHeader = @1", pageId, logHeader)
                    .OrderByDescending(x => x)
                    .FirstOrDefault();
            }
        }


    }
}
