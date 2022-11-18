using System;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments =
                                            new Dictionary<Guid, Tuple<string, string>>();

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public PostAggregate()
        {

        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _active = true;
            _author = @event.Author;
        }

        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot edit inactive post");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException("Message is empty");
            }

            RaiseEvent(new MessageUpdateEvent
            {
                Id = _id,
                Message = message,
            });
        }

        public void Apply(MessageUpdateEvent @event)
        {
            _id = @event.Id;
        }

        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot like inactive post");
            }

            RaiseEvent(new PostLikedEvent
            {
                Id = _id,
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }

        public void AddComment(string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot add comment to an inactive post");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentNullException("Comment is empty");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("UserName is empty");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                UserName = userName,
                CommentDate = DateTime.Now
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            Id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.UserName));
        }

        public void EditPost(Guid commentId, string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot edit comment to an inactive post");
            }

            if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You cannot modify not yours post");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                UserName = userName,
                EditDate = DateTime.Now
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Id, @event.Comment, @event.UserName);
        }

        public void RemoveComment(Guid commentId, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You cannot remove comment to an inactive post");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("UserName is empty");
            }

            if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You cannot rempve not your post");
            }

            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove[@event.CommentId];
        }

        public void RemovePost(string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("The post has already been removed");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("UserName is empty");
            }

            if (!_author.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("You are not allowed to remove this post");
            }

            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
