using DemoExam.Data;
using DemoExam.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace DemoExam.ViewModels
{
    public class CommentsPageViewModel : ViewModel
    {
        private Comment _comment;
        public Comment Comment
        {
            get => _comment;
            set => SetField(ref _comment, value);
        }

        private ObservableCollection<Comment> _comments;
        public ObservableCollection<Comment> Comments
        {
            get => _comments;
            set => SetField(ref _comments, value);
        }

        private ObservableCollection<Request> _requests;
        public ObservableCollection<Request> Requests
        {
            get => _requests;
            set => SetField(ref _requests, value);
        }

        public CommentsPageViewModel()
        {
            using (var db = new Db())
            {
                Comments = new ObservableCollection<Comment>();
                Requests = new ObservableCollection<Request>(
                    db.Select<Request>(null, db.MyRequestsWhereClause, db.MyRequestsWhereArgs));

                foreach (var request in Requests)
                {
                    var comments = db.Select<Comment>(null, db.CommentsSentToMeWhereClause, new object[1] { request.Id });

                    if (comments.Any())
                        foreach (var comment in comments)
                            Comments.Add(comment);
                }
            }
        }
    }
}
