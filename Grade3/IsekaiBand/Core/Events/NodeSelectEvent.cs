using _Work.CHUH.Code.Tree;
using Chuh007Lib.Bus;

namespace _Work.CHUH.Code.Core.Events
{
    public struct NodeSelectEvent : IEvent
    {
        public TreeNodeDataSO NodeData;

        public NodeSelectEvent(TreeNodeDataSO nodeData)
        {
            NodeData = nodeData;
        }
    }
}