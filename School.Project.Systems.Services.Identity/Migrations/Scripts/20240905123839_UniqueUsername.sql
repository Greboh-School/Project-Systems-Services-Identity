START TRANSACTION;

CREATE UNIQUE INDEX `IX_AspNetUsers_UserName` ON `AspNetUsers` (`UserName`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240905123839_UniqueUsername', '8.0.8');

COMMIT;

