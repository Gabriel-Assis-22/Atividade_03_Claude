using Application.UseCases.Payments;
using Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api/payments")]
[Route("api/assinaturas")]
public class PaymentsController(
    CreateCheckoutSessionUseCase createCheckoutSession,
    CreateCustomerPortalUseCase createCustomerPortal,
    HandleStripeWebhookUseCase handleStripeWebhook,
    IAuditClient audit,
    ILogger<PaymentsController> logger) : ControllerBase
{
    private int? CurrentUserId
    {
        get
        {
            var rawId = User.FindFirst("userId")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value
                     ?? User.FindFirst("id")?.Value;

            if (int.TryParse(rawId, out var id)) return id;
            return null;
        }
    }

    private string? ClientIp =>
        Request.Headers["X-Forwarded-For"].FirstOrDefault()
        ?? Request.Headers["X-Real-IP"].FirstOrDefault()
        ?? HttpContext.Connection.RemoteIpAddress?.ToString();

    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> CreateCheckout()
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Sessão expirada. Faça login novamente." });

        try
        {
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
                ?? "https://gabriel-assis-isw055.lapps.studio";

            var successUrl = $"{frontendUrl.TrimEnd('/')}/feed?payment=success&session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{frontendUrl.TrimEnd('/')}/feed?payment=cancelled";

            var checkoutUrl = await createCheckoutSession.ExecuteAsync(CurrentUserId.Value, successUrl, cancelUrl);

            await audit.LogAsync(
                CurrentUserId.Value,
                "iniciar_checkout_stripe",
                "Sessão de assinatura do UniRide Pro criada com sucesso no Stripe",
                ClientIp);

            return Ok(new { checkoutUrl });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Erro de configuração ao criar sessão de checkout");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado ao criar sessão de checkout Stripe");
            return StatusCode(500, new { erro = "Erro ao iniciar sessão de pagamento." });
        }
    }

    [HttpPost("portal")]
    [Authorize]
    public async Task<IActionResult> CreatePortal()
    {
        if (!CurrentUserId.HasValue)
            return Unauthorized(new { erro = "Sessão expirada. Faça login novamente." });

        try
        {
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
                ?? "https://gabriel-assis-isw055.lapps.studio";

            var returnUrl = $"{frontendUrl.TrimEnd('/')}/feed";
            var portalUrl = await createCustomerPortal.ExecuteAsync(CurrentUserId.Value, returnUrl);

            await audit.LogAsync(
                CurrentUserId.Value,
                "acessar_portal_cliente_stripe",
                "Sessão do Stripe Customer Portal gerada",
                ClientIp);

            return Ok(new { portalUrl });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao gerar portal do cliente Stripe");
            return StatusCode(500, new { erro = "Erro ao acessar o portal de assinaturas." });
        }
    }

    [HttpPost("webhook")]
    [HttpPost("/api/webhooks/stripe")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleWebhook()
    {
        string jsonPayload;
        using (var reader = new StreamReader(Request.Body))
        {
            jsonPayload = await reader.ReadToEndAsync();
        }

        var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();
        if (string.IsNullOrEmpty(stripeSignature))
        {
            logger.LogWarning("Webhook Stripe chamado sem cabeçalho Stripe-Signature");
            return BadRequest(new { erro = "Cabeçalho Stripe-Signature ausente." });
        }

        try
        {
            var (handled, userId, customerId) = await handleStripeWebhook.ExecuteAsync(jsonPayload, stripeSignature);

            if (handled && userId.HasValue)
            {
                logger.LogInformation("Webhook Stripe processado com sucesso para usuário {UserId} (Customer: {CustomerId})",
                    userId.Value, customerId);

                await audit.LogAsync(
                    userId.Value,
                    "webhook_stripe_sucesso",
                    $"Status do Plano UniRide Pro atualizado via webhook Stripe (Customer/ID: {customerId})",
                    ClientIp);
            }

            return Ok(new { received = true });
        }
        catch (Stripe.StripeException ex)
        {
            logger.LogWarning(ex, "Falha na validação da assinatura ou evento do webhook Stripe: {Message}", ex.Message);
            return BadRequest(new { erro = $"Validação do webhook falhou: {ex.Message}" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao processar webhook do Stripe");
            return StatusCode(500, new { erro = "Erro interno ao processar webhook." });
        }
    }
}

