#if !DT_EXCLUDE_SAMPLES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class TopicUIController : MonoBehaviour
    {
        public event CreateTopicAction CreateTopic;
        public event UpdateTopicTitleAction UpdateTopicTitle;
        public event UpdateTopicResolutionAction UpdateTopicResolution;
        public event DeleteTopicAction DeleteTopic;

        public event CreateCommentAction CreateComment;
        public event UpdateCommentAction UpdateComment;
        public event DeleteCommentAction DeleteComment;

        [SerializeField]
        TopicContainer m_TopicContainerPrefab;

        [SerializeField]
        RectTransform m_TopicsAnchor;

        [SerializeField]
        Button m_CreateTopicButton;

        [SerializeField]
        InputDialog m_InputDialog;

        readonly Dictionary<Guid, TopicContainer> m_TopicContainers = new();

        void Awake()
        {
            m_CreateTopicButton.onClick.AddListener(CreateTopicPrompt);
        }

        void OnDestroy()
        {
            m_CreateTopicButton.onClick.RemoveListener(CreateTopicPrompt);
        }

        public async Task OnInitialize(IEnumerable<ITopic> topics)
        {
            foreach (var topicContainer in m_TopicContainers)
            {
                Destroy(topicContainer.Value.gameObject);
            }
            m_TopicContainers.Clear();

            if (topics != null)
            {
                foreach (var topic in topics)
                    await AddOrUpdateTopic(topic, false);

                OrderTopics();
            }
        }

        void CreateTopicPrompt()
        {
            m_InputDialog.OpenDialog(title => OnCreateTopic(title), "Input topic title..");
        }

        async void OnCreateTopic(string topicTitle)
        {
            if (CreateTopic != null)
            {
                var topic = await CreateTopic.Invoke(topicTitle);
                await AddOrUpdateTopic(topic);
            }
        }

        async Task<ITopic> OnUpdateTopicTitle(ITopic topicToUpdate, string topicTitle)
        {
            if (UpdateTopicTitle != null)
            {
                var topic = await UpdateTopicTitle.Invoke(topicToUpdate, topicTitle);
                await AddOrUpdateTopic(topic);

                return topic;
            }

            return null;
        }

        async Task<ITopic> OnUpdateTopicResolution(ITopic topicToUpdate, bool resolution)
        {
            if (UpdateTopicResolution != null)
            {
                var topic = await UpdateTopicResolution.Invoke(topicToUpdate, resolution);
                await AddOrUpdateTopic(topic);

                return topic;
            }

            return null;
        }

        async Task OnDeleteTopic(Guid topicId)
        {
            DeleteTopicContainer(topicId);

            if (DeleteTopic != null)
                await DeleteTopic.Invoke(topicId);
        }

        public async Task AddOrUpdateTopic(ITopic topic, bool orderTopics = true)
        {
            var topicContainer = GetOrCreateTopicContainer(topic.Id);
            await topicContainer.Set(topic);
            if (orderTopics)
                OrderTopics();
        }

        public void DeleteTopicContainer(Guid topicId)
        {
            if (m_TopicContainers.ContainsKey(topicId))
            {
                Destroy(m_TopicContainers[topicId].gameObject);
                m_TopicContainers.Remove(topicId);
            }
        }

        TopicContainer GetOrCreateTopicContainer(Guid topicId)
        {
            if (m_TopicContainers.ContainsKey(topicId))
                return m_TopicContainers[topicId];

            var topicContainer = Instantiate(m_TopicContainerPrefab, m_TopicsAnchor);
            m_TopicContainers[topicId] = topicContainer;

            topicContainer.UpdateTopicTitle += OnUpdateTopicTitle;
            topicContainer.DeleteTopic += OnDeleteTopic;
            topicContainer.UpdateTopicResolution += OnUpdateTopicResolution;

            topicContainer.CreateComment += CreateComment;
            topicContainer.UpdateComment += UpdateComment;
            topicContainer.DeleteComment += DeleteComment;

            return topicContainer;
        }

        void OrderTopics()
        {
            // Order topics newest-to-oldest

            var orderedTopics = m_TopicContainers.OrderByDescending(t => t.Value.Topic.CreationDate);

            var siblingIndex = 0;
            foreach (var topicContainer in orderedTopics)
                topicContainer.Value.transform.SetSiblingIndex(siblingIndex++);
        }
    }
}
#endif
