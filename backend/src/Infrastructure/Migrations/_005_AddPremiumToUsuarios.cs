using FluentMigrator;

namespace Infrastructure.Migrations;

[Migration(5, "Adiciona campos de is_premium e stripe_customer_id na tabela usuarios")]
public class _005_AddPremiumToUsuarios : Migration
{
    public override void Up()
    {
        Execute.Sql(@"
            ALTER TABLE `usuarios` 
            ADD COLUMN IF NOT EXISTS `is_premium` TINYINT(1) NOT NULL DEFAULT 0,
            ADD COLUMN IF NOT EXISTS `stripe_customer_id` VARCHAR(100) NULL;
        ");
    }

    public override void Down()
    {
        Execute.Sql(@"
            ALTER TABLE `usuarios` 
            DROP COLUMN IF EXISTS `is_premium`,
            DROP COLUMN IF EXISTS `stripe_customer_id`;
        ");
    }
}
