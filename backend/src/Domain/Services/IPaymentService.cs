namespace Domain.Services;

public record PaymentWebhookResult(bool IsHandled, string? EventType, int? UsuarioId, string? StripeCustomerId, bool? IsSubscriptionActive);

public interface IPaymentService
{
    Task<string> CreateCheckoutSessionAsync(int usuarioId, string userEmail, string successUrl, string cancelUrl);
    Task<string> CreateCustomerPortalSessionAsync(string stripeCustomerId, string returnUrl);
    PaymentWebhookResult ProcessWebhook(string jsonPayload, string stripeSignature);
}

