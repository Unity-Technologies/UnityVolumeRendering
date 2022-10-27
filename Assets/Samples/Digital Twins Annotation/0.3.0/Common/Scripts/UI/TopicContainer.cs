#if !DT_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class TopicContainer : MonoBehaviour
    {
        public event UpdateTopicTitleAction UpdateTopicTitle;
        public event DeleteTopicAction DeleteTopic;
        public event UpdateTopicResolutionAction UpdateTopicResolution;

        public event CreateCommentAction CreateComment;
        public event UpdateCommentAction UpdateComment;
        public event DeleteCommentAction DeleteComment;

        [SerializeField]
        Text m_Author;

        [SerializeField]
        Text m_Date;

        [SerializeField]
        Text m_Title;

        [SerializeField]
        RectTransform m_ResolutionIcon;

        [SerializeField]
        InputField m_CommentInput;

        [SerializeField]
        CommentContainer m_CommentContainerPrefab;

        [SerializeField]
        RectTransform m_CommentsAnchor;

        [SerializeField]
        Button m_FlyoutButton;

        [SerializeField]
        FlyoutDialog m_FlyoutDialog;

        readonly Dictionary<Guid, CommentContainer> m_CommentContainers = new();

        ITopic m_Topic;
        SynchronizationContext m_UnityMainThread;

        public ITopic Topic => m_Topic;

        void Awake()
        {
            m_UnityMainThread = SynchronizationContext.Current;

            m_CommentInput.onSubmit.AddListener(commentText => OnCreateComment(commentText));

            m_FlyoutButton.onClick.AddListener(ToggleFlyout);
            m_FlyoutDialog.Edit += OnEdit;
            m_FlyoutDialog.Delete += OnDelete;
            m_FlyoutDialog.ToggleResolution += OnToggleResolution;
        }

        void OnDestroy()
        {
            UpdateTopicTitle = null;
            DeleteTopic = null;
            UpdateTopicResolution = null;

            CreateComment = null;
            DeleteComment = null;

            m_CommentInput.onSubmit.RemoveAllListeners();

            m_FlyoutButton.onClick.RemoveListener(ToggleFlyout);
            m_FlyoutDialog.Edit -= OnEdit;
            m_FlyoutDialog.Delete -= OnDelete;
            m_FlyoutDialog.ToggleResolution -= OnToggleResolution;

            if (m_Topic != null)
            {
                m_Topic.CommentCreated -= OnCommentAddedOrUpdatedNotification;
                m_Topic.CommentUpdated -= OnCommentAddedOrUpdatedNotification;
                m_Topic.CommentRemoved -= OnCommentRemovedNotification;
            }
        }

        public async Task Set(ITopic topic)
        {
            m_Topic = topic;

            m_Topic.CommentCreated += OnCommentAddedOrUpdatedNotification;
            m_Topic.CommentUpdated += OnCommentAddedOrUpdatedNotification;
            m_Topic.CommentRemoved += OnCommentRemovedNotification;

            m_Author.text = m_Topic.CreationAuthor.FullName;
            m_Date.text = m_Topic.CreationDate.ToString();
            m_Title.text = m_Topic.Title;
            m_ResolutionIcon.gameObject.SetActive(m_Topic.State == TopicState.Resolved);

            await PopulateComments();
        }

        async Task PopulateComments()
        {
            foreach (var commentContainer in m_CommentContainers)
            {
                Destroy(commentContainer.Value.gameObject);
            }
            m_CommentContainers.Clear();

            var comments = await m_Topic.GetCommentsAsync();

            if (comments != null)
            {
                foreach (var comment in comments)
                    AddOrUpdateComment(comment);
            }
        }

        async void OnCreateComment(string commentText)
        {
            if (!string.IsNullOrEmpty(commentText) && CreateComment != null)
            {
                m_CommentInput.text = string.Empty;

                var comment = await CreateComment.Invoke(m_Topic, commentText);
                AddOrUpdateComment(comment);
                OrderComments();
            }
        }

        async Task<IComment> OnUpdateComment(ITopic topic, IComment commentToUpdate, string commentText)
        {
            if (!string.IsNullOrEmpty(commentText) && UpdateComment != null)
            {
                var comment = await UpdateComment.Invoke(topic, commentToUpdate, commentText);
                AddOrUpdateComment(comment);
                OrderComments();

                return comment;
            }

            return null;
        }

        async Task OnDeleteComment(ITopic topic, Guid commentId)
        {
            DeleteCommentContainer(commentId);

            if (DeleteComment != null)
                await DeleteComment.Invoke(topic, commentId);
        }

        void AddOrUpdateComment(IComment comment)
        {
            var commentContainer = GetOrCreateCommentContainer(comment.Id);
            commentContainer.Set(m_Topic, comment);
        }

        CommentContainer GetOrCreateCommentContainer(Guid commentId)
        {
            if (m_CommentContainers.ContainsKey(commentId))
                return m_CommentContainers[commentId];

            var commentContainer = Instantiate(m_CommentContainerPrefab, m_CommentsAnchor);
            m_CommentContainers[commentId] = commentContainer;

            commentContainer.UpdateComment += OnUpdateComment;
            commentContainer.DeleteComment += OnDeleteComment;

            return commentContainer;
        }

        void DeleteCommentContainer(Guid commentId)
        {
            if (m_CommentContainers.ContainsKey(commentId))
            {
                Destroy(m_CommentContainers[commentId].gameObject);
                m_CommentContainers.Remove(commentId);
            }
        }

        void OrderComments()
        {
            // Order comments oldest-to-newest

            var orderedComments = m_CommentContainers.OrderBy(c => c.Value.Comment.Date);

            var siblingIndex = 0;
            foreach (var commentContainer in orderedComments)
                commentContainer.Value.transform.SetSiblingIndex(siblingIndex++);
        }

        void ToggleFlyout()
        {
            if (!m_FlyoutDialog.gameObject.activeSelf)
                m_FlyoutDialog.OpenDialog(m_Topic.State == TopicState.Resolved);
            else
                m_FlyoutDialog.CloseDialog();
        }

        void OnEdit(string updatedTitle)
        {
            if (UpdateTopicTitle != null)
                UpdateTopicTitle.Invoke(m_Topic, updatedTitle);
        }

        void OnDelete()
        {
            if (DeleteTopic != null)
                DeleteTopic.Invoke(m_Topic.Id);
        }

        void OnToggleResolution()
        {
            bool newState = m_Topic.State != TopicState.Resolved;

            if (UpdateTopicResolution != null)
                UpdateTopicResolution.Invoke(m_Topic, newState);
        }

        void OnCommentAddedOrUpdatedNotification(IComment comment)
        {
            // Ensure we process the notification on the Unity main thread
            m_UnityMainThread.Post(_ => AddOrUpdateComment(comment), null);
        }

        void OnCommentRemovedNotification(IComment comment)
        {
            // Ensure we process the notification on the Unity main thread
            m_UnityMainThread.Post(_ => DeleteCommentContainer(comment.Id), null);
        }
    }
}
#endif
