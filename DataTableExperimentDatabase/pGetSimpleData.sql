CREATE PROCEDURE [dbo].[pGetSimpleData]
	--@param1 int = 0,
	--@param2 int
AS
	SELECT
		[Id],
		[Name],
		[Value]
	FROM [dbo].[SimpleData]
RETURN 0
