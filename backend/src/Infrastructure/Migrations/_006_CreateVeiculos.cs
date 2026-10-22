using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(6, "Cria tabela de veiculos")]
public class _006_CreateVeiculos : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            CREATE TABLE IF NOT EXISTS `veiculos` (
                `id` INT NOT NULL AUTO_INCREMENT,
                `usuario_id` INT NOT NULL,
                `modelo` VARCHAR(100) NOT NULL,
                `placa` VARCHAR(20) NOT NULL,
                `cor` VARCHAR(50) NOT NULL,
                `ano` INT NOT NULL,
                `foto_url` VARCHAR(500) NULL,
                `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (`id`),
                CONSTRAINT `fk_veiculos_usuario` FOREIGN KEY (`usuario_id`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
        ");
    }

    public override void Down()
    {
        Execute.Sql("DROP TABLE IF EXISTS `veiculos`;");
    }
}
