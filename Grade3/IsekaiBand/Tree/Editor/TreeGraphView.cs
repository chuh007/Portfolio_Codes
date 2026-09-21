using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Work.CHUH.Code.Tree.Editor
{
    public class TreeGraphView : GraphView
    {
        public Action<TreeNodeDataSO> OnNodeSelected;
        public Action<TreeNodeDataSO, TreeGraphNode> OnNodeCreated; 
        public Action<TreeNodeDataSO> OnNodeDeletedFromGraph; 
        public Action OnGraphChanged;
        private readonly EditorWindow _ownerWindow;
        private TreeSearchWindow _searchWindow;

        public TreeGraphView(EditorWindow ownerWindow)
        {
            _ownerWindow = ownerWindow;
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentZoomer());

            this.graphViewChanged = (changes) => {
                if (changes.edgesToCreate != null || changes.elementsToRemove != null) {
                    foreach (var node in nodes.ToList().Cast<TreeGraphNode>()) {
                        if (node.Data != null) {
                            Undo.RecordObject(node.Data, "Graph Change");
                            EditorUtility.SetDirty(node.Data);
                        }
                    }
                }
                OnGraphChanged?.Invoke();
                return changes;
            };

            _searchWindow = ScriptableObject.CreateInstance<TreeSearchWindow>();
            _searchWindow.Init(this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
            deleteSelection = (operationName, askUser) => {
                foreach (var selectable in selection) if (selectable is TreeGraphNode node) OnNodeDeletedFromGraph?.Invoke(node.Data);
                DeleteSelection(); 
            };
            style.flexGrow = 1;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        public void CheckSelection()
        {
            var selectedNode = selection.FirstOrDefault(x => x is TreeGraphNode) as TreeGraphNode;
            OnNodeSelected?.Invoke(selectedNode?.Data);
        }

        public void CreateNewNodeOnMouse(Vector2 screenMousePosition)
        {
            string targetFolder = "Assets/_Work/CHUH/SO/Tree";
            CreateFolderRecursively(targetFolder);

            TreeNodeDataSO newData = ScriptableObject.CreateInstance<TreeNodeDataSO>();
            newData.nodeName = "New Node";
            newData.nodeID = Guid.NewGuid().ToString();
            newData.name = newData.nodeName;
            string path = AssetDatabase.GenerateUniqueAssetPath($"{targetFolder}/{SanitizeAssetName(newData.nodeName)}.asset");
            
            AssetDatabase.CreateAsset(newData, path);
            AssetDatabase.ImportAsset(path); // 즉시 임포트하여 인식 유도

            Vector2 windowMousePosition = _ownerWindow != null
                ? screenMousePosition - _ownerWindow.position.position
                : screenMousePosition;
            var localPos = contentViewContainer.WorldToLocal(windowMousePosition);
            newData.graphPosition = localPos;

            var node = CreateNode(newData);
            node.SetPosition(new Rect(localPos, new Vector2(200, 150)));
            EditorUtility.SetDirty(newData);
            AssetDatabase.SaveAssets();

            OnNodeCreated?.Invoke(newData, node);
            
            node.RefreshTitle();
            ClearSelection(); AddToSelection(node); CheckSelection();
            
            Undo.RegisterCreatedObjectUndo(newData, "Create Node");
        }

        private void CreateFolderRecursively(string path)
        {
            string[] folders = path.Split('/'); string currentPath = folders[0];
            for (int i = 1; i < folders.Length; i++) {
                string nextPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(nextPath)) AssetDatabase.CreateFolder(currentPath, folders[i]);
                currentPath = nextPath;
            }
        }

        public TreeGraphNode CreateNode(TreeNodeDataSO data)
        {
            var node = new TreeGraphNode(data);
            AddElement(node);
            return node;
        }

        public void ConnectPorts(Port output, Port input)
        {
            var edge = output.ConnectTo(input);
            AddElement(edge);
        }

        private static string SanitizeAssetName(string value)
        {
            string assetName = string.IsNullOrWhiteSpace(value) ? "New Node" : value.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                assetName = assetName.Replace(invalidChar, '_');
            return assetName;
        }
    }
}
