using Umbraco.Cms.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Astroboard.Services;
using Microsoft.AspNetCore.Mvc;

#if !DEBUG
using Umbraco.Cms.Web.BackOffice.Controllers;
#else
using Umbraco.Cms.Web.Common.Controllers;
#endif

namespace Astroboard.Controllers
{
#if !DEBUG
    public class AstroboardController : UmbracoAuthorizedApiController
#else
    public class AstroboardController : UmbracoApiController
#endif
    {
        private readonly IUserService _userService;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly IAuditService _auditService;
        private readonly IPackagingService _packagingService;
        private readonly IMemberService _memberService;
        private readonly IMemberGroupService _memberGroupService;
        private readonly ILogService _logService;

        public AstroboardController(
            IUserService userService,
            IContentService contentService,
            IMediaService mediaService,
            IAuditService auditService,
            IPackagingService packagingService,
            IMemberService memberService,
            IMemberGroupService memberGroupService,
            ILogService logService)
        {
            _userService = userService;
            _contentService = contentService;
            _mediaService = mediaService;
            _auditService = auditService;
            _packagingService = packagingService;
            _memberService = memberService;
            _memberGroupService = memberGroupService;
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public class DataPoint
        {
            public string? X { get; set; }
            public int Y { get; set; }
        }

        public class Series
        {
            public string? Name { get; set; }
            public string? Color { get; set; }
            public List<DataPoint>? Data { get; set; }
        }

        public class ContentsLineChartData
        {
            public string? Period { get; set; }
            public int PagesPerPeriod { get; set; }
            public double Rate { get; set; }
            public int Contributors { get; set; }
            public double DailyChangeRate { get; set; }
            public List<Series>? Series { get; set; }
            public List<string>? Categories { get; set; }
        }

        public class MemberViewModel
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Group { get; set; }
            public string? LastLogin { get; set; }
        }

        public class PeriodRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime PreviousStartDate { get; set; }
            public DateTime PreviousEndDate { get; set; }
        }

        private PeriodRange CalculatePeriodRange(string period, DateTime? referenceDate = null)
        {
            var now = referenceDate ?? DateTime.UtcNow;
            var periodLower = period.ToLower();

            switch (periodLower)
            {
                case "today":
                    return new PeriodRange
                    {
                        StartDate = now.Date,
                        EndDate = now.Date.AddDays(1).AddSeconds(-1),
                        PreviousStartDate = now.Date.AddDays(-1),
                        PreviousEndDate = now.Date.AddSeconds(-1)
                    };
                case "yesterday":
                    var yesterday = now.Date.AddDays(-1);
                    return new PeriodRange
                    {
                        StartDate = yesterday,
                        EndDate = yesterday.AddDays(1).AddSeconds(-1),
                        PreviousStartDate = yesterday.AddDays(-1),
                        PreviousEndDate = yesterday.AddSeconds(-1)
                    };
                case "currentweek":
                    var monday = GetMondayOfCurrentWeek(now.Date);
                    return new PeriodRange
                    {
                        StartDate = monday,
                        EndDate = monday.AddDays(6).AddSeconds(86399),
                        PreviousStartDate = monday.AddDays(-7),
                        PreviousEndDate = monday.AddDays(-1).AddSeconds(86399)
                    };
                case "lastweek":
                    var lastWeekMonday = GetMondayOfCurrentWeek(now.Date).AddDays(-7);
                    return new PeriodRange
                    {
                        StartDate = lastWeekMonday,
                        EndDate = lastWeekMonday.AddDays(6).AddSeconds(86399),
                        PreviousStartDate = lastWeekMonday.AddDays(-7),
                        PreviousEndDate = lastWeekMonday.AddDays(-1).AddSeconds(86399)
                    };
                case "lastmonth":
                    var lastMonthStart = now.AddDays(-30);
                    return new PeriodRange
                    {
                        StartDate = lastMonthStart,
                        EndDate = now,
                        PreviousStartDate = lastMonthStart.AddDays(-30),
                        PreviousEndDate = lastMonthStart.AddSeconds(-1)
                    };
                case "last90days":
                    var last90Start = now.AddDays(-90);
                    return new PeriodRange
                    {
                        StartDate = last90Start,
                        EndDate = now,
                        PreviousStartDate = last90Start.AddDays(-90),
                        PreviousEndDate = last90Start.AddSeconds(-1)
                    };
                default:
                    throw new ArgumentException("Invalid period specified.");
            }
        }

