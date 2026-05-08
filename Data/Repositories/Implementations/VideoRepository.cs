using PremierLeague_Backend.Data.Repositories.Interfaces;
using PremierLeague_Backend.Models.DTOs;
using PremierLeague_Backend.Services.Interfaces;
using static PremierLeague_Backend.Helper.SqlCommands.VideoCommands;
using System.Data.SqlClient;
using PremierLeague_Backend.Helper;
using System.Text.Json;

namespace PremierLeague_Backend.Data.Repositories.Implementations;

public class VideoRepository : IVideoRepository
{
    private readonly IExecute execute;

    public VideoRepository(IExecute execute)
    {
        this.execute = execute;
    }

    public async Task<bool> AddVideoAsync(VideoDto videoDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = AddVideoCommand;
            cmd.Parameters.AddWithValue("@Title", videoDto.Title);
            cmd.Parameters.AddWithValue("@Description", videoDto.Description);
            cmd.Parameters.AddWithValue("@Channel", videoDto.Channel);
            cmd.Parameters.AddWithValue("@ThumbnailUrl", videoDto.ThumbnailUrl);
            cmd.Parameters.AddWithValue("@VideoUrl", videoDto.VideoUrl);
            cmd.Parameters.AddWithValue("@ReferenceUrl", videoDto.ReferenceUrl);
            cmd.Parameters.AddWithValue("@VideoCategoryId", videoDto.VideoCategoryId);
            cmd.Parameters.AddWithValue("@VideoTagId", videoDto.VideoTagId);
            cmd.Parameters.AddWithValue("@DurationSeconds", videoDto.DurationSeconds);
            cmd.Parameters.AddWithValue("@Publisher", videoDto.Publisher);
            cmd.Parameters.AddWithValue("@PublishedDate", videoDto.PublishedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", videoDto.ExpiryDate);
            cmd.Parameters.AddWithValue("@IsStory", videoDto.IsStory);
            cmd.Parameters.AddWithValue("@IsReference", videoDto.IsReference);
            cmd.Parameters.AddWithValue("@IsFeatured", videoDto.IsFeatured);
            cmd.Parameters.AddWithValue("@IsActive", videoDto.IsActive);
            cmd.Parameters.AddWithValue("@MatchId", videoDto.MatchId);
            cmd.Parameters.AddWithValue("@SeasonId", videoDto.SeasonId);
            cmd.Parameters.AddWithValue("@ClubJson", JsonSerializer.Serialize(videoDto.ClubIds));
            cmd.Parameters.AddWithValue("@PlayerJson", JsonSerializer.Serialize(videoDto.PlayerIds));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database add video error: {ex.Message}");
        }
    }

