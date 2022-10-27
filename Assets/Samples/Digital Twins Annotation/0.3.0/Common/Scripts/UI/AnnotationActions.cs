#if !DT_EXCLUDE_SAMPLES
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public delegate Task<ITopic> CreateTopicAction(string topicTitle);

    public delegate Task<ITopic> UpdateTopicTitleAction(ITopic topicToUpdate, string updatedTopicTitle);

    public delegate Task<ITopic> UpdateTopicResolutionAction(ITopic topicToUpdate, bool resolution);

    public delegate Task DeleteTopicAction(Guid topicId);

    public delegate Task<IComment> CreateCommentAction(ITopic topic, string commentText);

    public delegate Task<IComment> UpdateCommentAction(ITopic topic, IComment commentToUpdate, string commentText);

    public delegate Task DeleteCommentAction(ITopic topic, Guid commentId);
}
#endif
