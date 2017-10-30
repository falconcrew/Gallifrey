using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gallifrey
{
    public partial class Form1 : Form
    {
        private string[][] consonants = new string[4][];
        private List<char> specHCons;
        private List<char> specCons;
        private List<char> vowels;

        private List<List<string>> sentence;

        // Graphics
        private Pen pen = new Pen(Brushes.DarkGreen, 5);

        public Form1()
        {
            InitializeComponent();
            consonants[0] = new string[] { "b", "", "ch", "d", "", "g", "h", "f" };
            consonants[1] = new string[] { "j", "ph", "k", "l", "c", "n", "p", "m" };
            consonants[2] = new string[] { "t", "wh", "sh", "r", "", "v", "w", "s" };
            consonants[3] = new string[] { "th", "gh", "y", "z", "q", "qu", "x", "ng" };
            specHCons = new List<char> { 'c', 'p', 'w', 's', 'g', 't' };
            specCons = new List<char> { 'u', 'g' };
            vowels = new List<char> { 'a', 'e', 'i', 'o', 'u' };
            sentence = new List<List<string>>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sentence.Clear();
            string input = textBox1.Text.ToLower();
            string[] words = input.Split(' ');
            string syllable = "";
            int signs = 0;
            string output = "";
            foreach (string word in words)
            {
                List<string> ord = new List<string>();
                for (int i = 0; i < word.Length; i++)
                {
                    char c = word[i];
                    if (specCons.Contains(c) && signs == 1)
                    {
                        if (syllable[0]=='n' && c == 'g')
                        {
                            syllable += c;
                            signs++;
                        }
                        else if(syllable[0]=='q' && c == 'u')
                        {
                            syllable += c;
                            signs++;
                        }
                    }
                    else if (vowels.Contains(c))
                    {
                        output += syllable + c + "\n";
                        ord.Add(syllable + c);
                        syllable = "";
                        signs = 0;
                    }
                    else if (signs == 1)
                    {
                        if (c == 'h' && specHCons.Contains(syllable[0]))
                        {
                            syllable += c;
                            signs++;
                        }
                        else
                        {
                            output += syllable + "\n";
                            ord.Add(syllable);
                            syllable = "" + c;
                            signs = 1;
                        }
                    }
                    else
                    {
                        syllable += c;
                        signs++;
                    }
                    if (signs == 2)
                    {
                        ord.Add(syllable);
                        syllable = "";
                        signs = 0;
                    }
                }
                if (syllable.Length > 0)
                {
                    output += syllable + "\n";
                    ord.Add(syllable);
                    syllable = "";
                    signs = 0;
                }
                sentence.Add(ord);
            }
            foreach (List<string> word in sentence)
            {
                foreach (string syll in word)
                {
                    Console.WriteLine(syll);
                }
                Console.WriteLine();
            }
            panel1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (sentence.Count > 0)
            {
                Panel p = (Panel)sender;
                Graphics gfx = e.Graphics;
                float centerX = p.Width / 2;
                float centerY = p.Height / 2;
                float diameter = Math.Min(p.Height, p.Width) - 20;
                float topX = centerX - diameter / 2;
                float topY = centerY - diameter / 2;
                gfx.DrawArc(pen, topX, topY, diameter, diameter, 0, 360);
                float radius = (float)((diameter / 2) / (1 + Math.Sin(Math.PI / sentence.Count)));
                float diameter2 = (float)(2 * radius * Math.Sin(Math.PI / sentence.Count) - 20);
                float wordCenterX = centerX + (float)(radius * Math.Cos(270.0 / 360 * 2 * Math.PI));
                float wordCenterY = centerY - (float)(radius * Math.Sin(270.0 / 360 * 2 * Math.PI));
                topX = wordCenterX - diameter2 / 2;
                topY = wordCenterY - diameter2 / 2;

                for (int i = 0; i < sentence.Count; i++)
                {
                    float angle = 360.0F * i / sentence.Count + 270;
                    wordCenterX = centerX + (float)(radius * Math.Cos(2 * Math.PI * i / sentence.Count + 3.0 / 2 * Math.PI));
                    wordCenterY = centerY - (float)(radius * Math.Sin(2 * Math.PI * i / sentence.Count + 3.0 / 2 * Math.PI));
                    topX = wordCenterX - diameter2 / 2;
                    topY = wordCenterY - diameter2 / 2;
                    DrawWord(sentence[i], gfx, topX, topY, diameter2);
                }
            }
        }

        private int[] GetIndices(string character)
        {
            if (character.Length == 1 && vowels.Contains(character[0]))
            {
                return new int[] { vowels.IndexOf(character[0]) };
            }
            for(int i = 0; i < consonants.Length; i++)
            {
                for(int j = 0; j < consonants[0].Length; j++)
                {
                    if (consonants[i][j].Equals(character))
                    {
                        return new int[] { i, j };
                    }
                }
            }
            return new int[] { -1, -1 };
        }

        private void DrawWord(List<string> word, Graphics gfx, float topX, float topY, float diameter)
        {
            float angleStep = 360.0F / word.Count;
            float centerX = topX + diameter / 2;
            float centerY = topY + diameter / 2;
            float radius = diameter / 2;
            for (int j = 0; j < word.Count; j++)
            {
                string syllable = word[j];
                List<string> characters = new List<string>();
                for (int i = 0; i < syllable.Length; i++)
                {
                    char c = syllable[i];
                    try
                    {
                        if (specHCons.Contains(c) && syllable[i + 1] == 'h')
                        {
                            characters.Add(c + "h");
                            i++;
                        }
                        else if (c == 'q' && syllable[i + 1] == 'u')
                        {
                            characters.Add("qu");
                            i++;
                        }
                        else if (c == 'n' && syllable[i + 1] == 'g')
                        {
                            characters.Add("ng");
                            i++;
                        }
                        else characters.Add("" + c);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        characters.Add("" + c);
                    }
                }
                int[] indices = GetIndices(characters[0]);
                float drawAngle = -360.0F * j / word.Count + 90;
                float wordAngle = 360.0F * j / word.Count + 270;
                if (indices.Length > 1)
                {
                    float radius2 = Math.Min(diameter / 6, (float)(diameter / 2 * Math.Sin(Math.PI / word.Count)) * 0.9F);
                    float diameter2 = radius2 * 2;
                    float wordRadius;
                    float deltaAngle;
                    float syllCenterX;
                    float syllCenterY;
                    float newTopX;
                    float newTopY;
                    switch (indices[0])
                    {
                        case 0:
                            radius2 *= 0.9F;
                            wordRadius = radius - (float)(radius2*Math.Cos(20*Math.PI/180));//5.0F / 7 * radius;// diameter2 * 15.0F / 14;
                            deltaAngle = (float)(180.0 / Math.PI * Math.Asin(1 - Math.Cos(Math.PI / 180 * 35)));
                            syllCenterX = (float)(centerX + wordRadius * Math.Cos(wordAngle * Math.PI / 180));
                            syllCenterY = (float)(centerY - wordRadius * Math.Sin(wordAngle * Math.PI / 180));
                            newTopX = syllCenterX - diameter2 / 2;
                            newTopY = syllCenterY - diameter2 / 2;
                            gfx.DrawArc(pen, newTopX, newTopY, diameter2, diameter2, drawAngle + 35, 290);
                            gfx.DrawArc(pen, topX, topY, diameter, diameter, drawAngle - angleStep / 2, angleStep / 2 - deltaAngle);
                            gfx.DrawArc(pen, topX, topY, diameter, diameter, drawAngle + deltaAngle, angleStep / 2 - deltaAngle);
                            break;
                        case 1:
                            radius2 *= 0.9F;
                            wordRadius = 4.0F / 7 * radius;// diameter2 * 12.0F / 14;
                            syllCenterX = (float)(centerX + wordRadius * Math.Cos(wordAngle * Math.PI / 180));
                            syllCenterY = (float)(centerY - wordRadius * Math.Sin(wordAngle * Math.PI / 180));
                            newTopX = syllCenterX - diameter2 / 2;
                            newTopY = syllCenterY - diameter2 / 2;
                            gfx.DrawArc(pen, newTopX, newTopY, diameter2, diameter2, 0, 360);
                            gfx.DrawArc(pen, topX, topY, diameter, diameter, drawAngle - angleStep / 2, angleStep);
                            break;
                        case 2:
                            wordRadius = diameter / 2;
                            syllCenterX = (float)(centerX + wordRadius * Math.Cos(wordAngle * Math.PI / 180));
                            syllCenterY = (float)(centerY - wordRadius * Math.Sin(wordAngle * Math.PI / 180));
                            newTopX = syllCenterX - diameter2 / 2;
                            newTopY = syllCenterY - diameter2 / 2;
                            deltaAngle = (float)(2.0F * 180.0 / Math.PI * Math.Asin(radius2 / 2.0 / radius));
                            double b = Math.Asin(radius2 / 2.0 / radius);
                            float beta = 180 - 2 * deltaAngle+25;
                            float a = drawAngle + 180 - beta / 2;
                            gfx.DrawArc(pen, newTopX, newTopY, diameter2, diameter2, drawAngle + 180 - beta / 2, beta);
                            gfx.DrawArc(pen, topX, topY, diameter, diameter, drawAngle - angleStep / 2, angleStep / 2 - deltaAngle);
                            gfx.DrawArc(pen, topX, topY, diameter, diameter, drawAngle + deltaAngle, angleStep / 2 - deltaAngle);
                            break;
                        case 3:
                            wordRadius = diameter / 2;
                            syllCenterX = (float)(centerX + wordRadius * Math.Cos(wordAngle * Math.PI / 180));
                            syllCenterY = (float)(centerY - wordRadius * Math.Sin(wordAngle * Math.PI / 180));
                            newTopX = syllCenterX - diameter2 / 2;
                            newTopY = syllCenterY - diameter2 / 2;
                            gfx.DrawArc(pen, newTopX, newTopY, diameter2, diameter2, 0, 360);
                            gfx.DrawArc(pen, topX, topY, diameter, diameter, drawAngle - angleStep / 2, angleStep);
                            break;
                    }
                    switch (indices[1])
                    {
                        case 1:

                            break;
                    }
                }
            }
        }
    }
}