    public async Task<bool> DeleteVideoAsync(int videoId)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = DeleteVideoCommand;
            cmd.Parameters.AddWithValue("@VideoId", videoId);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database delete video error: {ex.Message}");
        }
    }

    public async Task<bool> FindVideoExisting(VideoDto videoDto, int? videoId = 0)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = FindVideoExistingCommand;
            cmd.Parameters.AddWithValue("@VideoId", videoId);
            cmd.Parameters.AddWithValue("@Title", videoDto.Title);
            cmd.Parameters.AddWithValue("@VideoCategoryId", videoDto.VideoCategoryId);
            cmd.Parameters.AddWithValue("@PublishedDate", videoDto.PublishedDate);
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database find video existing error: {ex.Message}");
        }
    }

    public async Task<IEnumerable<PlayerStatGetPlayersDto>> GetAllPlayersAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllPlayersCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var players = new List<PlayerStatGetPlayersDto>();
            if (rdr is not null)
            {
                do
                {
                    players.Add(new PlayerStatGetPlayersDto(
                        MatchId: rdr.SafeGetInt("MatchId"),
                        ClubId: rdr.SafeGetInt("ClubId"),
                        PlayerId: rdr.SafeGetInt("PlayerId"),
                        ClubName: rdr.SafeGetString("ClubName"),
                        ClubCrest: rdr.SafeGetString("ClubCrest"),
                        ClubTheme: rdr.SafeGetString("ClubTheme"),
                        FirstName: rdr.SafeGetString("FirstName"),
                        LastName: rdr.SafeGetString("LastName"),
                        Position: rdr.SafeGetString("Position"),
                        PlayerNumber: rdr.SafeGetInt("PlayerNumber"),
                        Photo: rdr.SafeGetString("Photo")
                    ));
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }
            return players;
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<VideoDetailDto>> GetAllVideosAsync(int? page = 1, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetAllVideosCommand;
            cmd.Parameters.AddWithValue("@Page", page);
            var rdr = await execute.ExecuteReaderAsync(cmd, ct);
            var videos = new List<VideoDetailDto>();
            do
            {
                videos.Add(new VideoDetailDto
                {
                    VideoId = rdr.SafeGetInt("VideoId"),
                    Title = rdr.SafeGetString("Title"),
                    Channel = rdr.SafeGetString("Channel"),
                    Description = rdr.SafeGetString("Description"),
                    ThumbnailUrl = rdr.SafeGetString("ThumbnailUrl"),
                    VideoUrl = rdr.SafeGetString("VideoUrl"),
                    VideoCategory = rdr.SafeGetString("VideoCategory"),
                    DurationSeconds = rdr.SafeGetString("DurationSeconds"),
                    Publisher = rdr.SafeGetString("Publisher"),
                    PublishedDate = rdr.SafeGetString("PublishedDate"),
                    ExpiryDate = rdr.SafeGetString("ExpiryDate"),
                    IsFeatured = rdr.SafeGetBoolean("IsFeatured"),
                    IsActive = rdr.SafeGetBoolean("IsActive")
                });
            } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            return videos;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database get all videos error: {ex.Message}");
        }
    }

    public async Task<VideoDto> GetVideoByIdAsync(int videoId, CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = GetVideoByIdCommand;
            cmd.Parameters.AddWithValue("@VideoId", videoId);
            var rdr = await execute.ExecuteReaderAsync(cmd);
            var video = new VideoDto();

            if (rdr is not null)
            {
                do
                {
                    video = new VideoDto
                    {
                        VideoId = rdr.SafeGetInt("VideoId"),
                        Title = rdr.SafeGetString("Title"),
                        Channel = rdr.SafeGetString("Channel"),
                        Description = rdr.SafeGetString("Description"),
                        ThumbnailUrl = rdr.SafeGetString("ThumbnailUrl"),
                        VideoUrl = rdr.SafeGetString("VideoUrl"),
                        ReferenceUrl = rdr.SafeGetString("ReferenceUrl"),
                        VideoCategoryId = rdr.SafeGetInt("VideoCategoryId"),
                        VideoTagId = rdr.SafeGetInt("VideoTagId"),
                        DurationSeconds = rdr.SafeGetDecimal("DurationSeconds"),
                        PublishedDate = rdr.SafeGetDateTime("PublishedDate"),
                        ExpiryDate = rdr.SafeGetDateTime("ExpiryDate"),
                        MatchId = rdr.SafeGetInt("MatchId"),
                        SeasonId = rdr.SafeGetInt("SeasonId"),
                        IsTheArchive = rdr.SafeGetBoolean("IsTheArchive"),
                        IsStory = rdr.SafeGetBoolean("IsStory"),
                        IsReference = rdr.SafeGetBoolean("IsReference"),
                        IsFeatured = rdr.SafeGetBoolean("IsFeatured"),
                        IsActive = rdr.SafeGetBoolean("IsActive"),
                        ClubIds = !string.IsNullOrEmpty(rdr.SafeGetString("Clubs")) ? JsonSerializer.Deserialize<List<int>>(rdr.SafeGetString("Clubs"))! : new List<int>(),
                        PlayerIds = !string.IsNullOrEmpty(rdr.SafeGetString("Players")) ? JsonSerializer.Deserialize<List<int>>(rdr.SafeGetString("Players"))! : new List<int>()
                    };
                } while (await rdr.ReadAsync(ct).ConfigureAwait(false));
            }

            return video;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database get video by id error: {ex.Message}");
        }
    }

    public async Task<int> GetVideoCountAsync(CancellationToken ct = default)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = CountVideosCommand;
            var rdr = await execute.ExecuteReaderAsync(cmd);
            return rdr is not null ? rdr.GetInt32(rdr.GetOrdinal("TotalCount")) : 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database count video error: {ex.Message}");
        }
    }

    public async Task<bool> UpdateVideoAsync(int videoId, VideoDto videoDto)
    {
        try
        {
            var cmd = new SqlCommand();
            cmd.CommandText = UpdateVideoCommand;
            cmd.Parameters.AddWithValue("@VideoId", videoId);
            cmd.Parameters.AddWithValue("@Title", videoDto.Title);
            cmd.Parameters.AddWithValue("@Description", videoDto.Description);
            cmd.Parameters.AddWithValue("@Channel", videoDto.Channel);
            cmd.Parameters.AddWithValue("@ThumbnailUrl", videoDto.ThumbnailUrl);
            cmd.Parameters.AddWithValue("@VideoUrl", videoDto.VideoUrl);
            cmd.Parameters.AddWithValue("@ReferenceUrl", videoDto.ReferenceUrl);
            cmd.Parameters.AddWithValue("@VideoCategoryId", videoDto.VideoCategoryId);
            cmd.Parameters.AddWithValue("@VideoTagId", videoDto.VideoTagId);
            cmd.Parameters.AddWithValue("@DurationSeconds", videoDto.DurationSeconds);
            cmd.Parameters.AddWithValue("@Publisher", videoDto.Publisher);
            cmd.Parameters.AddWithValue("@PublishedDate", videoDto.PublishedDate);
            cmd.Parameters.AddWithValue("@ExpiryDate", videoDto.ExpiryDate);
            cmd.Parameters.AddWithValue("@IsStory", videoDto.IsStory);
            cmd.Parameters.AddWithValue("@IsReference", videoDto.IsReference);
            cmd.Parameters.AddWithValue("@IsFeatured", videoDto.IsFeatured);
            cmd.Parameters.AddWithValue("@IsActive", videoDto.IsActive);
            cmd.Parameters.AddWithValue("@MatchId", videoDto.MatchId);
            cmd.Parameters.AddWithValue("@SeasonId", videoDto.SeasonId);
            cmd.Parameters.AddWithValue("@ClubJson", JsonSerializer.Serialize(videoDto.ClubIds));
            cmd.Parameters.AddWithValue("@PlayerJson", JsonSerializer.Serialize(videoDto.PlayerIds));
            return await execute.ExecuteScalarAsync<bool>(cmd) ? false : true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Database update video error: {ex.Message}");
        }
    }
}
