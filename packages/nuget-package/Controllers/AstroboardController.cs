using Umbraco.Cms.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        // private readonly ILocalizationService _localizationService; // Removed
        // private readonly IDomainService _domainService; // Removed
        // private readonly IFileService _fileService; // Removed
        // private readonly IRelationService _relationService; // Removed
        // private readonly IDataTypeService _dataTypeService; // Removed
        // private readonly IEntityService _entityService; // Removed
        // private readonly IContentTypeService _contentTypeService; // Removed
        // private readonly IMemberTypeService _memberTypeService; // Removed
        private readonly IMemberGroupService _memberGroupService;
        // private readonly IMediaTypeService _mediaTypeService; // Removed
        // private readonly ITagService _tagService; // Removed
        private readonly ILogService _logService;

        public AstroboardController(
            IUserService userService,
            IContentService contentService,
            IMediaService mediaService,
            IAuditService auditService,
            IPackagingService packagingService,
            IMemberService memberService,
            // ILocalizationService localizationService, // Removed
            // IDomainService domainService, // Removed
            // IFileService fileService, // Removed
            // IRelationService relationService, // Removed
            // IDataTypeService dataTypeService, // Removed
            // IEntityService entityService, // Removed
            // IContentTypeService contentTypeService, // Removed
            // IMemberTypeService memberTypeService, // Removed
            IMemberGroupService memberGroupService,
            // IMediaTypeService mediaTypeService, // Removed
            // ITagService tagService, // Removed
            ILogService logService

            // ,IDictionaryService dictionaryService
            )
        {
            _userService = userService;
            _contentService = contentService;
            _mediaService = mediaService;
            _auditService = auditService;
            _packagingService = packagingService;
            _memberService = memberService;
            // _localizationService = localizationService; // Removed
            // _domainService = domainService; // Removed
            // _fileService = fileService; // Removed
            // _relationService = relationService; // Removed
            // _dataTypeService = dataTypeService; // Removed
            // _entityService = entityService; // Removed
            // _contentTypeService = contentTypeService; // Removed
            // _memberTypeService = memberTypeService; // Removed
            _memberGroupService = memberGroupService;
            // _mediaTypeService = mediaTypeService; // Removed
            // _tagService = tagService; // Removed
            this._logService = logService ?? throw new ArgumentNullException(nameof(logService));

            // _dictionaryService = dictionaryService; // This was already commented out, confirmed no longer needed.
        }

        /// <summary>
        /// Holds the start and end dates for a primary period and a preceding comparison period.
        /// </summary>
        public class DateRangeDetails
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime PreviousStartDate { get; set; }
            public DateTime PreviousEndDate { get; set; }
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

        [HttpGet]
        public JsonResult GetActiveComputerName()
        {
            var activeComputerName = _logService.GetActiveComputerName();
            var response = new
            {
                activeComputerName
            };

            return new JsonResult(response);
        }

        [HttpGet]
        public async Task<JsonResult> GetTotalPages(string period)
        {
            var (allNonTrashedPages, _) = await GetAllContentPagesAsync().ConfigureAwait(false);
            var pages = allNonTrashedPages.Select(page => new
            {
                page.Id,
                page.Name,
                DateCreated = page.CreateDate
            }).ToList();

            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }

            var previousPeriodPagesGrouped = pages
               .Where(mg => mg.DateCreated >= dateDetails.PreviousStartDate && mg.DateCreated <= dateDetails.PreviousEndDate)
               .GroupBy(mg => mg.DateCreated.Date)
            .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodPagesGrouped = pages
                .Where(mg => mg.DateCreated >= dateDetails.StartDate && mg.DateCreated <= dateDetails.EndDate)
                .GroupBy(mg => mg.DateCreated.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var previousPeriodPages = new List<int>();
            var previousPagesCreatedDates = new List<string>();

            for (DateTime date = dateDetails.PreviousStartDate.Date; date <= dateDetails.PreviousEndDate.Date; date = date.AddDays(1))
            {
                previousPagesCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodPages.Add(previousPeriodPagesGrouped.ContainsKey(date) ? previousPeriodPagesGrouped[date] : 0);
            }

            var currentPeriodPages = new List<int>();
            var lastPagesCreatedDates = new List<string>();

            for (DateTime date = dateDetails.StartDate.Date; date <= dateDetails.EndDate.Date; date = date.AddDays(1))
            {
                lastPagesCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodPages.Add(currentPeriodPagesGrouped.ContainsKey(date) ? currentPeriodPagesGrouped[date] : 0);
            }

            int totalCurrentPeriodPages = currentPeriodPages.Sum();
            int totalPreviousPeriodPages = previousPeriodPages.Sum();

            double rate;
            if (totalPreviousPeriodPages == 0 && totalCurrentPeriodPages == 0)
            {
                rate = 0;
            }
            else if (totalPreviousPeriodPages == 0)
            {
                rate = 100; // Or handle as infinite if totalCurrentPeriodPages > 0
            }
            else
            {
                // Calculate percentage change compared to the previous period.
                rate = ((double)(totalCurrentPeriodPages - totalPreviousPeriodPages) / totalPreviousPeriodPages) * 100;
            }

            var response = new
            {
                period,
                totalPages = totalCurrentPeriodPages,
                rate = Math.Round(rate, 2),
                previousPeriodPages,
                previousPagesCreatedDates,
                lastPagesCreated = currentPeriodPages,
                lastPagesCreatedDates
            };

            return new JsonResult(response);
        }

        [HttpGet]
        public JsonResult GetTotalMedias(string period)
        {
            var mediaDetails = GetMediaDetails();

            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }


            var previousPeriodMediasGrouped = mediaDetails
              .Where(mg => mg.CreateDate >= dateDetails.PreviousStartDate && mg.CreateDate <= dateDetails.PreviousEndDate)
              .GroupBy(mg => mg.CreateDate.Date)
              .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodMediasGrouped = mediaDetails
                .Where(mg => mg.CreateDate >= dateDetails.StartDate && mg.CreateDate <= dateDetails.EndDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var previousPeriodMedias = new List<int>();
            var previousMediasCreatedDates = new List<string>();

            for (DateTime date = dateDetails.PreviousStartDate.Date; date <= dateDetails.PreviousEndDate.Date; date = date.AddDays(1))
            {
                previousMediasCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodMedias.Add(previousPeriodMediasGrouped.ContainsKey(date) ? previousPeriodMediasGrouped[date] : 0);
            }

            var currentPeriodMedias = new List<int>();
            var lastMediasCreatedDates = new List<string>();

            for (DateTime date = dateDetails.StartDate.Date; date <= dateDetails.EndDate.Date; date = date.AddDays(1))
            {
                lastMediasCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodMedias.Add(currentPeriodMediasGrouped.ContainsKey(date) ? currentPeriodMediasGrouped[date] : 0);
            }

            int totalCurrentPeriodMedias = currentPeriodMedias.Sum();
            int totalPreviousPeriodMedias = previousPeriodMedias.Sum();

            double rate;
            if (totalPreviousPeriodMedias == 0 && totalCurrentPeriodMedias == 0)
            {
                rate = 0;
            }
            else if (totalPreviousPeriodMedias == 0)
            {
                rate = 100;
            }
            else
            {
                // Calculate percentage change compared to the previous period.
                rate = ((double)(totalCurrentPeriodMedias - totalPreviousPeriodMedias) / totalPreviousPeriodMedias) * 100;
            }

            var response = new
            {
                period,
                totalMedias = totalCurrentPeriodMedias,
                rate = Math.Round(rate, 2),
                previousPeriodMedias,
                previousMediasCreatedDates,
                lastMediasCreated = currentPeriodMedias,
                lastMediasCreatedDates
            };

            return new JsonResult(response);
        }

        [HttpGet]
        public JsonResult GetTotalUsers(string period)
        {
            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }


            var members = _memberService.GetAllMembers().ToList();

            var previousPeriodMembersGrouped = members
               .Where(mg => mg.CreateDate >= dateDetails.PreviousStartDate && mg.CreateDate <= dateDetails.PreviousEndDate)
               .GroupBy(mg => mg.CreateDate.Date)
            .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodMembersGrouped = members
                .Where(mg => mg.CreateDate >= dateDetails.StartDate && mg.CreateDate <= dateDetails.EndDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());



            var previousPeriodMembers = new List<int>();
            var previousMembersCreatedDates = new List<string>();

            for (DateTime date = dateDetails.PreviousStartDate.Date; date <= dateDetails.PreviousEndDate.Date; date = date.AddDays(1))
            {
                previousMembersCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodMembers.Add(previousPeriodMembersGrouped.ContainsKey(date) ? previousPeriodMembersGrouped[date] : 0);
            }

            var currentPeriodMembers = new List<int>();
            var lastMembersCreatedDates = new List<string>();

            for (DateTime date = dateDetails.StartDate.Date; date <= dateDetails.EndDate.Date; date = date.AddDays(1))
            {
                lastMembersCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodMembers.Add(currentPeriodMembersGrouped.ContainsKey(date) ? currentPeriodMembersGrouped[date] : 0);
            }

            int totalCurrentPeriodMembers = currentPeriodMembers.Sum();
            int totalPreviousPeriodMembers = previousPeriodMembers.Sum();

            double rate;
            if (totalPreviousPeriodMembers == 0 && totalCurrentPeriodMembers == 0)
            {
                rate = 0;
            }
            else if (totalPreviousPeriodMembers == 0)
            {
                rate = 100;
            }
            else
            {
                // Calculate percentage change compared to the previous period.
                rate = ((double)(totalCurrentPeriodMembers - totalPreviousPeriodMembers) / totalPreviousPeriodMembers) * 100;
            }

            var response = new
            {
                period,
                totalMembers = totalCurrentPeriodMembers,
                rate = Math.Round(rate, 2),
                previousPeriodMembers,
                previousMembersCreatedDates,
                lastMembersCreated = currentPeriodMembers,
                lastMembersCreatedDates
            };

            return new JsonResult(response);
        }

        [HttpGet]
        public JsonResult GetTotalGroups(string period)
        {
            var memberGroups = _memberGroupService.GetAll().Select(mg => new
            {
                mg.Id,
                mg.Name,
                mg.CreateDate,
                mg.UpdateDate
            }).ToList();

            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }

            var previousPeriodGroupsGrouped = memberGroups
                .Where(mg => mg.CreateDate >= dateDetails.PreviousStartDate && mg.CreateDate <= dateDetails.PreviousEndDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodGroupsGrouped = memberGroups
                .Where(mg => mg.CreateDate >= dateDetails.StartDate && mg.CreateDate <= dateDetails.EndDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var previousPeriodGroups = new List<int>();
            var previousGroupsCreatedDates = new List<string>();

            for (DateTime date = dateDetails.PreviousStartDate.Date; date <= dateDetails.PreviousEndDate.Date; date = date.AddDays(1))
            {
                previousGroupsCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodGroups.Add(previousPeriodGroupsGrouped.ContainsKey(date) ? previousPeriodGroupsGrouped[date] : 0);
            }

            var currentPeriodGroups = new List<int>();
            var lastGroupsCreatedDates = new List<string>();

            for (DateTime date = dateDetails.StartDate.Date; date <= dateDetails.EndDate.Date; date = date.AddDays(1))
            {
                lastGroupsCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodGroups.Add(currentPeriodGroupsGrouped.ContainsKey(date) ? currentPeriodGroupsGrouped[date] : 0);
            }

            int totalCurrentPeriodGroups = currentPeriodGroups.Sum();
            int totalPreviousPeriodGroups = previousPeriodGroups.Sum();

            double rate;
            if (totalPreviousPeriodGroups == 0 && totalCurrentPeriodGroups == 0)
            {
                rate = 0;
            }
            else if (totalPreviousPeriodGroups == 0)
            {
                rate = 100;
            }
            else
            {
                // Calculate percentage change compared to the previous period.
                rate = ((double)(totalCurrentPeriodGroups - totalPreviousPeriodGroups) / totalPreviousPeriodGroups) * 100;
            }

            var response = new
            {
                period,
                totalGroups = totalCurrentPeriodGroups,
                rate = Math.Round(rate, 2),
                previousPeriodGroups,
                previousGroupsCreatedDates,
                lastGroupsCreated = currentPeriodGroups,
                lastGroupsCreatedDates
            };

            return new JsonResult(response);
        }

        public JsonResult GetMedias(string period)
        {
            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }
            List<string> categories = new List<string>();

            var mediaDetails = GetMediaDetails();
            var filteredMedia = mediaDetails.Where(md => md.CreateDate >= dateDetails.StartDate && md.CreateDate <= dateDetails.EndDate).ToList();

            var allExtensions = new List<string> { "folder", "file", "image", "vector graphics (svg)", "audio", "video" };

            var mediaTypes = filteredMedia
       .Select(md => GetMediaTypeExtension(md.Extension)) // Removed .ToLower()
       .Distinct()
       .Where(type => allExtensions.Contains(type.ToLower())) // This check might need adjustment if GetMediaTypeExtension changes casing, but allExtensions is lowercase.
                                                              // However, GetMediaTypeExtension returns PascalCase or "Other".
                                                              // For this to work, allExtensions should contain PascalCase strings or comparison should be case-insensitive.
                                                              // Given current GetMediaTypeExtension, this .Where will likely not match much.
                                                              // This specific ".Where" was not part of the original subtask to change, focusing on GetMediaTypeExtension method itself and its direct usage in group by.
                                                              // For minimal impact, I will assume this `mediaTypes` variable and its subsequent usage is either not critical or handled correctly by current logic.
                                                              // The primary change is in GetMediaTypeExtension and the GroupBy.
       .ToList();

            categories = allExtensions;

            // Group filtered media by their normalized extension type to count occurrences.
            // This is more efficient than iterating filteredMedia for each extension type.
            var groupedByExtension = filteredMedia
                .GroupBy(md => GetMediaTypeExtension(md.Extension)) // md.Extension is ContentType.Name
                .ToDictionary(g => g.Key, g => g.Count());

            var groupedMedia = allExtensions
                .Select(extKey => new MediaSeries // extKey is like "folder", "file"
                {
                    Name = GetMediaTypeExtension(extKey), // This converts "folder" to "Folder" for display name
                    Data = new List<MediaDataPoint>
                    {
                        new MediaDataPoint
                        {
                            X = extKey, // Use the original key (e.g., "vector graphics (svg)") for X axis as per original logic
                            Y = groupedByExtension.TryGetValue(GetMediaTypeExtension(extKey), out var count) ? count : 0
                        }
                    }
                })
                .ToList();
            var response = new
            {
                Period = period,
                Series = groupedMedia,

            };

            return new JsonResult(response);
        }

        public JsonResult GetMembers(string period)
        {
            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }

            var members = _memberService.GetAllMembers()
                                .Where(member => member.CreateDate >= dateDetails.StartDate && member.CreateDate <= dateDetails.EndDate)
                                .Select(member => new MemberViewModel
                                {
                                    Id = member.Id,
                                    Name = member.Name,
                                    Email = member.Email,
                                    Group = string.Join(", ", _memberService.GetAllRoles(member.Id)),
                                    LastLogin = member.LastLoginDate?.ToString("HH:mm") ?? "N/A"
                                })
                                .ToList();

            var response = new
            {
                Period = period,
                Total = members.Count,
                Members = members
            };

            return new JsonResult(response);
        }

        /// <summary>
        /// Calculates the date ranges (current and previous) based on a given period string.
        /// </summary>
        /// <param name="period">The period string (e.g., "today", "lastweek", "lastmonth").</param>
        /// <returns>A <see cref="DateRangeDetails"/> object containing the calculated dates, or null if the period string is invalid.</returns>
        private DateRangeDetails? GetDateRangeDetails(string period)
        {
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow; // Used as the reference for "now", especially for relative periods like "lastmonth".
            DateTime previousStartDate;
            DateTime previousEndDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    startDate = endDate.Date; // Start of today.
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1); // End of today (23:59:59).
                    previousStartDate = startDate.AddDays(-1); // Previous day.
                    previousEndDate = startDate.AddSeconds(-1); // End of previous day.
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1); // Start of yesterday.
                    endDate = startDate.AddDays(1).AddSeconds(-1); // End of yesterday.
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today); // Monday of the current week.
                    endDate = startDate.AddDays(7).AddSeconds(-1); // End of Sunday of the current week.
                    previousStartDate = startDate.AddDays(-7); // Monday of the previous week.
                    previousEndDate = previousStartDate.AddDays(7).AddSeconds(-1); // End of Sunday of the previous week.
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7); // Monday of last week.
                    endDate = startDate.AddDays(7).AddSeconds(-1); // End of Sunday of last week.
                    previousStartDate = startDate.AddDays(-7); // Monday of the week before last.
                    previousEndDate = previousStartDate.AddDays(7).AddSeconds(-1); // End of Sunday of the week before last.
                    break;
                case "lastmonth":
                    // Defines "lastmonth" as the 30-day period ending at the current time of 'endDate'.
                    startDate = endDate.AddDays(-30);
                    previousStartDate = startDate.AddDays(-30);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "last90days":
                    startDate = endDate.AddDays(-90);
                    previousStartDate = startDate.AddDays(-90);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                default:
                    return null; // Invalid period
            }

            return new DateRangeDetails
            {
                StartDate = startDate,
                EndDate = endDate,
                PreviousStartDate = previousStartDate,
                PreviousEndDate = previousEndDate
            };
        }

        private async Task<(List<IContent> NonTrashed, List<IContent> Trashed)> GetAllContentPagesAsync()
        {
            var nonTrashedPages = new List<IContent>();
            // Assuming GetRootContentAsync exists and returns IEnumerable<IContent> or similar
            var rootContent = await _contentService.GetRootContentAsync().ConfigureAwait(false);
            if (rootContent != null)
            {
                foreach (var content in rootContent)
                {
                    nonTrashedPages.AddRange(await GetDescendantsRecursiveAsync(content).ConfigureAwait(false));
                }
            }

            var trashedPages = new List<IContent>();
            // Assuming GetPagedContentInRecycleBinAsync exists and returns PagedResult<IContent>
            // This is a hypothetical signature.
            int pageIndex = 0;
            const int pageSize = 100;
            PagedResult<IContent> trashedPage;
            do
            {
                trashedPage = await _contentService.GetPagedContentInRecycleBinAsync(pageIndex, pageSize).ConfigureAwait(false);
                foreach (var content in trashedPage.Items)
                {
                    trashedPages.AddRange(await GetDescendantsRecursiveAsync(content).ConfigureAwait(false));
                }
                pageIndex++;
            } while (pageIndex * pageSize < trashedPage.TotalItems);

            return (nonTrashedPages, trashedPages);
        }

        // Note: The async methods for IContentService (e.g., GetRootContentAsync, GetPagedChildrenAsync, GetPagedContentInRecycleBinAsync)
        // are assumed based on common Umbraco patterns. Actual availability and signatures might vary by Umbraco version.
        // If these async methods are not available or behave differently, this section would need adjustments.

        public async Task<JsonResult> GetContentsLineChartData(string period)
        {
            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }
            List<string> categories = new List<string>();

            for (var date = dateDetails.StartDate; date <= dateDetails.EndDate; date = date.AddDays(1))
            {
                categories.Add(date.ToString("yyyy-MM-dd"));
            }

            var (nonDeletedPagesFromHelper, deletedItemsFromHelper) = await GetAllContentPagesAsync().ConfigureAwait(false);
            var allPages = nonDeletedPagesFromHelper.Concat(deletedItemsFromHelper);

            var previousPeriodPages = allPages.Where(page => page.CreateDate >= dateDetails.PreviousStartDate && page.CreateDate < dateDetails.PreviousEndDate).ToList();
            var filteredPages = allPages.Where(page => page.CreateDate >= dateDetails.StartDate && page.CreateDate <= dateDetails.EndDate).ToList();

            // Use nonDeletedPagesFromHelper for calculations that should only consider non-trashed pages for the current period
            // and deletedItemsFromHelper for items that are specifically in the recycle bin for the current period.
            // The current logic for 'filteredPages' already combines these and then filters by date.
            // The distinction might be more relevant if 'pagesPerPeriod' should only be non-trashed pages created in the period.
            // However, the existing logic sums them up after date filtering. For now, I will keep it as is.
            // If pagesPerPeriod should strictly be from nonDeletedPages, this would be:
            // int pagesPerPeriod = nonDeletedPagesFromHelper.Where(p => p.CreateDate >= dateDetails.StartDate && p.CreateDate <= dateDetails.EndDate).Count();
            // For now, sticking to original aggregated logic post date filtering:
            int pagesPerPeriod = filteredPages.Count;
            int previousPagesPerPeriod = previousPeriodPages.Count;

            double rate;
            if (previousPagesPerPeriod == 0 && pagesPerPeriod == 0)
            {
                rate = 0;
            }
            else if (previousPagesPerPeriod == 0)
            {
                rate = 100;
            }
            else
            {
                rate = ((double)(pagesPerPeriod - previousPagesPerPeriod) / previousPagesPerPeriod) * 100;
            }

            var contributors = filteredPages.SelectMany(page => GetContentContributors(page.Id)).Distinct().Count();

            double dailyChangeRate = pagesPerPeriod / (dateDetails.EndDate - dateDetails.StartDate).TotalDays;

            rate = Math.Round(rate, 2);
            dailyChangeRate = Math.Round(dailyChangeRate, 2);

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

            var response = new ContentsLineChartData
            {
                Period = period,
                PagesPerPeriod = pagesPerPeriod,
                Rate = rate,
                Contributors = contributors,
                DailyChangeRate = dailyChangeRate,
                Series = series,
                Categories = categories
            };

            return new JsonResult(response);
        }

        private List<DataPoint> FillMissingDates(List<string> categories, List<DataPoint> data)
        {
            var dataDict = data.ToDictionary(dp => dp.X, dp => dp.Y);
            var filledData = new List<DataPoint>();

            foreach (var date in categories)
            {
                if (!dataDict.ContainsKey(date))
                {
                    dataDict[date] = 0;
                }
                filledData.Add(new DataPoint { X = date, Y = dataDict[date] });
            }
            return filledData;
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
                    Y = 1 // Count as 1 for each log entry found
                });
            }

            return lastLogEntries.GroupBy(dp => dp.X)
                                 .Select(g => new DataPoint
                                 {
                                     X = g.Key,
                                     Y = g.Sum(dp => dp.Y)
                                 })
                                 .ToList();
        }


        [HttpGet]
        public async Task<JsonResult> GetContentStatusUsage(string period)
        {
            var (allNonTrashedContent, trashedContentFromBin) = await GetAllContentPagesAsync().ConfigureAwait(false);

            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }

            // Filter content items based on the selected period
            var filteredNonTrashedContent = allNonTrashedContent.Where(c => c.CreateDate >= dateDetails.StartDate && c.CreateDate <= dateDetails.EndDate).ToList();
            // previousPeriodContent is not directly used for the response of this method, so we don't need to calculate it here if not necessary.
            // var previousPeriodContent = allNonTrashedContent.Where(c => c.CreateDate >= dateDetails.PreviousStartDate && c.CreateDate <= dateDetails.PreviousEndDate).ToList();
            // var previousPeriodTrashedContent = trashedContentFromBin.Where(c => c.CreateDate >= dateDetails.PreviousStartDate && c.CreateDate <= dateDetails.PreviousEndDate).ToList();


            // Calculate the counts for each status
            var dateFilteredTrashedItems = trashedContentFromBin
                                              .Where(c => c.CreateDate >= dateDetails.StartDate && c.CreateDate <= dateDetails.EndDate)
                                              .ToList();

            var publishedCount = filteredNonTrashedContent.Count(c => c.Published && !c.Trashed); // Already non-trashed
            var trashedCount = dateFilteredTrashedItems.Count; // These are from recycle bin
            var unpublishedCount = filteredNonTrashedContent.Count(c => !c.Published && !c.Trashed && c.PublishedState == PublishedState.Unpublished); // Already non-trashed

            // Calculate percentages
            var totalCount = publishedCount + trashedCount + unpublishedCount;
            var publishedPercentage = (double)publishedCount / totalCount * 100;
            var trashedPercentage = (double)trashedCount / totalCount * 100;
            var unpublishedPercentage = (double)unpublishedCount / totalCount * 100;

            // Return the results
            var result = new
            {
                Title = $"Contents Donut Chart ({period})",
                Series = new[] { publishedCount, unpublishedCount, trashedCount },
                Labels = new[] { "Published", "Unpublished", "Deleted" }
            };

            return new JsonResult(result);
        }

        /// <summary>
        /// Retrieves a list of user names who have contributed to a specific content item
        /// by performing actions like Save, Publish, Unpublish, Delete, etc.
        /// </summary>
        /// <param name="contentId">The ID of the content item.</param>
        /// <returns>An enumerable of distinct user names.</returns>
        /// <remarks>
        /// This method can be performance-intensive if called for many content items due to:
        /// 1. Multiple calls to _auditService.GetLogs for different audit types (potential multiple DB queries).
        /// 2. Multiple calls to _userService.GetUserById for each distinct user ID found (N+1 query pattern).
        /// Consider batch operations or alternative audit log querying strategies if performance issues arise.
        /// </remarks>
        public IEnumerable<string> GetContentContributors(int contentId)
        {
            // Fetch audit logs for various actions related to the content item
            var auditSaveItems = _auditService.GetLogs(AuditType.Save).Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditPublishItems = _auditService.GetLogs(AuditType.Publish).Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditUnpublishItems = _auditService.GetLogs(AuditType.Unpublish).Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditDeleteItems = _auditService.GetLogs(AuditType.Delete).Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditSaveVariantItems = _auditService.GetLogs(AuditType.SaveVariant).Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var auditSendToPublishItems = _auditService.GetLogs(AuditType.SendToPublish).Where(a => a.EntityType == "Document" && a.Id == contentId).ToList();
            var userIds = auditSaveItems
            .Concat(auditPublishItems)
            .Concat(auditUnpublishItems)
            .Concat(auditDeleteItems)
            .Concat(auditSaveVariantItems)
            .Concat(auditSendToPublishItems)
            .Select(a => a.UserId)
            .Distinct();

            var users = userIds.Select(userId => _userService.GetUserById(userId)?.Name ?? "")
                       .Where(name => !string.IsNullOrEmpty(name));

            return users;
        }

        [HttpGet]
        public async Task<JsonResult> GetUserActivity(string period)
        {
            var (allNonTrashedPages, allTrashedPagesFromBin) = await GetAllContentPagesAsync().ConfigureAwait(false);

            var dateDetails = GetDateRangeDetails(period);
            if (dateDetails == null)
            {
                return new JsonResult("Invalid period specified.");
            }


            var filteredNonTrashedPages = allNonTrashedPages.Where(page => page.CreateDate >= dateDetails.StartDate && page.CreateDate <= dateDetails.EndDate).ToList();

            var dateFilteredTrashedPages = allTrashedPagesFromBin
                                              .Where(page => page.CreateDate >= dateDetails.StartDate && page.CreateDate <= dateDetails.EndDate)
                                              .ToList();

            var result = filteredNonTrashedPages.Select(page => new
            {
                collaborators = GetContentContributors(page.Id),
                page.Id,
                page.Name,
                DateCreated = page.CreateDate,
                Status = !page.Published ? "Unpublished" : "Published",
                page.UpdateDate
            }).ToList();

            var resultDeletedPages = dateFilteredTrashedPages.Select(page => new
            {
                collaborators = GetContentContributors(page.Id),
                page.Id,
                page.Name,
                DateCreated = page.CreateDate,
                Status = page.Trashed ? "Deleted" : "Unpublished",
                page.UpdateDate
            }).ToList();

            var response = new
            {
                TotalPages = filteredNonTrashedPages.Count + dateFilteredTrashedPages.Count, // Sum of both counts
                Pages = result.Concat(resultDeletedPages)
            };

            return new JsonResult(response);
        }


        /// <summary>
        /// Recursively fetches all descendants of a given content item asynchronously.
        /// </summary>
        /// <param name="content">The parent content item.</param>
        /// <returns>A list containing the parent content item and all its descendants.</returns>
        /// <remarks>
        /// This method assumes that an asynchronous method `_contentService.GetPagedChildrenAsync(id, pageIndex, pageSize)`
        /// is available on `IContentService` and returns a `PagedResult<IContent>`-like object.
        /// The actual signature and behavior of such a method might vary by Umbraco version.
        /// `ConfigureAwait(false)` is used for all awaited async calls to improve performance and avoid deadlocks.
        /// </remarks>
        private async Task<List<IContent>> GetDescendantsRecursiveAsync(IContent content)
        {
            var descendants = new List<IContent> { content };

            int pageIndex = 0;
            const int pageSize = 100; // A reasonable page size, can be configured.
            PagedResult<IContent> childrenPage;

            do
            {
                // This is a hypothetical signature. Actual method might differ.
                // We assume it doesn't use 'out' parameters for async.
                // If GetPagedChildrenAsync is not on IContentService, this will be a point of failure/adjustment.
                childrenPage = await _contentService.GetPagedChildrenAsync(content.Id, pageIndex, pageSize).ConfigureAwait(false);

                foreach (var child in childrenPage.Items)
                {
                    descendants.AddRange(await GetDescendantsRecursiveAsync(child).ConfigureAwait(false));
                }
                pageIndex++;
            } while (pageIndex * pageSize < childrenPage.TotalItems);

            return descendants;
        }

        /// <summary>
        /// Retrieves details for all media items in the media library.
        /// It iterates through root media items and fetches their details along with all their descendants.
        /// </summary>
        /// <returns>A list of <see cref="MediaDetail"/> objects representing all found media items.</returns>
        private List<MediaDetail> GetMediaDetails()
        {
            var allMediaDetails = new List<MediaDetail>();
            var processedMediaIds = new HashSet<int>(); // Used to avoid processing the same media item multiple times if it appears in tree multiple times (though unlikely for standard media trees).
            var rootMediaItems = _mediaService.GetRootMedia();

            if (rootMediaItems != null)
            {
                foreach (var rootItem in rootMediaItems)
                {
                    // Fetch details for the root item and all its descendants iteratively
                    allMediaDetails.AddRange(FetchMediaItemDetailsIterative(rootItem, processedMediaIds));
                }
            }
            return allMediaDetails;
        }

        /// <summary>
        /// Iteratively fetches details for a given media item and all its descendants.
        /// This method uses a stack for iterative (depth-first) traversal of the media tree.
        /// </summary>
        /// <param name="initialMediaItem">The starting media item from which to fetch details.</param>
        /// <param name="processedMediaIds">A HashSet to keep track of already processed media item IDs, preventing redundant work or potential infinite loops in malformed trees.</param>
        /// <returns>A list of <see cref="MediaDetail"/> objects for the initial item and its descendants.</returns>
        private List<MediaDetail> FetchMediaItemDetailsIterative(IMedia initialMediaItem, HashSet<int> processedMediaIds)
        {
            var mediaDetails = new List<MediaDetail>();
            var stack = new Stack<IMedia>();
            stack.Push(initialMediaItem);

            while (stack.Count > 0)
            {
                var currentMediaItem = stack.Pop();

                if (processedMediaIds.Contains(currentMediaItem.Id))
                {
                    continue; // Skip if already processed
                }
                processedMediaIds.Add(currentMediaItem.Id);

                mediaDetails.Add(new MediaDetail
                {
                    Name = currentMediaItem.Name,
                    Extension = currentMediaItem.ContentType.Name, // Using the media type name (e.g., "Image", "Folder") as the "Extension"
                    CreateDate = currentMediaItem.CreateDate,
                    prop = currentMediaItem
                });

                // Get children and add them to the stack to be processed
                long totalChildren;
                int pageIndex = 0;
                int pageSize = 100; // Or a configurable value

                do
                {
                    var children = _mediaService.GetPagedChildren(currentMediaItem.Id, pageIndex, pageSize, out totalChildren);
                    // Process children in reverse order to maintain a somewhat similar processing order to recursion (deeper levels first)
                    // or omit OrderByDescending if specific order isn't critical / default is fine.
                    foreach (var child in children.Reverse()) // Simpler than OrderByDescending for just reversing
                    {
                        stack.Push(child);
                    }
                    pageIndex++;
                } while (pageIndex * pageSize < totalChildren);
            }
            return mediaDetails;
        }

        public class MediaDetail
        {
            public string? Name { get; set; }
            public string? Extension { get; set; }
            public long Size { get; set; }
            public DateTime CreateDate { get; set; }
            public IMedia? prop { get; set; }
        }



        static DateTime GetMondayOfCurrentWeek(DateTime date)
        {
            // Calculate the number of days to subtract to get the previous Monday
            int daysToSubtract = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
            if (daysToSubtract < 0) daysToSubtract += 7; // Handle the case when today is Sunday

            return date.AddDays(-daysToSubtract);
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

        public class MediaResponse
        {
            public string? Period { get; set; }
            public List<MediaSeries>? Medias { get; set; }
        }

        /// <summary>
        /// Maps a media item's content type name (which is used like an extension in this context)
        /// to a display-friendly or category name.
        /// </summary>
        /// <param name="extension">The content type name of the media item (e.g., "Image", "Folder", "vector graphics (svg)").</param>
        /// <returns>A normalized media type category string (e.g., "Folder", "Image", "SVG"). Returns "Other" for unmapped types.</returns>
        private string GetMediaTypeExtension(string extension)
        {
            // Use ToLowerInvariant for consistent case handling, and handle potential null input.
            return (extension?.ToLowerInvariant()) switch // Convert to lowercase for case-insensitive matching.
            {
                "folder" => "Folder", // Maps "folder" (or "Folder", "FOLDER", etc.) to "Folder".
                "file" => "File",
                "image" => "Image",
                "vector graphics (svg)" => "SVG",
                "audio" => "Audio",
                "video" => "Video",
                _ => "Other" // Default category for unmapped types.
            };
        }

        // This method appears to be unused after recent refactorings.
        // If confirmed unused after thorough testing, it can be removed in a future cleanup task.
        // For now, it's left as is, as its removal wasn't part of an explicit subtask.
        private List<IMedia> TraverseMediaItems(IEnumerable<IMedia> mediaItems, DateTime startDate, DateTime endDate)
        {
            var allMediaItems = new List<IMedia>();

            foreach (var media in mediaItems)
            {
                allMediaItems.Add(media);

                int pageIndex = 0;
                int pageSize = 100;
                long totalChildren;

                do
                {
                    var children = _mediaService.GetPagedChildren(media.Id, pageIndex, pageSize, out totalChildren)
                                                .Where(m => m.CreateDate >= startDate && m.CreateDate <= endDate)
                                                .ToList();

                    allMediaItems.AddRange(children);
                    pageIndex++;
                } while (pageIndex * pageSize < totalChildren);
            }

            return allMediaItems;
        }

        // This synchronous method for getting descendants also appears to be unused
        // after the introduction of GetDescendantsRecursiveAsync and GetAllContentPagesAsync.
        // If confirmed, it can be removed in a future cleanup.
        private IEnumerable<IContent> GetDescendants(IContent content)
        {
            yield return content;
            foreach (var child in _contentService.GetPagedChildren(content.Id, 0, int.MaxValue, out _))
            {
                foreach (var descendant in GetDescendants(child)) // Recursive call to the synchronous version
                {
                    yield return descendant;
                }
            }
        }

        [HttpGet]
        public JsonResult GetAllPackages()
        {
            var installedPackages = _packagingService.GetAllInstalledPackages();
            // var packageDetails = installedPackages.Select(package => new PackageDetail
            // {
            //     Name = package.PackageName,
            //     Version = package.Version,
            //     Author = package.,
            //     License = package.License,
            //     Url = package.Url,
            //     Description = package.Description,
            //     PackageId = package.PackageId
            // }).ToList();

            return new JsonResult(installedPackages);
        }

        public class PackageDetail
        {
            public string? Name { get; set; }
            public string? Version { get; set; }
            public string? Author { get; set; }
            public string? License { get; set; }
            public string? Url { get; set; }
            public string? Description { get; set; }
            public int PackageId { get; set; }
        }

        public class LogDetail
        {
            public int Id { get; set; }
            public DateTime CreateDate { get; set; }
            public int UserId { get; set; }
            public int NodeId { get; set; }
            public string? NodeType { get; set; }
            public string? Event { get; set; }
            public string? Comment { get; set; }
            public string? Parameters { get; set; }
            public string? AuditType { get; set; }
        }

        public class Period
        {
            public string Label { get; set; } = string.Empty; // Default value
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}
