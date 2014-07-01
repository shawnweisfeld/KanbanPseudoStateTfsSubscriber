using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanPseudoStateTfsSubscriber
{
    public class KanbanCopySubscriber : ISubscriber
    {
        public string Name
        {
            get { return this.GetType().FullName; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Low; }
        }


        public Type[] SubscribedTypes()
        {
            return new[]
                       {
                           typeof(WorkItemChangedEvent)
                       };
        }

        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, 
            NotificationType notificationType, object notificationEventArgs, out int statusCode, out string statusMessage, 
            out Microsoft.TeamFoundation.Common.ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            properties = null;
            statusMessage = string.Empty;

            try
            {
                var workItemEvent = notificationEventArgs as WorkItemChangedEvent;

                if (notificationType == NotificationType.Notification
                    && workItemEvent != null
                    && workItemEvent.ChangedFields != null)
                {
                    int id = GetWorkItemId(workItemEvent);

                    if (id != 0)
                    {
                        var kanbanColumnName = GetCurrentKanbanColumn(requestContext, id);

                        if (!string.IsNullOrEmpty(kanbanColumnName))
                        {
                            TfsClientHelper.UpdateKanbanColumn(id, kanbanColumnName, requestContext);
                        }

                        return EventNotificationStatus.ActionPermitted;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new EventLog();
                log.Source = "TFS Services";
                if (!EventLog.SourceExists(log.Source))
                {
                    EventLog.CreateEventSource(log.Source, "Application");
                }

                string strMessage = string.Format("{0} - {1}\r\n{2}", this.GetType().FullName, ex.Message, ex.StackTrace);
                log.WriteEntry(strMessage, EventLogEntryType.Error);

                return EventNotificationStatus.ActionDenied;
            }

            return EventNotificationStatus.ActionApproved;
        }

        private string GetCurrentKanbanColumn(TeamFoundationRequestContext requestContext, int id)
        {
            var extService = new WorkItemTypeExtensionService();
            var workItemService = new WorkItemService();
            var svrWI = workItemService.GetWorkItem(requestContext, id);

            foreach (var wef in extService.GetExtensions(requestContext, svrWI.WorkItemTypeExtensionIds))
            {
                var foo = wef.Fields.Select(x => new { x.Field.FieldId, x.LocalName, x.LocalReferenceName });

                foreach (var field in wef.Fields)
                {
                    if (field.LocalReferenceName == "Kanban.Column" && field.LocalName == "Kanban Column")
                    {
                        return svrWI.LatestData[field.Field.FieldId].ToString();
                    }
                }
            }

            return string.Empty;
        }

        private int GetWorkItemId(WorkItemChangedEvent workItemEvent)
        {
            foreach (IntegerField item in workItemEvent.CoreFields.IntegerFields)
            {
                if (string.Compare(item.Name, "ID", true) == 0)
                {
                    return item.NewValue;
                }
            }
            return 0;
        }

    }
}
