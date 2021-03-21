using System;
using System.Collections.Generic;
using System.Text;

namespace Conventor.Structures
{
    class HuffmanTree
    {
        private string value;
        private HuffmanNode head=new HuffmanNode();
        private HuffmanNode currentNode=new HuffmanNode();

        HuffmanTree()
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
        }

        public void PushFromHex(string value,int level)
        {
            currentNode.level++;
            if(currentNode.level==level)
            {
                if(currentNode.left==null)
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = value;
                    currentNode.left = temp;
                }
                else
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = value;
                    currentNode.right = temp;
                }
            }
            else
            {
                if(currentNode.left==null)
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = "None";
                    currentNode.left = temp;
                    currentNode = temp;
                }
                else
                {
                    HuffmanNode temp = new HuffmanNode();
                    temp.value = "None";
                    currentNode.right = temp;
                    currentNode = temp;
                }
                PushFromHex(value,level);
            }
        }
    }
}
