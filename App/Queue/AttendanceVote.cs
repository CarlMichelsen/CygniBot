namespace App.Queue;

public record AttendanceVote(
    string UserId,
    string ChannelId,
    string MessageTs,
    DateOnly Value);