using System;

namespace Ecommerce.Models;

public partial class VerificationDocument
{
    public int DocumentId { get; set; }

    public string OwnerType { get; set; } = "SELLER"; // SELLER, DELIVERY_PARTNER

    public int OwnerEntityId { get; set; } // SellerId or DeliveryPartnerId

    public int? SellerVerificationId { get; set; }

    public int? DeliveryVerificationId { get; set; }

    public string DocumentType { get; set; } = null!; // GST_CERTIFICATE, PAN_CARD, BANK_CHEQUE, DRIVING_LICENCE, VEHICLE_RC, INSURANCE

    public string DocumentName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public string? OcrText { get; set; }

    public string? ExtractedDataJson { get; set; }

    public string Status { get; set; } = "VERIFIED"; // VERIFIED, PENDING, REJECTED

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public virtual SellerVerification? SellerVerification { get; set; }

    public virtual DeliveryPartnerVerification? DeliveryVerification { get; set; }
}
