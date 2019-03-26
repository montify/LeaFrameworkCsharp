using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using LeaFramework.Graphics;
using SharpDX;


namespace LeaFramework.PlayGround.TerrainSrc
{
    public enum NodeSide
    {
        left = 1,
        right = 2,
        top = 3,
        bottom = 4,
        front = 5,
        back = 6
    }

    public enum NodeType
    {
        root,
        lowerLeft,
        lowerRight,
        upperLeft,
        upperRight
    }

    public class QuadNode : IDisposable
    {
        private static readonly QueuedTaskScheduler TaskShedular = new QueuedTaskScheduler(0, "", false, ThreadPriority.BelowNormal);
        private Terrain terrain;
        private QuadNode parent, lowerLeft, lowerRight, upperRight, upperLeft;
        public QuadNodeExtents extents;
        private bool hasChildren;
        private NodeType nodeType;
       
        public int level;
        private QuadMesh mesh;
        private GraphicsDevice graphicsDevice;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _splitCompletionTask, _backgroundMergeTask;
        public bool isFinished;
        private bool isSplitting;
        private float scale;
        public NodeSide Side;
        
        //RootNode
        public QuadNode(QuadNodeExtents extents, Terrain terrain, GraphicsDevice graphicsDevice, NodeSide nodeSide)
        {
            this.graphicsDevice = graphicsDevice;
            this.extents = extents;
            this.terrain = terrain;
         
            this.level = 0;
            this.hasChildren = false;
            this.nodeType = NodeType.root;
            Side = nodeSide;
            scale = 1.0f;

            mesh = new QuadMesh(extents, graphicsDevice);
        }

        //ChildNode
        private QuadNode(QuadNode parent, NodeType nodeType, Color color)
        {
            this.graphicsDevice = parent.graphicsDevice;
            this.parent = parent;
            this.nodeType = nodeType;
            this.level = parent.level + 1;
            this.Side = parent.Side;
            this.scale = parent.scale * 0.5f;
            this.terrain = parent.terrain;

            CreateMesh(parent.extents);
        }

        private void CreateMesh(QuadNodeExtents ex)
        {
            var tmp = ex.Split();

            switch (nodeType)
            {
                case NodeType.lowerLeft:
                    extents = tmp.ElementAt(0);
                    //SpinWait.SpinUntil(() => isFinished);
                    mesh = new QuadMesh(extents, graphicsDevice);
                    break;
                case NodeType.lowerRight:
                    extents = tmp.ElementAt(2);
                   // SpinWait.SpinUntil(() => isFinished);
                    mesh = new QuadMesh(extents,  graphicsDevice);
                    break;

                case NodeType.upperLeft:
                    extents = tmp.ElementAt(1);
                  //  SpinWait.SpinUntil(() => isFinished);
                    mesh = new QuadMesh(extents, graphicsDevice);
                    break;
                case NodeType.upperRight:
                    extents = tmp.ElementAt(3);
                   // SpinWait.SpinUntil(() => isFinished);
                    mesh = new QuadMesh(extents, graphicsDevice);
                    break;
            }
        }

        private bool NeedsSplit()
        {
            // TODO: 128 = ScreenspaceError in Globals geben
            var geometricError = 128 * (float)Math.Pow(0.5, level + 1);
            geometricError *= (float)(extents.Radius / extents.Radius);    // TODO : adjust error based on earth-sized planet settings

            // limit splitting  

            //maxNODE -1 wegn neighbor
            if (level >= Globals.maxNodeDepth - 1) return false;

            // get distance between camera and terrain node center - both are already in planet space
            float viewDistance = (float)mesh.distanceFromCamera;

            // TODO : splitting not working properly when zooming in/out - seems to be the opposite of what's needed

            // calculate screen space error
            float k = 1024.0f * 0.5f * (float)Math.Tan((Math.PI/ 4) * 0.5f);
            float screenSpaceError = (geometricError / viewDistance) * k;

            // return true if the screen space error is greater than the max
            return (screenSpaceError > .7f);
        }

        public void Update()
        {
            mesh.Update();

            if (hasChildren && !isSplitting)
            {
                lowerLeft.Update();
                lowerRight.Update();
                upperLeft.Update();
                upperRight.Update();
            }

            if (NeedsSplit() && !isSplitting && !hasChildren)
                Split();

            if (!NeedsSplit() && !isSplitting)
                Merge();

            if (isSplitting && !_cancellationTokenSource.IsCancellationRequested && !NeedsSplit())
                _cancellationTokenSource.Cancel();
        }


        private void Split()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var res = CreateBackgroundSplitTasks();
            CreateSplitCompletionTask(res);
        }

        private List<Task<QuadNode>> CreateBackgroundSplitTasks()
        {
            var list = new List<Task<QuadNode>>();

            isSplitting = true;

            var upperRightTask = Task.Factory.StartNew(() =>
            {
                var ur = new QuadNode(this, NodeType.upperRight, Color.Aqua);

                return ur;

            }, _cancellationTokenSource.Token, TaskCreationOptions.None, TaskShedular);

            var lowerLeftTask = Task.Factory.StartNew(() =>
            {
                var ll = new QuadNode(this, NodeType.lowerLeft, Color.Green);

                return ll;

            }, _cancellationTokenSource.Token, TaskCreationOptions.None, TaskShedular);

            var lowerRightTask = Task.Factory.StartNew(() =>
            {
                var lr = new QuadNode(this, NodeType.lowerRight, Color.Red);

                return lr;

            }, _cancellationTokenSource.Token, TaskCreationOptions.None, TaskShedular);

            var upperLeftTask = Task.Factory.StartNew(() =>
            {
                var ul = new QuadNode(this, NodeType.upperLeft, Color.Gray);

                return ul;

            }, _cancellationTokenSource.Token, TaskCreationOptions.None, TaskShedular);

            list.Add(upperLeftTask);
            list.Add(upperRightTask);
            list.Add(lowerLeftTask);
            list.Add(lowerRightTask);

            return list;
        }

        private void CreateSplitCompletionTask(List<Task<QuadNode>> taskList)
        {
            _splitCompletionTask = Task.Factory.ContinueWhenAll(taskList.ToArray(), finishedTasks =>
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    upperLeft = finishedTasks[0].Result;
                    lowerLeft = finishedTasks[1].Result;
                    lowerRight = finishedTasks[2].Result;
                    upperRight = finishedTasks[3].Result;

                    hasChildren = true;
                }
                else
                {
                    foreach (var task in finishedTasks.Where(task => task.Status == TaskStatus.RanToCompletion))
                    {
                        ((IDisposable)task.Result).Dispose();
                    }
                }

                isSplitting = false;
            });
        }

        private void Merge()
        {
            if (hasChildren)
            {
                lowerLeft.Dispose();
                lowerRight.Dispose();
                upperLeft.Dispose();
                upperRight.Dispose();

                lowerLeft = null;
                lowerRight = null;
                upperLeft = null;
                upperRight = null;

                hasChildren = false;
            }
        }

        public void Draw()
        {
            if (hasChildren && !isSplitting)
            {
                upperLeft.Draw();
                lowerLeft.Draw();
                upperRight.Draw();
                lowerRight.Draw();
            }
            else
            {
                mesh.Draw(0);
            }
        }

        public void Dispose()
        {
            mesh.Dispose();
        }

    }
}