        [HttpGet]
        public JsonResult GetTotalPages(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var allPages = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();

                var pages = allPages.Select(page => new
                {
                    page.Id,
                    page.Name,
                    DateCreated = page.CreateDate
                }).ToList();

                var previousPeriodPagesGrouped = pages
                   .Where(mg => mg.DateCreated >= range.PreviousStartDate && mg.DateCreated <= range.PreviousEndDate)
                   .GroupBy(mg => mg.DateCreated.Date)
                   .ToDictionary(g => g.Key, g => g.Count());

                var currentPeriodPagesGrouped = pages
                    .Where(mg => mg.DateCreated >= range.StartDate && mg.DateCreated <= range.EndDate)
                    .GroupBy(mg => mg.DateCreated.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var previousPeriodPages = new List<int>();
                var previousPagesCreatedDates = new List<string>();

                for (DateTime date = range.PreviousStartDate.Date; date <= range.PreviousEndDate.Date; date = date.AddDays(1))
                {
                    previousPagesCreatedDates.Add(date.ToString("dd MMM"));
                    previousPeriodPages.Add(previousPeriodPagesGrouped.ContainsKey(date) ? previousPeriodPagesGrouped[date] : 0);
                }

                var currentPeriodPages = new List<int>();
                var lastPagesCreatedDates = new List<string>();

                for (DateTime date = range.StartDate.Date; date <= range.EndDate.Date; date = date.AddDays(1))
                {
                    lastPagesCreatedDates.Add(date.ToString("dd MMM"));
                    currentPeriodPages.Add(currentPeriodPagesGrouped.ContainsKey(date) ? currentPeriodPagesGrouped[date] : 0);
                }

                int totalPages = currentPeriodPages.Sum();
                double rate = CalculateRate(previousPeriodPages.Sum(), totalPages);

                return new JsonResult(new
                {
                    period,
                    totalPages,
                    rate = Math.Round(rate, 2),
                    previousPeriodPages,
                    previousPagesCreatedDates,
                    lastPagesCreated = currentPeriodPages,
                    lastPagesCreatedDates
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTotalMedias(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var mediaDetails = GetMediaDetails();

                var previousPeriodMediasGrouped = mediaDetails
                    .Where(mg => mg.CreateDate >= range.PreviousStartDate && mg.CreateDate <= range.PreviousEndDate)
                    .GroupBy(mg => mg.CreateDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var currentPeriodMediasGrouped = mediaDetails
                    .Where(mg => mg.CreateDate >= range.StartDate && mg.CreateDate <= range.EndDate)
                    .GroupBy(mg => mg.CreateDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var (previousPeriodData, previousDates) = GetDateRangeData(range.PreviousStartDate, range.PreviousEndDate, previousPeriodMediasGrouped);
                var (currentPeriodData, currentDates) = GetDateRangeData(range.StartDate, range.EndDate, currentPeriodMediasGrouped);

                int totalMedias = currentPeriodData.Sum();
                double rate = CalculateRate(previousPeriodData.Sum(), totalMedias);

                return new JsonResult(new
                {
                    period,
                    totalMedias,
                    rate = Math.Round(rate, 2),
                    previousPeriodMedias = previousPeriodData,
                    previousMediasCreatedDates = previousDates,
                    lastMediasCreated = currentPeriodData,
                    lastMediasCreatedDates = currentDates
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTotalUsers(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var members = _memberService.GetAllMembers().ToList();

                var previousPeriodMembersGrouped = members
                    .Where(mg => mg.CreateDate >= range.PreviousStartDate && mg.CreateDate <= range.PreviousEndDate)
                    .GroupBy(mg => mg.CreateDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var currentPeriodMembersGrouped = members
                    .Where(mg => mg.CreateDate >= range.StartDate && mg.CreateDate <= range.EndDate)
                    .GroupBy(mg => mg.CreateDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var (previousPeriodData, previousDates) = GetDateRangeData(range.PreviousStartDate, range.PreviousEndDate, previousPeriodMembersGrouped);
                var (currentPeriodData, currentDates) = GetDateRangeData(range.StartDate, range.EndDate, currentPeriodMembersGrouped);

                int totalMembers = currentPeriodData.Sum();
                double rate = CalculateRate(previousPeriodData.Sum(), totalMembers);

                return new JsonResult(new
                {
                    period,
                    totalMembers,
                    rate = Math.Round(rate, 2),
                    previousPeriodMembers = previousPeriodData,
                    previousMembersCreatedDates = previousDates,
                    lastMembersCreated = currentPeriodData,
                    lastMembersCreatedDates = currentDates
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetTotalGroups(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var memberGroups = _memberGroupService.GetAll().Select(mg => new
                {
                    mg.Id,
                    mg.Name,
                    mg.CreateDate,
                    mg.UpdateDate
                }).ToList();

                var previousPeriodGroupsGrouped = memberGroups
                    .Where(mg => mg.CreateDate >= range.PreviousStartDate && mg.CreateDate <= range.PreviousEndDate)
                    .GroupBy(mg => mg.CreateDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var currentPeriodGroupsGrouped = memberGroups
                    .Where(mg => mg.CreateDate >= range.StartDate && mg.CreateDate <= range.EndDate)
                    .GroupBy(mg => mg.CreateDate.Date)
                    .ToDictionary(g => g.Key, g => g.Count());

                var (previousPeriodData, previousDates) = GetDateRangeData(range.PreviousStartDate, range.PreviousEndDate, previousPeriodGroupsGrouped);
                var (currentPeriodData, currentDates) = GetDateRangeData(range.StartDate, range.EndDate, currentPeriodGroupsGrouped);

                int totalGroups = currentPeriodData.Sum();
                double rate = CalculateRate(previousPeriodData.Sum(), totalGroups);

                return new JsonResult(new
                {
                    period,
                    totalGroups,
                    rate = Math.Round(rate, 2),
                    previousPeriodGroups = previousPeriodData,
                    previousGroupsCreatedDates = previousDates,
                    lastGroupsCreated = currentPeriodData,
                    lastGroupsCreatedDates = currentDates
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetMedias(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var mediaDetails = GetMediaDetails();
                var filteredMedia = mediaDetails.Where(md => md.CreateDate >= range.StartDate && md.CreateDate <= range.EndDate).ToList();

                var allExtensions = new List<string> { "folder", "file", "image", "vector graphics (svg)", "audio", "video" };

                var groupedMedia = allExtensions
                    .Select(ext => new MediaSeries
                    {
                        Name = GetMediaTypeExtension(ext),
                        Data = new List<MediaDataPoint>
                        {
                            new MediaDataPoint
                            {
                                X = ext,
                                Y = filteredMedia.Count(md => GetMediaTypeExtension(md.Extension.ToLower()) == GetMediaTypeExtension(ext))
                            }
                        }
                    })
                    .ToList();

                return new JsonResult(new
                {
                    Period = period,
                    Series = groupedMedia,
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetMembers(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var members = _memberService.GetAllMembers()
                    .Where(member => member.CreateDate >= range.StartDate && member.CreateDate <= range.EndDate)
                    .Select(member => new MemberViewModel
                    {
                        Id = member.Id,
                        Name = member.Name,
                        Email = member.Email,
                        Group = string.Join(", ", _memberService.GetAllRoles(member.Id)),
                        LastLogin = member.LastLoginDate?.ToString("HH:mm") ?? "N/A"
                    })
                    .ToList();

                return new JsonResult(new
                {
                    Period = period,
                    Total = members.Count,
                    Members = members
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // Helper method to reduce duplication in date range calculations
        private (List<int> data, List<string> dates) GetDateRangeData(DateTime startDate, DateTime endDate, Dictionary<DateTime, int> groupedData)
        {
            var data = new List<int>();
            var dates = new List<string>();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                dates.Add(date.ToString("dd MMM"));
                data.Add(groupedData.TryGetValue(date, out var count) ? count : 0);
            }

            return (data, dates);
        }

        private double CalculateRate(int previousCount, int currentCount)
        {
            if (previousCount == 0 && currentCount == 0) return 0;
            if (previousCount == 0) return 100;
            return ((double)(currentCount - previousCount) / previousCount) * 100;
        }

        private List<DataPoint> FillMissingDates(List<string> categories, List<DataPoint> data)
        {
            var dataDict = data.ToDictionary(dp => dp.X, dp => dp.Y);
            return categories.Select(date => new DataPoint
            {
                X = date,
                Y = dataDict.TryGetValue(date, out var count) ? count : 0
            }).ToList();
        }

        private IEnumerable<IContent> GetDescendantsRecursive(IContent content)
        {
            yield return content;
            foreach (var child in _contentService.GetPagedChildren(content.Id, 0, int.MaxValue, out _))
            {
                foreach (var descendant in GetDescendantsRecursive(child))
                {
                    yield return descendant;
                }
            }
        }

        static DateTime GetMondayOfCurrentWeek(DateTime date)
        {
            int daysToSubtract = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysToSubtract < 0) daysToSubtract += 7;
            return date.AddDays(-daysToSubtract);
        }

        private List<MediaDetail> GetMediaDetails()
        {
            var mediaDetails = new List<MediaDetail>();
            var processedMediaIds = new HashSet<int>();

            var rootMedia = _mediaService.GetRootMedia();

            foreach (var rootItem in rootMedia)
            {
                mediaDetails.AddRange(GetMediaDetailsRecursive(rootItem, processedMediaIds));

                var pageIndex = 0;
                var pageSize = 100;
                long totalChildren;

                do
                {
                    var children = _mediaService.GetPagedChildren(rootItem.Id, pageIndex, pageSize, out totalChildren);
                    foreach (var child in children)
                    {
                        mediaDetails.AddRange(GetMediaDetailsRecursive(child, processedMediaIds));
                    }
                    pageIndex++;
                } while (pageIndex * pageSize < totalChildren);
            }

            return mediaDetails;
        }

        private List<MediaDetail> GetMediaDetailsRecursive(IMedia mediaItem, HashSet<int> processedMediaIds)
        {
            var mediaDetails = new List<MediaDetail>();

            if (processedMediaIds.Contains(mediaItem.Id)) return mediaDetails;
            processedMediaIds.Add(mediaItem.Id);

            mediaDetails.Add(new MediaDetail
            {
                Name = mediaItem.Name,
                Extension = mediaItem.ContentType.Name,
                CreateDate = mediaItem.CreateDate,
                prop = mediaItem
            });

            var pageIndex = 0;
            var pageSize = 100;
            long totalChildren;

            do
            {
                var children = _mediaService.GetPagedChildren(mediaItem.Id, pageIndex, pageSize, out totalChildren);
                foreach (var child in children)
                {
                    mediaDetails.AddRange(GetMediaDetailsRecursive(child, processedMediaIds));
                }
                pageIndex++;
            } while (pageIndex * pageSize < totalChildren);

            return mediaDetails;
        }

        private string GetMediaTypeExtension(string extension)
        {
            return extension.ToLower() switch
            {
                "folder" => "Folder",
                "file" => "File",
                "image" => "Image",
                "vector graphics (svg)" => "SVG",
                "svg" => "SVG",
                "audio" => "Audio",
                "video" => "Video",
                _ => "Other"
            };
        }

        [HttpGet]
        public JsonResult GetUserActivity(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var allPages = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();

                var filteredPages = allPages
                    .Where(page => page.CreateDate >= range.StartDate && page.CreateDate <= range.EndDate)
                    .ToList();

                long totalRecords;
                var deletedPages = _contentService.GetPagedContentInRecycleBin(0, int.MaxValue, out totalRecords)
                    .Where(page => page.CreateDate >= range.StartDate && page.CreateDate <= range.EndDate)
                    .ToList();

                var result = filteredPages.Select(page => new
                {
                    collaborators = GetContentContributors(page.Id),
                    page.Id,
                    page.Name,
                    DateCreated = page.CreateDate,
                    Status = !page.Published ? "Unpublished" : "Published",
                    page.UpdateDate
                }).ToList();

                var resultDeletedPages = deletedPages.Select(page => new
                {
                    collaborators = GetContentContributors(page.Id),
                    page.Id,
                    page.Name,
                    DateCreated = page.CreateDate,
                    Status = page.Trashed ? "Deleted" : "Unpublished",
                    page.UpdateDate
                }).ToList();

                return new JsonResult(new
                {
                    TotalPages = filteredPages.Count,
                    Pages = result.Concat(resultDeletedPages)
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetContentStatusUsage(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var allContent = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();

                var filteredContent = allContent
                    .Where(c => c.CreateDate >= range.StartDate && c.CreateDate <= range.EndDate)
                    .ToList();

                var deletedItems = _contentService.GetPagedContentInRecycleBin(0, int.MaxValue, out long totalContent)
                    .SelectMany(GetDescendantsRecursive)
                    .Where(c => c.CreateDate >= range.StartDate && c.CreateDate <= range.EndDate)
                    .ToList();

                var publishedCount = filteredContent.Count(c => c.Published && !c.Trashed);
                var trashedCount = deletedItems.Count;
                var unpublishedCount = filteredContent.Count(c => !c.Published && !c.Trashed && c.PublishedState == PublishedState.Unpublished);

                var totalCount = publishedCount + trashedCount + unpublishedCount;
                var publishedPercentage = totalCount > 0 ? (double)publishedCount / totalCount * 100 : 0;
                var trashedPercentage = totalCount > 0 ? (double)trashedCount / totalCount * 100 : 0;
                var unpublishedPercentage = totalCount > 0 ? (double)unpublishedCount / totalCount * 100 : 0;

                return new JsonResult(new
                {
                    Title = $"Contents Donut Chart ({period})",
                    Series = new[] { publishedCount, unpublishedCount, trashedCount },
                    Labels = new[] { "Published", "Unpublished", "Deleted" }
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        private List<DataPoint> GroupDataByLastLogDate(List<IContent> pages, string logHeader)
        {
            var lastLogEntries = new List<DataPoint>();

            foreach (var page in pages)
            {
                var logDate = _logService.GetLogDateByPageIdAndHeader(page.Id, logHeader) ?? page.CreateDate;

                lastLogEntries.Add(new DataPoint
                {
                    X = logDate.Date.ToString("yyyy-MM-dd"),
                    Y = 1
                });
            }

            return lastLogEntries
                .GroupBy(dp => dp.X)
                .Select(g => new DataPoint { X = g.Key, Y = g.Sum(dp => dp.Y) })
                .ToList();
        }

        // Add this method to your AstroboardController class
        private IEnumerable<string> GetContentContributors(int contentId)
        {
            var auditSaveItems = _auditService.GetLogs(AuditType.Save)
                .Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditPublishItems = _auditService.GetLogs(AuditType.Publish)
                .Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditUnpublishItems = _auditService.GetLogs(AuditType.Unpublish)
                .Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditDeleteItems = _auditService.GetLogs(AuditType.Delete)
                .Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditSaveVariantItems = _auditService.GetLogs(AuditType.SaveVariant)
                .Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditSendToPublishItems = _auditService.GetLogs(AuditType.SendToPublish)
                .Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();

            var userIds = auditSaveItems
                .Concat(auditPublishItems)
                .Concat(auditUnpublishItems)
                .Concat(auditDeleteItems)
                .Concat(auditSaveVariantItems)
                .Concat(auditSendToPublishItems)
                .Select(a => a.UserId)
                .Distinct();

            return userIds.Select(userId => _userService.GetUserById(userId)?.Name ?? "")
                         .Where(name => !string.IsNullOrEmpty(name));
        }

        [HttpGet]
        public JsonResult GetContentsLineChartData(string period)
        {
            try
            {
                var range = CalculatePeriodRange(period);
                var categories = new List<string>();

                for (var date = range.StartDate; date <= range.EndDate; date = date.AddDays(1))
                {
                    categories.Add(date.ToString("yyyy-MM-dd"));
                }

                var nonDeletedPages = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();
                long totalContent;
                var deletedItems = _contentService.GetPagedContentInRecycleBin(0, int.MaxValue, out totalContent)
                    .SelectMany(GetDescendantsRecursive)
                    .ToList();
                var allPages = nonDeletedPages.Concat(deletedItems);

                var previousPeriodPages = allPages
                    .Where(page => page.CreateDate >= range.PreviousStartDate && page.CreateDate < range.PreviousEndDate)
                    .ToList();
                var filteredPages = allPages
                    .Where(page => page.CreateDate >= range.StartDate && page.CreateDate <= range.EndDate)
                    .ToList();

                int pagesPerPeriod = filteredPages.Count;
                int previousPagesPerPeriod = previousPeriodPages.Count;
                double rate = CalculateRate(previousPagesPerPeriod, pagesPerPeriod);
                int contributors = filteredPages.SelectMany(page => GetContentContributors(page.Id)).Distinct().Count();
                double dailyChangeRate = (range.EndDate - range.StartDate).TotalDays > 0 ?
                    pagesPerPeriod / (range.EndDate - range.StartDate).TotalDays : 0;

                var publishedData = filteredPages.Where(page => !page.Trashed && page.PublishDate.HasValue).ToList();
                var unpublishedData = filteredPages.Where(page => !page.Published && !page.Trashed && page.PublishedState == PublishedState.Unpublished).ToList();
                var trashedData = filteredPages.Where(page => page.Trashed).ToList();

                var series = new List<Series>
                {
                    new Series
                    {
                        Name = "Published",
                        Color = "#2bc37c",
                        Data = FillMissingDates(categories, GroupDataByLastLogDate(publishedData, "Publish"))
                    },
                    new Series
                    {
                        Name = "UnPublished",
                        Color = "#ff9412",
                        Data = FillMissingDates(categories, GroupDataByLastLogDate(unpublishedData, "Unpublish"))
                    },
                    new Series
                    {
                        Name = "Deleted",
                        Color = "#d42054",
                        Data = FillMissingDates(categories, GroupDataByLastLogDate(trashedData, "Delete"))
                    }
                };

                return new JsonResult(new ContentsLineChartData
                {
                    Period = period,
                    PagesPerPeriod = pagesPerPeriod,
                    Rate = Math.Round(rate, 2),
                    Contributors = contributors,
                    DailyChangeRate = Math.Round(dailyChangeRate, 2),
                    Series = series,
                    Categories = categories
                });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(ex.Message);
            }
        }
    }

    public class MediaSeries
    {
        public string? Name { get; set; }
        public List<MediaDataPoint>? Data { get; set; }
    }

    public class MediaDataPoint
    {
        public string? X { get; set; }
        public int? Y { get; set; }
    }

    public class MediaDetail
    {
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public long Size { get; set; }
        public DateTime CreateDate { get; set; }
        public IMedia? prop { get; set; }
    }
}
