using System;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Astroboard.Services
{
    public abstract class LogServiceBase
    {
        protected readonly IScopeProvider scopeProvider;
        protected readonly IAppPolicyCache cache;

        public LogServiceBase(IScopeProvider scopeProvider, AppCaches caches)
        {
            this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            this.cache = caches?.RuntimeCache ?? throw new ArgumentNullException(nameof(caches));
        }
    }
}
