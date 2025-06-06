using System;

namespace Astroboard.Services
{
    public interface ILogService
    {
        DateTime? GetLogDateByPageIdAndHeader(int pageId, string logHeader);
    }
}
