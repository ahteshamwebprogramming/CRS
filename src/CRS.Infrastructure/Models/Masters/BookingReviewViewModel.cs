using CRS.Infrastructure.ViewModels.Summary;

namespace CRS.Infrastructure.Models.Masters
{
    public class BookingReviewViewModel
    {
        public BookingDTO Booking { get; set; }
        public SummaryInputDTO Summary { get; set; }

        public SummaryViewModelNew Summary1 { get; set; }
    }
}
