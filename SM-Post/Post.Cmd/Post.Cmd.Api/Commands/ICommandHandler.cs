using System;

namespace Post.Cmd.Api.Commands
{
    public interface ICommandHandler
    {
        Task HandleAsync(NewPostCommand command);
        Task HandleAsync(EditMessageCommand command);
        Task HandleAsync(LikePostCommand command);
        Task HandleAsync(EditPostCommand command);
        Task HandleAsync(RemoveCommentCommand command);
        Task HandleAsync(DeletPostCommand command);

    }
}
