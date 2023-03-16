using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace grimbil_ef.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userid = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    useremail = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValueSql: "''"),
                    userpassword = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, defaultValueSql: "'0'"),
                    usertype = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.userid);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    postid = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userid = table.Column<int>(type: "int(11)", nullable: false),
                    title = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.postid);
                    table.ForeignKey(
                        name: "FK__users",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    commentid = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    comment = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<int>(type: "int(11)", nullable: false),
                    postid = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.commentid);
                    table.ForeignKey(
                        name: "FK_comments_posts",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "postid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comments_users",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pictures",
                columns: table => new
                {
                    pictureid = table.Column<int>(type: "int(11)", nullable: true),
                    picture = table.Column<byte[]>(type: "longblob", nullable: true),
                    postid = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_pictures_posts",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "postid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rating",
                columns: table => new
                {
                    ratingid = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    rating = table.Column<int>(type: "int(11)", nullable: false),
                    userid = table.Column<int>(type: "int(11)", nullable: false),
                    postid = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ratingid);
                    table.ForeignKey(
                        name: "FK_rating_posts",
                        column: x => x.postid,
                        principalTable: "posts",
                        principalColumn: "postid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rating_users",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_comments_posts",
                table: "comments",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "FK_comments_users",
                table: "comments",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "FK_pictures_posts",
                table: "pictures",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "userid",
                table: "posts",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "FK_rating_posts",
                table: "rating",
                column: "postid");

            migrationBuilder.CreateIndex(
                name: "FK_rating_users",
                table: "rating",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "pictures");

            migrationBuilder.DropTable(
                name: "rating");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
