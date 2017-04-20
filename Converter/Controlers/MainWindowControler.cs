using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Converter.Handlers;
using Converter.Models;
using Formatting = Newtonsoft.Json.Formatting;

namespace Converter.Controlers
{
    public class MainWindowControler : INotifyPropertyChanged
    {
        private bool _inputDataIsModified = false;
        private readonly Text _text = new Text();

        private ICommand _convertToJsonFormatCommand;

        private ICommand _convertToXmlFormatCommand;


        private string _inputData;

        private ICommand _openFileCommand;

        private string _outputData;

        private ICommand _saveAsFileCommand;


        public ICommand SaveAsFileCommand =>
            _saveAsFileCommand ?? (_saveAsFileCommand = new CommandHandler(SaveAsFile, true));


        public ICommand ConvertToXmlFormatCommand
            => _convertToXmlFormatCommand ?? (_convertToXmlFormatCommand =
                   new CommandHandler(ConvertToXmlFormat, true));

        public ICommand ConvertToJsonFormatCommand
            => _convertToJsonFormatCommand ?? (_convertToJsonFormatCommand =
                   new CommandHandler(ConvertToJsonFormat, true));

        public string InputData
        {
            get => _inputData;
            set
            {
                _inputData = value;
                _inputDataIsModified = true;
                OnPropertyChanged("InputData");
            }
        }

        public string OutputData
        {
            get => _outputData;
            set
            {
                _outputData = value;
                OnPropertyChanged("OutputData");
            }
        }

        public ICommand OpenFileCommand =>
            _openFileCommand ?? (_openFileCommand = new CommandHandler(OpenFile, true));

        public event PropertyChangedEventHandler PropertyChanged;

        private void SaveAsFile()
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = @"Extensible Markup Language|*.xml|JavaScript Object Notation|*.json";
                var result = saveFileDialog.ShowDialog();
                switch (result)
                {
                    case DialogResult.OK:
                        var extension = Path.GetExtension(saveFileDialog.FileName);
                        switch (extension)
                        {
                            case ".xml":
                                ConvertToXmlFormat();
                                break;
                            case ".json":
                                ConvertToJsonFormat();
                                break;
                            default:
                                break;
                        }
                        File.WriteAllText(saveFileDialog.FileName, OutputData);
                        break;
                    default:
                        break;
                }
            }
        }


        private void ConvertToObjectFormat(Text textInstance, string inputData, Regex regularExpression)
        {
            textInstance.Sentences.Clear();

            try
            {
                var arrayOfSentences = regularExpression.Replace(inputData, " ").Split('.');

                foreach (var sentence in arrayOfSentences)
                {
                    var singleSentence = new Sentence();

                    var arrayOfWords = sentence.Split(' ');

                    foreach (var word in arrayOfWords)
                    {
                        var trrimedWord = word.Trim();

                        if (!string.IsNullOrEmpty(trrimedWord))
                            singleSentence.Words.Add(new Word {Value = trrimedWord});
                    }
                    if (singleSentence.Words.Count > 0)
                        textInstance.Sentences.Add(singleSentence);
                    _inputDataIsModified = false;
                }
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(@"Wprowadź jakiś tekst");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
            
        }

        private void ConvertToXmlFormat()
        {
            if (_inputDataIsModified)
            {
                ConvertToObjectFormat(_text, InputData, new Regex(@"[^\p{L}\p{Nd}]+"));
            }
            
            var xmlSerializerNamespaces = new XmlSerializerNamespaces();
            xmlSerializerNamespaces.Add(string.Empty, string.Empty);

            var xmlWritterSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            var xmlSerializer = new XmlSerializer(typeof(Text));

            using (var textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, _text, xmlSerializerNamespaces);
                OutputData = textWriter.ToString();
            }
            //TODO: Zmienić kodowanie na utf-8
        }

        private void ConvertToJsonFormat()
        {
            if (_inputDataIsModified)
            {
                ConvertToObjectFormat(_text, InputData, new Regex(@"[^\p{L}\p{Nd}]+"));
            }
            OutputData = JsonConvert.SerializeObject(_text, Formatting.Indented);
        }


        private void LoadFile(string fileName)
        {
            InputData = File.ReadAllText(fileName, Encoding.UTF8);
        }

        private void OpenFile()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                var result = openFileDialog.ShowDialog();
                switch (result)
                {
                    case DialogResult.OK:
                        LoadFile(openFileDialog.FileName);
                        break;
                    default:
                        break;
                }
            }
        }


        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}