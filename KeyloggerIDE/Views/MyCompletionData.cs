using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using System;

namespace KeyloggerIDE.Views
{
    /// <summary>
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the completion drop down.
    /// </summary>
    public class MyCompletionData : ICompletionData
    {
        public MyCompletionData(string text ,string description)
        {
            this.Text = text;
            this.Description = description;
        }

        public Avalonia.Media.IImage Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the drop down list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description { get; private set; }

        public double Priority { get { return 0; } }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
