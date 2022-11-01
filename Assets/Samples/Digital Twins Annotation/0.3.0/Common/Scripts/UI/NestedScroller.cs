#if !DT_EXCLUDE_SAMPLES
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.DigitalTwins.Annotation.Samples
{
    public class NestedScroller : ScrollRect
    {
        bool m_IsNested;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            m_IsNested = IsNestedEvent(eventData);

            if (m_IsNested)
                SendNestedEvent<IBeginDragHandler>(parent => parent.OnBeginDrag(eventData));
            else
                base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (m_IsNested)
                SendNestedEvent<IDragHandler>(parent => parent.OnDrag(eventData));
            else
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (m_IsNested)
                SendNestedEvent<IEndDragHandler>(parent => parent.OnEndDrag(eventData));
            else
                base.OnEndDrag(eventData);
        }

        void SendNestedEvent<T>(Action<T> scrollEvent) where T : IEventSystemHandler
        {
            var parent = transform.parent;
            while (parent != null)
            {
                var nestedScroller = parent.GetComponent<ScrollRect>();
                if (nestedScroller is T)
                    scrollEvent((T) (IEventSystemHandler) nestedScroller);

                parent = parent.parent;
            }
        }

        bool IsNestedEvent(PointerEventData eventData)
        {
            bool isNested = false;

            if (!horizontal)
                isNested |= Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y);

            if (!vertical)
                isNested |= Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y);

            return isNested;
        }
    }
}
#endif
