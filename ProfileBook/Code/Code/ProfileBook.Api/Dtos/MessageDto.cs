using System;

namespace ProfileBook.Api.Dtos
{
    // Message DTO now includes sender and receiver full info (nullable when unknown)
    public record MessageDto(int Id, string SenderId, string ReceiverId, string Body, DateTime SentAt,
                             UserDto? Sender = null, UserDto? Receiver = null);
}
