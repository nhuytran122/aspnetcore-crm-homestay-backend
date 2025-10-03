using System.Net;
using AutoMapper;
using CRM_Homestay.Contract.BookingServices;
using CRM_Homestay.Core.Consts.ErrorCodes;
using CRM_Homestay.Core.Enums;
using CRM_Homestay.Core.Exceptions;
using CRM_Homestay.Database.Repositories;
using CRM_Homestay.Entity.Bookings;
using CRM_Homestay.Entity.BookingServiceItems;
using CRM_Homestay.Entity.BookingServices;
using CRM_Homestay.Entity.HomestayServices;
using CRM_Homestay.Entity.ServiceItems;
using CRM_Homestay.Entity.Users;
using CRM_Homestay.Localization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace CRM_Homestay.Service.BookingServices
{
    public class BookingServiceService : BaseService, IBookingServiceService, ITransientDependency
    {
        public BookingServiceService(IUnitOfWork unitOfWork, IMapper mapper, ILocalizer l)
            : base(unitOfWork, mapper, l)
        {
        }

        #region Create Booking
        public async Task<List<BookingServiceDto>> CreateBookingServicesFromBooking(Guid bookingId, CreateBookingServicesDto input)
        {
            var booking = await ValidateAndGetBooking(bookingId);

            await CheckPendingServices(bookingId);

            await ValidateStaff(input.Services);

            var bookingServices = new List<BookingService>();

            using (_unitOfWork.BeginTransaction())
            {
                try
                {
                    foreach (var serviceInput in input.Services)
                    {
                        var service = await ValidateAndGetService(serviceInput.ServiceId);

                        ValidatePrepaidQuantity(service, serviceInput);

                        var serviceItems = service.ServiceItems;
                        var serviceInputItems = serviceInput.Items ?? new List<CreateBookingServiceItemDto>();

                        CheckDuplicateServices(input.Services);
                        CheckDuplicateServiceItems(serviceInputItems, serviceItems);

                        ValidateInventory(service, serviceInput, serviceInputItems);

                        var bookingService = await CreateBookingService(bookingId, service, serviceInput, serviceInputItems);
                        await _unitOfWork.GenericRepository<BookingService>().AddAsync(bookingService);
                        bookingServices.Add(bookingService);
                    }

                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    return MapToDto(bookingServices);
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(BaseErrorCode.UnexpectedError,
                    e.Message,
                    HttpStatusCode.BadRequest);
                }
            }
        }

        private async Task<Booking> ValidateAndGetBooking(Guid bookingId)
        {
            var booking = await _unitOfWork.GenericRepository<Booking>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.BookingRooms)
                .FirstOrDefaultAsync(x => x.Id == bookingId && !x.DeletedAt.HasValue);

            if (booking == null)
                throw new GlobalException(BookingErrorCode.NotFound,
                L[BookingErrorCode.NotFound],
                HttpStatusCode.NotFound);

            return booking;
        }

        private async Task CheckPendingServices(Guid bookingId)
        {
            var hasPendingService = await _unitOfWork.GenericRepository<BookingService>()
                .GetQueryable()
                .AsNoTracking()
                .AnyAsync(x => x.BookingId == bookingId && x.Status == BookingServiceStatuses.Pending);

            if (hasPendingService)
                throw new GlobalException(BookingServiceErrorCode.PendingServiceExists,
                L[BookingServiceErrorCode.PendingServiceExists],
                HttpStatusCode.BadRequest);
        }

        private async Task ValidateStaff(List<CreateBookingServiceDto> services)
        {
            var staffIds = services.Where(x => x.AssignedStaffId.HasValue).Select(x => x.AssignedStaffId!.Value).ToList();
            if (!staffIds.Any()) return;

            var existingStaffIds = await _unitOfWork.GenericRepository<User>()
                .GetQueryable()
                .AsNoTracking()
                .Where(x => staffIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            var invalidStaffIds = staffIds.Except(existingStaffIds).ToList();
            if (invalidStaffIds.Any())
                throw new GlobalException(UserErrorCode.NotFound,
                L[UserErrorCode.NotFound],
                HttpStatusCode.BadRequest);
        }

        private async Task<HomestayService> ValidateAndGetService(Guid serviceId)
        {
            var service = await _unitOfWork.GenericRepository<HomestayService>()
                .GetQueryable()
                .AsNoTracking()
                .Include(s => s.ServiceItems)
                .FirstOrDefaultAsync(x => x.Id == serviceId && !x.DeletedAt.HasValue);

            if (service == null)
                throw new GlobalException(HomestayServiceErrorCode.NotFound,
                L[HomestayServiceErrorCode.NotFound],
                HttpStatusCode.BadRequest);

            return service;
        }

        private void ValidatePrepaidQuantity(HomestayService service, CreateBookingServiceDto serviceInput)
        {
            if (!service.IsPrepaid) return;

            if (!serviceInput.Quantity.HasValue)
                throw new GlobalException(BookingServiceErrorCode.QuantityRequired,
                L[BookingServiceErrorCode.QuantityRequired, service.Name],
                HttpStatusCode.BadRequest);

            if (serviceInput.Quantity <= 0)
                throw new GlobalException(BookingServiceErrorCode.MustBeGreaterThanZero,
                L[BookingServiceErrorCode.MustBeGreaterThanZero, service.Name],
                HttpStatusCode.BadRequest);
        }

        private void CheckDuplicateServices(List<CreateBookingServiceDto> services)
        {
            var duplicates = services.GroupBy(x => x.ServiceId).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicates.Any())
                throw new GlobalException(BookingServiceErrorCode.ServiceDuplicateInRequest,
                L[BookingServiceErrorCode.ServiceDuplicateInRequest],
                HttpStatusCode.BadRequest);
        }

        private void CheckDuplicateServiceItems(List<CreateBookingServiceItemDto> inputItems, List<ServiceItem>? serviceItems)
        {
            var duplicates = inputItems.GroupBy(x => x.ServiceItemId).Where(g => g.Count() > 1).Select(g => g.First()).ToList();
            if (!duplicates.Any()) return;

            var identifiers = duplicates.Select(i => serviceItems?.FirstOrDefault(si => si.Id == i.ServiceItemId)?.Identifier ?? "")
                .Where(s => !string.IsNullOrEmpty(s)).ToList();

            throw new GlobalException(BookingServiceErrorCode.ServiceItemDuplicateInRequest,
                                        L[BookingServiceErrorCode.ServiceItemDuplicateInRequest, string.Join(", ", identifiers)],
                                        HttpStatusCode.BadRequest
                                    );
        }

        private void ValidateInventory(HomestayService service, CreateBookingServiceDto serviceInput, List<CreateBookingServiceItemDto> serviceInputItems)
        {
            if (!service.HasInventory) return;

            if (service.ServiceItems == null || !service.ServiceItems.Any())
                throw new GlobalException(BookingServiceErrorCode.ServiceItemNotConfigured,
                L[BookingServiceErrorCode.ServiceItemNotConfigured, service.Name],
                HttpStatusCode.BadRequest);

            if (!serviceInputItems.Any())
                throw new GlobalException(BookingServiceErrorCode.ServiceItemRequired,
                L[BookingServiceErrorCode.ServiceItemRequired, service.Name],
                HttpStatusCode.BadRequest);

            if (serviceInput.Quantity.HasValue && serviceInput.Quantity.Value != serviceInputItems.Count)
                throw new GlobalException(BookingServiceErrorCode.QuantityMismatch,
                L[BookingServiceErrorCode.QuantityMismatch, service.Name, serviceInputItems.Count],
                HttpStatusCode.BadRequest);
        }

        private async Task<BookingService> CreateBookingService(Guid bookingId, HomestayService service, CreateBookingServiceDto serviceInput, List<CreateBookingServiceItemDto> serviceInputItems)
        {
            var bookingService = new BookingService
            {
                BookingId = bookingId,
                ServiceId = service.Id,
                BookingRoomId = serviceInput.BookingRoomId,
                AssignedStaffId = serviceInput.AssignedStaffId,
                Description = serviceInput.Description,
                Quantity = service.HasInventory ? serviceInputItems.Count : serviceInput.Quantity ?? null,
                BookingServiceItems = new List<BookingServiceItem>()
            };
            var bookingServiceItems = bookingService.BookingServiceItems;

            if (service.IsPrepaid)
            {
                bool isVehicle = bookingServiceItems != null &&
                                         bookingServiceItems.Any(bsi =>
                                             bsi.ServiceItem != null &&
                                             (bsi.ServiceItem.Type != ServiceItemTypes.Others));

                if (isVehicle)
                {
                    // Tính tổng số ngày thuê của tất cả item
                    var totalDays = bookingServiceItems!.Sum(bsi =>
                        (bsi.EndDate!.Value.Date - bsi.StartDate!.Value.Date).Days + 1);

                    bookingService.TotalPrice = service.Price * totalDays;
                }
                else
                {
                    bookingService.TotalPrice = service.Price * (decimal)bookingService.Quantity!;
                }
            }


            foreach (var itemInput in serviceInputItems)
            {
                if (itemInput.StartDate == null || itemInput.EndDate == null)
                    throw new GlobalException(BookingServiceErrorCode.DateRequired,
                    L[BookingServiceErrorCode.DateRequired, service.Name],
                    HttpStatusCode.BadRequest);

                var conflicts = await _unitOfWork.GenericRepository<BookingServiceItem>()
                    .GetQueryable()
                    .AsNoTracking()
                    .Include(bsi => bsi.BookingService)
                        .ThenInclude(bs => bs!.Service)
                    .Where(bsi =>
                        bsi.ServiceItemId == itemInput.ServiceItemId &&
                        bsi.BookingService!.Status != BookingServiceStatuses.Cancelled &&
                        bsi.StartDate <= itemInput.EndDate &&
                        bsi.EndDate >= itemInput.StartDate)
                    .ToListAsync();

                if (conflicts.Any())
                {
                    var firstConflict = conflicts.First();
                    throw new GlobalException(
                        ServiceItemErrorCode.ServiceItemUnavailable,
                        L[ServiceItemErrorCode.ServiceItemUnavailable,
                            string.Join(", ", conflicts.Select(c => c.ServiceItem?.Identifier ?? c.ServiceItemId.ToString())),
                            firstConflict.StartDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                            firstConflict.EndDate?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
                        ],
                        HttpStatusCode.BadRequest
                    );
                }

                bookingServiceItems!.Add(new BookingServiceItem
                {
                    ServiceItemId = itemInput.ServiceItemId,
                    StartDate = itemInput.StartDate,
                    EndDate = itemInput.EndDate
                });
            }

            return bookingService;
        }

        private List<BookingServiceDto> MapToDto(List<BookingService> bookingServices)
        {
            return bookingServices.Select(bs => new BookingServiceDto
            {
                Id = bs.Id,
                BookingId = bs.BookingId,
                ServiceId = bs.ServiceId,
                ServiceName = bs.Service?.Name,
                AssignedStaffId = bs.AssignedStaffId,
                AssignedStaffName = bs.AssignedStaff?.FullName,
                Description = bs.Description,
                Quantity = bs.Quantity,
                Status = bs.Status,
                TotalPrice = bs.TotalPrice,
                Items = bs.BookingServiceItems?.Select(bsi => new BookingServiceItemDto
                {
                    Id = bsi.Id,
                    ServiceItemId = bsi.ServiceItemId,
                    Identifier = bsi.ServiceItem?.Identifier,
                    StartDate = bsi.StartDate,
                    EndDate = bsi.EndDate
                }).ToList()
            }).ToList();
        }
        #endregion

        #region Update BookingService
        public async Task<BookingServiceDto> UpdateAsync(Guid bookingServiceId, UpdateBookingServiceDto input)
        {
            // Bước 1: Load để validate (AsNoTracking OK ở đây)
            var bookingServiceForValidation = await _unitOfWork.GenericRepository<BookingService>()
                .GetQueryable()
                .AsNoTracking()
                .Include(x => x.BookingServiceItems!).ThenInclude(y => y.ServiceItem)
                .Include(x => x.Service).ThenInclude(y => y!.ServiceItems)
                .FirstOrDefaultAsync(x => x.Id == bookingServiceId);

            if (bookingServiceForValidation == null)
                throw new GlobalException(BookingServiceErrorCode.NotFound,
                    L[BookingServiceErrorCode.NotFound],
                    HttpStatusCode.NotFound);

            var service = bookingServiceForValidation.Service!;
            var oldBookingServiceItems = bookingServiceForValidation.BookingServiceItems;

            // Validations...
            if (service.IsPrepaid)
            {
                if (!input.Quantity.HasValue)
                    throw new GlobalException(BookingServiceErrorCode.QuantityRequired,
                        L[BookingServiceErrorCode.QuantityRequired, service.Name],
                        HttpStatusCode.BadRequest);

                if (input.Quantity <= 0)
                    throw new GlobalException(BookingServiceErrorCode.MustBeGreaterThanZero,
                        L[BookingServiceErrorCode.MustBeGreaterThanZero, service.Name],
                        HttpStatusCode.BadRequest);
            }

            if (input.AssignedStaffId.HasValue)
            {
                var staffExists = await _unitOfWork.GenericRepository<User>()
                    .GetQueryable()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == input.AssignedStaffId.Value);

                if (!staffExists)
                    throw new GlobalException(UserErrorCode.NotFound,
                        L[UserErrorCode.NotFound],
                        HttpStatusCode.BadRequest);
            }

            var inputItems = input.Items ?? new List<UpdateBookingServiceItemDto>();

            if (service.HasInventory)
            {
                if (!inputItems.Any())
                    throw new GlobalException(BookingServiceErrorCode.ServiceItemRequired,
                        L[BookingServiceErrorCode.ServiceItemRequired, service.Name],
                        HttpStatusCode.BadRequest);

                var duplicates = inputItems.GroupBy(i => i.ServiceItemId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.First())
                    .ToList();

                if (duplicates.Any())
                {
                    var identifiers = duplicates.Select(i => service.ServiceItems
                            ?.FirstOrDefault(si => si.Id == i.ServiceItemId)?.Identifier ?? "")
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();

                    throw new GlobalException(BookingServiceErrorCode.ServiceItemDuplicateInRequest,
                        L[BookingServiceErrorCode.ServiceItemDuplicateInRequest, string.Join(", ", identifiers)],
                        HttpStatusCode.BadRequest);
                }

                if (service.IsPrepaid && input.Quantity.HasValue && input.Quantity.Value != inputItems.Count)
                    throw new GlobalException(BookingServiceErrorCode.QuantityMismatch,
                        L[BookingServiceErrorCode.QuantityMismatch, service.Name, inputItems.Count],
                        HttpStatusCode.BadRequest);
            }

            using (_unitOfWork.BeginTransaction())
            {
                try
                {
                    // Bước 2: Load lại entity KHÔNG bao gồm ServiceItem để tránh tracking conflict
                    var bookingService = await _unitOfWork.GenericRepository<BookingService>()
                        .GetQueryable()
                        .Include(x => x.BookingServiceItems)
                        .FirstOrDefaultAsync(x => x.Id == bookingServiceId);

                    bookingService!.AssignedStaffId = input.AssignedStaffId;
                    bookingService.Description = input.Description;
                    bookingService.Quantity = service.HasInventory ? inputItems.Count : input.Quantity;

                    await UpdateBookingServiceItems(bookingService, inputItems, service);

                    if (service.IsPrepaid)
                    {
                        bool isVehicle = oldBookingServiceItems != null &&
                                         oldBookingServiceItems.Any(bsi =>
                                             bsi.ServiceItem != null &&
                                             (bsi.ServiceItem.Type != ServiceItemTypes.Others));

                        if (isVehicle)
                        {
                            var newItems = await _unitOfWork.GenericRepository<BookingServiceItem>()
                                .GetQueryable()
                                .AsNoTracking()
                                .Where(x => x.BookingServiceId == bookingService.Id)
                                .ToListAsync();

                            var totalDays = newItems.Sum(bsi =>
                                (bsi.EndDate!.Value.Date - bsi.StartDate!.Value.Date).Days + 1);
                            bookingService.TotalPrice = service.Price * totalDays;
                        }
                        else
                        {
                            bookingService.TotalPrice = service.Price * (decimal)bookingService.Quantity!;
                        }
                    }

                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    bookingService = await _unitOfWork.GenericRepository<BookingService>()
                        .GetQueryable()
                        .AsNoTracking()
                        .Include(x => x.BookingServiceItems!).ThenInclude(y => y.ServiceItem)
                        .Include(x => x.Service)
                        .FirstOrDefaultAsync(x => x.Id == bookingServiceId);

                    return ObjectMapper.Map<BookingService, BookingServiceDto>(bookingService!);
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(BaseErrorCode.UnexpectedError,
                        e.Message,
                        HttpStatusCode.BadRequest);
                }
            }
        }

        private async Task UpdateBookingServiceItems(
            BookingService bookingService,
            List<UpdateBookingServiceItemDto> inputItems,
            HomestayService service)
        {
            var existingItems = bookingService.BookingServiceItems?.ToList() ?? new List<BookingServiceItem>();
            var allowedTypes = service.ServiceItems?.Select(si => si.Type).Distinct().ToList() ?? new List<ServiceItemTypes>();

            var inputItemIds = inputItems.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToList();
            var itemsToRemove = existingItems.Where(x => !inputItemIds.Contains(x.Id)).ToList();

            if (itemsToRemove.Any())
                _unitOfWork.GenericRepository<BookingServiceItem>().RemoveRange(itemsToRemove);

            foreach (var itemInput in inputItems)
            {
                if (!itemInput.StartDate.HasValue || !itemInput.EndDate.HasValue)
                {
                    throw new GlobalException(
                        BookingServiceErrorCode.DateRequired,
                        L[BookingServiceErrorCode.DateRequired, service.Name],
                        HttpStatusCode.BadRequest
                    );
                }

                var serviceItem = service.ServiceItems?.FirstOrDefault(si => si.Id == itemInput.ServiceItemId);

                if (serviceItem == null || !allowedTypes.Contains(serviceItem.Type))
                {
                    throw new GlobalException(
                        BookingServiceErrorCode.InvalidServiceItem,
                        L[BookingServiceErrorCode.InvalidServiceItem, serviceItem?.Identifier ?? ""],
                        HttpStatusCode.BadRequest
                    );
                }

                var conflictQuery = _unitOfWork.GenericRepository<BookingServiceItem>()
                    .GetQueryable()
                    .Include(x => x.BookingService)
                    .Where(x =>
                        x.ServiceItemId == itemInput.ServiceItemId &&
                        x.BookingServiceId != bookingService.Id &&
                        x.BookingService!.Status != BookingServiceStatuses.Cancelled &&
                        x.StartDate <= itemInput.EndDate &&
                        x.EndDate >= itemInput.StartDate
                    );

                if (itemInput.Id.HasValue)
                    conflictQuery = conflictQuery.Where(x => x.Id != itemInput.Id.Value);

                var conflicts = await conflictQuery.ToListAsync();

                if (conflicts.Any())
                {
                    var firstConflict = conflicts.First();
                    throw new GlobalException(
                        ServiceItemErrorCode.ServiceItemUnavailable,
                        L[ServiceItemErrorCode.ServiceItemUnavailable,
                            serviceItem.Identifier ?? itemInput.ServiceItemId.ToString(),
                            firstConflict.StartDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
                            firstConflict.EndDate?.ToString("dd/MM/yyyy HH:mm") ?? ""
                        ],
                        HttpStatusCode.BadRequest
                    );
                }

                if (itemInput.Id.HasValue)
                {
                    var existingItem = existingItems.FirstOrDefault(x => x.Id == itemInput.Id.Value);
                    if (existingItem != null)
                    {
                        existingItem.ServiceItemId = itemInput.ServiceItemId;
                        existingItem.StartDate = itemInput.StartDate;
                        existingItem.EndDate = itemInput.EndDate;
                    }
                }
                else
                {
                    var newItem = new BookingServiceItem
                    {
                        BookingServiceId = bookingService.Id,
                        ServiceItemId = itemInput.ServiceItemId,
                        StartDate = itemInput.StartDate,
                        EndDate = itemInput.EndDate
                    };
                    await _unitOfWork.GenericRepository<BookingServiceItem>().AddAsync(newItem);
                }
            }
        }
        #endregion

        public async Task<bool> CancelAsync(Guid bookingServiceId)
        {
            using (_unitOfWork.BeginTransaction())
            {
                try
                {
                    var bookingService = await _unitOfWork.GenericRepository<BookingService>()
                        .GetQueryable()
                        .Include(x => x.BookingServiceItems)
                        .Include(x => x.Service)
                        .FirstOrDefaultAsync(x => x.Id == bookingServiceId && !x.DeletedAt.HasValue);

                    if (bookingService == null)
                        throw new GlobalException(
                            BookingServiceErrorCode.NotFound,
                            L[BookingServiceErrorCode.NotFound],
                            HttpStatusCode.NotFound
                        );

                    if (bookingService.Status == BookingServiceStatuses.Cancelled)
                    {
                        throw new GlobalException(
                            BookingServiceErrorCode.AlreadyCancelled,
                            L[BookingServiceErrorCode.AlreadyCancelled],
                            HttpStatusCode.BadRequest
                        );
                    }

                    if (bookingService.Status == BookingServiceStatuses.Completed)
                    {
                        throw new GlobalException(
                            BookingServiceErrorCode.CannotCancelCompleted,
                            L[BookingServiceErrorCode.CannotCancelCompleted],
                            HttpStatusCode.BadRequest
                        );
                    }
                    if (bookingService.Status == BookingServiceStatuses.InProgress)
                    {
                        throw new GlobalException(
                            BookingServiceErrorCode.CannotCancelInProgress,
                            L[BookingServiceErrorCode.CannotCancelInProgress],
                            HttpStatusCode.BadRequest
                        );
                    }
                    bookingService.Status = BookingServiceStatuses.Cancelled;
                    _unitOfWork.GenericRepository<BookingService>().Update(bookingService);

                    await _unitOfWork.SaveChangeAsync();
                    _unitOfWork.Commit();

                    return true;
                }
                catch (Exception e)
                {
                    _unitOfWork.Rollback();
                    throw new GlobalException(BaseErrorCode.UnexpectedError,
                        e.Message,
                        HttpStatusCode.BadRequest);
                }
            }
        }
    }
}