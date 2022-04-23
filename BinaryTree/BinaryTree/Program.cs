// See https://aka.ms/new-console-template for more information

//inserting elements that constantly go to one side
var bt = new BinaryTree();
bt.InsertValue(new Node(5));
bt.InsertValue(new Node(3));
bt.InsertValue(new Node(1));
bt.InsertValue(new Node(0));
bt.InsertValue(new Node(-1));
bt.InsertValue(new Node(-2));


//var bt = new BinaryTree();
//bt.InsertValue(new Node(5));
//bt.InsertValue(new Node(3));
//bt.InsertValue(new Node(7));
//bt.InsertValue(new Node(1));
//bt.InsertValue(new Node(4));
//bt.InsertValue(new Node(0));

Console.WriteLine($"\n\n***********************************\n\n\n Summary:");
bt.PrintTree();

//This is by no means an efficient method, but it serves only as a learning tool
// many segments of this code can be vastly improved, but given it has served its
// original purpose, I will leave it as is


public class BinaryTree
{
    #region Properties
    public Node root;
    //private int rootCount;
    //-------
    public Node nodeLeft;
    public BinaryTree treeLeft;
    //-------
    public Node nodeRight;
    public BinaryTree treeRight;
    //-------
    public BinaryTree parent;
    //-------
    public Guid uniqueIdentifier = Guid.NewGuid();
    #endregion

    private void CleanHouse() 
    {
        this.parent = null;
        this.root = null;
        this.nodeLeft = null;
        this.nodeRight = null;
        this.treeRight = null;
        this.treeLeft = null;
    }
    public void InsertValue(Node insert)
    {
        if (root == null) { root = insert;}
        
        //if (insert.value == root.value) { rootCount++; }
        //else 
        if (insert.value < root.value) { InsertLeft(insert); }
        else if (insert.value > root.value) { InsertRight(insert); }
    }

    private void BalanceThisAndParent(Node insert) 
    {
        var dic = new Dictionary<int, Node>();

        if (insert != null) { dic.Add(insert.value, insert); }

        dic.Add(this.root.value,      this.root);
        if (this.nodeLeft != null)  { dic.Add(this.nodeLeft.value, this.nodeLeft); };
        if (this.nodeRight != null) { dic.Add(this.nodeRight.value, this.nodeRight); };

        dic.Add(this.parent.root.value,      this.parent.root);
        if (this.parent.nodeLeft != null)  { dic.Add(this.parent.nodeLeft.value, this.parent.nodeLeft); };
        if (this.parent.nodeRight != null) { dic.Add(this.parent.nodeRight.value, this.parent.nodeRight); };

        if (dic.Count < 5) { Console.WriteLine("Balancing this tree and its parents with less than 5 nodes in total? This must not be ok."); }

        dic = dic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        Node[] arr = dic.Values.ToArray();


        this.parent.CleanHouse();


        this.parent.treeLeft = new BinaryTree();
        this.parent.treeRight = new BinaryTree();
        this.parent.root = arr[2];
        this.parent.nodeLeft = null;
        this.parent.nodeRight = null;

        this.parent.treeLeft.root = arr[0];
        this.parent.treeLeft.nodeRight = arr[1];



        this.parent.treeRight.root = arr[3];
        this.parent.treeRight.nodeRight = arr[4];

    }

    private void CheckTreeImbalance()
    {
        if (this.parent == null) { return; }

        //what we want to avoid?
        // I am a subtree my both nodes are full or I have a subtree, and my parent has only one node on the opposite side

        var bothNodesUsed = this.nodeLeft != null && this.nodeRight != null ? true : false;
        var anySubTree = this.treeLeft != null || this.treeRight != null;

        var parentNotBalanced = parent.nodeLeft != null || parent.nodeRight != null;

        if ( (bothNodesUsed == true || anySubTree == true) && parentNotBalanced) 
        {
            BalanceThisAndParent(null);
        }

    }


