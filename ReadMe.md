About
-----
By default the Kanban column is not reportable. The goal of this TFS Server Plugin is to copy the Kanban pseudo state to the work item where it will then flow through to the warehouse for reporting.


Thank you:
----------
The idea for this project came from the work done by 

Gordon Beeming http://gordonbeeming.azurewebsites.net/2013/05/29/get-tfs-kanban-column-state-to-the-warehouse/

and

Joe Gaggler http://stackoverflow.com/questions/18193036/access-the-kanban-column-a-team-specific-field-for-a-work-item/19824253#19824253


it was tested with FabrikamFiberCollection from the demo install of TFS 2013.2 provided by Brian Keller http://aka.ms/almvms

Potential issues:
-----------------
* Work items that have multiple kanban pseudo-states
* There is a delay between when you move the card and when the field gets updated. if you open the work item right after moving the card it will show the old state, and you will need to refresh the card to see the state change.

Other Solutions:
----------------
* TFS 2013 Tabular and PowerPivot Kanban Model by Jeff Levinson (http://visualstudiogallery.msdn.microsoft.com/cc78e860-38c1-4e33-97eb-4e23ac4d4b9b)
