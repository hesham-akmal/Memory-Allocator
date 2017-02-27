using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MemoryAllocation
{
    class Address
    {
        public int start;
        public int end;
    }
    class MemoryObject
    {
        public int i;
        public int size;
        public Address address = new Address();
    }
    class Process : MemoryObject
    {
        public static int count = 1;
        public bool deallocated = false;
        public Process(int size)
        {
            this.size = size;
            this.i = count++;
        }
    }
    class Block : MemoryObject
    {
        public static int count = 1;
        public Block(int size, int startAddress)
        {
            this.size = size;
            address.start = startAddress;
            address.end = this.size + address.start;
            i = count++;
        }
        internal void updatesize()
        {
            size = address.end - address.start ;
        }
    }
    class SystemProg
    {
        public static SystemProg instance = new SystemProg();

        public List<Process> allProcesses = new List<Process>();

        public List<Block> allBlocks = new List<Block>();

        public int memorySizeOriginal;

        public bool randomSizes = true;
        public bool mergeHolesAuto = false;

        Random r = new Random();

        //drawing
        Pen blackPen = new Pen(Color.Black, 1);
        Brush redBrush = new SolidBrush(Color.Red);
        Brush whiteBrush = new SolidBrush(Color.White);
        Font myfont = new Font("Arial", 8);

        public void createBlocks(int numberOfBlocks)
        {
            resetBlocks();

            memorySizeOriginal = 1000;

            int memorySizeLeft = memorySizeOriginal;

            int randSize;

            int lastAddressUsed = 0;

            for (int i = 0; i < numberOfBlocks; i++)
            {
                int x = (memorySizeOriginal / numberOfBlocks);

                if (randomSizes)
                    randSize = x + r.Next(40);
                else
                    randSize = x;

                if (randSize > memorySizeLeft)
                {
                    allBlocks.Add(new Block(memorySizeLeft, lastAddressUsed));
                    break;
                }

                if (i == 0)
                    allBlocks.Add(new Block(randSize, 0));

                else if (i == numberOfBlocks - 1)
                    allBlocks.Add(new Block(memorySizeLeft, lastAddressUsed));

                else
                    allBlocks.Add(new Block(randSize, lastAddressUsed));

                lastAddressUsed = allBlocks[i].address.end;
                memorySizeLeft -= randSize;
            }
            update();
        }
        public void resetBlocks()
        {
            Process.count = 1;
            Block.count = 1;
            Program.mainForm.richTextBox1.Text = "";
            allBlocks.Clear();
            allProcesses.Clear();
        }
        public void addPfirstFit(int size)
        {
            bool noPlaceAvailable = true;
            Process p = new Process(size);
			
			 allBlocks = allBlocks.OrderBy(o => o.address).ToList();/////////////////
			
            foreach (Block b in allBlocks)
            {
                if (b.size >= p.size)
                {
                    p.address.start = b.address.start;
                    p.address.end = p.size + p.address.start;
                    b.address.start = p.address.end;
                    b.size = b.address.end - b.address.start;
                    if (b.size == 0 || b.size == -1)
                        allBlocks.Remove(b);
                    noPlaceAvailable = false;
                    break;
                }
            }
            if (noPlaceAvailable)
            {
                MessageBox.Show("No block available.");
                Process.count--;
                p = null;
            }
            else
            {
                allProcesses.Add(p);
                update();
            }
        }
        public void addProcessFit(int size, bool bestFit)
        {
            Process p = new Process(size);

            List<Block> bigEnoughBlocks = new List<Block>();

            foreach (Block ba in allBlocks)
            {
                if (ba.size >= p.size)
                    bigEnoughBlocks.Add(ba);
            }
            if (bigEnoughBlocks.Count == 0)
            {
                MessageBox.Show("No block available.");
                Process.count--;
                p = null;
                return;
            }

            List<Block> bigEnoughBlocksSortedList;
            bigEnoughBlocksSortedList = bigEnoughBlocks.OrderBy(o => o.size).ToList();

            Block b;
            if (bestFit)
                b = bigEnoughBlocksSortedList[0];
            else
                b = bigEnoughBlocksSortedList[bigEnoughBlocksSortedList.Count - 1];

            p.address.start = b.address.start;
            p.address.end = p.size + p.address.start;
            b.address.start = p.address.end;
            b.updatesize();
            if (b.size == 0 || b.size == -1)
                allBlocks.Remove(b);

            allProcesses.Add(p);
            update();
        }

        public void deallocateProcess(Process p)
        {
            foreach (Process pro in allProcesses)
            {
                if (p == pro)
                {
                    Block b = new Block(p.size, p.address.start);
                    b.address.end = p.address.end;
                    allBlocks.Add(b);
                    p.deallocated = true;
                    update();
                    return;
                }
            }
        }

        public void mergeHoles()
        {
            for (int k = 0; k < 10; k++)
            {
                bool superBreak = false;
                foreach (Block b1 in allBlocks)
                {
                    foreach (Block b2 in allBlocks)
                    {
                        if (b1 == b2)
                            continue;

                        if (b1.address.end == b2.address.start)
                        {
                            b1.address.end = b2.address.end;
                            b1.updatesize();
                            allBlocks.Remove(b2);
                            superBreak = true;
                            break;
                        }
                    }
                    if (superBreak) break;
                }
            }
            printAll();
        }

        public void update()
        {
            Program.mainForm.comboBox3.Items.Clear();
            foreach (Process p in allProcesses)
                if (!p.deallocated)
                    Program.mainForm.comboBox3.Items.Add("Process " + p.i);

            List<Block> sortedblocks = allBlocks.OrderBy(o => o.address.start).ToList();
            int u = 1;
            foreach (Block b in sortedblocks)
                b.i = u++;

            if (mergeHolesAuto)
                mergeHoles();

            printAll();
        }

        public void printAll()
        {
            Program.mainForm.richTextBox1.Text = "";

            List<MemoryObject> sortedObjs1 = new List<MemoryObject>();
            sortedObjs1.AddRange(allBlocks);
            sortedObjs1.AddRange(allProcesses);
            List<MemoryObject> sortedObjs2 = sortedObjs1.OrderBy(o => o.address.start).ToList();
            Program.g.Clear(Color.White);

            int x = 3;


            foreach (MemoryObject mo in sortedObjs2)
            {
                if (mo.GetType() == typeof(Block))
                {
                    Program.g.FillRectangle(whiteBrush, new Rectangle(10, mo.address.start / x, 200, mo.size / x));
                    Program.g.DrawString("Block " + mo.i, myfont, Brushes.Blue, 10, mo.address.start / x);
                    Program.mainForm.richTextBox1.Text += "\n" + "Block " + mo.i + ", startAddress: " + mo.address.start + ", endAddress: " + mo.address.end + ", Size: " + mo.size;
                }
                else
                {
                    if (((Process)mo).deallocated)
                        continue;
                    Program.g.FillRectangle(redBrush, new Rectangle(10, (mo.address.start / x), 200, (mo.size / x)));
                    Program.g.DrawString("Process " + mo.i, myfont, Brushes.Blue, 80, mo.address.start / x);
                    Program.mainForm.richTextBox1.Text += "\n" + "Process " + mo.i + ", startAddress: " + mo.address.start + ", endAddress: " + mo.address.end + ", Size: " + mo.size;
                }
                Program.g.DrawString( "0" , myfont, Brushes.Blue, 210, 0);
                Program.g.DrawString("" + mo.address.start, myfont, Brushes.Blue, 210, (mo.address.start / x) - 10);
                Program.g.DrawString("1000", myfont, Brushes.Blue, 210, 328);
                Program.g.DrawRectangle(blackPen, new Rectangle(10, mo.address.start / x, 200, mo.size / x));
            }
        }
    }
}