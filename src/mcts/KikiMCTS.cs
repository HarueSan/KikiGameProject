using System;
using System.Collections.Generic;
using KikiProject.boards;
using KikiProject.ui;
using KikiProject.units;
using System.Linq;
using Godot;

namespace KikiProject
{
    public class KikiMCTS
    {
        private DynamicBoard _board;
        private List<KikiNode> _nodes;
        private KikiUnitList unitsList;
        private KikiBoard _kikiBoard;
        private KikiNode root;
        private UnitPanel _panel;
        Random random = new Random();

        KikiNode CreateNextNode(KikiNode parent, string action, List<KikiNode> nodes)
        {
            return new KikiNode(parent, action, nodes);
        }

        KikiNode CreateNextRandomChildNode(KikiNode parent, List<KikiNode> nodes)
        {
            if (parent.ActionList.Count == 0) return null;
            int next = random.Next(parent.ActionList.Count);
            string action = parent.ActionList[next];
            parent.ActionList.RemoveAt(next);
            if (parent.ActionList.Count == 0) parent.FiniteNode = true;

            return CreateNextNode(parent, action, nodes);
        }

        void PrintParentChild(KikiNode parent, KikiNode child)
        {
            Console.WriteLine(parent.Code + " -> " + child.ActionPerform + " -> " + child.Code);
        }

        string ActionCode(KikiNode parent, KikiNode child)
        {
            return parent.Code + " -> " + child.ActionPerform + " -> " + child.Code;
        }

        void PrintStackTrace(KikiNode child)
        {
            KikiNode parent = child.Parent;

            string output = "";
            while (parent != null)
            {
                output = ActionCode(parent, child) + "\n" + output;
                child = parent;
                parent = child.Parent;
            }

            Console.WriteLine(output);
        }


        bool MCTSIteration(List<KikiNode> nodes, int iterationCount)
        {
            KikiNode parent = null;
            KikiNode child = null;

            for (int i = 0; i < iterationCount; i++)
            {
                child = null;
                // Console.WriteLine("Node counts : " + nodes.Count);
                if (nodes.All(n => n.FiniteNode))
                {
                    return false;
                }

                // Console.WriteLine("All finite : " + nodes.All(n => n.FiniteNode));
                int r = random.Next(nodes.Count);
                parent = nodes[r];
                while (parent.FiniteNode == true)
                {
                    r = random.Next(nodes.Count);
                    parent = nodes[r];
                }

                if (parent.ActionList.Count > 0)
                {
                    parent.ActionList.ForEach(a => Console.WriteLine(a));
                    child = CreateNextRandomChildNode(parent, nodes);
                }
                else
                {
                    parent.FiniteNode = true;
                }

                if (child != null)
                {
                    Console.Write("iteration " + i + " : ");
                    PrintParentChild(parent, child);
                    if (child.FiniteNode == true && child.WinStatus == true)
                    {
                        // Console.WriteLine("FOUND : " + child.Code);

                        PrintStackTrace(child);
                        return true;
                    }
                }
                else
                {
                    // Console.WriteLine("iteration " + i + " : " + parent.Code + " -> null");
                }
            }

            return false;
        }


        public KikiMCTS(DynamicBoard board, UnitPanel panel)
        {
            _board = board;
            _panel = panel;

            _nodes = new List<KikiNode>();

            CreateUnitList();
            CreateBoard();

            root = new KikiNode(unitsList, _kikiBoard);

            _nodes.Add(root);
        }

        public bool Run()
        {
            return MCTSIteration(_nodes, 9999);
        }

        private void CreateBoard()
        {
            _kikiBoard = new KikiBoard(
                _board.Rows, _board.Columns, (int) _board.PlayerStartPosition.y, (int) _board.PlayerStartPosition.x,
                (int) _board.GoalPosition.y, (int) _board.GoalPosition.x
            );
        }

        public void CreateUnitList()
        {
            String output = "";
            var units = _panel.UnitContainers.GetChildren();

            foreach (Control control in units)
            {
                if (control.GetChild<Unit>(0) is HorizontalUnit)
                {
                    output += ((int) KikiUnit.Unit.Horizontal).ToString();
                }
                else if (control.GetChild<Unit>(0) is VerticleUnit)
                {
                    output += ((int) KikiUnit.Unit.Vertical).ToString();
                }
                else if (control.GetChild<Unit>(0) is SingleUnit)
                {
                    output += ((int) KikiUnit.Unit.Single).ToString();
                }
            }

            GD.Print(output);

            this.unitsList = new KikiUnitList(output);
        }
    }
}