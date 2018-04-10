DELETE FROM [dbo].[SimpleData];

DECLARE @max INT = 100000;
DECLARE @cnt INT = 0;
WHILE @cnt < @max
BEGIN
	INSERT INTO [dbo].[SimpleData] ([Name], [Value])
	VALUES (CONCAT('Test ', @cnt), RAND(CHECKSUM(@cnt) * (100)))

	SET @cnt = @cnt + 1;
END;