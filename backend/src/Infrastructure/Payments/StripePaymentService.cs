using Domain.Services;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;

namespace Infrastructure.Payments;

public class StripePaymentService : IPaymentService
{
    private readonly string _secretKey;
    private readonly string _webhookSecret;
    private readonly string? _priceId;

    public StripePaymentService()
    {
        _secretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY") ?? "";
        _webhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET") ?? "";
        _priceId = Environment.GetEnvironmentVariable("STRIPE_PRICE_ID");

        if (!string.IsNullOrEmpty(_secretKey))
        {
            StripeConfiguration.ApiKey = _secretKey;
        }
    }

    public async Task<string> CreateCheckoutSessionAsync(int usuarioId, string userEmail, string successUrl, string cancelUrl)
    {
        if (string.IsNullOrEmpty(_secretKey))
            throw new InvalidOperationException("STRIPE_SECRET_KEY não configurada no servidor.");

        var options = new Stripe.Checkout.SessionCreateOptions
        {
            Mode = "subscription",
            CustomerEmail = string.IsNullOrWhiteSpace(userEmail) ? null : userEmail,
            ClientReferenceId = usuarioId.ToString(),
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "usuarioId", usuarioId.ToString() },
                { "tipo", "uniride_pro" }
            },
            SubscriptionData = new Stripe.Checkout.SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "usuarioId", usuarioId.ToString() }
                }
            }
        };

        if (!string.IsNullOrWhiteSpace(_priceId))
        {
            options.LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            {
                new Stripe.Checkout.SessionLineItemOptions
                {
                    Price = _priceId,
                    Quantity = 1
                }
            };
        }
        else
        {
            options.LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            {
                new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        Currency = "brl",
                        UnitAmount = 1490, // R$ 14,90 em centavos (UniRide Pro)
                        Recurring = new Stripe.Checkout.SessionLineItemPriceDataRecurringOptions
                        {
                            Interval = "month"
                        },
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Plano UniRide Pro — Caronas Universitárias",
                            Description = "Reservas ilimitadas de viagens, publicação de rotas e destaque de condutor"
                        }
                    },
                    Quantity = 1
                }
            };
        }

        var service = new Stripe.Checkout.SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public async Task<string> CreateCustomerPortalSessionAsync(string stripeCustomerId, string returnUrl)
    {
        if (string.IsNullOrEmpty(_secretKey))
            throw new InvalidOperationException("STRIPE_SECRET_KEY não configurada no servidor.");

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = stripeCustomerId,
            ReturnUrl = returnUrl
        };

        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public PaymentWebhookResult ProcessWebhook(string jsonPayload, string stripeSignature)
    {
        if (string.IsNullOrEmpty(_webhookSecret))
            throw new InvalidOperationException("STRIPE_WEBHOOK_SECRET não configurada.");

        var stripeEvent = EventUtility.ConstructEvent(jsonPayload, stripeSignature, _webhookSecret);

        if (stripeEvent.Type == "checkout.session.completed")
        {
            if (stripeEvent.Data.Object is Stripe.Checkout.Session session)
            {
                int? userId = null;
                if (int.TryParse(session.ClientReferenceId, out var parsedId))
                {
                    userId = parsedId;
                }
                else if (session.Metadata != null && session.Metadata.TryGetValue("usuarioId", out var metaId) && int.TryParse(metaId, out var parsedMetaId))
                {
                    userId = parsedMetaId;
                }

                var customerId = session.CustomerId ?? session.CustomerDetails?.Email ?? session.Id;
                return new PaymentWebhookResult(true, stripeEvent.Type, userId, customerId, true);
            }
        }
        else if (stripeEvent.Type == "customer.subscription.created" || stripeEvent.Type == "customer.subscription.updated")
        {
            if (stripeEvent.Data.Object is Subscription sub)
            {
                int? userId = null;
                if (sub.Metadata != null && sub.Metadata.TryGetValue("usuarioId", out var mId) && int.TryParse(mId, out var parsed))
                {
                    userId = parsed;
                }
                var isActive = sub.Status == "active" || sub.Status == "trialing";
                return new PaymentWebhookResult(true, stripeEvent.Type, userId, sub.CustomerId, isActive);
            }
        }
        else if (stripeEvent.Type == "customer.subscription.deleted")
        {
            if (stripeEvent.Data.Object is Subscription sub)
            {
                int? userId = null;
                if (sub.Metadata != null && sub.Metadata.TryGetValue("usuarioId", out var mId) && int.TryParse(mId, out var parsed))
                {
                    userId = parsed;
                }
                return new PaymentWebhookResult(true, stripeEvent.Type, userId, sub.CustomerId, false);
            }
        }

        return new PaymentWebhookResult(false, stripeEvent.Type, null, null, null);
    }
}

