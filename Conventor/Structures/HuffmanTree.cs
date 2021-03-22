using System;
using System.Collections.Generic;
using System.Text;

namespace Conventor.Structures
{
    class HuffmanTree
    {
        private HuffmanNode head=new HuffmanNode();
        private HuffmanNode currentNode=new HuffmanNode();

        public HuffmanTree()
        {
            head.value = "None";
            head.level = 0;
            currentNode = head;
        }

        private class HuffmanNode
        {
            public string value;
            public int level;
            public HuffmanNode left;
            public HuffmanNode right;
            public bool HasChilds = false;            
        }

        public void PushFromHex(string value,int level)
        {            
            if(currentNode.level+1==level)
            {
                if(currentNode.left==null)
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = value;
                    temp.level = currentNode.level+1;
                    currentNode.left = temp;
                }
                else
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = value;
                    temp.level = currentNode.level+1;
                    currentNode.HasChilds = true;
                    currentNode.right = temp;
                }
            }
            else
            {
                if(currentNode.left==null && !currentNode.HasChilds)
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = "None";
                    temp.level = currentNode.level+1;
                    currentNode.left = temp;
                    currentNode = temp;
                }else if (currentNode.left != null && currentNode.left.value=="None" && !currentNode.left.HasChilds)
                {
                    HuffmanNode temp = currentNode.left;
                    currentNode = temp;
                }else if (currentNode.right != null && currentNode.right.value=="None")
                {
                    HuffmanNode temp = currentNode.right;
                    currentNode = temp;
                }else if (currentNode.right==null)
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = "None";
                    temp.level = currentNode.level+1;
                    currentNode.right = temp;
                    currentNode = temp;
                }
                PushFromHex(value,level);
            }
        }

        public void ResetCurrentNode()
        {                      
            currentNode = head;
        }
    }
}
