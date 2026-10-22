using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(9, "Cria tabela de avaliacoes")]
public class _009_CreateAvaliacoes : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE IF NOT EXISTS `avaliacoes` (
                `id` INT NOT NULL AUTO_INCREMENT,
                `carona_id` INT NOT NULL,
                `avaliador_id` INT NOT NULL,
                `avaliado_id` INT NOT NULL,
                `nota` INT NOT NULL,
                `comentario` VARCHAR(500) NULL,
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`id`),
                CONSTRAINT `fk_avaliacoes_carona` FOREIGN KEY (`carona_id`) REFERENCES `caronas` (`id`) ON DELETE CASCADE,
                CONSTRAINT `fk_avaliacoes_avaliador` FOREIGN KEY (`avaliador_id`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE,
                CONSTRAINT `fk_avaliacoes_avaliado` FOREIGN KEY (`avaliado_id`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS `avaliacoes`;");
    }
}
