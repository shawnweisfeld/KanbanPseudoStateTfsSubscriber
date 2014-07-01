using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanbanPseudoStateTfsSubscriber
{
    public static class TfsClientHelper
    {
        public static void UpdateKanbanColumn(int id, string newKanbanColumn, TeamFoundationRequestContext requestContext)
        {
            Uri uri = GetTFSUri(requestContext);
            using (var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(uri))
            {
                var wis = tfs.GetService<WorkItemStore>();
                WorkItem wiCurrent = wis.GetWorkItem(id);

                var wiTypes = new List<string>()
                {
                    "Product Backlog Item",
                    "User Story"
                };

                var currentWIType = wiCurrent.Fields["Work Item Type"].Value.ToString();

                if (wiCurrent.Fields.Contains("KanBan State")
                    && wiTypes.Any(x => x.Equals(currentWIType, StringComparison.InvariantCultureIgnoreCase))
                    && wiCurrent.Fields["KanBan State"].Value.ToString() != newKanbanColumn)
                {
                    wiCurrent.Fields["KanBan State"].Value = newKanbanColumn;
                    wiCurrent.Save();
                    wiCurrent.Close();
                }

            }
        }

        public static Uri GetTFSUri(TeamFoundationRequestContext requestContext)
        {
            return new Uri(requestContext.GetService<TeamFoundationLocationService>().GetServerAccessMapping(requestContext).AccessPoint.Replace("localhost", Environment.MachineName) + "/" + requestContext.ServiceHost.Name);
        }

    }
}