    private void InsertLeft(Node insert) 
    {
        if (nodeLeft == null && treeLeft == null) { nodeLeft = insert; CheckTreeImbalance(); return; }//no obstruction to set a node

        //----------------
        //at this point we know we need to create a subtree

        //check parent's opposite side
        bool parentOppositeSideBalanced = (this.parent != null && (this.parent.treeRight == null || this.parent.nodeRight == null)) ? false : true ;

        bool thisOppositeSideBalanced = (nodeRight == null && treeRight == null) ? false : true;

        if (parentOppositeSideBalanced == false) 
        { BalanceThisAndParent(insert); } // rebalance everything


        //node is not empty, we need to create a new tree, now we need to know if we need to balance the tree or create a new one
        else if (thisOppositeSideBalanced == true) // in this case the opposite side is balanced and we are ok to create a subtree
        {
            //check if there's a tree/branch on this sie
            if (treeLeft != null) { treeLeft.InsertValue(insert); return; }
            //----
            var newBtree = new BinaryTree();
            newBtree.parent = this;
            treeLeft = newBtree;
            treeLeft.InsertValue(nodeLeft);
            treeLeft.InsertValue(insert);
            nodeLeft = null;
            //----
        }
        else //rotate tree
        {
            RotateTreeClockWise(insert);
        }

        CheckTreeImbalance();

    }



    private void InsertRight(Node insert)
    {
        if (nodeRight == null && treeRight == null) { nodeRight = insert; CheckTreeImbalance(); return; }//no obstruction to set a node

        //----------------
        //at this point we know we need to create a subtree

        //check parent's opposite side
        bool parentOppositeSideBalanced = (this.parent != null && (this.parent.treeLeft == null || this.parent.nodeLeft == null)) ? false : true;

        bool thisOppositeSideBalanced = (nodeLeft == null && treeLeft == null) ? false : true;

        if (parentOppositeSideBalanced == false)
        { BalanceThisAndParent(insert); } // rebalance everything


        //node is not empty, we need to create a new tree, now we need to know if we need to balance the tree or create a new one
        else if (thisOppositeSideBalanced == true) // in this case the opposite side is balanced and we are ok to create a subtree
        {
            //check if there's a tree/branch on this sie
            if (treeRight != null) { treeRight.InsertValue(insert); return; }
            //----
            var newBtree = new BinaryTree();
            newBtree.parent = this;
            treeRight = newBtree;
            treeRight.InsertValue(nodeRight);
            treeRight.InsertValue(insert);
            nodeRight = null;
            //----
        }
        else //rotate tree
        {
            RotateTreeCounterClockWise(insert);
        }

        CheckTreeImbalance();
    }

    #region Rotate
    private void RotateTreeCounterClockWise(Node newNode) 
    {
        Console.WriteLine("RotateTreeCounterClockWise...");
        //when we do this we should have checked that there's no trees set in here
        if (treeLeft != null) { Console.WriteLine("Warning - treeLeft was not empty"); }
        if (treeRight != null) { Console.WriteLine("Warning - treeRight was not empty"); }

        var list = new List<Node>() { newNode, nodeRight, root };
        Node[] arr = list.OrderBy(x => x?.value).ToArray();

        //var arr = new Node[]{root, nodeRight, newNode };
        root = null; nodeRight = null; nodeLeft = null; treeRight = null; treeLeft = null;

        nodeLeft = arr[0];
        root = arr[1];
        nodeRight = arr[2];

        PrintTree();
    }


    private void RotateTreeClockWise(Node newNode)
    {
        Console.WriteLine("RotateTreeClockWise...");
        //when we do this we should have checked that there's no trees set in here
        if (treeLeft != null) { Console.WriteLine("Warning - treeLeft was not empty"); }
        if (treeRight != null) { Console.WriteLine("Warning - treeRight was not empty"); }

        var list = new List<Node>() { newNode, nodeLeft, root };
        Node[] arr = list.OrderBy(x => x?.value).ToArray();

        //var arr = new Node[] { newNode, nodeLeft, root  };
        root = null; nodeRight = null; nodeLeft = null; treeRight = null; treeLeft = null;

        nodeLeft = arr[0];
        root = arr[1];
        nodeRight = arr[2];

        PrintTree();
    }

    #endregion



    public void PrintTree()
    {
        Console.WriteLine("----------------------------------\n" +
        $"Root: {this.root?.value} - GUID : { this.uniqueIdentifier };\n" +
        $"    LeftNode: { this.nodeLeft?.value } , RightNode: { this.nodeRight?.value };");

        if (this.treeLeft != null) 
        {
            Console.WriteLine($"    Left Tree below. ID: {treeLeft.uniqueIdentifier}");
            treeLeft.PrintTree();
        }

        if (this.treeRight != null)
        {
            Console.WriteLine($"    Right Tree below. ID: {treeRight.uniqueIdentifier}");
            treeRight.PrintTree();
        }

    }
}



public class Node
{
    public Guid uniqueIdentifier = Guid.NewGuid();
    public int value;
    public Node(int value) { this.value = value; }

}