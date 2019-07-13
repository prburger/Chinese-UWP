using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml;
using Xamarin.Forms;


namespace v1
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public struct Word
        {
            // p = Pinyin
            // c = Character
            // m = Meaning
            // ps = Part of speech
            // f = Formality
            public Word(string p, string c, string m, string ps, string f)
            {
                Pinyin = p;
                Character = c;
                Meaning = m;
                Formality = f;
                PartOfSpeech = ps;
            }

            public string Pinyin { get; set; }
            public string Character { get; set; }
            public string Meaning { get; set; }
            public string PartOfSpeech { get; set; }
            public string Formality { get; set; }
        }

        private readonly Random rnd = new Random();
        private readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private List<Word> wordList;
        private List<Word> nextWordList;
        private int word_idx;
        private int nextword_idx;
        private Word word_curr;
        private string previous_fs;
        private const string fn = "/storage/sdcard1/pbflashcards/wordlist.xml";

        public MainPage()
        {
            InitializeComponent();            
            InitGlobalVariables();
            BuildXmlList();
            word_idx = 0;
            word_curr = (Word)wordList[word_idx];
            nextWordList = wordList.ToList();
            ShowWord();
        }

        private void InitGlobalVariables()
        {
            this.PreviousWord.Text = "<<";
            this.NextWord.Text = ">>";
            previous_fs = "";
            word_idx = 0;
            nextword_idx = 0;
            wordList = new List<Word>();
            nextWordList = new List<Word>();
        }

        //
        // ShowWord: fill the Pinyin text box 
        //
        private void ShowWord()
        {
            if (nextWordList.Count != 0)
            {
                if (nextword_idx > nextWordList.Count) { nextword_idx = 0; }
                word_curr = nextWordList.ElementAt(nextword_idx);
                word_curr.PartOfSpeech = previous_fs;
                this.Character.Text = textInfo.ToTitleCase(word_curr.Character.ToString());
                this.Meaning.Text = textInfo.ToTitleCase(word_curr.Meaning.ToString());
                this.Pinyin.Text = textInfo.ToTitleCase(word_curr.Pinyin.ToString());
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {
            foreach (var w in wordList)
            {
                for (int gi = 0; gi < w.Meaning.Length; gi++)
                    if (String.Equals(w.Meaning, this.Search.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        word_curr = w;
                        ShowWord();
                        return;
                    }
            }
        }

        private void BuildXmlList()
        {            
            try
            {
                XmlTextReader xmlreader = new XmlTextReader(fn);
                DataSet ds = new DataSet();

                //read the xml data
                ds.ReadXml(xmlreader);

                for (int r = 0; r < ds.Tables[0].Rows.Count; r++)
                {
                    DataRow dr = ds.Tables[0].Rows[r];
                    Word w = new Word(
                    dr.ItemArray[0].ToString(),
                    dr.ItemArray[1].ToString(),
                    dr.ItemArray[2].ToString(),
                    dr.ItemArray[3].ToString(),
                    dr.ItemArray[4].ToString());
                    wordList.Add(w);
                }
                xmlreader.Close();
            }
            //}
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }
        }
        private void PreviousWord_Clicked(object sender, EventArgs e)
        {
            if (nextword_idx > 0)
            {
                nextword_idx--;
            }
            else
            {
                nextword_idx = nextWordList.Count - 1;
            }            
            ShowWord();
        }

        private void NextWord_Clicked(object sender, EventArgs e)
        {
            if (nextword_idx < nextWordList.Count - 1)
            {
                nextword_idx++;
            }
            else
            {
                nextword_idx = 0;
            }
            ShowWord();
        }

        private void PinyinSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PinyinSwitch.IsToggled)
            {
                this.Pinyin.TextColor = Color.White;
            }
            else
            {
                this.Pinyin.TextColor = Color.Black;
            }
        }

        private void MeaningSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.MeaningSwitch.IsToggled)
            {
                this.Meaning.TextColor = Color.White;
            }
            else
            {
                this.Meaning.TextColor = Color.Black;
            }
        }

        private void Search_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                foreach (var w in wordList)
                {
                    if (String.Equals(w.Meaning, this.Search.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        word_curr = w;
                        ShowWord();
                        return;
                    }
                }
            }
            catch (Exception) { return; }
        }

        private void WordTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string wordGroup = this.WordTypePicker.SelectedItem.ToString();
            nextWordList = wordList.Where(w => w.PartOfSpeech == wordGroup).Select(w => w).ToList();
            if (nextWordList.Count != 0)
            {
                nextword_idx = 1;
                ShowWord();
            }
        }

        private void CharacterSwitch_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.CharacterSwitch.IsToggled)
            {
                this.Character.TextColor = Color.White;
            }
            else
            {
                this.Character.TextColor = Color.Black;
            }
        }
        private void RandomWord_Clicked(object sender, EventArgs e)
        {
            nextword_idx = rnd.Next(1, wordList.Count);
            ShowWord();
        }

        private void ClearFilters_Clicked(object sender, EventArgs e)
        {
            nextWordList = wordList.ToList();
            ShowWord();
        }
    }
}
