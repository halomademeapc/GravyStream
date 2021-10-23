using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GravyStream.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Vod");

            migrationBuilder.CreateTable(
                name: "People",
                schema: "Vod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SsoId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Registered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.UniqueConstraint("AK_People_UserName", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                schema: "Vod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                schema: "Vod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Uploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Published = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Views = table.Column<long>(type: "bigint", nullable: false),
                    AllowsComments = table.Column<bool>(type: "bit", nullable: false),
                    AllowsReactions = table.Column<bool>(type: "bit", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OriginalFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.UniqueConstraint("AK_Videos_Slug", x => x.Slug);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "Vod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Vod",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_People_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Vod",
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Videos_VideoId",
                        column: x => x.VideoId,
                        principalSchema: "Vod",
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contributors",
                schema: "Vod",
                columns: table => new
                {
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributors", x => new { x.VideoId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_Contributors_People_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Vod",
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contributors_Videos_VideoId",
                        column: x => x.VideoId,
                        principalSchema: "Vod",
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Streams",
                schema: "Vod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Label = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codec = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Bitrate = table.Column<long>(type: "bigint", nullable: true),
                    ResolutionX = table.Column<int>(type: "int", nullable: true),
                    ResolutionY = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Streams_Videos_VideoId",
                        column: x => x.VideoId,
                        principalSchema: "Vod",
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Vod",
                columns: table => new
                {
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => new { x.VideoId, x.Tag });
                    table.ForeignKey(
                        name: "FK_Tags_Videos_VideoId",
                        column: x => x.VideoId,
                        principalSchema: "Vod",
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoReactions",
                schema: "Vod",
                columns: table => new
                {
                    VideoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoReactions", x => new { x.PersonId, x.VideoId });
                    table.ForeignKey(
                        name: "FK_VideoReactions_People_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "Vod",
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoReactions_Reactions_ReactionId",
                        column: x => x.ReactionId,
                        principalSchema: "Vod",
                        principalTable: "Reactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoReactions_Videos_VideoId",
                        column: x => x.VideoId,
                        principalSchema: "Vod",
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversionJobs",
                schema: "Vod",
                columns: table => new
                {
                    StreamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletedDuration = table.Column<TimeSpan>(type: "time", nullable: true),
                    TotalDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    TimeStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeCompleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionJobs", x => x.StreamId);
                    table.ForeignKey(
                        name: "FK_ConversionJobs_Streams_StreamId",
                        column: x => x.StreamId,
                        principalSchema: "Vod",
                        principalTable: "Streams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                schema: "Vod",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PersonId",
                schema: "Vod",
                table: "Comments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_VideoId",
                schema: "Vod",
                table: "Comments",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contributors_PersonId",
                schema: "Vod",
                table: "Contributors",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_VideoId",
                schema: "Vod",
                table: "Streams",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoReactions_ReactionId",
                schema: "Vod",
                table: "VideoReactions",
                column: "ReactionId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoReactions_VideoId",
                schema: "Vod",
                table: "VideoReactions",
                column: "VideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "Contributors",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "ConversionJobs",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "VideoReactions",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "Streams",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "People",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "Reactions",
                schema: "Vod");

            migrationBuilder.DropTable(
                name: "Videos",
                schema: "Vod");
        }
    }
}
