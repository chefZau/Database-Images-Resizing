-- =============================================
-- Author:      Luca Zhou
-- Create date: Dec 9, 2020
-- Description: Resize the Top 100 oversize(> 300KB) product images. 
-- =============================================

USE btprod

PRINT 'Execution Time';
PRINT GETDATE(); 

DECLARE db_cursor CURSOR 
FOR SELECT TOP 100
	PI.ProductID, 
	PI.Image, 
	datalength(PI.Image) as len
FROM 
	productImage PI
	left join product P on PI.ProductID = P.ProductID
WHERE 
	datalength(PI.Image) > 300000
ORDER BY len DESC;

OPEN db_cursor;

DECLARE 
    @img_id int, 
    @img_binary varbinary(max),
	@img_len int;

FETCH NEXT FROM db_cursor INTO 
	@img_id,
	@img_binary,
	@img_len;

WHILE @@FETCH_STATUS = 0
    BEGIN
		DECLARE @x VARBINARY(max);
		EXEC ResizeImage @value = @x OUTPUT, @img=@img_binary, @width=750, @height=750;

		print('Image ID: ' +  CAST(@img_id AS VARCHAR) + ' Original Size: ' + CAST(@img_len AS VARCHAR) + ' Resized: ' + CAST(datalength(@x) AS VARCHAR));
        
		SELECT 
			@img_id AS ImageID, 
			(SUM(DATALENGTH(@img_binary)) / 1024.0 / 1024.0) AS Original_MB, 
			(SUM(DATALENGTH(@img_binary)) / 1024.0) AS Original_KB,
			(SUM(DATALENGTH(@x)) / 1024.0) AS Resized_KB;

		UPDATE productImage
		SET	
			Image = @x
		WHERE 
			ProductID = @img_id;

        print '
		';

        FETCH NEXT FROM db_cursor INTO 
			@img_id,
			@img_binary,
			@img_len;
    END;

CLOSE db_cursor;
DEALLOCATE db_cursor;