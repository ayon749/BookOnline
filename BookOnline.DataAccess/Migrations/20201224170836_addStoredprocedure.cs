using Microsoft.EntityFrameworkCore.Migrations;

namespace BookOnline.DataAccess.Migrations
{
    public partial class addStoredprocedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROC usp_GetCoverTypes 
                                    AS 
                                    BEGIN 
                                     SELECT * FROM   dbo.Covers
                                    END");

            migrationBuilder.Sql(@"CREATE PROC usp_GetCoverType 
                                    @CoverId int 
                                    AS 
                                    BEGIN 
                                     SELECT * FROM   dbo.Covers  WHERE  (CoverId = @CoverId) 
                                    END ");

            migrationBuilder.Sql(@"CREATE PROC usp_UpdateCoverType
	                                @CoverId int,
	                                @Name varchar(100)
                                    AS 
                                    BEGIN 
                                     UPDATE dbo.Covers
                                     SET  Name = @Name
                                     WHERE  CoverId = @CoverId
                                    END");

            migrationBuilder.Sql(@"CREATE PROC usp_DeleteCoverType
	                                @CoverId int
                                    AS 
                                    BEGIN 
                                     DELETE FROM dbo.Covers
                                     WHERE  CoverId = @CoverId
                                    END");

            migrationBuilder.Sql(@"CREATE PROC usp_CreateCoverType
                                   @Name varchar(100)
                                   AS 
                                   BEGIN 
                                    INSERT INTO dbo.Covers(Name)
                                    VALUES (@Name)
                                   END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE usp_GetCoverTypes");
            migrationBuilder.Sql(@"DROP PROCEDURE usp_GetCoverType");
            migrationBuilder.Sql(@"DROP PROCEDURE usp_UpdateCoverType");
            migrationBuilder.Sql(@"DROP PROCEDURE usp_DeleteCoverType");
            migrationBuilder.Sql(@"DROP PROCEDURE usp_CreateCoverType");

        }
    }
}
