#if !DT_EXCLUDE_SAMPLES
using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.DigitalTwins.Common;
using UnityEngine;

namespace Unity.DigitalTwins.Annotation.Samples.TopicsAndComments
{
    public class TopicsAndCommentsSample : AnnotationSampleBehaviour
    {
        [SerializeField]
        TopicUIController m_AnnotationUI;

        AnnotationRepository m_AnnotationRepository;
        SynchronizationContext m_UnityMainThread;

        void Awake()
        {
            m_UnityMainThread = SynchronizationContext.Current;
        }

        void Start()
        {

            m_AnnotationUI.CreateTopic += OnCreateTopicAsync;
            m_AnnotationUI.UpdateTopicTitle += OnUpdateTopicTitleAsync;
            m_AnnotationUI.UpdateTopicResolution += OnUpdateTopicResolutionAsync;
            m_AnnotationUI.DeleteTopic += OnDeleteTopicAsync;

            m_AnnotationUI.CreateComment += OnCreateCommentAsync;
            m_AnnotationUI.UpdateComment += OnUpdateCommentAsync;
            m_AnnotationUI.DeleteComment += OnDeleteCommentAsync;
        }

        void OnDestroy()
        {
            m_AnnotationUI.CreateTopic -= OnCreateTopicAsync;
            m_AnnotationUI.UpdateTopicTitle -= OnUpdateTopicTitleAsync;
            m_AnnotationUI.UpdateTopicResolution -= OnUpdateTopicResolutionAsync;
            m_AnnotationUI.DeleteTopic -= OnDeleteTopicAsync;

            m_AnnotationUI.CreateComment -= OnCreateCommentAsync;
            m_AnnotationUI.UpdateComment -= OnUpdateCommentAsync;
            m_AnnotationUI.DeleteComment -= OnDeleteCommentAsync;

            if (m_AnnotationRepository != null)
            {
                m_AnnotationRepository.TopicCreated -= OnTopicAddedOrUpdatedNotification;
                m_AnnotationRepository.TopicUpdated -= OnTopicAddedOrUpdatedNotification;
                m_AnnotationRepository.TopicRemoved -= OnTopicRemovedNotification;
                m_AnnotationRepository.Dispose();
            }
        }

        public override async Task InitializeAsync(IScene scene, IAccessTokenProvider accessTokenProvider, CloudConfiguration cloudConfiguration)
        {
            m_AnnotationRepository = new AnnotationRepository( scene, PlatformServices.ServiceHttpClient, cloudConfiguration);

            try
            {
                var topics =  await m_AnnotationRepository.GetTopicsAsync();

                await m_AnnotationUI.OnInitialize(topics);

                m_AnnotationRepository.TopicCreated += OnTopicAddedOrUpdatedNotification;
                m_AnnotationRepository.TopicUpdated += OnTopicAddedOrUpdatedNotification;
                m_AnnotationRepository.TopicRemoved += OnTopicRemovedNotification;
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        async Task<ITopic> OnCreateTopicAsync(string topicTitle)
        {
            var topicCreation = new TopicCreation() { Title = topicTitle };

            try
            {
                var topic = await m_AnnotationRepository.CreateTopicAsync(topicCreation);

                return topic;
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        async Task<ITopic> OnUpdateTopicTitleAsync(ITopic topicToUpdate, string topicTitle)
        {
            var topicUpdate = new TopicUpdate(topicToUpdate)
            {
                Title = topicTitle
            };

            try
            {
                var topic = await m_AnnotationRepository.UpdateTopicAsync(topicUpdate);

                return topic;
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        async Task<ITopic> OnUpdateTopicResolutionAsync(ITopic topicToUpdate, bool resolution)
        {
            var topicUpdate = new TopicUpdate(topicToUpdate)
            {
                State = resolution ? TopicState.Resolved : TopicState.NeedsResolution
            };

            try
            {
                var topic = await m_AnnotationRepository.UpdateTopicAsync(topicUpdate);

                return topic;

            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        async Task OnDeleteTopicAsync(Guid topicId)
        {
            try
            {
                await m_AnnotationRepository.DeleteTopicAsync(topicId);
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        static async Task<IComment> OnCreateCommentAsync(ITopic topic, string commentText)
        {
            var commentCreation = new CommentCreation()
            {
                Text = commentText
            };

            try
            {
                var comment = await topic.CreateCommentAsync(commentCreation);

                return comment;
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        static async Task<IComment> OnUpdateCommentAsync(ITopic topic, IComment commentToUpdate, string commentText)
        {
            var commentUpdate = new CommentUpdate(commentToUpdate)
            {
                Text = commentText
            };

            try
            {
                var comment = await topic.UpdateCommentAsync(commentUpdate);

                return comment;
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        static async Task OnDeleteCommentAsync(ITopic topic, Guid commentId)
        {
            try
            {
                await topic.DeleteCommentAsync(commentId);
            }
            catch (ServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        void OnTopicAddedOrUpdatedNotification(ITopic topic)
        {
            // Ensure we process the notification on the Unity main thread
            m_UnityMainThread.Post(_ => m_AnnotationUI.AddOrUpdateTopic(topic), null);
        }

        void OnTopicRemovedNotification(ITopic topic)
        {
            // Ensure we process the notification on the Unity main thread
            m_UnityMainThread.Post(_ => m_AnnotationUI.DeleteTopicContainer(topic.Id), null);
        }
    }
}
#endif
