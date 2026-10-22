using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(8, "Cria tabela de reservas")]
public class _008_CreateReservas : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE IF NOT EXISTS `reservas` (
                `id` INT NOT NULL AUTO_INCREMENT,
                `carona_id` INT NOT NULL,
                `passageiro_id` INT NOT NULL,
                `status` VARCHAR(20) NOT NULL DEFAULT 'confirmada',
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`id`),
                UNIQUE KEY `uk_reserva_carona_passageiro` (`carona_id`, `passageiro_id`),
                CONSTRAINT `fk_reservas_carona` FOREIGN KEY (`carona_id`) REFERENCES `caronas` (`id`) ON DELETE CASCADE,
                CONSTRAINT `fk_reservas_passageiro` FOREIGN KEY (`passageiro_id`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS `reservas`;");
    }
}
