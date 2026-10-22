using Domain.Repositories;
using Domain.Services;

namespace Application.UseCases.Payments;

public class CreateCheckoutSessionUseCase(IUsuarioRepository usuarioRepo, IPaymentService paymentService)
{
    public async Task<string> ExecuteAsync(int usuarioId, string successUrl, string cancelUrl)
    {
        var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        return await paymentService.CreateCheckoutSessionAsync(usuario.Id, usuario.Email, successUrl, cancelUrl);
    }
}

public class CreateCustomerPortalUseCase(IUsuarioRepository usuarioRepo, IPaymentService paymentService)
{
    public async Task<string> ExecuteAsync(int usuarioId, string returnUrl)
    {
        var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        if (string.IsNullOrWhiteSpace(usuario.StripeCustomerId))
            throw new InvalidOperationException("Usuário não possui uma assinatura vinculada no Stripe.");

        return await paymentService.CreateCustomerPortalSessionAsync(usuario.StripeCustomerId, returnUrl);
    }
}

public class HandleStripeWebhookUseCase(IUsuarioRepository usuarioRepo, IPaymentService paymentService)
{
    public async Task<(bool Success, int? UsuarioId, string? CustomerId)> ExecuteAsync(string jsonPayload, string stripeSignature)
    {
        var result = paymentService.ProcessWebhook(jsonPayload, stripeSignature);
        if (result.IsHandled && result.UsuarioId.HasValue)
        {
            var isAtivo = result.IsSubscriptionActive ?? true;
            await usuarioRepo.SetPremiumAsync(result.UsuarioId.Value, isAtivo, result.StripeCustomerId);
            return (true, result.UsuarioId.Value, result.StripeCustomerId);
        }

        return (false, null, null);
    }
}

