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

        public string GetActiveComputerName()
        {
            using (var scope = this.scopeProvider.CreateScope(autoComplete: true))
            {
                return scope.Database
                    .Query<string>("SELECT computerName FROM umbracoServer WHERE isActive = 1")
                    .FirstOrDefault();
            }
        }

        // public string GetActiveComputerName()
        // {
        //     using (var scope = this.scopeProvider.CreateScope(autoComplete: true))
        //     {
        //         const string sql = @"SELECT computerName
        //                              FROM umbracoServer
        //                              WHERE isActive = 1
        //                              LIMIT 1";

        //         // Execute query and return the active computerName
        //         return scope.Database.ExecuteScalar<string>(sql);
        //     }
        // }

    }
}
