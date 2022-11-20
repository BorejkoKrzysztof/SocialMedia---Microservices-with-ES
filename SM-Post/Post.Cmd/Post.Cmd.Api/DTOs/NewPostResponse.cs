using System;

namespace Post.Cmd.Api.DTOs
{
    public class NewPostResponse : PostResponse
    {
        public Guid Id { get; set; }
    }
}
