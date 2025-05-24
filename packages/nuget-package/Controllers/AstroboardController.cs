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
        private readonly ILocalizationService _localizationService;
        private readonly IDomainService _domainService;
        private readonly IFileService _fileService;
        private readonly IRelationService _relationService;
        private readonly IDataTypeService _dataTypeService;
        private readonly IEntityService _entityService;
        private readonly IContentTypeService _contentTypeService;
        private readonly IMemberTypeService _memberTypeService;
        private readonly IMemberGroupService _memberGroupService;
        private readonly IMediaTypeService _mediaTypeService;
        private readonly ITagService _tagService;
        private readonly ILogService _logService;

        public AstroboardController(
            IUserService userService,
            IContentService contentService,
            IMediaService mediaService,
            IAuditService auditService,
            IPackagingService packagingService,
            IMemberService memberService,
            ILocalizationService localizationService,
            IDomainService domainService,
            IFileService fileService,
            IRelationService relationService,
            IDataTypeService dataTypeService,
            IEntityService entityService,
            IContentTypeService contentTypeService,
            IMemberTypeService memberTypeService,
            IMemberGroupService memberGroupService,
            IMediaTypeService mediaTypeService,
            ITagService tagService,
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
            _localizationService = localizationService;
            _domainService = domainService;
            _fileService = fileService;
            _relationService = relationService;
            _dataTypeService = dataTypeService;
            _entityService = entityService;
            _contentTypeService = contentTypeService;
            _memberTypeService = memberTypeService;
            _memberGroupService = memberGroupService;
            _mediaTypeService = mediaTypeService;
            _tagService = tagService;
            this._logService = logService ?? throw new ArgumentNullException(nameof(logService));

            // _dictionaryService = dictionaryService;
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
        public JsonResult GetTotalPages(string period)
        {
            var allPages = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();
            var pages = allPages.Select(page => new
            {
                page.Id,
                page.Name,
                DateCreated = page.CreateDate
            }).ToList();

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            var previousPeriodPagesGrouped = pages
               .Where(mg => mg.DateCreated >= previousStartDate && mg.DateCreated <= previousEndDate)
               .GroupBy(mg => mg.DateCreated.Date)
            .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodPagesGrouped = pages
                .Where(mg => mg.DateCreated >= startDate && mg.DateCreated <= endDate)
                .GroupBy(mg => mg.DateCreated.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var previousPeriodPages = new List<int>();
            var previousPagesCreatedDates = new List<string>();

            for (DateTime date = previousStartDate.Date; date <= previousEndDate.Date; date = date.AddDays(1))
            {
                previousPagesCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodPages.Add(previousPeriodPagesGrouped.ContainsKey(date) ? previousPeriodPagesGrouped[date] : 0);
            }

            var currentPeriodPages = new List<int>();
            var lastPagesCreatedDates = new List<string>();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                lastPagesCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodPages.Add(currentPeriodPagesGrouped.ContainsKey(date) ? currentPeriodPagesGrouped[date] : 0);
            }

            int totalPages = currentPeriodPages.Sum();

            double rate;
            if (previousPeriodPages.Sum() == 0 && totalPages == 0)
            {
                rate = 0;
            }
            else if (previousPeriodPages.Sum() == 0)
            {
                rate = 100;
            }
            else
            {
                rate = ((double)(totalPages - previousPeriodPages.Sum()) / previousPeriodPages.Sum()) * 100;
            }

            var response = new
            {
                period,
                totalPages,
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

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }


            var previousPeriodMediasGrouped = mediaDetails
              .Where(mg => mg.CreateDate >= previousStartDate && mg.CreateDate <= previousEndDate)
              .GroupBy(mg => mg.CreateDate.Date)
              .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodMediasGrouped = mediaDetails
                .Where(mg => mg.CreateDate >= startDate && mg.CreateDate <= endDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var previousPeriodMedias = new List<int>();
            var previousMediasCreatedDates = new List<string>();

            for (DateTime date = previousStartDate.Date; date <= previousEndDate.Date; date = date.AddDays(1))
            {
                previousMediasCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodMedias.Add(previousPeriodMediasGrouped.ContainsKey(date) ? previousPeriodMediasGrouped[date] : 0);
            }

            var currentPeriodMedias = new List<int>();
            var lastMediasCreatedDates = new List<string>();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                lastMediasCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodMedias.Add(currentPeriodMediasGrouped.ContainsKey(date) ? currentPeriodMediasGrouped[date] : 0);
            }

            int totalMedias = currentPeriodMedias.Sum();

            double rate;
            if (previousPeriodMedias.Sum() == 0 && totalMedias == 0)
            {
                rate = 0;
            }
            else if (previousPeriodMedias.Sum() == 0)
            {
                rate = 100;
            }
            else
            {
                rate = ((double)(totalMedias - previousPeriodMedias.Sum()) / previousPeriodMedias.Sum()) * 100;
            }

            var response = new
            {
                period,
                totalMedias,
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
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }


            var members = _memberService.GetAllMembers().ToList();

            var previousPeriodMembersGrouped = members
               .Where(mg => mg.CreateDate >= previousStartDate && mg.CreateDate <= previousEndDate)
               .GroupBy(mg => mg.CreateDate.Date)
            .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodMembersGrouped = members
                .Where(mg => mg.CreateDate >= startDate && mg.CreateDate <= endDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());



            var previousPeriodMembers = new List<int>();
            var previousMembersCreatedDates = new List<string>();

            for (DateTime date = previousStartDate.Date; date <= previousEndDate.Date; date = date.AddDays(1))
            {
                previousMembersCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodMembers.Add(previousPeriodMembersGrouped.ContainsKey(date) ? previousPeriodMembersGrouped[date] : 0);
            }

            var currentPeriodMembers = new List<int>();
            var lastMembersCreatedDates = new List<string>();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                lastMembersCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodMembers.Add(currentPeriodMembersGrouped.ContainsKey(date) ? currentPeriodMembersGrouped[date] : 0);
            }

            int totalMembers = currentPeriodMembers.Sum();

            double rate;
            if (previousPeriodMembers.Sum() == 0 && totalMembers == 0)
            {
                rate = 0;
            }
            else if (previousPeriodMembers.Sum() == 0)
            {
                rate = 100;
            }
            else
            {
                rate = ((double)(totalMembers - previousPeriodMembers.Sum()) / previousPeriodMembers.Sum()) * 100;
            }

            var response = new
            {
                period,
                totalMembers,
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

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            var previousPeriodGroupsGrouped = memberGroups
                .Where(mg => mg.CreateDate >= previousStartDate && mg.CreateDate <= previousEndDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var currentPeriodGroupsGrouped = memberGroups
                .Where(mg => mg.CreateDate >= startDate && mg.CreateDate <= endDate)
                .GroupBy(mg => mg.CreateDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var previousPeriodGroups = new List<int>();
            var previousGroupsCreatedDates = new List<string>();

            for (DateTime date = previousStartDate.Date; date <= previousEndDate.Date; date = date.AddDays(1))
            {
                previousGroupsCreatedDates.Add(date.ToString("dd MMM"));
                previousPeriodGroups.Add(previousPeriodGroupsGrouped.ContainsKey(date) ? previousPeriodGroupsGrouped[date] : 0);
            }

            var currentPeriodGroups = new List<int>();
            var lastGroupsCreatedDates = new List<string>();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                lastGroupsCreatedDates.Add(date.ToString("dd MMM"));
                currentPeriodGroups.Add(currentPeriodGroupsGrouped.ContainsKey(date) ? currentPeriodGroupsGrouped[date] : 0);
            }

            int totalGroups = currentPeriodGroups.Sum();

            double rate;
            if (previousPeriodGroups.Sum() == 0 && totalGroups == 0)
            {
                rate = 0;
            }
            else if (previousPeriodGroups.Sum() == 0)
            {
                rate = 100;
            }
            else
            {
                rate = ((double)(totalGroups - previousPeriodGroups.Sum()) / previousPeriodGroups.Sum()) * 100;
            }

            var response = new
            {
                period,
                totalGroups,
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
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;
            List<string> categories = new List<string>();

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            var mediaDetails = GetMediaDetails();
            var filteredMedia = mediaDetails.Where(md => md.CreateDate >= startDate && md.CreateDate <= endDate).ToList();

            var allExtensions = new List<string> { "folder", "file", "image", "vector graphics (svg)", "audio", "video" };

            var mediaTypes = filteredMedia
       .Select(md => GetMediaTypeExtension(md.Extension.ToLower()))
       .Distinct()
       .Where(type => allExtensions.Contains(type.ToLower()))
       .ToList();

            categories = allExtensions;

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
            var response = new
            {
                Period = period,
                Series = groupedMedia,

            };

            return new JsonResult(response);
        }

        public JsonResult GetMembers(string period)
        {
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            var members = _memberService.GetAllMembers()
                                .Where(member => member.CreateDate >= startDate && member.CreateDate <= endDate)
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


        public JsonResult GetContentsLineChartData(string period)
        {
            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate = DateTime.MinValue;
            DateTime previousEndDate = DateTime.MinValue;
            List<string> categories = new List<string>();

            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399);
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399);
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                categories.Add(date.ToString("yyyy-MM-dd"));
            }

            var nonDeletedPages = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();
            long totalContent;
            var deletedItems = _contentService.GetPagedContentInRecycleBin(0, int.MaxValue, out totalContent)
                                              .SelectMany(GetDescendantsRecursive)
                                              .ToList();
            var allPages = nonDeletedPages.Concat(deletedItems);

            var previousPeriodPages = allPages.Where(page => page.CreateDate >= previousStartDate && page.CreateDate < previousEndDate).ToList();
            var filteredPages = allPages.Where(page => page.CreateDate >= startDate && page.CreateDate <= endDate).ToList();

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

            double dailyChangeRate = pagesPerPeriod / (endDate - startDate).TotalDays;

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
        public JsonResult GetContentStatusUsage(string period)
        {
            // Retrieve all content items
            var allContent = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;

            // Apply period filtering
            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            // Filter content items based on the selected period
            var filteredContent = allContent.Where(c => c.CreateDate >= startDate && c.CreateDate <= endDate).ToList();
            var previousPeriodContent = allContent.Where(c => c.CreateDate >= previousStartDate && c.CreateDate <= previousEndDate).ToList();

            // Calculate the counts for each status
            var deletedItems = _contentService.GetPagedContentInRecycleBin(0, int.MaxValue, out long totalContent)
                                              .SelectMany(GetDescendantsRecursive)
                                              .Where(c => c.CreateDate >= startDate && c.CreateDate <= endDate)
                                              .ToList();

            var publishedCount = filteredContent.Count(c => c.Published && !c.Trashed);
            var trashedCount = deletedItems.Count;
            var unpublishedCount = filteredContent.Count(c => !c.Published && !c.Trashed && c.PublishedState == PublishedState.Unpublished);

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

        public IEnumerable<string> GetContentContributors(int contentId)
        {
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
        public JsonResult GetUserActivity(string period)
        {
            // Retrieve all published pages
            var allPages = _contentService.GetRootContent().SelectMany(GetDescendantsRecursive).ToList();

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;
            DateTime previousStartDate;
            DateTime previousEndDate;


            switch (period.ToLower())
            {
                case "today":
                    startDate = endDate.Date;
                    endDate = endDate.Date.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "yesterday":
                    startDate = endDate.Date.AddDays(-1);
                    endDate = startDate.AddDays(1).AddSeconds(-1);
                    previousStartDate = startDate.AddDays(-1);
                    previousEndDate = startDate.AddSeconds(-1);
                    break;
                case "currentweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastweek":
                    startDate = GetMondayOfCurrentWeek(DateTime.Today).AddDays(-7);
                    endDate = startDate.AddDays(6).AddSeconds(86399); // End of last week's Sunday
                    previousStartDate = startDate.AddDays(-7);
                    previousEndDate = previousStartDate.AddDays(6).AddSeconds(86399);
                    break;
                case "lastmonth":
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
                    return new JsonResult("Invalid period specified.");
            }

            var filteredPages = allPages.Where(page => page.CreateDate >= startDate && page.CreateDate <= endDate).ToList();

            long totalRecords;
            var deletedPages = _contentService.GetPagedContentInRecycleBin(0, int.MaxValue, out totalRecords)
                                              .Where(page => page.CreateDate >= startDate && page.CreateDate <= endDate)
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

            var response = new
            {
                TotalPages = filteredPages.Count,
                Pages = result.Concat(resultDeletedPages)
            };

            return new JsonResult(response);
        }


        // Recursive method to get all descendants
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


        private List<MediaDetail> GetMediaDetails()
        {
            var mediaDetails = new List<MediaDetail>();
            var processedMediaIds = new HashSet<int>(); // To track processed media items

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

        private string GetMediaTypeExtension(string extension)
        {
            return extension switch
            {
                "folder" => "Folder",
                "file" => "File",
                "image" => "Image",
                "vector graphics (svg)" => "SVG",
                "audio" => "Audio",
                "video" => "Video",
                _ => "Other"
            };
        }

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

        private IEnumerable<IContent> GetDescendants(IContent content)
        {
            yield return content;
            foreach (var child in _contentService.GetPagedChildren(content.Id, 0, int.MaxValue, out _))
            {
                foreach (var descendant in GetDescendants(child))
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
