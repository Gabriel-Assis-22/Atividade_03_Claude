using Application.DTOs.Favorites;
using Domain.Repositories;

namespace Application.UseCases.Favorites;

public class GetFavoritesUseCase(IFavoritoRepository repo)
{
    public async Task<IEnumerable<FavoriteDto>> ExecuteAsync(int usuarioId)
    {
        var favs = await repo.GetByUsuarioAsync(usuarioId);
        return favs.Select(f => new FavoriteDto(f.Id, f.TmdbMovieId, f.Titulo, f.PosterPath, f.CriadoEm));
    }
}

public class AddFavoriteUseCase(IFavoritoRepository repo, IUsuarioRepository usuarioRepo)
{
    public const int LimiteGratuito = 3;

    public async Task ExecuteAsync(int usuarioId, AddFavoriteRequest req)
    {
        var usuario = await usuarioRepo.GetByIdAsync(usuarioId);
        if (usuario != null && !usuario.IsPremium)
        {
            var favs = await repo.GetByUsuarioAsync(usuarioId);
            if (favs.Count() >= LimiteGratuito)
            {
                throw new InvalidOperationException($"Limite de {LimiteGratuito} favoritos atingido para o plano gratuito. Assine o Plano Premium para ter favoritos ilimitados!");
            }
        }

        await repo.AddAsync(usuarioId, req.TmdbMovieId, req.Titulo, req.PosterPath);
    }
}

public class RemoveFavoriteUseCase(IFavoritoRepository repo)
{
    public Task ExecuteAsync(int usuarioId, int tmdbMovieId) =>
        repo.RemoveAsync(usuarioId, tmdbMovieId);
}

public class CheckFavoriteUseCase(IFavoritoRepository repo)
{
    public Task<bool> ExecuteAsync(int usuarioId, int tmdbMovieId) =>
        repo.ExistsAsync(usuarioId, tmdbMovieId);
}
