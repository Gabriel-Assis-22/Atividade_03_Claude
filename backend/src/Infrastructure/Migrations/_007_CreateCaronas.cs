using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(7, "Cria tabela de caronas")]
public class _007_CreateCaronas : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE IF NOT EXISTS `caronas` (
                `id` INT NOT NULL AUTO_INCREMENT,
                `motorista_id` INT NOT NULL,
                `veiculo_id` INT NOT NULL,
                `cidade_origem` VARCHAR(100) NOT NULL,
                `cidade_destino` VARCHAR(100) NOT NULL,
                `ponto_encontro` VARCHAR(255) NOT NULL,
                `data_hora_saida` DATETIME NOT NULL,
                `vagas_totais` INT NOT NULL,
                `vagas_disponiveis` INT NOT NULL,
                `valor_rateio` DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
                `status` VARCHAR(20) NOT NULL DEFAULT 'disponivel',
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`id`),
                INDEX `idx_caronas_busca` (`cidade_origem`, `cidade_destino`, `data_hora_saida`),
                CONSTRAINT `fk_caronas_motorista` FOREIGN KEY (`motorista_id`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE,
                CONSTRAINT `fk_caronas_veiculo` FOREIGN KEY (`veiculo_id`) REFERENCES `veiculos` (`id`) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS `caronas`;");
    }
}
