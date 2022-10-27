#if !DT_EXCLUDE_SAMPLES
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class CommentContainer : MonoBehaviour
    {
        public event UpdateCommentAction UpdateComment;
        public event DeleteCommentAction DeleteComment;

        [SerializeField]
        Text m_Author;

        [SerializeField]
        Text m_Date;

        [SerializeField]
        Text m_CommentText;

        [SerializeField]
        Button m_FlyoutButton;

        [SerializeField]
        FlyoutDialog m_FlyoutDialog;

        ITopic m_Topic;
        IComment m_Comment;

        public IComment Comment => m_Comment;

        void Awake()
        {
            m_FlyoutButton.onClick.AddListener(ToggleFlyout);
            m_FlyoutDialog.Edit += OnEdit;
            m_FlyoutDialog.Delete += OnDelete;
        }

        void OnDestroy()
        {
            UpdateComment = null;
            DeleteComment = null;

            m_FlyoutButton.onClick.RemoveListener(ToggleFlyout);
            m_FlyoutDialog.Edit -= OnEdit;
            m_FlyoutDialog.Delete -= OnDelete;
        }

        public void Set(ITopic topic, IComment comment)
        {
            m_Topic = topic;
            m_Comment = comment;

            m_Author.text = m_Comment.Author.FullName;
            m_Date.text = m_Comment.Date.ToString();
            m_CommentText.text = m_Comment.Text;
        }

        void ToggleFlyout()
        {
            if (!m_FlyoutDialog.gameObject.activeSelf)
                m_FlyoutDialog.OpenDialog();
            else
                m_FlyoutDialog.CloseDialog();
        }

        void OnEdit(string updatedComment)
        {
            if (UpdateComment != null)
                UpdateComment.Invoke(m_Topic, m_Comment, updatedComment);
        }

        void OnDelete()
        {
            if (DeleteComment != null)
                DeleteComment.Invoke(m_Topic, m_Comment.Id);
        }
    }
}
#endif
